namespace Unity.VisualScripting
{
    /// <summary>
    /// Selects a value from a set by checking if a condition is true or false.
    /// </summary>
    [UnitCategory("Control")]
    [UnitTitle("Select")]
    [TypeIcon(typeof(ISelectUnit))]
    [UnitOrder(6)]
    public sealed class SelectUnit : Unit, ISelectUnit
    {
        /// <summary>
        /// The condition to check.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput condition { get; private set; }

        /// <summary>
        /// The value to return if the condition is true.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("True")]
        public ValueInput ifTrue { get; private set; }

        /// <summary>
        /// The value to return if the condition is false.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("False")]
        public ValueInput ifFalse { get; private set; }

        /// <summary>
        /// The returned value.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput selection { get; private set; }

        protected override void Definition()
        {
            condition = ValueInput<bool>(nameof(condition));
            ifTrue = ValueInput<object>(nameof(ifTrue)).AllowsNull();
            ifFalse = ValueInput<object>(nameof(ifFalse)).AllowsNull();
            selection = ValueOutput(nameof(selection), Branch).Predictable();

            Requirement(condition, selection);
            Requirement(ifTrue, selection);
            Requirement(ifFalse, selection);
        }

        public object Branch(Flow flow)
        {
            return flow.GetValue(flow.GetValue<bool>(condition) ? ifTrue : ifFalse);
        }
    }
}
