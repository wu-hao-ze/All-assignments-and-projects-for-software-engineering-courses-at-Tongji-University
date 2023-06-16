namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnTransformChildrenChangedMessageListener : MessageListener
    {
        private void OnTransformChildrenChanged()
        {
            EventBus.Trigger(EventHooks.OnTransformChildrenChanged, gameObject);
        }
    }
}
