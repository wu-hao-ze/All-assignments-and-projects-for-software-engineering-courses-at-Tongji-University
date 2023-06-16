using UnityEngine;

namespace Unity.VisualScripting
{
    public static class Ensure
    {
        private static readonly EnsureThat instance = new EnsureThat();

        public static bool IsActive { get; set; }

        public static void Off() => IsActive = false;

        public static void On() => IsActive = true;

        public static EnsureThat That(string paramName)
        {
            instance.paramName = paramName;
            return instance;
        }

        internal static void OnRuntimeMethodLoad()
        {
            IsActive = Application.isEditor || Debug.isDebugBuild;
        }
    }
}
