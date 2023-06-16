namespace Unity.VisualScripting
{
    [UnitOrder(402)]
    public abstract class Distance<T> : Unit
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
        /// The distance between A and B.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput distance { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<T>(nameof(a));
            b = ValueInput<T>(nameof(b));
            distance = ValueOutput(nameof(distance), Operation).Predictable();

            Requirement(a, distance);
            Requirement(b, distance);
        }

        private float Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(a), flow.GetValue<T>(b));
        }

        public abstract float Operation(T a, T b);
    }
}
