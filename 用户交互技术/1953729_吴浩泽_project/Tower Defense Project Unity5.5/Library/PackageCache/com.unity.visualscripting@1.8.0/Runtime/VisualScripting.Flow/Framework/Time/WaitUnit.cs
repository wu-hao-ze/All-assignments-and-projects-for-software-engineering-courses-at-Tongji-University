using System.Collections;

namespace Unity.VisualScripting
{
    [UnitCategory("Time")]
    public abstract class WaitUnit : Unit
    {
        /// <summary>
        /// The moment at which to start the delay.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The action to execute after the delay has elapsed.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInputCoroutine(nameof(enter), Await);
            exit = ControlOutput(nameof(exit));
            Succession(enter, exit);
        }

        protected abstract IEnumerator Await(Flow flow);
    }
}
