using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the root at the nth degree of a radicand.
    /// </summary>
    [UnitCategory("Math/Scalar")]
    [UnitTitle("Root")]
    [UnitOrder(106)]
    public sealed class ScalarRoot : Unit
    {
        /// <summary>
        /// The radicand.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("x")]
        public ValueInput radicand { get; private set; }

        /// <summary>
        /// The degree.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("n")]
        public ValueInput degree { get; private set; }

        /// <summary>
        /// The nth degree root of the radicand.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("\u207f\u221ax")]
        public ValueOutput root { get; private set; }

        protected override void Definition()
        {
            radicand = ValueInput<float>(nameof(radicand), 1);
            degree = ValueInput<float>(nameof(degree), 2);
            root = ValueOutput(nameof(root), Root);

            Requirement(radicand, root);
            Requirement(degree, root);
        }

        public float Root(Flow flow)
        {
            var degree = flow.GetValue<float>(this.degree);
            var radicand = flow.GetValue<float>(this.radicand);

            if (degree == 2)
            {
                return Mathf.Sqrt(radicand);
            }
            else
            {
                return Mathf.Pow(radicand, 1 / degree);
            }
        }
    }
}
