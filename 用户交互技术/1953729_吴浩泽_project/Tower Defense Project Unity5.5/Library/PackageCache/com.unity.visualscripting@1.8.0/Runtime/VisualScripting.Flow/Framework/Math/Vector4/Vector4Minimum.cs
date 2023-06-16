using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise minimum between two or more 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Minimum")]
    public sealed class Vector4Minimum : Minimum<UnityEngine.Vector4>
    {
        public override UnityEngine.Vector4 Operation(UnityEngine.Vector4 a, UnityEngine.Vector4 b)
        {
            return UnityEngine.Vector4.Min(a, b);
        }

        public override UnityEngine.Vector4 Operation(IEnumerable<UnityEngine.Vector4> values)
        {
            var defined = false;
            var minimum = UnityEngine.Vector4.zero;

            foreach (var value in values)
            {
                if (!defined)
                {
                    minimum = value;
                    defined = true;
                }
                else
                {
                    minimum = UnityEngine.Vector4.Min(minimum, value);
                }
            }

            return minimum;
        }
    }
}
