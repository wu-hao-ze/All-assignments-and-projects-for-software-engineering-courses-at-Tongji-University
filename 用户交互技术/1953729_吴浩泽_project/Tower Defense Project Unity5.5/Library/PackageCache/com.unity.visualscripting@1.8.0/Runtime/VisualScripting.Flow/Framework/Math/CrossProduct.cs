namespace Unity.VisualScripting
{
    [UnitOrder(405)]
    [TypeIcon(typeof(Multiply<>))]
    public abstract class CrossProduct<T> : Unit
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
        /// The cross product of A and B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u00D7 B")]
        public ValueOutput crossProduct { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<T>(nameof(a));
            b = ValueInput<T>(nameof(b));
            crossProduct = ValueOutput(nameof(crossProduct), Operation).Predictable();

            Requirement(a, crossProduct);
            Requirement(b, crossProduct);
        }

        private T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(a), flow.GetValue<T>(b));
        }

        public abstract T Operation(T a, T b);
    }
}
