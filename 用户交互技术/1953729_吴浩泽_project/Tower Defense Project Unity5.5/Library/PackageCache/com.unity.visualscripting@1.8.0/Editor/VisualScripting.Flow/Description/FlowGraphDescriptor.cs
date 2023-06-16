namespace Unity.VisualScripting
{
    [Descriptor(typeof(FlowGraph))]
    public sealed class FlowGraphDescriptor : GraphDescriptor<FlowGraph, GraphDescription>
    {
        public FlowGraphDescriptor(FlowGraph target) : base(target) { }
    }
}
