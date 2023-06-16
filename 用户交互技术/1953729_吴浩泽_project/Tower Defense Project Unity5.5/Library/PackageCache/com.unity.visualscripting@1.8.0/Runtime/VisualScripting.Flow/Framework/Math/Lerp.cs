namespace Unity.VisualScripting
{
    [UnitOrder(501)]
    public abstract class Lerp<T> : Unit
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
        /// The interpolation value.
        /// </summary>
        [DoNotSerialize]
        public ValueInput t { get; private set; }

        /// <summary>
        /// The linear interpolation between A and B at T.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput interpolation { get; private set; }

        [DoNotSerialize]
        protected virtual T defaultA => default(T);

        [DoNotSerialize]
        protected virtual T defaultB => default(T);

        protected override void Definition()
        {
            a = ValueInput(nameof(a), defaultA);
            b = ValueInput(nameof(b), defaultB);
            t = ValueInput<float>(nameof(t), 0);
            interpolation = ValueOutput(nameof(interpolation), Operation).Predictable();

            Requirement(a, interpolation);
            Requirement(b, interpolation);
            Requirement(t, interpolation);
        }

        private T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(a), flow.GetValue<T>(b), flow.GetValue<float>(t));
        }

        public abstract T Operation(T a, T b, float t);
    }
}
