using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Runs a timer and outputs elapsed and remaining measurements.
    /// </summary>
    [UnitCategory("Time")]
    [UnitOrder(7)]
    public sealed class Timer : Unit, IGraphElementWithData, IGraphEventListener
    {
        public sealed class Data : IGraphElementData
        {
            public float elapsed;

            public float duration;

            public bool active;

            public bool paused;

            public bool unscaled;

            public Delegate update;

            public bool isListening;
        }

        /// <summary>
        /// The moment at which to start the timer.
        /// If the timer is already started, this will reset it.
        /// If the timer is paused, this will resume it.
        /// </summary>
        [DoNotSerialize]
        public ControlInput start { get; private set; }

        /// <summary>
        /// Trigger to pause the timer.
        /// </summary>
        [DoNotSerialize]
        public ControlInput pause { get; private set; }

        /// <summary>
        /// Trigger to resume the timer.
        /// </summary>
        [DoNotSerialize]
        public ControlInput resume { get; private set; }

        /// <summary>
        /// Trigger to toggle the timer.
        /// If it is idle, it will start.
        /// If it is active, it will pause.
        /// If it is paused, it will resume.
        /// </summary>
        [DoNotSerialize]
        public ControlInput toggle { get; private set; }

        /// <summary>
        /// The total duration of the timer.
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
        /// Called when the timer is started.co
        /// </summary>
        [DoNotSerialize]
        public ControlOutput started { get; private set; }

        /// <summary>
        /// Called each frame while the timer is active.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput tick { get; private set; }

        /// <summary>
        /// Called when the timer completes.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput completed { get; private set; }

        /// <summary>
        /// The number of seconds elapsed since the timer started.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Elapsed")]
        public ValueOutput elapsedSeconds { get; private set; }

        /// <summary>
        /// The proportion of the duration that has elapsed (0-1).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Elapsed %")]
        public ValueOutput elapsedRatio { get; private set; }

        /// <summary>
        /// The number of seconds remaining until the timer is elapsed.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Remaining")]
        public ValueOutput remainingSeconds { get; private set; }

        /// <summary>
        /// The proportion of the duration remaining until the timer is elapsed (0-1).
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Remaining %")]
        public ValueOutput remainingRatio { get; private set; }

        protected override void Definition()
        {
            isControlRoot = true;

            start = ControlInput(nameof(start), Start);
            pause = ControlInput(nameof(pause), Pause);
            resume = ControlInput(nameof(resume), Resume);
            toggle = ControlInput(nameof(toggle), Toggle);

            duration = ValueInput(nameof(duration), 1f);
            unscaledTime = ValueInput(nameof(unscaledTime), false);

            started = ControlOutput(nameof(started));
            tick = ControlOutput(nameof(tick));
            completed = ControlOutput(nameof(completed));

            elapsedSeconds = ValueOutput<float>(nameof(elapsedSeconds));
            elapsedRatio = ValueOutput<float>(nameof(elapsedRatio));

            remainingSeconds = ValueOutput<float>(nameof(remainingSeconds));
            remainingRatio = ValueOutput<float>(nameof(remainingRatio));
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

        private ControlOutput Start(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            data.elapsed = 0;
            data.duration = flow.GetValue<float>(duration);
            data.active = true;
            data.paused = false;
            data.unscaled = flow.GetValue<bool>(unscaledTime);

            AssignMetrics(flow, data);

            return started;
        }

        private ControlOutput Pause(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            data.paused = true;

            return null;
        }

        private ControlOutput Resume(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            data.paused = false;

            return null;
        }

        private ControlOutput Toggle(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (!data.active)
            {
                return Start(flow);
            }
            else
            {
                data.paused = !data.paused;

                return null;
            }
        }

        private void AssignMetrics(Flow flow, Data data)
        {
            flow.SetValue(elapsedSeconds, data.elapsed);
            flow.SetValue(elapsedRatio, Mathf.Clamp01(data.elapsed / data.duration));

            flow.SetValue(remainingSeconds, Mathf.Max(0, data.duration - data.elapsed));
            flow.SetValue(remainingRatio, Mathf.Clamp01((data.duration - data.elapsed) / data.duration));
        }

        public void Update(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            if (!data.active || data.paused)
            {
                return;
            }

            data.elapsed += data.unscaled ? Time.unscaledDeltaTime : Time.deltaTime;

            data.elapsed = Mathf.Min(data.elapsed, data.duration);

            AssignMetrics(flow, data);

            var stack = flow.PreserveStack();

            flow.Invoke(tick);

            if (data.elapsed >= data.duration)
            {
                data.active = false;

                flow.RestoreStack(stack);

                flow.Invoke(completed);
            }

            flow.DisposePreservedStack(stack);
        }
    }
}
