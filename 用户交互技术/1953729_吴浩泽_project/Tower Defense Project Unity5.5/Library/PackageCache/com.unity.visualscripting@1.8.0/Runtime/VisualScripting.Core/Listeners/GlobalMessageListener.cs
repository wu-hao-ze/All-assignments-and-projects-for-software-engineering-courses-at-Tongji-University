using UnityEngine;

namespace Unity.VisualScripting
{
    [Singleton(Name = "VisualScripting GlobalEventListener", Automatic = true, Persistent = true)]
    [DisableAnnotation]
    [AddComponentMenu("")]
    [IncludeInSettings(false)]
    [TypeIcon(typeof(MessageListener))]
    public sealed class GlobalMessageListener : MonoBehaviour, ISingleton
    {
        private void OnGUI()
        {
            EventBus.Trigger(EventHooks.OnGUI);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                EventBus.Trigger(EventHooks.OnApplicationFocus);
            }
            else
            {
                EventBus.Trigger(EventHooks.OnApplicationLostFocus);
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                EventBus.Trigger(EventHooks.OnApplicationPause);
            }
            else
            {
                EventBus.Trigger(EventHooks.OnApplicationResume);
            }
        }

        private void OnApplicationQuit()
        {
            EventBus.Trigger(EventHooks.OnApplicationQuit);
        }

        public static void Require()
        {
            // Call the singleton getter to force instantiation
            var instance = Singleton<GlobalMessageListener>.instance;
        }
    }
}
