namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the remainder of the division of two scalars.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Modulo")]
    public sealed class ScalarModulo : Modulo<float>
    {
        protected override float defaultDividend => 1;

        protected override float defaultDivisor => 1;

        public override float Operation(float a, float b)
        {
            return a % b;
        }
    }
}
