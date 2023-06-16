namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnMouseUpMessageListener : MessageListener
    {
        private void OnMouseUp()
        {
            EventBus.Trigger(EventHooks.OnMouseUp, gameObject);
        }
    }
}
