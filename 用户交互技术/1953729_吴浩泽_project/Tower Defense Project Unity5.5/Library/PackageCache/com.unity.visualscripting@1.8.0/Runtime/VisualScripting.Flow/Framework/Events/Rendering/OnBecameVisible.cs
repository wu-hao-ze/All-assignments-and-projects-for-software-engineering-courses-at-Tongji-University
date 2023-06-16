using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the renderer became visible by any camera.
    /// </summary>
    [UnitCategory("Events/Rendering")]
    public sealed class OnBecameVisible : GameObjectEventUnit<EmptyEventArgs>
    {
        public override Type MessageListenerType => typeof(UnityOnBecameVisibleMessageListener);
        protected override string hookName => EventHooks.OnBecameVisible;
    }
}
