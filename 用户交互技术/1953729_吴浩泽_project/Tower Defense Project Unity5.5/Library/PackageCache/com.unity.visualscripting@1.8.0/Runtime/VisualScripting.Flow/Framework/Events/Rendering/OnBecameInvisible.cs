using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the renderer is no longer visible by any camera.
    /// </summary>
    [UnitCategory("Events/Rendering")]
    public sealed class OnBecameInvisible : GameObjectEventUnit<EmptyEventArgs>
    {
        public override Type MessageListenerType => typeof(UnityOnBecameInvisibleMessageListener);
        protected override string hookName => EventHooks.OnBecameInvisible;
    }
}
