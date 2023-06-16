using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the sum of two or more 2D vectors.
    /// </summary>
    [UnitCategory("Math/Vector 2")]
    [UnitTitle("Add")]
    public sealed class Vector2Sum : Sum<Vector2>, IDefaultValue<Vector2>
    {
        [DoNotSerialize]
        public Vector2 defaultValue => Vector2.zero;

        public override Vector2 Operation(Vector2 a, Vector2 b)
        {
            return a + b;
        }

        public override Vector2 Operation(IEnumerable<Vector2> values)
        {
            var sum = Vector2.zero;

            foreach (var value in values)
            {
                sum += value;
            }

            return sum;
        }
    }
}
