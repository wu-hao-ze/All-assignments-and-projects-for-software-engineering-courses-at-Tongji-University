using UnityEngine;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Listens to the OnApplicationQuit on OnApplicationPause
    /// hooks to trigger the serialization of saved variables into PlayerPrefs.
    /// </summary>
    [Singleton(Name = "VisualScripting SavedVariablesSerializer", Automatic = true, Persistent = true)]
    [AddComponentMenu("")]
    [DisableAnnotation]
    [IncludeInSettings(false)]
    public class VariablesSaver : MonoBehaviour, ISingleton
    {
        private void Awake()
        {
            Singleton<VariablesSaver>.Awake(this);
        }

        private void OnDestroy()
        {
            Singleton<VariablesSaver>.OnDestroy(this);
        }

        private void OnApplicationQuit()
        {
            SavedVariables.OnExitPlayMode();
            ApplicationVariables.OnExitPlayMode();
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
            {
                return;
            }

            SavedVariables.OnExitPlayMode();
            ApplicationVariables.OnExitPlayMode();
        }

        public static VariablesSaver instance => Singleton<VariablesSaver>.instance;

        public static void Instantiate() => Singleton<VariablesSaver>.Instantiate();
    }
}
