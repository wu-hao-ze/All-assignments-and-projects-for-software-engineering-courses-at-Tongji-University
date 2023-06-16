using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Rounds each component of a 2D vector.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Round")]
    public sealed class Vector2Round : Round<Vector2, Vector2>
    {
        protected override Vector2 Floor(Vector2 input)
        {
            return new Vector2
            (
                Mathf.Floor(input.x),
                Mathf.Floor(input.y)
            );
        }

        protected override Vector2 AwayFromZero(Vector2 input)
        {
            return new Vector2
            (
                Mathf.Round(input.x),
                Mathf.Round(input.y)
            );
        }

        protected override Vector2 Ceiling(Vector2 input)
        {
            return new Vector2
            (
                Mathf.Ceil(input.x),
                Mathf.Ceil(input.y)
            );
        }
    }
}
