#pragma warning disable 618

namespace Unity.VisualScripting
{
    [UnitShortTitle("Set Variable")]
    public abstract class SetVariableUnit : VariableUnit
    {
        protected SetVariableUnit() : base() { }

        protected SetVariableUnit(string defaultName) : base(defaultName) { }

        /// <summary>
        /// The entry point to assign the variable reference.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput assign { get; set; }

        /// <summary>
        /// The value to assign to the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("New Value")]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The action to execute once the variable has been assigned.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput assigned { get; set; }

        /// <summary>
        /// The value assigned to the variable.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Value")]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            assign = ControlInput(nameof(assign), Assign);
            input = ValueInput<object>(nameof(input));
            output = ValueOutput<object>(nameof(output));
            assigned = ControlOutput(nameof(assigned));

            Requirement(input, assign);
            Requirement(name, assign);
            Assignment(assign, output);
            Succession(assign, assigned);
        }

        protected virtual ControlOutput Assign(Flow flow)
        {
            var input = flow.GetValue<object>(this.input);
            var name = flow.GetValue<string>(this.name);

            GetDeclarations(flow).Set(name, input);

            flow.SetValue(output, input);

            return assigned;
        }
    }
}
