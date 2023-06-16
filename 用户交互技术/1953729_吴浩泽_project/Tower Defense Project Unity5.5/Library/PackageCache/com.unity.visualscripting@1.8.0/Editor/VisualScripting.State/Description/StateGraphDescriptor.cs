namespace Unity.VisualScripting
{
    [Descriptor(typeof(StateGraph))]
    public sealed class StateGraphDescriptor : GraphDescriptor<StateGraph, GraphDescription>
    {
        public StateGraphDescriptor(StateGraph target) : base(target) { }
    }
}
