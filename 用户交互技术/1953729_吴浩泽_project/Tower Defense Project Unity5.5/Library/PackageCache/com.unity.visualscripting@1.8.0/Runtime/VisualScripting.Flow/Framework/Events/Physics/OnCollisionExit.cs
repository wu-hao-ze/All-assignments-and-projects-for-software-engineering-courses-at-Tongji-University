using System;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_EXISTS
    /// <summary>
    /// Called when this collider / rigidbody has stopped touching another rigidbody / collider.
    /// </summary>
    public sealed class OnCollisionExit : CollisionEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnCollisionExitMessageListener);
        protected override string hookName => EventHooks.OnCollisionExit;
    }
#endif
}
