using UnityEngine;

namespace Unity.VisualScripting
{
    public static class RuntimeVSUsageUtility
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitializeOnLoadBeforeSceneLoad()
        {
            UnityThread.RuntimeInitialize();

            Ensure.OnRuntimeMethodLoad();

            Recursion.OnRuntimeMethodLoad();

            OptimizedReflection.OnRuntimeMethodLoad();

            SavedVariables.OnEnterPlayMode();

            ApplicationVariables.OnEnterPlayMode();

            ReferenceCollector.Initialize();
        }
    }
}
