using UnityEngine.UI;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnDropdownValueChangedMessageListener : MessageListener
    {
        private void Start()
        {
            GetComponent<Dropdown>()?.onValueChanged?.AddListener((value) =>
                EventBus.Trigger(EventHooks.OnDropdownValueChanged, gameObject, value));
        }
    }
}
