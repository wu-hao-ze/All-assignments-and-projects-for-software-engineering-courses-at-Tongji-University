namespace Unity.VisualScripting
{
    /// <summary>
    /// Caches the input so that all nodes connected to the output
    /// retrieve the value only once.
    /// </summary>
    [UnitCategory("Control")]
    [UnitOrder(15)]
    public sealed class Cache : Unit
    {
        /// <summary>
        /// The moment at which to cache the value.
        /// The output value will only get updated when this gets triggered.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The value to cache when the node is entered.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The cached value, as it was the last time this node was entered.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Cached")]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        /// <summary>
        /// The action to execute once the value has been cached.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Store);
            input = ValueInput<object>(nameof(input));
            output = ValueOutput<object>(nameof(output));
            exit = ControlOutput(nameof(exit));

            Requirement(input, enter);
            Assignment(enter, output);
            Succession(enter, exit);
        }

        private ControlOutput Store(Flow flow)
        {
            flow.SetValue(output, flow.GetValue(input));

            return exit;
        }
    }
}
