using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnTriggerExit2DMessageListener : MessageListener
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            EventBus.Trigger(EventHooks.OnTriggerExit2D, gameObject, other);
        }
    }
#endif
}
