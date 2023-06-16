using System;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_EXISTS
    /// <summary>
    /// Called when this collider / rigidbody has begun touching another rigidbody / collider.
    /// </summary>
    public sealed class OnCollisionEnter : CollisionEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnCollisionEnterMessageListener);
        protected override string hookName => EventHooks.OnCollisionEnter;
    }
#endif
}
