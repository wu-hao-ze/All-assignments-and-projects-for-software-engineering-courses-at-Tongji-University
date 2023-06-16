using UnityEngine.UI;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnScrollRectValueChangedMessageListener : MessageListener
    {
        private void Start()
        {
            GetComponent<ScrollRect>()?.onValueChanged?.AddListener((value) =>
                EventBus.Trigger(EventHooks.OnScrollRectValueChanged, gameObject, value));
        }
    }
}
