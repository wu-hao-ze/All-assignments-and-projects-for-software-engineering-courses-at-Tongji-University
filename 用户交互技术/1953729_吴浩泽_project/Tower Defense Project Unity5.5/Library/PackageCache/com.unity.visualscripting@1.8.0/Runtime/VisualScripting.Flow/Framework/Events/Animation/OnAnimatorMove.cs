using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called at each frame after the state machines and the animations have been evaluated, but before On Animator IK.
    /// This callback can be used for processing animation movements for modifying root motion.
    /// </summary>
    [UnitCategory("Events/Animation")]
    public sealed class OnAnimatorMove : GameObjectEventUnit<EmptyEventArgs>
    {
        public override Type MessageListenerType => typeof(AnimatorMessageListener);
        protected override string hookName => EventHooks.OnAnimatorMove;
    }
}
