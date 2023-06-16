namespace Unity.VisualScripting
{
    [UnitOrder(202)]
    public abstract class Round<TInput, TOutput> : Unit
    {
        public enum Rounding
        {
            Floor = 0,
            Ceiling = 1,
            AwayFromZero = 2,
        }

        /// <summary>
        /// The rounding mode.
        /// </summary>
        [Inspectable, UnitHeaderInspectable, Serialize]
        public Rounding rounding { get; set; } = Rounding.AwayFromZero;

        /// <summary>
        /// The value to round.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput input { get; private set; }

        /// <summary>
        /// The rounded value.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            input = ValueInput<TInput>(nameof(input));
            output = ValueOutput(nameof(output), Operation).Predictable();

            Requirement(input, output);
        }

        protected abstract TOutput Floor(TInput input);
        protected abstract TOutput AwayFromZero(TInput input);
        protected abstract TOutput Ceiling(TInput input);

        public TOutput Operation(Flow flow)
        {
            switch (rounding)
            {
                case Rounding.Floor:
                    return Floor(flow.GetValue<TInput>(input));
                case Rounding.AwayFromZero:
                    return AwayFromZero(flow.GetValue<TInput>(input));
                case Rounding.Ceiling:
                    return Ceiling(flow.GetValue<TInput>(input));
                default:
                    throw new UnexpectedEnumValueException<Rounding>(rounding);
            }
        }
    }
}
