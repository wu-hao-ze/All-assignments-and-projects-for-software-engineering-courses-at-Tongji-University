namespace Unity.VisualScripting
{
    /// <summary>
    /// Sets the value of a saved variable.
    /// </summary>
    [UnitSurtitle("Save")]
    public sealed class SetSavedVariable : SetVariableUnit, ISavedVariableUnit
    {
        public SetSavedVariable() : base() { }

        public SetSavedVariable(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Saved;
        }
    }
}
