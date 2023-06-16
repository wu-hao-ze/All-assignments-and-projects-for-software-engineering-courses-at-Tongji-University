using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnCollisionEnterMessageListener : MessageListener
    {
        private void OnCollisionEnter(Collision collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionEnter, gameObject, collision);
        }
    }
#endif
}
