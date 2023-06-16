using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two inputs to determine whether the first is greater than or equal to the second.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(12)]
    public sealed class GreaterOrEqual : BinaryComparisonUnit
    {
        /// <summary>
        /// Whether A is greater than or equal to B.
        /// </summary>
        [PortLabel("A \u2265 B")]
        public override ValueOutput comparison => base.comparison;

        protected override bool NumericComparison(float a, float b)
        {
            return a > b || Mathf.Approximately(a, b);
        }

        protected override bool GenericComparison(object a, object b)
        {
            return OperatorUtility.GreaterThanOrEqual(a, b);
        }
    }
}
