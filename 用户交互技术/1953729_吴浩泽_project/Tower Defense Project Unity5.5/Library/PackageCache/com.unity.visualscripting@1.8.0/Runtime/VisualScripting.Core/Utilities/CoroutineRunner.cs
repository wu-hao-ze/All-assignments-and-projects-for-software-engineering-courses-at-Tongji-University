using UnityEngine;

namespace Unity.VisualScripting
{
    [Singleton(Name = "VisualScripting CoroutineRunner", Automatic = true, Persistent = true)]
    [AddComponentMenu("")]
    [DisableAnnotation]
    [IncludeInSettings(false)]
    public sealed class CoroutineRunner : MonoBehaviour, ISingleton
    {
        private void Awake()
        {
            Singleton<CoroutineRunner>.Awake(this);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            Singleton<CoroutineRunner>.OnDestroy(this);
        }

        public static CoroutineRunner instance => Singleton<CoroutineRunner>.instance;
    }
}
