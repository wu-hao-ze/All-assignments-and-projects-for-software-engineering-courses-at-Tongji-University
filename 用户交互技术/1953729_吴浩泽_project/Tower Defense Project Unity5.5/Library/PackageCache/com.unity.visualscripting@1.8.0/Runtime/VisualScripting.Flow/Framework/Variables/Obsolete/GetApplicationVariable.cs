namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets the value of an application variable.
    /// </summary>
    [UnitSurtitle("Application")]
    public sealed class GetApplicationVariable : GetVariableUnit, IApplicationVariableUnit
    {
        public GetApplicationVariable() : base() { }

        public GetApplicationVariable(string defaultName) : base(defaultName) { }

        protected override VariableDeclarations GetDeclarations(Flow flow)
        {
            return Variables.Application;
        }
    }
}
