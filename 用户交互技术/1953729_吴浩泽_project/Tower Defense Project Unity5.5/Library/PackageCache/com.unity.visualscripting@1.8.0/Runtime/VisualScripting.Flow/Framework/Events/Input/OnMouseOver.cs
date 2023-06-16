using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called every frame while the mouse is over the GUI element or collider.
    /// </summary>
    [UnitCategory("Events/Input")]
    public sealed class OnMouseOver : GameObjectEventUnit<EmptyEventArgs>, IMouseEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnMouseOverMessageListener);
        protected override string hookName => EventHooks.OnMouseOver;
    }
}
