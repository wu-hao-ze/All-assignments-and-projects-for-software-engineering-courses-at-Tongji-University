namespace Unity.VisualScripting
{
    [UnitOrder(103)]
    public abstract class Multiply<T> : Unit
    {
        /// <summary>
        /// The first value.
        /// </summary>
        [DoNotSerialize]
        public ValueInput a { get; private set; }

        /// <summary>
        /// The second value.
        /// </summary>
        [DoNotSerialize]
        public ValueInput b { get; private set; }

        /// <summary>
        /// The product of A and B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u00D7 B")]
        public ValueOutput product { get; private set; }

        [DoNotSerialize]
        protected virtual T defaultB => default(T);

        protected override void Definition()
        {
            a = ValueInput<T>(nameof(a));
            b = ValueInput(nameof(b), defaultB);
            product = ValueOutput(nameof(product), Operation).Predictable();

            Requirement(a, product);
            Requirement(b, product);
        }

        private T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(a), flow.GetValue<T>(b));
        }

        public abstract T Operation(T a, T b);
    }
}
