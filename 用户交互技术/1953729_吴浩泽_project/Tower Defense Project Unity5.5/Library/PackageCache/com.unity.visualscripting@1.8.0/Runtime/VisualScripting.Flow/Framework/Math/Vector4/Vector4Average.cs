using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the average of two or more 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Average")]
    public sealed class Vector4Average : Average<UnityEngine.Vector4>
    {
        public override UnityEngine.Vector4 Operation(UnityEngine.Vector4 a, UnityEngine.Vector4 b)
        {
            return (a + b) / 2;
        }

        public override UnityEngine.Vector4 Operation(IEnumerable<UnityEngine.Vector4> values)
        {
            var average = UnityEngine.Vector4.zero;
            var count = 0;

            foreach (var value in values)
            {
                average += value;
                count++;
            }

            average /= count;
            return average;
        }
    }
}
