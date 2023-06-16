using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the difference between two 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Subtract")]
    public sealed class Vector3Subtract : Subtract<Vector3>
    {
        protected override Vector3 defaultMinuend => Vector3.zero;

        protected override Vector3 defaultSubtrahend => Vector3.zero;

        public override Vector3 Operation(Vector3 a, Vector3 b)
        {
            return a - b;
        }
    }
}
