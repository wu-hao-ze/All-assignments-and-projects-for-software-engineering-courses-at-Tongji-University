using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Moves a 4D vector towards a target.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Move Towards")]
    public sealed class Vector4MoveTowards : MoveTowards<Vector4>
    {
        protected override Vector4 defaultCurrent => Vector4.zero;

        protected override Vector4 defaultTarget => Vector4.one;

        public override Vector4 Operation(Vector4 current, Vector4 target, float maxDelta)
        {
            return Vector4.MoveTowards(current, target, maxDelta);
        }
    }
}
