using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns a version of a 2D vector where each component is positive.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Absolute")]
    public sealed class Vector2Absolute : Absolute<Vector2>
    {
        protected override Vector2 Operation(Vector2 input)
        {
            return new Vector2(Mathf.Abs(input.x), Mathf.Abs(input.y));
        }
    }
}
