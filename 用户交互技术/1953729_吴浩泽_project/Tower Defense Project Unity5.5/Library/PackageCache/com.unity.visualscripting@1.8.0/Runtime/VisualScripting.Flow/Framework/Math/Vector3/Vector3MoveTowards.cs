using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Moves a 3D vector towards a target.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Move Towards")]
    public sealed class Vector3MoveTowards : MoveTowards<Vector3>
    {
        protected override Vector3 defaultCurrent => Vector3.zero;

        protected override Vector3 defaultTarget => Vector3.one;

        public override Vector3 Operation(Vector3 current, Vector3 target, float maxDelta)
        {
            return Vector3.MoveTowards(current, target, maxDelta);
        }
    }
}
