using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the pointer presses the GUI element.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(12)]
    public sealed class OnPointerDown : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnPointerDownMessageListener);
        protected override string hookName => EventHooks.OnPointerDown;
    }
}
