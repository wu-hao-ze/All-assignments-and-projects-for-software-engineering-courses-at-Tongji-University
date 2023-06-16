namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the product of two scalars.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Multiply")]
    public sealed class ScalarMultiply : Multiply<float>
    {
        protected override float defaultB => 1;

        public override float Operation(float a, float b)
        {
            return a * b;
        }
    }
}
