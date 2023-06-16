using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the sum of two scalars.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Add")]
    [Obsolete("Use the new \"Add (Math/Scalar)\" node instead.")]
    [RenamedFrom("Bolt.ScalarAdd")]
    [RenamedFrom("Unity.VisualScripting.ScalarAdd")]
    public sealed class DeprecatedScalarAdd : Add<float>
    {
        protected override float defaultB => 1;

        public override float Operation(float a, float b)
        {
            return a + b;
        }
    }
}
