using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Called on the drag object when a drag finishes.
    /// </summary>
    [UnitCategory("Events/GUI")]
    [TypeIcon(typeof(OnDrag))]
    [UnitOrder(18)]
    public sealed class OnEndDrag : PointerEventUnit
    {
        public override Type MessageListenerType => typeof(UnityOnEndDragMessageListener);
        protected override string hookName => EventHooks.OnEndDrag;
    }
}
