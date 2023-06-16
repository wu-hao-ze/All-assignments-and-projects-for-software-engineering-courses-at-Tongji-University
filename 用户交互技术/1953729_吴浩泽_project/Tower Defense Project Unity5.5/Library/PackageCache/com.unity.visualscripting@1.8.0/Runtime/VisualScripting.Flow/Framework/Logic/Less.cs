namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two inputs to determine whether the first is less than the second.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(9)]
    public sealed class Less : BinaryComparisonUnit
    {
        /// <summary>
        /// Whether A is less than B.
        /// </summary>
        [PortLabel("A < B")]
        public override ValueOutput comparison => base.comparison;

        protected override bool NumericComparison(float a, float b)
        {
            return a < b;
        }

        protected override bool GenericComparison(object a, object b)
        {
            return OperatorUtility.LessThan(a, b);
        }
    }
}
