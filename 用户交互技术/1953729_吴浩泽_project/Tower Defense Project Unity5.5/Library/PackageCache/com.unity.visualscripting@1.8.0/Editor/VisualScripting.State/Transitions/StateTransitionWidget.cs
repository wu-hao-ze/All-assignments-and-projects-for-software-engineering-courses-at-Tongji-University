using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class StateTransitionWidget<TStateTransition> : NodeWidget<StateCanvas, TStateTransition>, IStateTransitionWidget
        where TStateTransition : class, IStateTransition
    {
        protected StateTransitionWidget(StateCanvas canvas, TStateTransition transition) : base(canvas, transition) { }


        #region Model

        protected TStateTransition transition => element;

        protected IStateTransitionDebugData transitionDebugData => GetDebugData<IStateTransitionDebugData>();

        private StateTransitionDescription description;

        private StateTransitionAnalysis analysis => transition.Analysis<StateTransitionAnalysis>(context);

        protected override void CacheDescription()
        {
            description = transition.Description<StateTransitionDescription>();

            label.text = description.label;
            label.image = description.icon?[IconSize.Small];
            label.tooltip = description.tooltip;

            if (!revealLabel)
            {
                label.tooltip = label.text + ": " + label.tooltip;
            }

            Reposition();
        }

        #endregion

        #region Lifecycle

        public override void BeforeFrame()
        {
            base.BeforeFrame();

            if (showDroplets)
            {
                GraphGUI.UpdateDroplets(canvas, droplets, transitionDebugData.lastBranchFrame, ref lastBranchTime, ref dropTime);
            }

            if (currentInnerWidth != targetInnerWidth)
            {
                Reposition();
            }
        }

        #endregion


        #region Contents

        private GUIContent label { get; } = new GUIContent();

        #endregion


        #region Positioning

        private readonly List<IStateTransition> siblingStateTransitions = new List<IStateTransition>();

        private Rect sourcePosition;

        private Rect destinationPosition;

        private Edge sourceEdge;

        private Edge entryEdge;

        private Edge exitEdge;

        private Edge destinationEdge;

        private Vector2 sourceEdgeCenter;

        private Vector2 entryEdgeCenter;

        private Vector2 exitEdgeCenter;

        private Vector2 destinationEdgeCenter;

        private Vector2 middle;

        private Rect _position;

        private Rect _clippingPosition;

        private float targetInnerWidth;

        private float currentInnerWidth;

        private bool revealInitialized;

        private float minBend
        {
            get
            {
                if (transition.source != transition.destination)
                {
                    return 15;
                }
                else
                {
                    return (middle.y - canvas.Widget(transition.source).position.center.y) / 2;
                }
            }
        }

        private float relativeBend => 1 / 4f;

        Edge IStateTransitionWidget.sourceEdge => sourceEdge;

        public override IEnumerable<IWidget> positionDependencies
        {
            get
            {
                yield return canvas.Widget(transition.source);
                yield return canvas.Widget(transition.destination);
            }
        }

        public override IEnumerable<IWidget> positionDependers
        {
            get
            {
                // Return all sibling transitions. This is an asymetrical dependency / depender
                // relation (because the siblings are not included in the dependers) to force
                // repositioning of siblings while avoiding stack overflow.

                foreach (var graphTransition in canvas.graph.transitions)
                {
                    var current = transition == graphTransition;

                    var analog =
                        transition.source == graphTransition.source &&
                        transition.destination == graphTransition.destination;

                    var inverted =
                        transition.source == graphTransition.destination &&
                        transition.destination == graphTransition.source;

                    if (!current && (analog || inverted))
                    {
                        var widget = canvas.Widget(graphTransition);

                        if (widget.isPositionValid) // Avoid stack overflow
                        {
                            yield return widget;
                        }
                    }
                }
            }
        }

        public Rect iconPosition { get; private set; }

        public Rect clipPosition { get; private set; }

        public Rect labelInnerPosition { get; private set; }

        public override Rect position
        {
            get { return _position; }
            set { }
        }

        public override Rect clippingPosition => _clippingPosition;

        public override void CachePositionFirstPass()
        {
            // Calculate the size immediately, because other transitions will rely on it for positioning

            targetInnerWidth = Styles.eventIcon.fixedWidth;

            var labelWidth = Styles.label.CalcSize(label).x;
            var labelHeight = EditorGUIUtility.singleLineHeight;

            if (revealLabel)
            {
                targetInnerWidth += Styles.spaceAroundIcon;
                targetInnerWidth += labelWidth;
            }

            if (!revealInitialized)
            {
                currentInnerWidth = targetInnerWidth;
                revealInitialized = true;
            }

            currentInnerWidth = Mathf.Lerp(currentInnerWidth, targetInnerWidth, canvas.repaintDeltaTime * Styles.revealSpeed);

            if (Mathf.Abs(targetInnerWidth - currentInnerWidth) < 1)
            {
                currentInnerWidth = targetInnerWidth;
            }

            var innerWidth = currentInnerWidth;
            var innerHeight = labelHeight;

            var edgeSize = InnerToEdgePosition(new Rect(0, 0, innerWidth, innerHeight)).size;
            var edgeWidth = edgeSize.x;
            var edgeHeight = edgeSize.y;

            _position.width = edgeWidth;
            _position.height = edgeHeight;
        }

        public override void CachePosition()
        {
            var innerWidth = innerPosition.width;
            var innerHeight = innerPosition.height;
            var edgeWidth = edgePosition.width;
            var edgeHeight = edgePosition.height;
            var labelWidth = Styles.label.CalcSize(label).x;
            var labelHeight = EditorGUIUtility.singleLineHeight;

            sourcePosition = canvas.Widget(transition.source).position;
            destinationPosition = canvas.Widget(transition.destination).position;

            Vector2 sourceClosestPoint;
            Vector2 destinationClosestPoint;
            LudiqGUIUtility.ClosestPoints(sourcePosition, destinationPosition, out sourceClosestPoint, out destinationClosestPoint);

            if (transition.destination != transition.source)
            {
                GraphGUI.GetConnectionEdge
                    (
                        sourceClosestPoint,
                        destinationClosestPoint,
                        out sourceEdge,
                        out destinationEdge
                    );
            }
            else
            {
                sourceEdge = Edge.Right;
                destinationEdge = Edge.Left;
            }

            sourceEdgeCenter = sourcePosition.GetEdgeCenter(sourceEdge);
            destinationEdgeCenter = destinationPosition.GetEdgeCenter(destinationEdge);

            siblingStateTransitions.Clear();

            var siblingIndex = 0;

            // Assign one common axis for transition for all siblings,
            // regardless of their inversion. The axis is arbitrarily
            // chosen as the axis for the first transition.
            var assignedTransitionAxis = false;
            var transitionAxis = Vector2.zero;

            foreach (var graphTransition in canvas.graph.transitions)
            {
                var current = transition == graphTransition;

                var analog =
                    transition.source == graphTransition.source &&
                    transition.destination == graphTransition.destination;

                var inverted =
                    transition.source == graphTransition.destination &&
                    transition.destination == graphTransition.source;

                if (current)
                {
                    siblingIndex = siblingStateTransitions.Count;
                }

                if (current || analog || inverted)
                {
                    if (!assignedTransitionAxis)
                    {
                        var siblingStateTransitionDrawer = canvas.Widget<IStateTransitionWidget>(graphTransition);

                        transitionAxis = siblingStateTransitionDrawer.sourceEdge.Normal();

                        assignedTransitionAxis = true;
                    }

                    siblingStateTransitions.Add(graphTransition);
                }
            }

            // Fix the edge case where the source and destination perfectly overlap

            if (transitionAxis == Vector2.zero)
            {
                transitionAxis = Vector2.right;
            }

            // Calculate the spread axis and origin for the set of siblings

            var spreadAxis = transitionAxis.Perpendicular1().Abs();
            var spreadOrigin = (sourceEdgeCenter + destinationEdgeCenter) / 2;

            if (transition.source == transition.destination)
            {
                spreadAxis = Vector2.up;
                spreadOrigin = sourcePosition.GetEdgeCenter(Edge.Bottom) - Vector2.down * 10;
            }

            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                Handles.BeginGUI();
                Handles.color = Color.yellow;
                Handles.DrawLine(spreadOrigin + spreadAxis * -1000, spreadOrigin + spreadAxis * 1000);
                Handles.EndGUI();
            }

            // Calculate the offset of the current sibling by iterating over its predecessors

            var spreadOffset = 0f;
            var previousSpreadSize = 0f;

            for (var i = 0; i <= siblingIndex; i++)
            {
                var siblingSize = canvas.Widget<IStateTransitionWidget>(siblingStateTransitions[i]).outerPosition.size;
                var siblingSizeProjection = GraphGUI.SizeProjection(siblingSize, spreadOrigin, spreadAxis);
                spreadOffset += previousSpreadSize / 2 + siblingSizeProjection / 2;
                previousSpreadSize = siblingSizeProjection;
            }

            if (transition.source != transition.destination)
            {
                // Calculate the total spread size to center the sibling set

                var totalSpreadSize = 0f;

                for (var i = 0; i < siblingStateTransitions.Count; i++)
                {
                    var siblingSize = canvas.Widget<IStateTransitionWidget>(siblingStateTransitions[i]).outerPosition.size;
                    var siblingSizeProjection = GraphGUI.SizeProjection(siblingSize, spreadOrigin, spreadAxis);
                    totalSpreadSize += siblingSizeProjection;
                }

                spreadOffset -= totalSpreadSize / 2;
            }

            // Finally, calculate the positions

            middle = spreadOrigin + spreadOffset * spreadAxis;

            var edgeX = middle.x - edgeWidth / 2;
            var edgeY = middle.y - edgeHeight / 2;

            _position = new Rect
                (
                edgeX,
                edgeY,
                edgeWidth,
                edgeHeight
                ).PixelPerfect();

            var innerX = innerPosition.x;
            var innerY = innerPosition.y;

            _clippingPosition = _position.Encompass(sourceEdgeCenter).Encompass(destinationEdgeCenter);

            if (transition.source != transition.destination)
            {
                entryEdge = destinationEdge;
                exitEdge = sourceEdge;
            }
            else
            {
                entryEdge = sourceEdge;
                exitEdge = destinationEdge;
            }

            entryEdgeCenter = edgePosition.GetEdgeCenter(entryEdge);
            exitEdgeCenter = edgePosition.GetEdgeCenter(exitEdge);

            var x = innerX;

            iconPosition = new Rect
                (
                x,
                innerY,
                Styles.eventIcon.fixedWidth,
                Styles.eventIcon.fixedHeight
                ).PixelPerfect();

            x += iconPosition.width;

            var clipWidth = innerWidth - (x - innerX);

            clipPosition = new Rect
                (
                x,
                edgeY,
                clipWidth,
                edgeHeight
                ).PixelPerfect();

            labelInnerPosition = new Rect
                (
                Styles.spaceAroundIcon,
                innerY - edgeY,
                labelWidth,
                labelHeight
                ).PixelPerfect();
        }

        #endregion


        #region Drawing

        protected virtual NodeColorMix baseColor => NodeColor.Gray;

        protected override NodeColorMix color
        {
            get
            {
                if (transitionDebugData.runtimeException != null)
                {
                    return NodeColor.Red;
                }

                var color = baseColor;

                if (analysis.warnings.Count > 0)
                {
                    var mostSevereWarning = Warning.MostSevereLevel(analysis.warnings);

                    switch (mostSevereWarning)
                    {
                        case WarningLevel.Error:
                            color = NodeColor.Red;

                            break;

                        case WarningLevel.Severe:
                            color = NodeColor.Orange;

                            break;

                        case WarningLevel.Caution:
                            color = NodeColor.Yellow;

                            break;
                    }
                }

                if (EditorApplication.isPlaying)
                {
                    if (EditorApplication.isPaused)
                    {
                        if (EditorTimeBinding.frame == transitionDebugData.lastBranchFrame)
                        {
                            color = NodeColor.Blue;
                        }
                    }
                    else
                    {
                        color.blue = Mathf.Lerp(1, 0, (EditorTimeBinding.time - transitionDebugData.lastBranchTime) / StateWidget<IState>.Styles.enterFadeDuration);
                    }
                }

                return color;
            }
        }

        protected override NodeShape shape => NodeShape.Hex;

        private bool revealLabel
        {
            get
            {
                switch (BoltState.Configuration.transitionsReveal)
                {
                    case StateRevealCondition.Always:

                        return true;
                    case StateRevealCondition.Never:

                        return false;
                    case StateRevealCondition.OnHover:

                        return isMouseOver;
                    case StateRevealCondition.OnHoverWithAlt:

                        return isMouseOver && e.alt;
                    case StateRevealCondition.WhenSelected:

                        return selection.Contains(transition);
                    case StateRevealCondition.OnHoverOrSelected:

                        return isMouseOver || selection.Contains(transition);
                    case StateRevealCondition.OnHoverWithAltOrSelected:

                        return isMouseOver && e.alt || selection.Contains(transition);
                    default:

                        throw new UnexpectedEnumValueException<StateRevealCondition>(BoltState.Configuration.transitionsReveal);
                }
            }
        }

        private bool revealedLabel;

        private void CheckReveal()
        {
            var revealLabel = this.revealLabel;

            if (revealLabel != revealedLabel)
            {
                Reposition();
            }

            revealedLabel = revealLabel;
        }

        protected override bool dim
        {
            get
            {
                var dim = BoltCore.Configuration.dimInactiveNodes && !(transition.source.Analysis<StateAnalysis>(context).isEntered && analysis.isTraversed);

                if (isMouseOver || isSelected)
                {
                    dim = false;
                }

                return dim;
            }
        }

        public override void DrawForeground()
        {
            BeginDim();

            base.DrawForeground();

            GUI.Label(iconPosition, label, Styles.eventIcon);

            GUI.BeginClip(clipPosition);

            GUI.Label(labelInnerPosition, label, invertForeground ? Styles.labelInverted : Styles.label);

            GUI.EndClip();

            EndDim();

            CheckReveal();
        }

        public override void DrawBackground()
        {
            BeginDim();

            base.DrawBackground();

            DrawConnection();

            if (showDroplets)
            {
                DrawDroplets();
            }

            EndDim();
        }

        private void DrawConnection()
        {
            GraphGUI.DrawConnectionArrow(Color.white, sourceEdgeCenter, entryEdgeCenter, sourceEdge, entryEdge, relativeBend, minBend);

            if (BoltState.Configuration.transitionsEndArrow)
            {
                GraphGUI.DrawConnectionArrow(Color.white, exitEdgeCenter, destinationEdgeCenter, exitEdge, destinationEdge, relativeBend, minBend);
            }
            else
            {
                GraphGUI.DrawConnection(Color.white, exitEdgeCenter, destinationEdgeCenter, exitEdge, destinationEdge, null, Vector2.zero, relativeBend, minBend);
            }
        }

        #endregion


        #region Selecting

        public override bool canSelect => true;

        #endregion


        #region Dragging

        public override bool canDrag => false;

        protected override bool snapToGrid => false;

        #endregion


        #region Deleting

        public override bool canDelete => true;

        #endregion


        #region Droplets

        private readonly List<float> droplets = new List<float>();

        private float dropTime;

        private float lastBranchTime;

        protected virtual bool showDroplets => BoltState.Configuration.animateTransitions;

        protected virtual Vector2 GetDropletSize()
        {
            return BoltFlow.Icons.valuePortConnected?[12].Size() ?? 12 * Vector2.one;
        }

        protected virtual void DrawDroplet(Rect position)
        {
            GUI.DrawTexture(position, BoltFlow.Icons.valuePortConnected?[12]);
        }

        private void DrawDroplets()
        {
            foreach (var droplet in droplets)
            {
                Vector2 position;

                if (droplet < 0.5f)
                {
                    var t = droplet / 0.5f;
                    position = GraphGUI.GetPointOnConnection(t, sourceEdgeCenter, entryEdgeCenter, sourceEdge, entryEdge, relativeBend, minBend);
                }
                else
                {
                    var t = (droplet - 0.5f) / 0.5f;
                    position = GraphGUI.GetPointOnConnection(t, exitEdgeCenter, destinationEdgeCenter, exitEdge, destinationEdge, relativeBend, minBend);
                }

                var size = GetDropletSize();

                using (LudiqGUI.color.Override(Color.white))
                {
                    DrawDroplet(new Rect(position.x - size.x / 2, position.y - size.y / 2, size.x, size.y));
                }
            }
        }

        #endregion


        public static class Styles
        {
            static Styles()
            {
                label = new GUIStyle(BoltCore.Styles.nodeLabel);
                label.alignment = TextAnchor.MiddleCenter;
                label.imagePosition = ImagePosition.TextOnly;

                labelInverted = new GUIStyle(label);
                labelInverted.normal.textColor = ColorPalette.unityBackgroundDark;

                eventIcon = new GUIStyle();
                eventIcon.imagePosition = ImagePosition.ImageOnly;
                eventIcon.fixedHeight = 16;
                eventIcon.fixedWidth = 16;
            }

            public static readonly GUIStyle label;

            public static readonly GUIStyle labelInverted;

            public static readonly GUIStyle eventIcon;

            public static readonly float spaceAroundIcon = 5;

            public static readonly float revealSpeed = 15;
        }
    }
}
