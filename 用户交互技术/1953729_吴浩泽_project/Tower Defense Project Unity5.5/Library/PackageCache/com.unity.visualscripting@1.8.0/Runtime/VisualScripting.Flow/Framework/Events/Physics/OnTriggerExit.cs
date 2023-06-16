using System;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_EXISTS
    /// <summary>
    /// Called when a collider exits the trigger.
    /// </summary>
    public sealed class OnTriggerExit : TriggerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnTriggerExitMessageListener);
        protected override string hookName => EventHooks.OnTriggerExit;
    }
#endif
}
