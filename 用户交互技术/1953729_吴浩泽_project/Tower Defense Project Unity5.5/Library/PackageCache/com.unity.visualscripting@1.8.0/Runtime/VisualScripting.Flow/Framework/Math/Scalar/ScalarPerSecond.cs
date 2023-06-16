using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the framerate-normalized value of a scalar.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Per Second")]
    public sealed class ScalarPerSecond : PerSecond<float>
    {
        public override float Operation(float input)
        {
            return input * Time.deltaTime;
        }
    }
}
