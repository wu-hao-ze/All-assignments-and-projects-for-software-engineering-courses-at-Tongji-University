using UnityEngine;

namespace Unity.VisualScripting
{
#if MODULE_PHYSICS_2D_EXISTS
    [AddComponentMenu("")]
    public sealed class UnityOnJointBreak2DMessageListener : MessageListener
    {
        private void OnJointBreak2D(Joint2D brokenJoint)
        {
            EventBus.Trigger(EventHooks.OnJointBreak2D, gameObject, brokenJoint);
        }
    }
#endif
}
