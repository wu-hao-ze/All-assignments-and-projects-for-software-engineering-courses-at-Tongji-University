using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the remainder of the component-wise division of two 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Modulo")]
    public sealed class Vector4Modulo : Modulo<Vector4>
    {
        protected override Vector4 defaultDividend => Vector4.zero;

        protected override Vector4 defaultDivisor => Vector4.zero;

        public override Vector4 Operation(Vector4 a, Vector4 b)
        {
            return new Vector4
            (
                a.x % b.x,
                a.y % b.y,
                a.z % b.z,
                a.w % b.w
            );
        }
    }
}
