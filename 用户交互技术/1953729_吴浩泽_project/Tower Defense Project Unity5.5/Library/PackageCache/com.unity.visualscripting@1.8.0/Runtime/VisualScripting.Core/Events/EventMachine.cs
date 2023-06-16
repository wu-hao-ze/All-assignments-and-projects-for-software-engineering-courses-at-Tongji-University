using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class EventMachine<TGraph, TMacro> : Machine<TGraph, TMacro>, IEventMachine
        where TGraph : class, IGraph, new()
        where TMacro : Macro<TGraph>, new()
    {
        protected void TriggerEvent(string name)
        {
            if (hasGraph)
            {
                TriggerRegisteredEvent(new EventHook(name, this), new EmptyEventArgs());
            }
        }

        protected void TriggerEvent<TArgs>(string name, TArgs args)
        {
            if (hasGraph)
            {
                TriggerRegisteredEvent(new EventHook(name, this), args);
            }
        }

        protected void TriggerUnregisteredEvent(string name)
        {
            if (hasGraph)
            {
                TriggerUnregisteredEvent(name, new EmptyEventArgs());
            }
        }

        protected virtual void TriggerRegisteredEvent<TArgs>(EventHook hook, TArgs args)
        {
            EventBus.Trigger(hook, args);
        }

        protected virtual void TriggerUnregisteredEvent<TArgs>(EventHook hook, TArgs args)
        {
            using (var stack = reference.ToStackPooled())
            {
                stack.TriggerEventHandler(_hook => _hook == hook, args, parent => true, true);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            GlobalMessageListener.Require();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            TriggerEvent(EventHooks.OnEnable);
        }

        protected virtual void Start()
        {
            TriggerEvent(EventHooks.Start);
        }

        protected override void OnInstantiateWhileEnabled()
        {
            base.OnInstantiateWhileEnabled();

            TriggerEvent(EventHooks.OnEnable);
        }

        protected virtual void Update()
        {
            TriggerEvent(EventHooks.Update);
        }

        protected virtual void FixedUpdate()
        {
            TriggerEvent(EventHooks.FixedUpdate);
        }

        protected virtual void LateUpdate()
        {
            TriggerEvent(EventHooks.LateUpdate);
        }

        protected override void OnUninstantiateWhileEnabled()
        {
            TriggerEvent(EventHooks.OnDisable);

            base.OnUninstantiateWhileEnabled();
        }

        protected override void OnDisable()
        {
            TriggerEvent(EventHooks.OnDisable);

            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            try
            {
                TriggerEvent(EventHooks.OnDestroy);
            }
            finally
            {
                base.OnDestroy();
            }
        }

#if MODULE_ANIMATION_EXISTS
        public override void TriggerAnimationEvent(AnimationEvent animationEvent)
        {
            TriggerEvent(EventHooks.AnimationEvent, animationEvent);
        }
#endif

        public override void TriggerUnityEvent(string name)
        {
            TriggerEvent(EventHooks.UnityEvent, name);
        }

        protected virtual void OnDrawGizmos()
        {
            TriggerUnregisteredEvent(EventHooks.OnDrawGizmos);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            TriggerUnregisteredEvent(EventHooks.OnDrawGizmosSelected);
        }
    }
}
