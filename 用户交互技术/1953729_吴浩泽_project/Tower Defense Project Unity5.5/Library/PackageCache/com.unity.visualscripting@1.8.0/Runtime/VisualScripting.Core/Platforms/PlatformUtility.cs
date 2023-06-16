using UnityEngine;

namespace Unity.VisualScripting
{
    public static class PlatformUtility
    {
        public static readonly bool supportsJit;

        static PlatformUtility()
        {
            supportsJit = CheckJitSupport();
        }

        private static bool CheckJitSupport()
        {
            // Temporary hotfix
            // Generally it seems like JIT is becoming more and more unreliable
            // And some of the generated IL we were using crashes in some cases, but it's hard to isolate
            // Because the delegate approach is very close in speed, we'll just disable it altogether until Bolt 2
            // generates full C# scripts.
            // https://forum.unity.com/threads/is-jit-no-longer-supported-on-standalone-mono.671572/
            // https://support.ludiq.io/communities/5/topics/3129-bolt-143-runtime-broken
            // https://support.ludiq.io/communities/5/topics/4013-unity-crash-randomly-after-hit-play
            return false;
        }

        public static bool IsEditor(this RuntimePlatform platform)
        {
            return
                platform == RuntimePlatform.WindowsEditor ||
                platform == RuntimePlatform.OSXEditor ||
                platform == RuntimePlatform.LinuxEditor;
        }

        public static bool IsStandalone(this RuntimePlatform platform)
        {
            return
                platform == RuntimePlatform.WindowsPlayer ||
                platform == RuntimePlatform.OSXPlayer ||
                platform == RuntimePlatform.LinuxPlayer;
        }
    }
}
