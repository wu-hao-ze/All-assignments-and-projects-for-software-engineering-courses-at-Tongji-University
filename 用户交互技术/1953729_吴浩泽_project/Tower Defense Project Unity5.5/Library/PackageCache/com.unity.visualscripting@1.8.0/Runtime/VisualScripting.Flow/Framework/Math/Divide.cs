namespace Unity.VisualScripting
{
    [UnitOrder(104)]
    public abstract class Divide<T> : Unit
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
        /// The quotient of the dividend and divisor (numerator / denominator).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u00F7 B")]
        public ValueOutput quotient { get; private set; }

        [DoNotSerialize]
        protected virtual T defaultDivisor => default(T);

        [DoNotSerialize]
        protected virtual T defaultDividend => default(T);

        protected override void Definition()
        {
            dividend = ValueInput(nameof(dividend), defaultDividend);
            divisor = ValueInput(nameof(divisor), defaultDivisor);
            quotient = ValueOutput(nameof(quotient), Operation).Predictable();

            Requirement(dividend, quotient);
            Requirement(divisor, quotient);
        }

        public abstract T Operation(T divident, T divisor);

        public T Operation(Flow flow)
        {
            return Operation(flow.GetValue<T>(dividend), flow.GetValue<T>(divisor));
        }
    }
}
