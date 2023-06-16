using UnityEngine.EventSystems;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnBeginDragMessageListener : MessageListener, IBeginDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnBeginDrag, gameObject, eventData);
        }
    }
}
