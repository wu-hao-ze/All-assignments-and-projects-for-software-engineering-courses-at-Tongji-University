using UnityEditor;
using UnityEditor.Build;

namespace Unity.VisualScripting
{
    public static class EditorPlatformUtility
    {
        internal static void InitializeActiveBuildTarget()
        {
            // To access the build target off the main thread
            activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        public static BuildTarget activeBuildTarget { get; internal set; }

        public static bool isTargettingJit => activeBuildTarget.SupportsJit();

        public static bool isTargettingAot => !activeBuildTarget.SupportsJit();

        public static bool SupportsJit(this BuildTarget target)
        {
            return target == BuildTarget.StandaloneWindows ||
                target == BuildTarget.StandaloneWindows64 ||
                target == BuildTarget.StandaloneOSX ||
#if !UNITY_2019_2_OR_NEWER
                target == BuildTarget.StandaloneLinuxUniversal ||
                target == BuildTarget.StandaloneLinux ||
#endif
                target == BuildTarget.StandaloneLinux64;
        }

        public static bool allowJit => !(BoltCore.Configuration?.aotSafeMode).GetValueOrDefault(false)
                                       || isTargettingJit;
    }

    public class EditorPlatformWatcher : IActiveBuildTargetChanged
    {
        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            EditorPlatformUtility.activeBuildTarget = newTarget;
        }
    }
}
