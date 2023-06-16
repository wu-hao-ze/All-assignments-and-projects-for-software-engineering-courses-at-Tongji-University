using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two inputs to determine whether they are equal.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitOrder(5)]
    public sealed class Equal : BinaryComparisonUnit
    {
        public Equal() : base()
        {
            numeric = false;
        }

        // Backward compatibility
        protected override string outputKey => "equal";

        /// <summary>
        /// Whether A is equal to B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A = B")]
        [PortKey("equal")]
        public override ValueOutput comparison => base.comparison;

        protected override bool NumericComparison(float a, float b)
        {
            return Mathf.Approximately(a, b);
        }

        protected override bool GenericComparison(object a, object b)
        {
            return OperatorUtility.Equal(a, b);
        }
    }
}
