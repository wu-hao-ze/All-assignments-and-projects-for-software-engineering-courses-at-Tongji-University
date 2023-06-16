namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the quotient of two scalars.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Divide")]
    public sealed class ScalarDivide : Divide<float>
    {
        protected override float defaultDividend => 1;

        protected override float defaultDivisor => 1;

        public override float Operation(float a, float b)
        {
            return a / b;
        }
    }
}
