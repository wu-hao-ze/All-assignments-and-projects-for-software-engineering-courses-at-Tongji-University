using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called on the drag object when dragging is about to begin.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(OnDrag))]
    [UnitOrder(16)]
    public sealed class OnBeginDrag : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnBeginDragMessageListener);
        protected override string hookName => EventHooks.OnBeginDrag;
    }
}
