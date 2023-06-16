namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnBecameVisibleMessageListener : MessageListener
    {
        private void OnBecameVisible()
        {
            EventBus.Trigger(EventHooks.OnBecameVisible, gameObject);
        }
    }
}
