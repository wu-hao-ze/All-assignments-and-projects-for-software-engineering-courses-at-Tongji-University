namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns true if one input is true and the other is false.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(2)]
    public sealed class ExclusiveOr : Unit
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
        /// True if either A or B is true but not the other; false otherwise.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2295 B")]
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
            return flow.GetValue<bool>(a) ^ flow.GetValue<bool>(b);
        }
    }
}
