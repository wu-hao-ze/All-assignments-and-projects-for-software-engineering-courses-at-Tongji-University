using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Rounds each component of a 4D vector.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Round")]
    public sealed class Vector4Round : Round<Vector4, Vector4>
    {
        protected override Vector4 Floor(Vector4 input)
        {
            return new Vector4
            (
                Mathf.Floor(input.x),
                Mathf.Floor(input.y),
                Mathf.Floor(input.z),
                Mathf.Floor(input.w)
            );
        }

        protected override Vector4 AwayFromZero(Vector4 input)
        {
            return new Vector4
            (
                Mathf.Round(input.x),
                Mathf.Round(input.y),
                Mathf.Round(input.z),
                Mathf.Round(input.w)
            );
        }

        protected override Vector4 Ceiling(Vector4 input)
        {
            return new Vector4
            (
                Mathf.Ceil(input.x),
                Mathf.Ceil(input.y),
                Mathf.Ceil(input.z),
                Mathf.Ceil(input.w)
            );
        }
    }
}
