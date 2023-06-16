namespace Unity.VisualScripting
{
    public abstract class GraphInspector<TGraphContext> : Inspector where TGraphContext : IGraphContext
    {
        protected GraphInspector(Metadata metadata) : base(metadata) { }

        #region Context Shortcuts

        protected TGraphContext context => (TGraphContext)LudiqGraphsEditorUtility.editedContext.value;

        protected GraphReference reference => context.reference;

        protected IGraph graph => context.graph;

        protected ICanvas canvas => context.canvas;

        protected GraphSelection selection => context.selection;

        #endregion
    }
}
