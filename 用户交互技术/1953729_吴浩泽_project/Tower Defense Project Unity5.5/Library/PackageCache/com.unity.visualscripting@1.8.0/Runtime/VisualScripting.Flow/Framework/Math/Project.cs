namespace Unity.VisualScripting
{
    [UnitOrder(406)]
    public abstract class Project<T> : Unit
    {
        /// <summary>
        /// The vector to project.
        /// </summary>
        [DoNotSerialize]
        public ValueInput a { get; private set; }

        /// <summary>
        /// The vector on which to project.
        /// </summary>
        [DoNotSerialize]
        public ValueInput b { get; private set; }

        /// <summary>
        /// The projection of A on B.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput projection { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<T>(nameof(a));
            b = ValueInput<T>(nameof(b));
            projection = ValueOutput(nameof(projection), Operation).Predictable();

            Requirement(a, projection);
            Requirement(b, projection);
        }

        private T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(a), flow.GetValue<T>(b));
        }

        public abstract T Operation(T a, T b);
    }
}
