namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns true if either input is true.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(1)]
    public sealed class Or : Unit
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
        /// True if either A or B is true; false otherwise.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A | B")]
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
            return flow.GetValue<bool>(a) || flow.GetValue<bool>(b);
        }
    }
}
