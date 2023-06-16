using UnityEditor;

namespace Unity.PlasticSCM.Editor
{
    internal static class VCSPlugin
    {
        internal static bool IsEnabled()
        {
            return GetVersionControl() == "PlasticSCM";
        }

        internal static void Disable()
        {
            SetVersionControl("Visible Meta Files");

            AssetDatabase.SaveAssets();
        }

        static string GetVersionControl()
        {
#if UNITY_2020_1_OR_NEWER
            return VersionControlSettings.mode;
#else
            return EditorSettings.externalVersionControl;
#endif
        }

        static void SetVersionControl(string versionControl)
        {
#if UNITY_2020_1_OR_NEWER
            VersionControlSettings.mode = versionControl;
#else
            EditorSettings.externalVersionControl = versionControl;
#endif
        }
    }
}
