using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the pointer exits the GUI element.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(15)]
    public sealed class OnPointerExit : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnPointerExitMessageListener);
        protected override string hookName => EventHooks.OnPointerExit;
    }
}
