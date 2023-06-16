namespace Unity.VisualScripting
{
    [Descriptor(typeof(IGraph))]
    public class GraphDescriptor<TGraph, TGraphDescription> : Descriptor<TGraph, TGraphDescription>
        where TGraph : class, IGraph
        where TGraphDescription : class, IGraphDescription, new()
    {
        protected GraphDescriptor(TGraph target) : base(target) { }

        protected TGraph graph => target;

        [Assigns(cache = false)]
        [RequiresUnityAPI]
        public override string Title()
        {
            return StringUtility.FallbackWhitespace(graph.title, graph.GetType().HumanName());
        }

        [Assigns]
        [RequiresUnityAPI]
        public override string Summary()
        {
            return graph.summary;
        }

        [Assigns]
        [RequiresUnityAPI]
        public override EditorTexture Icon()
        {
            return graph.GetType().Icon();
        }
    }
}
