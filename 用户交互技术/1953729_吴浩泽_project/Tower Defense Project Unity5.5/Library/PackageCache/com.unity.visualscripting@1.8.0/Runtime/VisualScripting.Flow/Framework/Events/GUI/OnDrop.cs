using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called on a target that can accept a drop.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(OnDrag))]
    [UnitOrder(19)]
    public sealed class OnDrop : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnDropMessageListener);
        protected override string hookName => EventHooks.OnDrop;
    }
}
