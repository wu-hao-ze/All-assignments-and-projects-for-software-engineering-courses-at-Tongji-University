using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnControllerColliderHitMessageListener : MessageListener
    {
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            EventBus.Trigger(EventHooks.OnControllerColliderHit, gameObject, hit);
        }
    }
#endif
}
