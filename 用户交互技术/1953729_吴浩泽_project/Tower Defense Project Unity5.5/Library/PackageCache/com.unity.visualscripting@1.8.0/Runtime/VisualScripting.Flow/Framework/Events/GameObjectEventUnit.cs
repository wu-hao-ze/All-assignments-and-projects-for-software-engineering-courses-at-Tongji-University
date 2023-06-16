using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class GameObjectEventUnit<TArgs> : EventUnit<TArgs>, IGameObjectEventUnit
    {
        protected sealed override bool register => true;

        public abstract Type MessageListenerType { get; }

        public new class Data : EventUnit<TArgs>.Data
        {
            public GameObject target;
        }

        public override IGraphElementData CreateData()
        {
            return new Data();
        }

        /// <summary>
        /// The game object that listens for the event.
        /// </summary>
        [DoNotSerialize]
        [NullMeansSelf]
        [PortLabel("Target")]
        [PortLabelHidden]
        public ValueInput target { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            target = ValueInput<GameObject>(nameof(target), null).NullMeansSelf();
        }

        public override EventHook GetHook(GraphReference reference)
        {
            if (!reference.hasData)
            {
                return hookName;
            }

            var data = reference.GetElementData<Data>(this);

            return new EventHook(hookName, data.target);
        }

        protected virtual string hookName => throw new InvalidImplementationException($"Missing event hook for '{this}'.");

        private void UpdateTarget(GraphStack stack)
        {
            var data = stack.GetElementData<Data>(this);

            var wasListening = data.isListening;

            var newTarget = Flow.FetchValue<GameObject>(target, stack.ToReference());

            if (newTarget != data.target)
            {
                if (wasListening)
                {
                    StopListening(stack);
                }

                data.target = newTarget;

                if (wasListening)
                {
                    StartListening(stack, false);
                }
            }
        }

        protected void StartListening(GraphStack stack, bool updateTarget)
        {
            if (updateTarget)
            {
                UpdateTarget(stack);
            }

            var data = stack.GetElementData<Data>(this);

            if (data.target == null)
            {
                return;
            }

            if (UnityThread.allowsAPI)
            {
                if (MessageListenerType != null) // can be null. CustomEvent doesn't need a message listener
                    MessageListener.AddTo(MessageListenerType, data.target);
            }

            base.StartListening(stack);
        }

        public override void StartListening(GraphStack stack)
        {
            StartListening(stack, true);
        }
    }
}
