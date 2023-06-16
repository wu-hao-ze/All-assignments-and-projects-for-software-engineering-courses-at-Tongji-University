using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the submit button is pressed.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(24)]
    public sealed class OnSubmit : GenericGuiEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnSubmitMessageListener);
        protected override string hookName => EventHooks.OnSubmit;
    }
}
