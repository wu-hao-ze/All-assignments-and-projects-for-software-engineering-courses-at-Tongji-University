using UnityEngine.EventSystems;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnPointerExitMessageListener : MessageListener, IPointerExitHandler
    {
        public void OnPointerExit(PointerEventData eventData) =>
            EventBus.Trigger(EventHooks.OnPointerExit, gameObject, eventData);
    }
}
