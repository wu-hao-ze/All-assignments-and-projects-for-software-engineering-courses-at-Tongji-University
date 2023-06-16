namespace Unity.VisualScripting
{
    [UnitOrder(601)]
    public abstract class PerSecond<T> : Unit
    {
        /// <summary>
        /// The input value.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The framerate-normalized value (multiplied by delta time).
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            input = ValueInput(nameof(input), default(T));
            output = ValueOutput(nameof(output), Operation);

            Requirement(input, output);
        }

        public abstract T Operation(T input);

        public T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(input));
        }
    }
}
