namespace Unity.VisualScripting
{
    [UnitCategory("Logic")]
    public abstract class BinaryComparisonUnit : Unit
    {
        /// <summary>
        /// The first input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput a { get; private set; }

        /// <summary>
        /// The second input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput b { get; private set; }

        [DoNotSerialize]
        public virtual ValueOutput comparison { get; private set; }

        /// <summary>
        /// Whether the compared inputs are numbers.
        /// </summary>
        [Serialize]
        [Inspectable]
        [InspectorToggleLeft]
        public bool numeric { get; set; } = true;

        // Backwards compatibility
        protected virtual string outputKey => nameof(comparison);

        protected override void Definition()
        {
            if (numeric)
            {
                a = ValueInput<float>(nameof(a));
                b = ValueInput<float>(nameof(b), 0);
                comparison = ValueOutput(outputKey, NumericComparison).Predictable();
            }
            else
            {
                a = ValueInput<object>(nameof(a)).AllowsNull();
                b = ValueInput<object>(nameof(b)).AllowsNull();
                comparison = ValueOutput(outputKey, GenericComparison).Predictable();
            }

            Requirement(a, comparison);
            Requirement(b, comparison);
        }

        private bool NumericComparison(Flow flow)
        {
            return NumericComparison(flow.GetValue<float>(a), flow.GetValue<float>(b));
        }

        private bool GenericComparison(Flow flow)
        {
            return GenericComparison(flow.GetValue(a), flow.GetValue(b));
        }

        protected abstract bool NumericComparison(float a, float b);

        protected abstract bool GenericComparison(object a, object b);
    }
}
