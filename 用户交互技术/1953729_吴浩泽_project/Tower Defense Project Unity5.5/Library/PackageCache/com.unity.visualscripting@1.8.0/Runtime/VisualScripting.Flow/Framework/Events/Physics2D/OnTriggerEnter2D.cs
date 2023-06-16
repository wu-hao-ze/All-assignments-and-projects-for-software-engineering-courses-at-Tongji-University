using System;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    /// <summary>
    /// Called when a collider enters the trigger.
    /// </summary>
    public sealed class OnTriggerEnter2D : TriggerEvent2DUnit
    {
        public override Type MessageListenerType => typeof(UnityOnTriggerEnter2DMessageListener);
        protected override string hookName => EventHooks.OnTriggerEnter2D;
    }
#endif
}
