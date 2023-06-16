using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the pointer clicks the GUI element.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(11)]
    public sealed class OnPointerClick : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnPointerClickMessageListener);
        protected override string hookName => EventHooks.OnPointerClick;
    }
}
