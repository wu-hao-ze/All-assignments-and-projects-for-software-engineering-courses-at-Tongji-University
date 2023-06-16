namespace Unity.VisualScripting
{
    public sealed class FlowGraphData : GraphData<FlowGraph>, IGraphDataWithVariables, IGraphEventListenerData
    {
        public VariableDeclarations variables { get; }

        public bool isListening { get; set; }

        public FlowGraphData(FlowGraph definition) : base(definition)
        {
            variables = definition.variables.CloneViaFakeSerialization();
        }
    }
}
