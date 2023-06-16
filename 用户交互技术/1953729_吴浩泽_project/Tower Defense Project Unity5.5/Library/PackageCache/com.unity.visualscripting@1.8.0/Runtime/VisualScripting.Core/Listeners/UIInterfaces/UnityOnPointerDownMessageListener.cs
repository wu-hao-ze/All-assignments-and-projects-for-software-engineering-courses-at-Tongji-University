using UnityEngine.EventSystems;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnPointerDownMessageListener : MessageListener, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnPointerDown, gameObject, eventData);
        }
    }
}
