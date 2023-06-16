namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets the value of a graph variable.
    /// </summary>
    [UnitSurtitle("Graph")]
    public sealed class GetGraphVariable : GetVariableUnit, IGraphVariableUnit
    {
        public GetGraphVariable() : base() { }

        public GetGraphVariable(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Graph(flow.stack);
        }
    }
}
