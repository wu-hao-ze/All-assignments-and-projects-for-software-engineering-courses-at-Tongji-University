using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnCollisionExit2DMessageListener : MessageListener
    {
        private void OnCollisionExit2D(Collision2D collision)
        {
            EventBus.Trigger(EventHooks.OnCollisionExit2D, gameObject, collision);
        }
    }
#endif
}
