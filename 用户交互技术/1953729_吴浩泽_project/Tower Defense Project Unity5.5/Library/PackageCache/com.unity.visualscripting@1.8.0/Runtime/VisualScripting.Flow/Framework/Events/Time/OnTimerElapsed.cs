using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when a specified number of seconds has elapsed.
    /// </summary>
    [UnitCategory("Events/Time")]
    [Obsolete("Use Wait For Seconds or Timer instead.")]
    public sealed class OnTimerElapsed : MachineEventUnit<EmptyEventArgs>
    {
        public new class Data : EventUnit<EmptyEventArgs>.Data
        {
            public float time;

            public bool triggered;
        }

        public override IGraphElementData CreateData()
        {
            return new Data();
        }

        protected override string hookName => EventHooks.Update;

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
        }

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);

            var data = stack.GetElementData<Data>(this);

            data.triggered = false;
            data.time = 0;
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (data.triggered)
            {
                return false;
            }

            var increment = flow.GetValue<bool>(unscaledTime) ? Time.unscaledDeltaTime : Time.deltaTime;
            var threshold = flow.GetValue<float>(seconds);

            data.time += increment;

            if (data.time >= threshold)
            {
                data.triggered = true;
                return true;
            }

            return false;
        }
    }
}
