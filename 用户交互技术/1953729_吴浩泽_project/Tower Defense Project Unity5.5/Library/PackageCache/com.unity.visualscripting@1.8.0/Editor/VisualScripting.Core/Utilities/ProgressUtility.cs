using UnityEditor;

namespace Unity.VisualScripting
{
    public static class ProgressUtility
    {
        private static string _progressBarTitleOverride = null;

        internal static void SetTitleOverride(string title)
        {
            _progressBarTitleOverride = title;
        }

        internal static void ClearTitleOverride()
        {
            _progressBarTitleOverride = null;
        }

        public static void DisplayProgressBar(string title, string info, float progress)
        {
            var actualTitle = $"Visual Scripting: {title}";
            if (_progressBarTitleOverride != null)
            {
                actualTitle = _progressBarTitleOverride;
            }

            if (UnityThread.allowsAPI)
            {
                EditorUtility.DisplayProgressBar(actualTitle, info, progress);
            }
            else
            {
                BackgroundWorker.ReportProgress(actualTitle, progress);
            }
        }

#if VISUAL_SCRIPT_INTERNAL
        [MenuItem("Tools/Bolt/Internal/Force Clear Progress Bar", priority = LudiqProduct.DeveloperToolsMenuPriority + 601)]
#endif
        public static void ClearProgressBar()
        {
            if (UnityThread.allowsAPI)
            {
                EditorUtility.ClearProgressBar();
            }

            BackgroundWorker.ClearProgress();
        }
    }
}
