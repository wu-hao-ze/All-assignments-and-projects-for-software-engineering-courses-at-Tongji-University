using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the user has pressed the mouse button while over the GUI element or collider.
    /// </summary>
    [UnitCategory("Events/Input")]
    public sealed class OnMouseDown : GameObjectEventUnit<EmptyEventArgs>, IMouseEventUnit
    {
        protected override string hookName => EventHooks.OnMouseDown;
        public override Type MessageListenerType => typeof(UnityOnMouseDownMessageListener);
    }
}
