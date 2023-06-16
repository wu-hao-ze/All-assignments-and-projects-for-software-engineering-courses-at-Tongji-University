using UnityEngine.EventSystems;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnMoveMessageListener : MessageListener, IMoveHandler
    {
        public void OnMove(AxisEventData eventData)
        {
            EventBus.Trigger(EventHooks.OnMove, gameObject, eventData);
        }
    }
}
