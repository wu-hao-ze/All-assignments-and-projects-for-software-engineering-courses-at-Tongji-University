using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two inputs to determine if they are equal or not equal.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitTitle("Equality Comparison")]
    [UnitSurtitle("Equality")]
    [UnitShortTitle("Comparison")]
    [UnitOrder(4)]
    [Obsolete("Use the Comparison node instead.")]
    public sealed class EqualityComparison : Unit
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
        /// Whether A is equal to B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A = B")]
        public ValueOutput equal { get; private set; }

        /// <summary>
        /// Whether A is different than B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2260 B")]
        public ValueOutput notEqual { get; private set; }

        protected override void Definition()
        {
            a = ValueInput<object>(nameof(a)).AllowsNull();
            b = ValueInput<object>(nameof(b)).AllowsNull();
            equal = ValueOutput(nameof(equal), Equal).Predictable();
            notEqual = ValueOutput(nameof(notEqual), NotEqual).Predictable();

            Requirement(a, equal);
            Requirement(b, equal);

            Requirement(a, notEqual);
            Requirement(b, notEqual);
        }

        private bool Equal(Flow flow)
        {
            return OperatorUtility.Equal(flow.GetValue(a), flow.GetValue(b));
        }

        private bool NotEqual(Flow flow)
        {
            return OperatorUtility.NotEqual(flow.GetValue(a), flow.GetValue(b));
        }
    }
}
