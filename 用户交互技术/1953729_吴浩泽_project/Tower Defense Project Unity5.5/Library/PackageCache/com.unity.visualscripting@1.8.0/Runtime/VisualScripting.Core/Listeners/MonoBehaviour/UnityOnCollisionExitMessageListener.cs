using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnCollisionExitMessageListener : MessageListener
    {
        private void OnCollisionExit(Collision collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionExit, gameObject, collision);
        }
    }
#endif
}
