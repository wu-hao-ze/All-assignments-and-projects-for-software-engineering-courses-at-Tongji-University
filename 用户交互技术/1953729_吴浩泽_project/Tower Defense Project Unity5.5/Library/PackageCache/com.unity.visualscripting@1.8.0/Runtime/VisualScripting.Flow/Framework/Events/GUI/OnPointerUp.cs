using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the pointer releases the GUI element.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(13)]
    public sealed class OnPointerUp : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnPointerUpMessageListener);
        protected override string hookName => EventHooks.OnPointerUp;
    }
}
