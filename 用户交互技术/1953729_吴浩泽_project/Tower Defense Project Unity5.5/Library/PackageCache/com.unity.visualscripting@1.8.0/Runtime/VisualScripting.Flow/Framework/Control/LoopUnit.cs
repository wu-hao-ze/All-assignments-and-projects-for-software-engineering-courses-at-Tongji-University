using System.Collections;

namespace Unity.VisualScripting
{
    public abstract class LoopUnit : Unit
    {
        /// <summary>
        /// The entry point for the loop.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The action to execute after the loop has been completed or broken.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput exit { get; private set; }

        /// <summary>
        /// The action to execute at each loop.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput body { get; private set; }

        protected override void Definition()
        {
            enter = ControlInputCoroutine(nameof(enter), Loop, LoopCoroutine);
            exit = ControlOutput(nameof(exit));
            body = ControlOutput(nameof(body));

            Succession(enter, body);
            Succession(enter, exit);
        }

        protected abstract ControlOutput Loop(Flow flow);

        protected abstract IEnumerator LoopCoroutine(Flow flow);
    }
}
