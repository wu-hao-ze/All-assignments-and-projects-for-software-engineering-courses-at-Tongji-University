using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the mouse is released over the same GUI element or collider as it was pressed.
    /// </summary>
    [UnitCategory("Events/Input")]
    public sealed class OnMouseUpAsButton : GameObjectEventUnit<EmptyEventArgs>, IMouseEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnMouseUpAsButtonMessageListener);
        protected override string hookName => EventHooks.OnMouseUpAsButton;
    }
}
