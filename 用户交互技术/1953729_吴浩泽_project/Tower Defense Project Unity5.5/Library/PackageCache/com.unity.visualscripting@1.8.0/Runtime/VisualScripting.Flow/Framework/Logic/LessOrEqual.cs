using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two inputs to determine whether the first is less than or equal to the second.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(10)]
    public sealed class LessOrEqual : BinaryComparisonUnit
    {
        /// <summary>
        /// Whether A is greater than or equal to B.
        /// </summary>
        [PortLabel("A \u2264 B")]
        public override ValueOutput comparison => base.comparison;

        protected override bool NumericComparison(float a, float b)
        {
            return a < b || Mathf.Approximately(a, b);
        }

        protected override bool GenericComparison(object a, object b)
        {
            return OperatorUtility.LessThanOrEqual(a, b);
        }
    }
}
