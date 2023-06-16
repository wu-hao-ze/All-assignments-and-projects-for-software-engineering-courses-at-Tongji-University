using UnityEngine.EventSystems;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnScrollMessageListener : MessageListener, IScrollHandler
    {
        public void OnScroll(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnScroll, gameObject, eventData);
        }
    }
}
