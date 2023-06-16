using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the pointer enters the GUI element.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(14)]
    public sealed class OnPointerEnter : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnPointerEnterMessageListener);
        protected override string hookName => EventHooks.OnPointerEnter;
    }
}
