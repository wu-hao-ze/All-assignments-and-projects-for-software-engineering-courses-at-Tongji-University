using System.Collections;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Delays flow by waiting until a condition becomes true.
    /// </summary>
    [UnitTitle("Wait Until")]
    [UnitShortTitle("Wait Until")]
    [UnitOrder(2)]
    public class WaitUntilUnit : WaitUnit
    {
        /// <summary>
        /// The condition to await.
        /// </summary>
        [DoNotSerialize]
        public ValueInput condition { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            condition = ValueInput<bool>(nameof(condition));
            Requirement(condition, enter);
        }

        protected override IEnumerator Await(Flow flow)
        {
            yield return new WaitUntil(() => flow.GetValue<bool>(condition));

            yield return exit;
        }
    }
}
