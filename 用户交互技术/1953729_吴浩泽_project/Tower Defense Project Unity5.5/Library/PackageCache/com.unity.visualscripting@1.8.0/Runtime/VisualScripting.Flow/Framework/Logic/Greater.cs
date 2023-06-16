namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two inputs to determine whether the first is greater than the second.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(11)]
    public sealed class Greater : BinaryComparisonUnit
    {
        /// <summary>
        /// Whether A is greater than B.
        /// </summary>
        [PortLabel("A > B")]
        public override ValueOutput comparison => base.comparison;

        protected override bool NumericComparison(float a, float b)
        {
            return a > b;
        }

        protected override bool GenericComparison(object a, object b)
        {
            return OperatorUtility.GreaterThan(a, b);
        }
    }
}
