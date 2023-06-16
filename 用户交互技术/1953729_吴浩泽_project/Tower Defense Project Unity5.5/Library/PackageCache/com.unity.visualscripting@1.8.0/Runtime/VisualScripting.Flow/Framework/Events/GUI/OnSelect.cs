using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the pointer selects the GUI element.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(22)]
    public sealed class OnSelect : GenericGuiEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnSelectMessageListener);
        protected override string hookName => EventHooks.OnSelect;
    }
}
