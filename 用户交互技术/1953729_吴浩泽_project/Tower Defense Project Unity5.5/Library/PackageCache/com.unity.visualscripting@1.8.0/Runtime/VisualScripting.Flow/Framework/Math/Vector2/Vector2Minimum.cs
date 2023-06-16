using System.Collections.Generic;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the component-wise minimum between two or more 2D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Minimum")]
    public sealed class Vector2Minimum : Minimum<UnityEngine.Vector2>
    {
        public override UnityEngine.Vector2 Operation(UnityEngine.Vector2 a, UnityEngine.Vector2 b)
        {
            return UnityEngine.Vector2.Min(a, b);
        }

        public override UnityEngine.Vector2 Operation(IEnumerable<UnityEngine.Vector2> values)
        {
            var defined = false;
            var minimum = UnityEngine.Vector2.zero;

            foreach (var value in values)
            {
                if (!defined)
                {
                    minimum = value;
                    defined = true;
                }
                else
                {
                    minimum = UnityEngine.Vector2.Min(minimum, value);
                }
            }

            return minimum;
        }
    }
}
