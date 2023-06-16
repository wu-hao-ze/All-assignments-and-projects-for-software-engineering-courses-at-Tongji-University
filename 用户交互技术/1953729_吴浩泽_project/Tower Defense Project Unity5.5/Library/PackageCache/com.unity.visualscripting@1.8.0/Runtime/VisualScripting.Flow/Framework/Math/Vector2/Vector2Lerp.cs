using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the linear interpolation between two 2D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Lerp")]
    public sealed class Vector2Lerp : Lerp<Vector2>
    {
        protected override Vector2 defaultA => Vector2.zero;

        protected override Vector2 defaultB => Vector2.one;

        public override Vector2 Operation(Vector2 a, Vector2 b, float t)
        {
            return Vector2.Lerp(a, b, t);
        }
    }
}
