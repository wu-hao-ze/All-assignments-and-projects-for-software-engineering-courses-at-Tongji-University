using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnCollisionStay2DMessageListener : MessageListener
    {
        private void OnCollisionStay2D(Collision2D collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionStay2D, gameObject, collision);
        }
    }
#endif
}
