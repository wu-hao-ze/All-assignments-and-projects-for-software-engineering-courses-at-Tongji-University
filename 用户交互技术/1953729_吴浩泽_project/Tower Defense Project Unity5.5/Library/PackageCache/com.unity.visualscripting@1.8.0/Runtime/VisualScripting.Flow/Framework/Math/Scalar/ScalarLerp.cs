using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the linear interpolation between two scalars.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Lerp")]
    public sealed class ScalarLerp : Lerp<float>
    {
        protected override float defaultA => 0;

        protected override float defaultB => 1;

        public override float Operation(float a, float b, float t)
        {
            return Mathf.Lerp(a, b, t);
        }
    }
}
