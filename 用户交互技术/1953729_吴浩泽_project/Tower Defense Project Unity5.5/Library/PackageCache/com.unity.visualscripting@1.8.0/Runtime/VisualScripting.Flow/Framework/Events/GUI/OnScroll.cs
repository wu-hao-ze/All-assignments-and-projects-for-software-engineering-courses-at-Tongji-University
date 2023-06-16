using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called when a mouse wheel scrolls.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [UnitOrder(20)]
    public sealed class OnScroll : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnScrollMessageListener);
        protected override string hookName => EventHooks.OnScroll;
    }
}
