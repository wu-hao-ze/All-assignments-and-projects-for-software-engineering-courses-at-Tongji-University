using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Moves a scalar towards a target.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Move Towards")]
    public sealed class ScalarMoveTowards : MoveTowards<float>
    {
        protected override float defaultCurrent => 0;

        protected override float defaultTarget => 1;

        public override float Operation(float current, float target, float maxDelta)
        {
            return Mathf.MoveTowards(current, target, maxDelta);
        }
    }
}
