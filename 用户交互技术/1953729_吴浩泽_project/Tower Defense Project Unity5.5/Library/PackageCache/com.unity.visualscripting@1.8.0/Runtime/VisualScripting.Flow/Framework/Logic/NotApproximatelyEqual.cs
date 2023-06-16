using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two numbers to determine if they are not approximately equal (disregarding floating point precision errors).
    /// </summary>
    [UnitCategory("Logic")]
    [UnitShortTitle("Not Equal")]
    [UnitSubtitle("(Approximately)")]
    [UnitOrder(8)]
    [Obsolete("Use the Not Equal node with Numeric enabled instead.")]
    public sealed class NotApproximatelyEqual : Unit
    {
        /// <summary>
        /// The first number.
        /// </summary>
        [DoNotSerialize]
        public ValueInput a { get; private set; }

        /// <summary>
        /// The second number.
        /// </summary>
        [DoNotSerialize]
        public ValueInput b { get; private set; }

        /// <summary>
        /// Whether A is not approximately equal to B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2249 B")]
        public ValueOutput notEqual { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<float>(nameof(a));
            b = ValueInput<float>(nameof(b), 0);
            notEqual = ValueOutput(nameof(notEqual), Comparison).Predictable();

            Requirement(a, notEqual);
            Requirement(b, notEqual);
        }

        public bool Comparison(Flow flow)
        {
            return !Mathf.Approximately(flow.GetValue<float>(a), flow.GetValue<float>(b));
        }
    }
}
