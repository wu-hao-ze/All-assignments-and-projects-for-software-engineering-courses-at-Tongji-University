using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class StateWidget<TState> : NodeWidget<StateCanvas, TState>, IStateWidget
        where TState : class, IState
    {
        protected StateWidget(StateCanvas canvas, TState state) : base(canvas, state)
        {
            minResizeSize = new Vector2(State.DefaultWidth, 0);
        }

        public virtual bool canForceEnter => true;

        public virtual bool canForceExit => true;

        public virtual bool canToggleStart => true;


        #region Model

        protected TState state => element;

        protected IStateDebugData stateDebugData => GetDebugData<IStateDebugData>();

        protected State.Data stateData => reference.hasData ? reference.GetElementData<State.Data>(state) : null;

        IState IStateWidget.state => state;

        protected StateDescription description { get; private set; }

        protected StateAnalysis analysis => state.Analysis<StateAnalysis>(context);

        protected override void CacheDescription()
        {
            description = state.Description<StateDescription>();

            title = description.title;
            summary = description.summary;

            titleContent.text = " " + title;
            titleContent.image = description.icon?[IconSize.Small];
            summaryContent.text = summary;

            Reposition();
        }

        #endregion


        #region Lifecycle

        public override void BeforeFrame()
        {
            base.BeforeFrame();

            if (currentContentOuterHeight != targetContentOuterHeight)
            {
                Reposition();
            }
        }

        public override void HandleInput()
        {
            if (e.IsMouseDrag(MouseButton.Left) &&
                e.ctrlOrCmd &&
                !canvas.isCreatingTransition)
            {
                if (state.canBeSource)
                {
                    canvas.StartTransition(state);
                }
                else
                {
                    Debug.LogWarning("Cannot create a transition from this state.\n");
                }

                e.Use();
            }
            else if (e.IsMouseDrag(MouseButton.Left) && canvas.isCreatingTransition)
            {
                e.Use();
            }
            else if (e.IsMouseUp(MouseButton.Left) && canvas.isCreatingTransition)
            {
                var source = canvas.transitionSource;
                var destination = (canvas.hoveredWidget as IStateWidget)?.state;

                if (destination == null)
                {
                    canvas.CompleteTransitionToNewState();
                }
                else if (destination == source)
                {
                    canvas.CancelTransition();
                }
                else if (destination.canBeDestination)
                {
                    canvas.EndTransition(destination);
                }
                else
                {
                    Debug.LogWarning("Cannot create a transition to this state.\n");
                    canvas.CancelTransition();
                }

                e.Use();
            }

            base.HandleInput();
        }

        #endregion


        #region Contents

        protected virtual string title { get; set; }

        protected virtual string summary { get; set; }

        private GUIContent titleContent { get; } = new GUIContent();

        private GUIContent summaryContent { get; } = new GUIContent();

        #endregion


        #region Positioning

        public override IEnumerable<IWidget> positionDependers => state.transitions.Select(transition => (IWidget)canvas.Widget(transition));

        public Rect titlePosition { get; private set; }

        public Rect summaryPosition { get; private set; }

        public Rect contentOuterPosition { get; private set; }

        public Rect contentBackgroundPosition { get; private set; }

        public Rect contentInnerPosition { get; private set; }

        private float targetContentOuterHeight;

        private float currentContentOuterHeight;

        private bool revealInitialized;

        private Rect _position;

        public override Rect position
        {
            get { return _position; }
            set
            {
                state.position = value.position;
                state.width = value.width;
            }
        }

        public override void CachePosition()
        {
            var edgeOrigin = state.position;
            var edgeX = edgeOrigin.x;
            var edgeY = edgeOrigin.y;
            var edgeWidth = state.width;
            var innerOrigin = EdgeToInnerPosition(new Rect(edgeOrigin, Vector2.zero)).position;
            var innerX = innerOrigin.x;
            var innerY = innerOrigin.y;
            var innerWidth = EdgeToInnerPosition(new Rect(0, 0, edgeWidth, 0)).width;
            var innerHeight = 0f;

            var y = innerY;

            if (showTitle)
            {
                using (LudiqGUIUtility.iconSize.Override(IconSize.Small))
                {
                    titlePosition = new Rect
                        (
                        innerX,
                        y,
                        innerWidth,
                        Styles.title.CalcHeight(titleContent, innerWidth)
                        );

                    y += titlePosition.height;
                    innerHeight += titlePosition.height;
                }
            }

            if (showTitle && showSummary)
            {
                y += Styles.spaceBetweenTitleAndSummary;
                innerHeight += Styles.spaceBetweenTitleAndSummary;
            }

            if (showSummary)
            {
                summaryPosition = new Rect
                    (
                    innerX,
                    y,
                    innerWidth,
                    Styles.summary.CalcHeight(summaryContent, innerWidth)
                    );

                y += summaryPosition.height;
                innerHeight += summaryPosition.height;
            }

            if (showContent)
            {
                var contentInnerWidth = edgeWidth - Styles.contentBackground.padding.left - Styles.contentBackground.padding.right;

                targetContentOuterHeight = revealContent ? (Styles.spaceBeforeContent + Styles.contentBackground.padding.top + GetContentHeight(contentInnerWidth) + Styles.contentBackground.padding.bottom) : 0;

                if (!revealInitialized)
                {
                    currentContentOuterHeight = targetContentOuterHeight;
                    revealInitialized = true;
                }

                currentContentOuterHeight = Mathf.Lerp(currentContentOuterHeight, targetContentOuterHeight, canvas.repaintDeltaTime * Styles.contentRevealSpeed);

                if (Mathf.Abs(targetContentOuterHeight - currentContentOuterHeight) < 1)
                {
                    currentContentOuterHeight = targetContentOuterHeight;
                }

                contentOuterPosition = new Rect
                    (
                    edgeX,
                    y,
                    edgeWidth,
                    currentContentOuterHeight
                    );

                contentBackgroundPosition = new Rect
                    (
                    0,
                    Styles.spaceBeforeContent,
                    edgeWidth,
                    currentContentOuterHeight - Styles.spaceBeforeContent
                    );

                contentInnerPosition = new Rect
                    (
                    Styles.contentBackground.padding.left,
                    Styles.spaceBeforeContent + Styles.contentBackground.padding.top,
                    contentInnerWidth,
                    contentBackgroundPosition.height - Styles.contentBackground.padding.top
                    );

                y += contentOuterPosition.height;
                innerHeight += contentOuterPosition.height;
            }

            var edgeHeight = InnerToEdgePosition(new Rect(0, 0, 0, innerHeight)).height;

            _position = new Rect
                (
                edgeX,
                edgeY,
                edgeWidth,
                edgeHeight
                );
        }

        protected virtual float GetContentHeight(float width) => 0;

        #endregion


        #region Drawing

        protected virtual bool showTitle => true;

        protected virtual bool showSummary => !StringUtility.IsNullOrWhiteSpace(summary);

        protected virtual bool showContent => false;

        protected virtual NodeColorMix baseColor => NodeColor.Gray;

        protected override NodeColorMix color
        {
            get
            {
                if (stateDebugData.runtimeException != null)
                {
                    return NodeColor.Red;
                }

                var color = baseColor;

                if (state.isStart)
                {
                    color = NodeColor.Green;
                }

                if (stateData?.isActive ?? false)
                {
                    color = NodeColor.Blue;
                }
                else if (EditorApplication.isPaused)
                {
                    if (EditorTimeBinding.frame == stateDebugData.lastEnterFrame)
                    {
                        color = NodeColor.Blue;
                    }
                }
                else
                {
                    color.blue = Mathf.Lerp(1, 0, (EditorTimeBinding.time - stateDebugData.lastExitTime) / Styles.enterFadeDuration);
                }

                return color;
            }
        }

        protected override NodeShape shape => NodeShape.Square;

        private bool revealContent
        {
            get
            {
                switch (BoltState.Configuration.statesReveal)
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

                        return selection.Contains(state);
                    case StateRevealCondition.OnHoverOrSelected:

                        return isMouseOver || selection.Contains(state);
                    case StateRevealCondition.OnHoverWithAltOrSelected:

                        return isMouseOver && e.alt || selection.Contains(state);
                    default:

                        throw new UnexpectedEnumValueException<StateRevealCondition>(BoltState.Configuration.statesReveal);
                }
            }
        }

        private bool revealedContent;

        private void CheckReveal()
        {
            var revealContent = this.revealContent;

            if (revealContent != revealedContent)
            {
                Reposition();
            }

            revealedContent = revealContent;
        }

        protected override bool dim
        {
            get
            {
                var dim = BoltCore.Configuration.dimInactiveNodes && !analysis.isEntered;

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

            if (showTitle)
            {
                DrawTitle();
            }

            if (showSummary)
            {
                DrawSummary();
            }

            if (showContent)
            {
                DrawContentWrapped();
            }

            EndDim();

            CheckReveal();
        }

        private void DrawTitle()
        {
            using (LudiqGUIUtility.iconSize.Override(IconSize.Small))
            {
                GUI.Label(titlePosition, titleContent, invertForeground ? Styles.titleInverted : Styles.title);
            }
        }

        private void DrawSummary()
        {
            GUI.Label(summaryPosition, summaryContent, invertForeground ? Styles.summaryInverted : Styles.summary);
        }

        private void DrawContentWrapped()
        {
            GUI.BeginClip(contentOuterPosition);

            DrawContentBackground();

            DrawContent();

            GUI.EndClip();
        }

        protected virtual void DrawContentBackground()
        {
            if (e.IsRepaint)
            {
                Styles.contentBackground.Draw(contentBackgroundPosition, false, false, false, false);
            }
        }

        protected virtual void DrawContent() { }

        #endregion


        #region Selecting

        public override bool canSelect => true;

        #endregion


        #region Dragging

        protected override bool snapToGrid => BoltCore.Configuration.snapToGrid;

        public override bool canDrag => true;

        public override void ExpandDragGroup(HashSet<IGraphElement> dragGroup)
        {
            if (BoltCore.Configuration.carryChildren)
            {
                foreach (var transition in state.outgoingTransitions)
                {
                    if (dragGroup.Contains(transition.destination))
                    {
                        continue;
                    }

                    dragGroup.Add(transition.destination);

                    canvas.Widget(transition.destination).ExpandDragGroup(dragGroup);
                }
            }
        }

        #endregion


        #region Deleting

        public override bool canDelete => true;

        #endregion


        #region Resizing

        public override bool canResizeHorizontal => true;

        #endregion


        #region Clipboard

        public override void ExpandCopyGroup(HashSet<IGraphElement> copyGroup)
        {
            copyGroup.UnionWith(state.transitions.Cast<IGraphElement>());
        }

        #endregion


        #region Actions

        protected override IEnumerable<DropdownOption> contextOptions
        {
            get
            {
                if (Application.isPlaying && reference.hasData)
                {
                    if (canForceEnter)
                    {
                        yield return new DropdownOption((Action)ForceEnter, "Force Enter");
                    }

                    if (canForceExit)
                    {
                        yield return new DropdownOption((Action)ForceExit, "Force Exit");
                    }
                }

                if (canToggleStart)
                {
                    yield return new DropdownOption((Action)ToggleStart, "Toggle Start");
                }

                if (state.canBeSource)
                {
                    yield return new DropdownOption((Action)MakeTransition, "Make Transition");
                }

                if (state.canBeSource && state.canBeDestination)
                {
                    yield return new DropdownOption((Action)MakeSelfTransition, "Make Self Transition");
                }

                foreach (var baseOption in base.contextOptions)
                {
                    yield return baseOption;
                }
            }
        }

        private void ForceEnter()
        {
            using (var flow = Flow.New(reference))
            {
                state.OnEnter(flow, StateEnterReason.Forced);
            }
        }

        private void ForceExit()
        {
            using (var flow = Flow.New(reference))
            {
                state.OnExit(flow, StateExitReason.Forced);
            }
        }

        protected void MakeTransition()
        {
            canvas.StartTransition(state);
        }

        protected void MakeSelfTransition()
        {
            canvas.StartTransition(state);
            canvas.EndTransition(state);
        }

        protected void ToggleStart()
        {
            UndoUtility.RecordEditedObject("Toggle State Start");

            state.isStart = !state.isStart;
        }

        #endregion


        public static class Styles
        {
            static Styles()
            {
                title = new GUIStyle(BoltCore.Styles.nodeLabel);
                title.fontSize = 12;
                title.alignment = TextAnchor.MiddleCenter;
                title.wordWrap = true;

                summary = new GUIStyle(BoltCore.Styles.nodeLabel);
                summary.fontSize = 10;
                summary.alignment = TextAnchor.MiddleCenter;
                summary.wordWrap = true;

                titleInverted = new GUIStyle(title);
                titleInverted.normal.textColor = ColorPalette.unityBackgroundDark;

                summaryInverted = new GUIStyle(summary);
                summaryInverted.normal.textColor = ColorPalette.unityBackgroundDark;

                contentBackground = new GUIStyle("In BigTitle");
                contentBackground.padding = new RectOffset(0, 0, 4, 4);
            }

            public static readonly GUIStyle title;

            public static readonly GUIStyle summary;

            public static readonly GUIStyle titleInverted;

            public static readonly GUIStyle summaryInverted;

            public static readonly GUIStyle contentBackground;

            public static readonly float spaceBeforeContent = 5;

            public static readonly float spaceBetweenTitleAndSummary = 0;

            public static readonly float enterFadeDuration = 0.5f;

            public static readonly float contentRevealSpeed = 15;
        }
    }
}
