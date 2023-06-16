namespace Unity.VisualScripting
{
    /// <summary>
    /// Inverts the value of a boolean.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(3)]
    public sealed class Negate : Unit
    {
        /// <summary>
        /// The input boolean.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("X")]
        public ValueInput input { get; private set; }

        /// <summary>
        /// True if the input is false, false if the input is true.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("~X")]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            input = ValueInput<bool>(nameof(input));
            output = ValueOutput(nameof(output), Operation).Predictable();

            Requirement(input, output);
        }

        public bool Operation(Flow flow)
        {
            return !flow.GetValue<bool>(input);
        }
    }
}
