using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the mouse enters the GUI element or collider.
    /// </summary>
    [UnitCategory("Events/Input")]
    public sealed class OnMouseEnter : GameObjectEventUnit<EmptyEventArgs>, IMouseEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnMouseEnterMessageListener);
        protected override string hookName => EventHooks.OnMouseEnter;
    }
}
