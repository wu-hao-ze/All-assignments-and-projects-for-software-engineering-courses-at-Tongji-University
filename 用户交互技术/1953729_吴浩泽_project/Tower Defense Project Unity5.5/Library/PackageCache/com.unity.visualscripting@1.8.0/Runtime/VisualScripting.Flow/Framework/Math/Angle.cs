namespace Unity.VisualScripting
{
    [UnitOrder(403)]
    public abstract class Angle<T> : Unit
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
        /// The angle between A and B.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput angle { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<T>(nameof(a));
            b = ValueInput<T>(nameof(b));
            angle = ValueOutput(nameof(angle), Operation).Predictable();

            Requirement(a, angle);
            Requirement(b, angle);
        }

        private float Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(a), flow.GetValue<T>(b));
        }

        public abstract float Operation(T a, T b);
    }
}
