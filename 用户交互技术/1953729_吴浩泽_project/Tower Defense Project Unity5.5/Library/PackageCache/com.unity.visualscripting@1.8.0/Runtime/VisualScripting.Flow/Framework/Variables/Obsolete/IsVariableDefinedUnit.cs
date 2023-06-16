#pragma warning disable 618

namespace Unity.VisualScripting
{
    [UnitShortTitle("Is Variable Defined")]
    public abstract class IsVariableDefinedUnit : VariableUnit
    {
        protected IsVariableDefinedUnit() : base() { }

        protected IsVariableDefinedUnit(string defaultName) : base(defaultName) { }

        /// <summary>
        /// Whether the variable is defined.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Defined")]
        [PortLabelHidden]
        public new ValueOutput isDefined { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            isDefined = ValueOutput(nameof(isDefined), IsDefined);

            Requirement(name, isDefined);
        }

        protected virtual bool IsDefined(Flow flow)
        {
            var name = flow.GetValue<string>(this.name);

            return GetDeclarations(flow).IsDefined(name);
        }
    }
}
