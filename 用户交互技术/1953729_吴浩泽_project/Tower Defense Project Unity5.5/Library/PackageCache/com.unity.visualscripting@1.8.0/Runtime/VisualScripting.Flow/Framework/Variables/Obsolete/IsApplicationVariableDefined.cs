namespace Unity.VisualScripting
{
    /// <summary>
    /// Checks if an application variable is defined.
    /// </summary>
    [UnitSurtitle("Application")]
    public sealed class IsApplicationVariableDefined : IsVariableDefinedUnit, IApplicationVariableUnit
    {
        public IsApplicationVariableDefined() : base() { }

        public IsApplicationVariableDefined(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Application;
        }
    }
}
