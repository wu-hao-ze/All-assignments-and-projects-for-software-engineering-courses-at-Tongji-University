using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Rounds each component of a 3D vector.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Round")]
    public sealed class Vector3Round : Round<Vector3, Vector3>
    {
        protected override Vector3 Floor(Vector3 input)
        {
            return new Vector3
            (
                Mathf.Floor(input.x),
                Mathf.Floor(input.y),
                Mathf.Floor(input.z)
            );
        }

        protected override Vector3 AwayFromZero(Vector3 input)
        {
            return new Vector3
            (
                Mathf.Round(input.x),
                Mathf.Round(input.y),
                Mathf.Round(input.z)
            );
        }

        protected override Vector3 Ceiling(Vector3 input)
        {
            return new Vector3
            (
                Mathf.Ceil(input.x),
                Mathf.Ceil(input.y),
                Mathf.Ceil(input.z)
            );
        }
    }
}
