#pragma warning disable 618

namespace Unity.VisualScripting
{
    [UnitShortTitle("Get Variable")]
    public abstract class GetVariableUnit : VariableUnit
    {
        protected GetVariableUnit() : base() { }

        protected GetVariableUnit(string defaultName) : base(defaultName) { }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            value = ValueOutput(nameof(value), Get).PredictableIf(IsDefined);

            Requirement(name, value);
        }

        protected virtual bool IsDefined(Flow flow)
        {
            var name = flow.GetValue<string>(this.name);

            return GetDeclarations(flow)?.IsDefined(name) ?? false;
        }

        protected virtual object Get(Flow flow)
        {
            var name = flow.GetValue<string>(this.name);

            return GetDeclarations(flow).Get(name);
        }
    }
}
