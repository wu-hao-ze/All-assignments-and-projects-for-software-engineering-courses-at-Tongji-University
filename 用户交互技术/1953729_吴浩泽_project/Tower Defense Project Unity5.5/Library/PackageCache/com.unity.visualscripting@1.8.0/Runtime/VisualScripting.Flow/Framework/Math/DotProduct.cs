namespace Unity.VisualScripting
{
    [UnitOrder(404)]
    public abstract class DotProduct<T> : Unit
    {
        /// <summary>
        /// The first vector.
        /// </summary>
        [DoNotSerialize]
        public ValueInput a { get; private set; }

        /// <summary>
        /// The second vector.
        /// </summary>
        [DoNotSerialize]
        public ValueInput b { get; private set; }

        /// <summary>
        /// The dot product of A and B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A\u2219B")]
        public ValueOutput dotProduct { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<T>(nameof(a));
            b = ValueInput<T>(nameof(b));
            dotProduct = ValueOutput(nameof(dotProduct), Operation).Predictable();

            Requirement(a, dotProduct);
            Requirement(b, dotProduct);
        }

        private float Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(a), flow.GetValue<T>(b));
        }

        public abstract float Operation(T a, T b);
    }
}
