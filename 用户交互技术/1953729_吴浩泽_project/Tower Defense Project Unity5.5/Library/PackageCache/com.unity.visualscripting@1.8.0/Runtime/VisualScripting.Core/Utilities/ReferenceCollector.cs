using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.VisualScripting
{
    public static class ReferenceCollector
    {
        public static event Action onSceneUnloaded;

        internal static void Initialize()
        {
            SceneManager.sceneUnloaded += scene => onSceneUnloaded?.Invoke();
        }
    }
}
