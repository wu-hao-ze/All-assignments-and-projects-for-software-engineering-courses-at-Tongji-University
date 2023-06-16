using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when the pointer deselects the GUI element.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(23)]
    public sealed class OnDeselect : GenericGuiEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnDeselectMessageListener);
        protected override string hookName => EventHooks.OnDeselect;
    }
}
