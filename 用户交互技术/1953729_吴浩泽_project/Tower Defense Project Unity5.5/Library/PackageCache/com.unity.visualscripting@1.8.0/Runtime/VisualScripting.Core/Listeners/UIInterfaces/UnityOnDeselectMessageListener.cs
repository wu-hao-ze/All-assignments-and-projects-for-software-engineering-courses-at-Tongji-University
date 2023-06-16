using UnityEngine.EventSystems;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnDeselectMessageListener : MessageListener, IDeselectHandler
    {
        public void OnDeselect(BaseEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnDeselect, gameObject, eventData);
        }
    }
}
