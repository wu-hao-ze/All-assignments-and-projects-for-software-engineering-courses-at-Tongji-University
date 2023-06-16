namespace Unity.VisualScripting
{
    /// <summary>
    /// Checks if a saved variable is defined.
    /// </summary>
    [UnitSurtitle("Save")]
    public sealed class IsSavedVariableDefined : IsVariableDefinedUnit, ISavedVariableUnit
    {
        public IsSavedVariableDefined() : base() { }

        public IsSavedVariableDefined(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Saved;
        }
    }
}
