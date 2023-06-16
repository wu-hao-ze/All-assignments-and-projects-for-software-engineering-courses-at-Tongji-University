using UnityEngine.EventSystems;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnPointerClickMessageListener : MessageListener, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnPointerClick, gameObject, eventData);
        }
    }
}
