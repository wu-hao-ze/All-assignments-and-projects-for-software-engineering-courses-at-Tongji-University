using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnTriggerStay2DMessageListener : MessageListener
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            EventBus.Trigger(EventHooks.OnTriggerStay2D, gameObject, other);
        }
    }
#endif
}
