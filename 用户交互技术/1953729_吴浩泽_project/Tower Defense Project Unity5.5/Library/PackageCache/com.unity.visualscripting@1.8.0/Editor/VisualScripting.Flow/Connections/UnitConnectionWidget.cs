using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class UnitConnectionWidget<TConnection> : GraphElementWidget<FlowCanvas, TConnection>, IUnitConnectionWidget
        where TConnection : class, IUnitConnection
    {
        protected UnitConnectionWidget(FlowCanvas canvas, TConnection connection) : base(canvas, connection) { }


        #region Model

        protected TConnection connection => element;

        protected IUnitConnectionDebugData ConnectionDebugData => GetDebugData<IUnitConnectionDebugData>();

        #endregion


        #region Lifecycle

        public override void BeforeFrame()
        {
            base.BeforeFrame();

            if (showDroplets)
            {
                GraphGUI.UpdateDroplets(canvas, droplets, ConnectionDebugData.lastInvokeFrame, ref lastInvokeTime, ref dropTime);
            }
        }

        #endregion


        #region Positioning

        public override IEnumerable<IWidget> positionDependencies
        {
            get
            {
                yield return canvas.Widget(connection.source);
                yield return canvas.Widget(connection.destination);
            }
        }

        protected override bool snapToGrid => false;

        public Rect sourceHandlePosition { get; private set; }

        public Rect destinationHandlePosition { get; private set; }

        public Vector2 sourceHandleEdgeCenter { get; private set; }

        public Vector2 destinationHandleEdgeCenter { get; private set; }

        public Vector2 middlePosition;

        private Rect _position;

        private Rect _clippingPosition;

        public override Rect position
        {
            get { return _position; }
            set { }
        }

        public override Rect clippingPosition => _clippingPosition;

        public override void CachePosition()
        {
            base.CachePosition();

            sourceHandlePosition = canvas.Widget<IUnitPortWidget>(connection.source).handlePosition;
            destinationHandlePosition = canvas.Widget<IUnitPortWidget>(connection.destination).handlePosition;

            sourceHandleEdgeCenter = sourceHandlePosition.GetEdgeCenter(Edge.Right);
            destinationHandleEdgeCenter = destinationHandlePosition.GetEdgeCenter(Edge.Left);

            middlePosition = (sourceHandlePosition.center + destinationHandlePosition.center) / 2;

            _position = new Rect
                (
                middlePosition.x,
                middlePosition.y,
                0,
                0
                );

            _clippingPosition = _position.Encompass(sourceHandleEdgeCenter).Encompass(destinationHandleEdgeCenter);
        }

        #endregion


        #region Drawing

        protected virtual bool colorIfActive => true;

        public abstract Color color { get; }

        protected override bool dim
        {
            get
            {
                var dim = BoltCore.Configuration.dimInactiveNodes && !connection.destination.unit.Analysis<UnitAnalysis>(context).isEntered;

                if (BoltCore.Configuration.dimIncompatibleNodes && canvas.isCreatingConnection)
                {
                    dim = true;
                }

                return dim;
            }
        }

        public override void DrawBackground()
        {
            base.DrawBackground();

            BeginDim();

            DrawConnection();

            if (showDroplets)
            {
                DrawDroplets();
            }

            EndDim();
        }

        protected virtual void DrawConnection()
        {
            var color = this.color;

            var sourceWidget = canvas.Widget<IUnitPortWidget>(connection.source);
            var destinationWidget = canvas.Widget<IUnitPortWidget>(connection.destination);

            var highlight = !canvas.isCreatingConnection && (sourceWidget.isMouseOver || destinationWidget.isMouseOver);

            var willDisconnect = sourceWidget.willDisconnect || destinationWidget.willDisconnect;

            if (willDisconnect)
            {
                color = UnitConnectionStyles.disconnectColor;
            }
            else if (highlight)
            {
                color = UnitConnectionStyles.highlightColor;
            }
            else if (colorIfActive)
            {
                if (EditorApplication.isPaused)
                {
                    if (EditorTimeBinding.frame == ConnectionDebugData.lastInvokeFrame)
                    {
                        color = UnitConnectionStyles.activeColor;
                    }
                }
                else
                {
                    color = Color.Lerp(UnitConnectionStyles.activeColor, color, (EditorTimeBinding.time - ConnectionDebugData.lastInvokeTime) / UnitWidget<IUnit>.Styles.invokeFadeDuration);
                }
            }

            var thickness = 3;

            GraphGUI.DrawConnection(color, sourceHandleEdgeCenter, destinationHandleEdgeCenter, Edge.Right, Edge.Left, null, Vector2.zero, UnitConnectionStyles.relativeBend, UnitConnectionStyles.minBend, thickness);
        }

        #endregion


        #region Selecting

        public override bool canSelect => false;

        #endregion


        #region Dragging

        public override bool canDrag => false;

        #endregion


        #region Deleting

        public override bool canDelete => true;

        #endregion


        #region Droplets

        private readonly List<float> droplets = new List<float>();

        private float dropTime;

        private float lastInvokeTime;

        private const float handleAlignmentMargin = 0.1f;

        protected virtual bool showDroplets => true;

        protected abstract Vector2 GetDropletSize();

        protected abstract void DrawDroplet(Rect position);

        protected virtual void DrawDroplets()
        {
            foreach (var droplet in droplets)
            {
                Vector2 position;

                if (droplet < handleAlignmentMargin)
                {
                    var t = droplet / handleAlignmentMargin;
                    position = Vector2.Lerp(sourceHandlePosition.center, sourceHandleEdgeCenter, t);
                }
                else if (droplet > 1 - handleAlignmentMargin)
                {
                    var t = (droplet - (1 - handleAlignmentMargin)) / handleAlignmentMargin;
                    position = Vector2.Lerp(destinationHandleEdgeCenter, destinationHandlePosition.center, t);
                }
                else
                {
                    var t = (droplet - handleAlignmentMargin) / (1 - 2 * handleAlignmentMargin);
                    position = GraphGUI.GetPointOnConnection(t, sourceHandleEdgeCenter, destinationHandleEdgeCenter, Edge.Right, Edge.Left, UnitConnectionStyles.relativeBend, UnitConnectionStyles.minBend);
                }

                var size = GetDropletSize();

                using (LudiqGUI.color.Override(GUI.color * color))
                {
                    DrawDroplet(new Rect(position.x - size.x / 2, position.y - size.y / 2, size.x, size.y));
                }
            }
        }

        #endregion
    }
}
