using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the parent property of the transform of the game object has changed.
    /// </summary>
    [UnitCategory("Events/Hierarchy")]
    public sealed class OnTransformParentChanged : GameObjectEventUnit<EmptyEventArgs>
    {
        public override Type MessageListenerType => typeof(UnityOnTransformParentChangedMessageListener);
        protected override string hookName => EventHooks.OnTransformParentChanged;
    }
}
