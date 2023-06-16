using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the average of two or more 2D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Average")]
    public sealed class Vector2Average : Average<UnityEngine.Vector2>
    {
        public override UnityEngine.Vector2 Operation(UnityEngine.Vector2 a, UnityEngine.Vector2 b)
        {
            return (a + b) / 2;
        }

        public override UnityEngine.Vector2 Operation(IEnumerable<UnityEngine.Vector2> values)
        {
            var average = UnityEngine.Vector2.zero;
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
