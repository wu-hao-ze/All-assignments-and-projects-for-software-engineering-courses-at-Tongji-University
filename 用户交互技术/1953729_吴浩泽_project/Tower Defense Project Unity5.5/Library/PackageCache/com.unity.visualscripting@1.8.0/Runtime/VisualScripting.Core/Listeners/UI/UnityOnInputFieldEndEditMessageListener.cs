using UnityEngine.UI;

namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnInputFieldEndEditMessageListener : MessageListener
    {
        private void Start()
        {
            GetComponent<InputField>()?.onEndEdit?.AddListener((value) =>
                EventBus.Trigger(EventHooks.OnInputFieldEndEdit, gameObject, value));
        }
    }
}
