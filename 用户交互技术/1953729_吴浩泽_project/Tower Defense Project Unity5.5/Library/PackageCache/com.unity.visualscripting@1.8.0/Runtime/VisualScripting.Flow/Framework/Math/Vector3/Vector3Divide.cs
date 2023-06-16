using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise quotient of two 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Divide")]
    public sealed class Vector3Divide : Divide<Vector3>
    {
        protected override Vector3 defaultDividend => Vector3.zero;

        protected override Vector3 defaultDivisor => Vector3.zero;

        public override Vector3 Operation(Vector3 a, Vector3 b)
        {
            return new Vector3
            (
                a.x / b.x,
                a.y / b.y,
                a.z / b.z
            );
        }
    }
}
