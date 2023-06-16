using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the linear interpolation between two 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Lerp")]
    public sealed class Vector3Lerp : Lerp<Vector3>
    {
        protected override Vector3 defaultA => Vector3.zero;

        protected override Vector3 defaultB => Vector3.one;

        public override Vector3 Operation(Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t);
        }
    }
}
