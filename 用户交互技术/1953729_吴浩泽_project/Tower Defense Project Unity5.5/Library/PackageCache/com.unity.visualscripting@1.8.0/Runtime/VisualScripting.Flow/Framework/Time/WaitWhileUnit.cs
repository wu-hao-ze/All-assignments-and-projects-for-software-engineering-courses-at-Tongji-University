using System.Collections;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Delays flow by waiting while a condition is true.
    /// </summary>
    [UnitTitle("Wait While")]
    [UnitShortTitle("Wait While")]
    [UnitOrder(3)]
    public class WaitWhileUnit : WaitUnit
    {
        /// <summary>
        /// The condition to check.
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
            yield return new WaitWhile(() => flow.GetValue<bool>(condition));

            yield return exit;
        }
    }
}
