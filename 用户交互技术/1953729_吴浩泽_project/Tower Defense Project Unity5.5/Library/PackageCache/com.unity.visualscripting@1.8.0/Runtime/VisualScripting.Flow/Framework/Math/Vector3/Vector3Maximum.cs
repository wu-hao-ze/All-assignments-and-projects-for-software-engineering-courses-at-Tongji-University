using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise maximum between two or more 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Maximum")]
    public sealed class Vector3Maximum : Maximum<UnityEngine.Vector3>
    {
        public override UnityEngine.Vector3 Operation(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
        {
            return UnityEngine.Vector3.Max(a, b);
        }

        public override UnityEngine.Vector3 Operation(IEnumerable<UnityEngine.Vector3> values)
        {
            var defined = false;
            var maximum = UnityEngine.Vector3.zero;

            foreach (var value in values)
            {
                if (!defined)
                {
                    maximum = value;
                    defined = true;
                }
                else
                {
                    maximum = UnityEngine.Vector3.Max(maximum, value);
                }
            }

            return maximum;
        }
    }
}
