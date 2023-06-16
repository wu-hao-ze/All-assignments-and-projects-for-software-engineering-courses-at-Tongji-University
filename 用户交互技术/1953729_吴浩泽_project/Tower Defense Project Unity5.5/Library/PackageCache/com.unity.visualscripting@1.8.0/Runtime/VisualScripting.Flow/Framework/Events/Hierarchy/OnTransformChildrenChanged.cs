using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the list of children of the transform of the game object has changed.
    /// </summary>
    [UnitCategory("Events/Hierarchy")]
    public sealed class OnTransformChildrenChanged : GameObjectEventUnit<EmptyEventArgs>
    {
        public override Type MessageListenerType => typeof(UnityOnTransformChildrenChangedMessageListener);
        protected override string hookName => EventHooks.OnTransformChildrenChanged;
    }
}
