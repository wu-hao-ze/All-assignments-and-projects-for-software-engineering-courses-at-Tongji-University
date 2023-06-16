using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the sum of two or more 3D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 3")]
    [UnitTitle("Add")]
    public sealed class Vector3Sum : Sum<Vector3>, IDefaultValue<Vector3>
    {
        [DoNotSerialize]
        public Vector3 defaultValue => Vector3.zero;

        public override Vector3 Operation(Vector3 a, Vector3 b)
        {
            return a + b;
        }

        public override Vector3 Operation(IEnumerable<Vector3> values)
        {
            var sum = Vector3.zero;

            foreach (var value in values)
            {
                sum += value;
            }

            return sum;
        }
    }
}
