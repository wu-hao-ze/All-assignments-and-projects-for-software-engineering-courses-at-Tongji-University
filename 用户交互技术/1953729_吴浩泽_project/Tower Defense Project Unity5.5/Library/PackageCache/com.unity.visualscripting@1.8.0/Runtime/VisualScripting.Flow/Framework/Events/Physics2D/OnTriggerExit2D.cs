using System;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    /// <summary>
    /// Called when a collider exits the trigger.
    /// </summary>
    public sealed class OnTriggerExit2D : TriggerEvent2DUnit
    {
        public override Type MessageListenerType => typeof(UnityOnTriggerExit2DMessageListener);
        protected override string hookName => EventHooks.OnTriggerExit2D;
    }
#endif
}
