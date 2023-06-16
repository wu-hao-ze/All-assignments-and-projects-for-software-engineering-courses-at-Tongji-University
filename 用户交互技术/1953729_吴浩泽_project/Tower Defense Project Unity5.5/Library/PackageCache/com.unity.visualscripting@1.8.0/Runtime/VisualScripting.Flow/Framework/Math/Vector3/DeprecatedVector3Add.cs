using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the sum of two 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Add")]
    [Obsolete("Use the new \"Add (Math/Vector 3)\" instead.")]
    [RenamedFrom("Bolt.Vector3Add")]
    [RenamedFrom("Unity.VisualScripting.Vector3Add")]
    public sealed class DeprecatedVector3Add : Add<Vector3>
    {
        protected override Vector3 defaultB => Vector3.zero;

        public override Vector3 Operation(Vector3 a, Vector3 b)
        {
            return a + b;
        }
    }
}
