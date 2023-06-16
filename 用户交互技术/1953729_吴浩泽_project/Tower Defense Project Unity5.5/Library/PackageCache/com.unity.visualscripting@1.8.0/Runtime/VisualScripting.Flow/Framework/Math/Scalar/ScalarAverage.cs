using System.Collections.Generic;
using System.Linq;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the average of two or more scalars.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Average")]
    public sealed class ScalarAverage : Average<float>
    {
        public override float Operation(float a, float b)
        {
            return (a + b) / 2;
        }

        public override float Operation(IEnumerable<float> values)
        {
            return values.Average();
        }
    }
}
