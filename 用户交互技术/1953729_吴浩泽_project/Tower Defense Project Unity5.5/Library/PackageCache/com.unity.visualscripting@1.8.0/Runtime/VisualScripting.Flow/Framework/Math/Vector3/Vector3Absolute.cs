using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns a version of a 3D vector where each component is positive.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Absolute")]
    public sealed class Vector3Absolute : Absolute<Vector3>
    {
        protected override Vector3 Operation(Vector3 input)
        {
            return new Vector3(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z));
        }
    }
}
