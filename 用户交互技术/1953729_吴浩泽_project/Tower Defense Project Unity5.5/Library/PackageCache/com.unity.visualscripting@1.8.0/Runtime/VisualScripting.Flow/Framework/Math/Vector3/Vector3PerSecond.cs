using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the framerate-normalized value of a 3D vector.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Per Second")]
    public sealed class Vector3PerSecond : PerSecond<Vector3>
    {
        public override Vector3 Operation(Vector3 input)
        {
            return input * Time.deltaTime;
        }
    }
}
