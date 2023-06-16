using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two numbers to determine if they are approximately equal (disregarding floating point precision errors).
    /// </summary>
    [UnitCategory("Logic")]
    [UnitShortTitle("Equal")]
    [UnitSubtitle("(Approximately)")]
    [UnitOrder(7)]
    [Obsolete("Use the Equal node with Numeric enabled instead.")]
    public sealed class ApproximatelyEqual : Unit
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
        /// Whether A is approximately equal to B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2248 B")]
        public ValueOutput equal { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<float>(nameof(a));
            b = ValueInput<float>(nameof(b), 0);
            equal = ValueOutput(nameof(equal), Comparison).Predictable();

            Requirement(a, equal);
            Requirement(b, equal);
        }

        public bool Comparison(Flow flow)
        {
            return Mathf.Approximately(flow.GetValue<float>(a), flow.GetValue<float>(b));
        }
    }
}
