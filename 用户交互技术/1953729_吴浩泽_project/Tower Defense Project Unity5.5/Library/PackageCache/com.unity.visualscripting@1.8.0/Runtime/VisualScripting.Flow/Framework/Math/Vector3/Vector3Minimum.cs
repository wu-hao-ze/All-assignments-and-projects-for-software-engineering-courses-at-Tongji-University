using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise minimum between two or more 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Minimum")]
    public sealed class Vector3Minimum : Minimum<UnityEngine.Vector3>
    {
        public override UnityEngine.Vector3 Operation(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
        {
            return UnityEngine.Vector3.Min(a, b);
        }

        public override UnityEngine.Vector3 Operation(IEnumerable<UnityEngine.Vector3> values)
        {
            var defined = false;
            var minimum = UnityEngine.Vector3.zero;

            foreach (var value in values)
            {
                if (!defined)
                {
                    minimum = value;
                    defined = true;
                }
                else
                {
                    minimum = UnityEngine.Vector3.Min(minimum, value);
                }
            }

            return minimum;
        }
    }
}
