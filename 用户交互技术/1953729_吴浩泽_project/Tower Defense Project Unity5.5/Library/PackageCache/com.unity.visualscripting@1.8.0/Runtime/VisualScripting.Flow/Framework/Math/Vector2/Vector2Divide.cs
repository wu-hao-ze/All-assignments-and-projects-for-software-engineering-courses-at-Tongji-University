using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise quotient of two 2D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Divide")]
    public sealed class Vector2Divide : Divide<Vector2>
    {
        protected override Vector2 defaultDividend => Vector2.zero;

        protected override Vector2 defaultDivisor => Vector2.zero;

        public override Vector2 Operation(Vector2 a, Vector2 b)
        {
            return new Vector2
            (
                a.x / b.x,
                a.y / b.y
            );
        }
    }
}
