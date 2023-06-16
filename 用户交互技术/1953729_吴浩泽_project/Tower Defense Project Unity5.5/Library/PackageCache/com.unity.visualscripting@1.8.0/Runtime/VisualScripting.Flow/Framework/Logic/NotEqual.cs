using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two inputs to determine whether they are not equal.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(6)]
    public sealed class NotEqual : BinaryComparisonUnit
    {
        public NotEqual() : base()
        {
            numeric = false;
        }

        // Backward compatibility
        protected override string outputKey => "notEqual";

        /// <summary>
        /// Whether A is different than B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2260 B")]
        [PortKey("notEqual")]
        public override ValueOutput comparison => base.comparison;

        protected override bool NumericComparison(float a, float b)
        {
            return !Mathf.Approximately(a, b);
        }

        protected override bool GenericComparison(object a, object b)
        {
            return OperatorUtility.NotEqual(a, b);
        }
    }
}
