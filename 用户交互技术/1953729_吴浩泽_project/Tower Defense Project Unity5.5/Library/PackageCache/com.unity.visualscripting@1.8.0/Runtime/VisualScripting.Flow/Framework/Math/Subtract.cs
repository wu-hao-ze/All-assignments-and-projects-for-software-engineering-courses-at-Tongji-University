namespace Unity.VisualScripting
{
    [UnitOrder(102)]
    public abstract class Subtract<T> : Unit
    {
        /// <summary>
        /// The first value (minuend).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A")]
        public ValueInput minuend { get; private set; }

        /// <summary>
        /// The second value (subtrahend).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("B")]
        public ValueInput subtrahend { get; private set; }

        /// <summary>
        /// The difference, that is the minuend minus the subtrahend.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2212 B")]
        public ValueOutput difference { get; private set; }

        [DoNotSerialize]
        protected virtual T defaultMinuend => default(T);

        [DoNotSerialize]
        protected virtual T defaultSubtrahend => default(T);

        protected override void Definition()
        {
            minuend = ValueInput(nameof(minuend), defaultMinuend);
            subtrahend = ValueInput(nameof(subtrahend), defaultSubtrahend);
            difference = ValueOutput(nameof(difference), Operation).Predictable();

            Requirement(minuend, difference);
            Requirement(subtrahend, difference);
        }

        public abstract T Operation(T a, T b);

        public T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(minuend), flow.GetValue<T>(subtrahend));
        }
    }
}
