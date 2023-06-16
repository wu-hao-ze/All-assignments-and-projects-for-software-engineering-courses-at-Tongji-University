using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the linear interpolation between two 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Lerp")]
    public sealed class Vector4Lerp : Lerp<Vector4>
    {
        protected override Vector4 defaultA => Vector4.zero;

        protected override Vector4 defaultB => Vector4.one;

        public override Vector4 Operation(Vector4 a, Vector4 b, float t)
        {
            return Vector4.Lerp(a, b, t);
        }
    }
}
