using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Rounds a decimal number to return an integer.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Round")]
    public sealed class ScalarRound : Round<float, int>
    {
        protected override int Floor(float input)
        {
            return Mathf.FloorToInt(input);
        }

        protected override int AwayFromZero(float input)
        {
            return Mathf.RoundToInt(input);
        }

        protected override int Ceiling(float input)
        {
            return Mathf.CeilToInt(input);
        }
    }
}
