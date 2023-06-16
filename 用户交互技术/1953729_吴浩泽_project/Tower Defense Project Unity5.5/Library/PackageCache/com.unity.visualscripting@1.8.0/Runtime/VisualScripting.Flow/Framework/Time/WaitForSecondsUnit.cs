using System.Collections;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Delays flow by waiting a specified number of seconds.
    /// </summary>
    [UnitTitle("Wait For Seconds")]
    [UnitOrder(1)]
    public class WaitForSecondsUnit : WaitUnit
    {
        /// <summary>
        /// The number of seconds to await.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Delay")]
        public ValueInput seconds { get; private set; }

        /// <summary>
        /// Whether to ignore the time scale.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Unscaled")]
        public ValueInput unscaledTime { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            seconds = ValueInput(nameof(seconds), 0f);
            unscaledTime = ValueInput(nameof(unscaledTime), false);

            Requirement(seconds, enter);
            Requirement(unscaledTime, enter);
        }

        protected override IEnumerator Await(Flow flow)
        {
            var seconds = flow.GetValue<float>(this.seconds);

            if (flow.GetValue<bool>(unscaledTime))
            {
                yield return new WaitForSecondsRealtime(seconds);
            }
            else
            {
                yield return new WaitForSeconds(seconds);
            }

            yield return exit;
        }
    }
}
