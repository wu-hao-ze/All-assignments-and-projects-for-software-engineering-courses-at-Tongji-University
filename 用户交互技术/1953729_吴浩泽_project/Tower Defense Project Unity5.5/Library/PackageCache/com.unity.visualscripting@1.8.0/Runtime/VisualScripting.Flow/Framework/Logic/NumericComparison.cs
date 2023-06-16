using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two numeric inputs.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitTitle("Numeric Comparison")]
    [UnitSurtitle("Numeric")]
    [UnitShortTitle("Comparison")]
    [UnitOrder(99)]
    [Obsolete("Use the Comparison node with Numeric enabled instead.")]
    public sealed class NumericComparison : Unit
    {
        /// <summary>
        /// The first input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput a { get; private set; }

        /// <summary>
        /// The second input.
        /// </summary>
        [DoNotSerialize]
        public ValueInput b { get; private set; }

        /// <summary>
        /// Whether A is less than B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A < B")]
        public ValueOutput aLessThanB { get; private set; }

        /// <summary>
        /// Whether A is less than or equal to B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2264 B")]
        public ValueOutput aLessThanOrEqualToB { get; private set; }

        /// <summary>
        /// Whether A is equal to B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A = B")]
        public ValueOutput aEqualToB { get; private set; }

        /// <summary>
        /// Whether A is greater than or equal to B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2265 B")]
        public ValueOutput aGreaterThanOrEqualToB { get; private set; }

        /// <summary>
        /// Whether A is greater than B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A > B")]
        public ValueOutput aGreatherThanB { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<float>(nameof(a));
            b = ValueInput<float>(nameof(b), 0);

            aLessThanB = ValueOutput(nameof(aLessThanB), Less).Predictable();
            aLessThanOrEqualToB = ValueOutput(nameof(aLessThanOrEqualToB), LessOrEqual).Predictable();
            aEqualToB = ValueOutput(nameof(aEqualToB), Equal).Predictable();
            aGreaterThanOrEqualToB = ValueOutput(nameof(aGreaterThanOrEqualToB), GreaterOrEqual).Predictable();
            aGreatherThanB = ValueOutput(nameof(aGreatherThanB), Greater).Predictable();

            Requirement(a, aLessThanB);
            Requirement(b, aLessThanB);

            Requirement(a, aLessThanOrEqualToB);
            Requirement(b, aLessThanOrEqualToB);

            Requirement(a, aEqualToB);
            Requirement(b, aEqualToB);

            Requirement(a, aGreaterThanOrEqualToB);
            Requirement(b, aGreaterThanOrEqualToB);

            Requirement(a, aGreatherThanB);
            Requirement(b, aGreatherThanB);
        }

        private bool Less(Flow flow)
        {
            return flow.GetValue<float>(a) < flow.GetValue<float>(b);
        }

        private bool LessOrEqual(Flow flow)
        {
            var a = flow.GetValue<float>(this.a);
            var b = flow.GetValue<float>(this.b);
            return a < b || Mathf.Approximately(a, b);
        }

        private bool Equal(Flow flow)
        {
            return Mathf.Approximately(flow.GetValue<float>(a), flow.GetValue<float>(b));
        }

        private bool GreaterOrEqual(Flow flow)
        {
            var a = flow.GetValue<float>(this.a);
            var b = flow.GetValue<float>(this.b);
            return a > b || Mathf.Approximately(a, b);
        }

        private bool Greater(Flow flow)
        {
            return flow.GetValue<float>(a) < flow.GetValue<float>(b);
        }
    }
}
