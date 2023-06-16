namespace Unity.VisualScripting
{
    /// <summary>
    /// Sets the value of a graph variable.
    /// </summary>
    [UnitSurtitle("Graph")]
    public sealed class SetGraphVariable : SetVariableUnit, IGraphVariableUnit
    {
        public SetGraphVariable() : base() { }

        public SetGraphVariable(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Graph(flow.stack);
        }
    }
}
