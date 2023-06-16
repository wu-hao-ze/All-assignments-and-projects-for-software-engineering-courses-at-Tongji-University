namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the difference between two scalars.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Subtract")]
    public sealed class ScalarSubtract : Subtract<float>
    {
        protected override float defaultMinuend => 1;

        protected override float defaultSubtrahend => 1;

        public override float Operation(float a, float b)
        {
            return a - b;
        }
    }
}
