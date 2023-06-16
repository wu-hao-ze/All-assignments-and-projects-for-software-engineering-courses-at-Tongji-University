using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise maximum between two or more 2D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Maximum")]
    public sealed class Vector2Maximum : Maximum<UnityEngine.Vector2>
    {
        public override UnityEngine.Vector2 Operation(UnityEngine.Vector2 a, UnityEngine.Vector2 b)
        {
            return UnityEngine.Vector2.Max(a, b);
        }

        public override UnityEngine.Vector2 Operation(IEnumerable<UnityEngine.Vector2> values)
        {
            var defined = false;
            var maximum = UnityEngine.Vector2.zero;

            foreach (var value in values)
            {
                if (!defined)
                {
                    maximum = value;
                    defined = true;
                }
                else
                {
                    maximum = UnityEngine.Vector2.Max(maximum, value);
                }
            }

            return maximum;
        }
    }
}
