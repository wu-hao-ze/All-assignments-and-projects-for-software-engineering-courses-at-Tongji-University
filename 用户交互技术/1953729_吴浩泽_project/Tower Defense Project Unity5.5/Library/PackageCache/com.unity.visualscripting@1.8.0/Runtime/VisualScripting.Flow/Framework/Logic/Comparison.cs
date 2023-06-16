using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Compares two inputs.
    /// </summary>
    [UnitCategory("Logic")]
    [UnitTitle("Comparison")]
    [UnitShortTitle("Comparison")]
    [UnitOrder(99)]
    public sealed class Comparison : Unit
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
        /// Whether the compared inputs are numbers.
        /// </summary>
        [Serialize]
        [Inspectable]
        public bool numeric { get; set; } = true;

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
        /// Whether A is not equal to B.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("A \u2260 B")]
        public ValueOutput aNotEqualToB { get; private set; }

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
            if (numeric)
            {
                a = ValueInput<float>(nameof(a));
                b = ValueInput<float>(nameof(b), 0);

                aLessThanB = ValueOutput(nameof(aLessThanB), (flow) => NumericLess(flow.GetValue<float>(a), flow.GetValue<float>(b))).Predictable();
                aLessThanOrEqualToB = ValueOutput(nameof(aLessThanOrEqualToB), (flow) => NumericLessOrEqual(flow.GetValue<float>(a), flow.GetValue<float>(b))).Predictable();
                aEqualToB = ValueOutput(nameof(aEqualToB), (flow) => NumericEqual(flow.GetValue<float>(a), flow.GetValue<float>(b))).Predictable();
                aNotEqualToB = ValueOutput(nameof(aNotEqualToB), (flow) => NumericNotEqual(flow.GetValue<float>(a), flow.GetValue<float>(b))).Predictable();
                aGreaterThanOrEqualToB = ValueOutput(nameof(aGreaterThanOrEqualToB), (flow) => NumericGreaterOrEqual(flow.GetValue<float>(a), flow.GetValue<float>(b))).Predictable();
                aGreatherThanB = ValueOutput(nameof(aGreatherThanB), (flow) => NumericGreater(flow.GetValue<float>(a), flow.GetValue<float>(b))).Predictable();
            }
            else
            {
                a = ValueInput<object>(nameof(a)).AllowsNull();
                b = ValueInput<object>(nameof(b)).AllowsNull();

                aLessThanB = ValueOutput(nameof(aLessThanB), (flow) => GenericLess(flow.GetValue(a), flow.GetValue(b)));
                aLessThanOrEqualToB = ValueOutput(nameof(aLessThanOrEqualToB), (flow) => GenericLessOrEqual(flow.GetValue(a), flow.GetValue(b)));
                aEqualToB = ValueOutput(nameof(aEqualToB), (flow) => GenericEqual(flow.GetValue(a), flow.GetValue(b)));
                aNotEqualToB = ValueOutput(nameof(aNotEqualToB), (flow) => GenericNotEqual(flow.GetValue(a), flow.GetValue(b)));
                aGreaterThanOrEqualToB = ValueOutput(nameof(aGreaterThanOrEqualToB), (flow) => GenericGreaterOrEqual(flow.GetValue(a), flow.GetValue(b)));
                aGreatherThanB = ValueOutput(nameof(aGreatherThanB), (flow) => GenericGreater(flow.GetValue(a), flow.GetValue(b)));
            }

            Requirement(a, aLessThanB);
            Requirement(b, aLessThanB);

            Requirement(a, aLessThanOrEqualToB);
            Requirement(b, aLessThanOrEqualToB);

            Requirement(a, aEqualToB);
            Requirement(b, aEqualToB);

            Requirement(a, aNotEqualToB);
            Requirement(b, aNotEqualToB);

            Requirement(a, aGreaterThanOrEqualToB);
            Requirement(b, aGreaterThanOrEqualToB);

            Requirement(a, aGreatherThanB);
            Requirement(b, aGreatherThanB);
        }

        private bool NumericLess(float a, float b)
        {
            return a < b;
        }

        private bool NumericLessOrEqual(float a, float b)
        {
            return a < b || Mathf.Approximately(a, b);
        }

        private bool NumericEqual(float a, float b)
        {
            return Mathf.Approximately(a, b);
        }

        private bool NumericNotEqual(float a, float b)
        {
            return !Mathf.Approximately(a, b);
        }

        private bool NumericGreaterOrEqual(float a, float b)
        {
            return a > b || Mathf.Approximately(a, b);
        }

        private bool NumericGreater(float a, float b)
        {
            return a > b;
        }

        private bool GenericLess(object a, object b)
        {
            return OperatorUtility.LessThan(a, b);
        }

        private bool GenericLessOrEqual(object a, object b)
        {
            return OperatorUtility.LessThanOrEqual(a, b);
        }

        private bool GenericEqual(object a, object b)
        {
            return OperatorUtility.Equal(a, b);
        }

        private bool GenericNotEqual(object a, object b)
        {
            return OperatorUtility.NotEqual(a, b);
        }

        private bool GenericGreaterOrEqual(object a, object b)
        {
            return OperatorUtility.GreaterThanOrEqual(a, b);
        }

        private bool GenericGreater(object a, object b)
        {
            return OperatorUtility.GreaterThan(a, b);
        }
    }
}
