using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the difference between two 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Subtract")]
    public sealed class Vector4Subtract : Subtract<Vector4>
    {
        protected override Vector4 defaultMinuend => Vector4.zero;

        protected override Vector4 defaultSubtrahend => Vector4.zero;

        public override Vector4 Operation(Vector4 a, Vector4 b)
        {
            return a - b;
        }
    }
}
