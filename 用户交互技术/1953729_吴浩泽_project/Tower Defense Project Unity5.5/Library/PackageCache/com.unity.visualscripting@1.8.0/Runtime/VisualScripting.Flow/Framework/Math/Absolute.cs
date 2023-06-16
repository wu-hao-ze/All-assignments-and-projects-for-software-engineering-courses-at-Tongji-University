namespace Unity.VisualScripting
{
    [UnitOrder(201)]
    public abstract class Absolute<TInput> : Unit
    {
        /// <summary>
        /// The value to make positive.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The positive value.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            input = ValueInput<TInput>(nameof(input));
            output = ValueOutput(nameof(output), Operation).Predictable();

            Requirement(input, output);
        }

        protected abstract TInput Operation(TInput input);

        public TInput Operation(Flow flow)
        {
            return Operation(flow.GetValue<TInput>(input));
        }
    }
}
