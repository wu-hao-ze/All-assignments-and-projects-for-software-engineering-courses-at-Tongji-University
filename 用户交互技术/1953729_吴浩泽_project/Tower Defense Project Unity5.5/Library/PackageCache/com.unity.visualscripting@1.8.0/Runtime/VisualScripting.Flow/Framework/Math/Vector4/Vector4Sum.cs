using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the sum of two or more 4D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 4")]
    [UnitTitle("Add")]
    public sealed class Vector4Sum : Sum<Vector4>, IDefaultValue<Vector4>
    {
        [DoNotSerialize]
        public Vector4 defaultValue => Vector4.zero;

        public override Vector4 Operation(Vector4 a, Vector4 b)
        {
            return a + b;
        }

        public override Vector4 Operation(IEnumerable<Vector4> values)
        {
            var sum = Vector4.zero;

            foreach (var value in values)
            {
                sum += value;
            }

            return sum;
        }
    }
}
