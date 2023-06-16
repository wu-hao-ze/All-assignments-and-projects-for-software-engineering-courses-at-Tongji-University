namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns true if both inputs are true.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(0)]
    public sealed class And : Unit
    {
        /// <summary>
        /// The first boolean.
        /// </summary>
        [DoNotSerialize]
        public ValueInput a { get; private set; }

        /// <summary>
        /// The second boolean.
        /// </summary>
        [DoNotSerialize]
        public ValueInput b { get; private set; }

        /// <summary>
        /// True if A and B are both true; false otherwise.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A & B")]
        public ValueOutput result { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<bool>(nameof(a));
            b = ValueInput<bool>(nameof(b));
            result = ValueOutput(nameof(result), Operation).Predictable();

            Requirement(a, result);
            Requirement(b, result);
        }

        public bool Operation(Flow flow)
        {
            return flow.GetValue<bool>(a) && flow.GetValue<bool>(b);
        }
    }
}
