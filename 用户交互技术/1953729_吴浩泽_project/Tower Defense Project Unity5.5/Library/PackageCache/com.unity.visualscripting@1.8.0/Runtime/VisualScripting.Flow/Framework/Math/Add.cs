namespace Unity.VisualScripting
{
    [UnitOrder(101)]
    public abstract class Add<T> : Unit
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
        /// The sum of A and B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A + B")]
        public ValueOutput sum { get; private set; }

        [DoNotSerialize]
        protected virtual T defaultB => default(T);

        protected override void Definition()
        {
            a = ValueInput<T>(nameof(a));
            b = ValueInput(nameof(b), defaultB);

            sum = ValueOutput(nameof(sum), Operation).Predictable();

            Requirement(a, sum);
            Requirement(b, sum);
        }

        private T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(a), flow.GetValue<T>(b));
        }

        public abstract T Operation(T a, T b);
    }
}
