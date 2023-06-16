using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class EditorTimeUtility
    {
        private static int _frameCount;

        private static int frame => EditorApplication.isPlaying ? Time.frameCount : _frameCount;

        private static float time => EditorApplication.isPlaying ? Time.realtimeSinceStartup : (float)EditorApplication.timeSinceStartup;

        internal static void Initialize()
        {
            EditorApplication.update += () => _frameCount++;

            EditorTimeBinding.frameBinding = () => frame;

            EditorTimeBinding.timeBinding = () => time;
        }
    }
}
