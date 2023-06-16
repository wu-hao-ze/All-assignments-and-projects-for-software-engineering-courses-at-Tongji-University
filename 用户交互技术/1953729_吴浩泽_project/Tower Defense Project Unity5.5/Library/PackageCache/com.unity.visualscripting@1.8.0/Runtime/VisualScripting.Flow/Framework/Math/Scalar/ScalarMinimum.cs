using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the minimum between two or more scalars.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Minimum")]
    public sealed class ScalarMinimum : Minimum<float>
    {
        public override float Operation(float a, float b)
        {
            return Mathf.Min(a, b);
        }

        public override float Operation(IEnumerable<float> values)
        {
            return values.Min();
        }
    }
}
