using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the average of two or more 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Average")]
    public sealed class Vector3Average : Average<UnityEngine.Vector3>
    {
        public override UnityEngine.Vector3 Operation(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
        {
            return (a + b) / 2;
        }

        public override UnityEngine.Vector3 Operation(IEnumerable<UnityEngine.Vector3> values)
        {
            var average = UnityEngine.Vector3.zero;
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
