namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets the value of a saved variable.
    /// </summary>
    [UnitSurtitle("Save")]
    public sealed class GetSavedVariable : GetVariableUnit, ISavedVariableUnit
    {
        public GetSavedVariable() : base() { }

        public GetSavedVariable(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Saved;
        }
    }
}
