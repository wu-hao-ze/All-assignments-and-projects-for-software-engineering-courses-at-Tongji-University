using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the cancel button is pressed.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(25)]
    public sealed class OnCancel : GenericGuiEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnCancelMessageListener);
        protected override string hookName => EventHooks.OnCancel;
    }
}
