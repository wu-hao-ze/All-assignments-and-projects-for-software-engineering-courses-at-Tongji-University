namespace Unity.VisualScripting
{
    /// <summary>
    /// Sets the value of an application variable.
    /// </summary>
    [UnitSurtitle("Application")]
    public sealed class SetApplicationVariable : SetVariableUnit, IApplicationVariableUnit
    {
        public SetApplicationVariable() : base() { }

        public SetApplicationVariable(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Application;
        }
    }
}
