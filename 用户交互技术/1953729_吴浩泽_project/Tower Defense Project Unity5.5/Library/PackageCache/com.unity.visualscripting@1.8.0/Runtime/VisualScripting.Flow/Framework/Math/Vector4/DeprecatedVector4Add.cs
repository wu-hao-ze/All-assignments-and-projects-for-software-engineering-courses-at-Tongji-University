using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the sum of two 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Add")]
    [Obsolete("Use the new \"Add (Math/Vector 4)\" instead.")]
    [RenamedFrom("Bolt.Vector4Add")]
    [RenamedFrom("Unity.VisualScripting.Vector4Add")]
    public sealed class DeprecatedVector4Add : Add<Vector4>
    {
        protected override Vector4 defaultB => Vector4.zero;

        public override Vector4 Operation(Vector4 a, Vector4 b)
        {
            return a + b;
        }
    }
}
