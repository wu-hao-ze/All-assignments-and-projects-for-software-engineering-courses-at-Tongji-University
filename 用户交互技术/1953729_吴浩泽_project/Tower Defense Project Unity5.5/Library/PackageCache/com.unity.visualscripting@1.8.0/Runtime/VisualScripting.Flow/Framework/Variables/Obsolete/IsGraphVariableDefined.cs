namespace Unity.VisualScripting
{
    /// <summary>
    /// Checks if a graph variable is defined.
    /// </summary>
    [UnitSurtitle("Graph")]
    public sealed class IsGraphVariableDefined : IsVariableDefinedUnit, IGraphVariableUnit
    {
        public IsGraphVariableDefined() : base() { }

        public IsGraphVariableDefined(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Graph(flow.stack);
        }
    }
}
