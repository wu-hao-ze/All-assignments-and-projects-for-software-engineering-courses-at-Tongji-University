using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class GraphContextExtension<TGraphContext> : IGraphContextExtension
        where TGraphContext : IGraphContext
    {
        protected GraphContextExtension(TGraphContext context)
        {
            this.context = context;
        }

        public TGraphContext context { get; }

        #region Context Shortcuts

        protected GraphReference reference => context.reference;

        protected IGraph graph => context.graph;

        protected ICanvas canvas => context.canvas;

        protected GraphSelection selection => context.selection;

        #endregion

        public virtual IEnumerable<GraphContextMenuItem> contextMenuItems => Enumerable.Empty<GraphContextMenuItem>();

        public virtual DragAndDropVisualMode dragAndDropVisualMode => DragAndDropVisualMode.Generic;

        public virtual bool AcceptsDragAndDrop() => false;

        public virtual void PerformDragAndDrop() { }

        public virtual void UpdateDragAndDrop() { }

        public virtual void DrawDragAndDropPreview() { }

        public virtual void ExitDragAndDrop() { }

        protected static Event e => Event.current;
    }
}
