namespace Unity.VisualScripting
{
    [UnityEngine.AddComponentMenu("")]
    public sealed class UnityOnJointBreakMessageListener : MessageListener
    {
        private void OnJointBreak(float breakForce)
        {
            EventBus.Trigger(EventHooks.OnJointBreak, gameObject, breakForce);
        }
    }
}
