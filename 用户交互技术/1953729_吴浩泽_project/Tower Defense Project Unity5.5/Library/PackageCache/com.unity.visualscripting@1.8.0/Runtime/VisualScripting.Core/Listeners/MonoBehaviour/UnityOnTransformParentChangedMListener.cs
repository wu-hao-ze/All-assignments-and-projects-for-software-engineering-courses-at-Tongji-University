namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnTransformParentChangedMessageListener : MessageListener
    {
        private void OnTransformParentChanged()
        {
            EventBus.Trigger(EventHooks.OnTransformParentChanged, gameObject);
        }
    }
}
