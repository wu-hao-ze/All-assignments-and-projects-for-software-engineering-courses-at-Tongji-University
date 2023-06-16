using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the framerate-normalized value of a 4D vector.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Per Second")]
    public sealed class Vector4PerSecond : PerSecond<Vector4>
    {
        public override Vector4 Operation(Vector4 input)
        {
            return input * Time.deltaTime;
        }
    }
}
