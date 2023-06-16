using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the node length version of a scalar.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Normalize")]
    public sealed class ScalarNormalize : Normalize<float>
    {
        public override float Operation(float input)
        {
            if (input == 0)
            {
                return 0;
            }

            return input / Mathf.Abs(input);
        }
    }
}
