using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IEventMachine : IMachine
    {
#if MODULE_ANIMATION_EXISTS
        void TriggerAnimationEvent(AnimationEvent animationEvent);
#endif

        void TriggerUnityEvent(string name);
    }
}
