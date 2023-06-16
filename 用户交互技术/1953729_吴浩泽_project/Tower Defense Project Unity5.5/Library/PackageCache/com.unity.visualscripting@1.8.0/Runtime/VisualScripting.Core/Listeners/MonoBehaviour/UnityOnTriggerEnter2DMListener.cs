using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnTriggerEnter2DMessageListener : MessageListener
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            EventBus.Trigger(EventHooks.OnTriggerEnter2D, gameObject, other);
        }
    }
#endif
}
