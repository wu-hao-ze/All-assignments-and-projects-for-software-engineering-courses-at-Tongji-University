using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise maximum between two or more 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Maximum")]
    public sealed class Vector4Maximum : Maximum<UnityEngine.Vector4>
    {
        public override UnityEngine.Vector4 Operation(UnityEngine.Vector4 a, UnityEngine.Vector4 b)
        {
            return UnityEngine.Vector4.Max(a, b);
        }

        public override UnityEngine.Vector4 Operation(IEnumerable<UnityEngine.Vector4> values)
        {
            var defined = false;
            var maximum = UnityEngine.Vector4.zero;

            foreach (var value in values)
            {
                if (!defined)
                {
                    maximum = value;
                    defined = true;
                }
                else
                {
                    maximum = UnityEngine.Vector4.Max(maximum, value);
                }
            }

            return maximum;
        }
    }
}
