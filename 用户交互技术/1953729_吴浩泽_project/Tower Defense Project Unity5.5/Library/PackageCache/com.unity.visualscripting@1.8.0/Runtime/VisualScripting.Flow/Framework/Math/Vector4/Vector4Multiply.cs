using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise product of two 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Multiply")]
    public sealed class Vector4Multiply : Multiply<Vector4>
    {
        protected override Vector4 defaultB => Vector4.zero;

        public override Vector4 Operation(Vector4 a, Vector4 b)
        {
            return new Vector4
            (
                a.x * b.x,
                a.y * b.y,
                a.z * b.z,
                a.w * b.w
            );
        }
    }
}
