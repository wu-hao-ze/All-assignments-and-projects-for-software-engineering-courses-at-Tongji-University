using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Runs a cooldown timer to throttle flow and outputs remaining measurements.
    /// </summary>
    [UnitCategory("Time")]
    [TypeIcon(typeof(Timer))]
    [UnitOrder(8)]
    public sealed class Cooldown : Unit, IGraphElementWithData, IGraphEventListener
    {
        public sealed class Data : IGraphElementData
        {
            public float remaining;

            public float duration;

            public bool unscaled;

            public bool isReady => remaining <= 0;

            public Delegate update;

            public bool isListening;
        }

        /// <summary>
        /// The moment at which to try using the cooldown.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// Trigger to force reset the cooldown.
        /// </summary>
        [DoNotSerialize]
        public ControlInput reset { get; private set; }

        /// <summary>
        /// The total duration of the cooldown.
        /// </summary>
        [DoNotSerialize]
        public ValueInput duration { get; private set; }

        /// <summary>
        /// Whether to ignore the time scale.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Unscaled")]
        public ValueInput unscaledTime { get; private set; }

        /// <summary>
        /// Called upon entry when the cooldown is ready.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Ready")]
        public ControlOutput exitReady { get; private set; }

        /// <summary>
        /// Called upon entry when the cooldown is not yet ready.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Not Ready")]
        public ControlOutput exitNotReady { get; private set; }

        /// <summary>
        /// Called each frame while the cooldown timer is active.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput tick { get; private set; }

        /// <summary>
        /// Called when the cooldown timer reaches zero.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Completed")]
        public ControlOutput becameReady { get; private set; }

        /// <summary>
        /// The number of seconds remaining until the cooldown is ready.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Remaining")]
        public ValueOutput remainingSeconds { get; private set; }

        /// <summary>
        /// The proportion of the duration remaining until the cooldown is ready (0-1).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Remaining %")]
        public ValueOutput remainingRatio { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            reset = ControlInput(nameof(reset), Reset);

            duration = ValueInput(nameof(duration), 1f);
            unscaledTime = ValueInput(nameof(unscaledTime), false);

            exitReady = ControlOutput(nameof(exitReady));
            exitNotReady = ControlOutput(nameof(exitNotReady));
            tick = ControlOutput(nameof(tick));
            becameReady = ControlOutput(nameof(becameReady));

            remainingSeconds = ValueOutput<float>(nameof(remainingSeconds));
            remainingRatio = ValueOutput<float>(nameof(remainingRatio));

            Requirement(duration, enter);
            Requirement(unscaledTime, enter);
            Succession(enter, exitReady);
            Succession(enter, exitNotReady);
            Succession(enter, tick);
            Succession(enter, becameReady);
            Assignment(enter, remainingSeconds);
            Assignment(enter, remainingRatio);
        }

        public IGraphElementData CreateData()
        {
            return new Data();
        }

        public void StartListening(GraphStack stack)
        {
            var data = stack.GetElementData<Data>(this);

            if (data.isListening)
            {
                return;
            }

            var reference = stack.ToReference();
            var hook = new EventHook(EventHooks.Update, stack.machine);
            Action<EmptyEventArgs> update = args => TriggerUpdate(reference);
            EventBus.Register(hook, update);
            data.update = update;
            data.isListening = true;
        }

        public void StopListening(GraphStack stack)
        {
            var data = stack.GetElementData<Data>(this);

            if (!data.isListening)
            {
                return;
            }

            var hook = new EventHook(EventHooks.Update, stack.machine);
            EventBus.Unregister(hook, data.update);
            data.update = null;
            data.isListening = false;
        }

        public bool IsListening(GraphPointer pointer)
        {
            return pointer.GetElementData<Data>(this).isListening;
        }

        private void TriggerUpdate(GraphReference reference)
        {
            using (var flow = Flow.New(reference))
            {
                Update(flow);
            }
        }

        private ControlOutput Enter(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (data.isReady)
            {
                return Reset(flow);
            }
            else
            {
                return exitNotReady;
            }
        }

        private ControlOutput Reset(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            data.duration = flow.GetValue<float>(duration);
            data.remaining = data.duration;
            data.unscaled = flow.GetValue<bool>(unscaledTime);

            return exitReady;
        }

        private void AssignMetrics(Flow flow, Data data)
        {
            flow.SetValue(remainingSeconds, data.remaining);
            flow.SetValue(remainingRatio, Mathf.Clamp01(data.remaining / data.duration));
        }

        public void Update(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (data.isReady)
            {
                return;
            }

            data.remaining -= data.unscaled ? Time.unscaledDeltaTime : Time.deltaTime;

            data.remaining = Mathf.Max(0f, data.remaining);

            AssignMetrics(flow, data);

            var stack = flow.PreserveStack();

            flow.Invoke(tick);

            if (data.isReady)
            {
                flow.RestoreStack(stack);

                flow.Invoke(becameReady);
            }

            flow.DisposePreservedStack(stack);
        }
    }
}
