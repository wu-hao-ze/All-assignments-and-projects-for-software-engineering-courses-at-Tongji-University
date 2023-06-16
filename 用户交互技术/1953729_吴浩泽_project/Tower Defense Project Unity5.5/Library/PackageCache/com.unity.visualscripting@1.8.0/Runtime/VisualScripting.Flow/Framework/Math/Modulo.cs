namespace Unity.VisualScripting
{
    [UnitOrder(105)]
    public abstract class Modulo<T> : Unit
    {
        /// <summary>
        /// The dividend (or numerator).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A")]
        public ValueInput dividend { get; private set; }

        /// <summary>
        /// The divisor (or denominator).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("B")]
        public ValueInput divisor { get; private set; }

        /// <summary>
        /// The remainder of the division of dividend and divison (numerator / denominator).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A % B")]
        public ValueOutput remainder { get; private set; }

        [DoNotSerialize]
        protected virtual T defaultDivisor => default(T);

        [DoNotSerialize]
        protected virtual T defaultDividend => default(T);

        protected override void Definition()
        {
            dividend = ValueInput(nameof(dividend), defaultDividend);
            divisor = ValueInput(nameof(divisor), defaultDivisor);
            remainder = ValueOutput(nameof(remainder), Operation).Predictable();

            Requirement(dividend, remainder);
            Requirement(divisor, remainder);
        }

        public abstract T Operation(T divident, T divisor);

        public T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(dividend), flow.GetValue<T>(divisor));
        }
    }
}
