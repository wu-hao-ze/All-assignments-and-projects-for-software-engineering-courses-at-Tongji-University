namespace Unity.VisualScripting
{
    [UnitOrder(401)]
    public abstract class Normalize<T> : Unit
    {
        /// <summary>
        /// The vector to normalize.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The normalized vector.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            input = ValueInput<T>(nameof(input));
            output = ValueOutput(nameof(output), Operation).Predictable();

            Requirement(input, output);
        }

        private T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(input));
        }

        public abstract T Operation(T input);
    }
}
