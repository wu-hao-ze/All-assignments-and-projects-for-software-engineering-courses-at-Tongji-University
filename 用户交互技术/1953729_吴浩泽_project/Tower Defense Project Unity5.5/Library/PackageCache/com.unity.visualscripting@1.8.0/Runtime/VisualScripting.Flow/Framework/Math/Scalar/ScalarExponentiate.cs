using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the power of a base and exponent.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Exponentiate")]
    [UnitOrder(105)]
    public sealed class ScalarExponentiate : Unit
    {
        /// <summary>
        /// The base.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("x")]
        public ValueInput @base { get; private set; }

        /// <summary>
        /// The exponent.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("n")]
        public ValueInput exponent { get; private set; }

        /// <summary>
        /// The power of base elevated to exponent.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("x\u207f")]
        public ValueOutput power { get; private set; }

        protected override void Definition()
        {
            @base = ValueInput<float>(nameof(@base), 1);
            exponent = ValueInput<float>(nameof(exponent), 2);
            power = ValueOutput(nameof(power), Exponentiate);

            Requirement(@base, power);
            Requirement(exponent, power);
        }

        public float Exponentiate(Flow flow)
        {
            return Mathf.Pow(flow.GetValue<float>(@base), flow.GetValue<float>(exponent));
        }
    }
}
