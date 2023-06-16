using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    [SerializationVersion("A")]
    [SpecialUnit]
    public abstract class EventUnit<TArgs> : Unit, IEventUnit, IGraphElementWithData, IGraphEventHandler<TArgs>
    {
        public class Data : IGraphElementData
        {
            public EventHook hook;

            public Delegate handler;

            public bool isListening;

            public HashSet<Flow> activeCoroutines = new HashSet<Flow>();
        }

        public virtual IGraphElementData CreateData()
        {
            return new Data();
        }

        /// <summary>
        /// Run this event in a coroutine, enabling asynchronous flow like wait nodes.
        /// </summary>
        [Serialize]
        [Inspectable]
        [InspectorExpandTooltip]
        public bool coroutine { get; set; } = false;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput trigger { get; private set; }

        [DoNotSerialize]
        protected abstract bool register { get; }

        protected override void Definition()
        {
            isControlRoot = true;

            trigger = ControlOutput(nameof(trigger));
        }

        public virtual EventHook GetHook(GraphReference reference)
        {
            throw new InvalidImplementationException($"Missing event hook for '{this}'.");
        }

        public virtual void StartListening(GraphStack stack)
        {
            var data = stack.GetElementData<Data>(this);

            if (data.isListening)
            {
                return;
            }

            if (register)
            {
                var reference = stack.ToReference();
                var hook = GetHook(reference);
                Action<TArgs> handler = args => Trigger(reference, args);
                EventBus.Register(hook, handler);

                data.hook = hook;
                data.handler = handler;
            }

            data.isListening = true;
        }

        public virtual void StopListening(GraphStack stack)
        {
            var data = stack.GetElementData<Data>(this);

            if (!data.isListening)
            {
                return;
            }

            // The coroutine's flow will dispose at the next frame, letting us
            // keep the current flow for clean up operations if needed
            foreach (var activeCoroutine in data.activeCoroutines)
            {
                activeCoroutine.StopCoroutine(false);
            }

            if (register)
            {
                EventBus.Unregister(data.hook, data.handler);
                data.handler = null;
            }

            data.isListening = false;
        }

        public override void Uninstantiate(GraphReference instance)
        {
            // Here, we're relying on the fact that OnDestroy calls Uninstantiate.
            // We need to force-dispose any remaining coroutine to avoid
            // memory leaks, because OnDestroy on the runner will not keep
            // executing MoveNext() until our soft-destroy call at the end of Flow.Coroutine
            // or even dispose the coroutine's enumerator (!).
            var data = instance.GetElementData<Data>(this);
            var coroutines = data.activeCoroutines.ToHashSetPooled();

#if UNITY_EDITOR
            new FrameDelayedCallback(() => StopAllCoroutines(coroutines), 1);
#else
            StopAllCoroutines(coroutines);
#endif

            base.Uninstantiate(instance);
        }

        static void StopAllCoroutines(HashSet<Flow> activeCoroutines)
        {
            // The coroutine's flow will dispose instantly, thus modifying
            // the activeCoroutines registry while we enumerate over it
            foreach (var activeCoroutine in activeCoroutines)
            {
                activeCoroutine.StopCoroutineImmediate();
            }
            activeCoroutines.Free();
        }

        public bool IsListening(GraphPointer pointer)
        {
            if (!pointer.hasData)
            {
                return false;
            }

            return pointer.GetElementData<Data>(this).isListening;
        }

        public void Trigger(GraphReference reference, TArgs args)
        {
            var flow = Flow.New(reference);

            if (!ShouldTrigger(flow, args))
            {
                flow.Dispose();
                return;
            }

            AssignArguments(flow, args);

            Run(flow);
        }

        protected virtual bool ShouldTrigger(Flow flow, TArgs args)
        {
            return true;
        }

        protected virtual void AssignArguments(Flow flow, TArgs args)
        {
        }

        private void Run(Flow flow)
        {
            if (flow.enableDebug)
            {
                var editorData = flow.stack.GetElementDebugData<IUnitDebugData>(this);

                editorData.lastInvokeFrame = EditorTimeBinding.frame;
                editorData.lastInvokeTime = EditorTimeBinding.time;
            }

            if (coroutine)
            {
                flow.StartCoroutine(trigger, flow.stack.GetElementData<Data>(this).activeCoroutines);
            }
            else
            {
                flow.Run(trigger);
            }
        }

        protected static bool CompareNames(Flow flow, ValueInput namePort, string calledName)
        {
            Ensure.That(nameof(calledName)).IsNotNull(calledName);

            return calledName.Trim().Equals(flow.GetValue<string>(namePort)?.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
