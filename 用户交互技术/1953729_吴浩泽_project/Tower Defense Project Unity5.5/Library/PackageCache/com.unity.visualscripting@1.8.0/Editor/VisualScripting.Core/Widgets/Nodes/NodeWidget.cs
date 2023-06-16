using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class NodeWidget<TCanvas, TNode> : GraphElementWidget<TCanvas, TNode>, INodeWidget
        where TCanvas : class, ICanvas
        where TNode : class, IGraphElement
    {
        protected NodeWidget(TCanvas canvas, TNode node) : base(canvas, node) { }


        #region Positioning

        public Rect outerPosition
        {
            get
            {
                return EdgeToOuterPosition(edgePosition);
            }
            set
            {
                edgePosition = OuterToEdgePosition(value);
            }
        }

        public Rect edgePosition
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public Rect innerPosition
        {
            get
            {
                return EdgeToInnerPosition(edgePosition);
            }
            set
            {
                edgePosition = InnerToEdgePosition(value);
            }
        }

        public override Rect clippingPosition => outerPosition;

        protected Rect EdgeToOuterPosition(Rect position)
        {
            return GraphGUI.GetNodeEdgeToOuterPosition(position, shape);
        }

        protected Rect OuterToEdgePosition(Rect position)
        {
            return GraphGUI.GetNodeOuterToEdgePosition(position, shape);
        }

        protected Rect EdgeToInnerPosition(Rect position)
        {
            return GraphGUI.GetNodeEdgeToInnerPosition(position, shape);
        }

        protected Rect InnerToEdgePosition(Rect position)
        {
            return GraphGUI.GetNodeInnerToEdgePosition(position, shape);
        }

        #endregion


        #region Drawing

        protected abstract NodeShape shape { get; }

        protected abstract NodeColorMix color { get; }

        protected bool invertForeground = false;

        public override void DrawForeground()
        {
            base.DrawForeground();

            if (e.IsRepaint)
            {
                GraphGUI.Node(edgePosition.PixelPerfect(), shape, color, isSelected);
            }
        }

        public override void DrawOverlay()
        {
            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                LudiqGUI.DrawEmptyRect(outerPosition, Color.yellow.WithAlpha(0.5f));
                LudiqGUI.DrawEmptyRect(edgePosition, Color.yellow.WithAlpha(0.5f));
                LudiqGUI.DrawEmptyRect(innerPosition, Color.yellow.WithAlpha(0.5f));
            }

            base.DrawOverlay();
        }

        #endregion
    }
}
