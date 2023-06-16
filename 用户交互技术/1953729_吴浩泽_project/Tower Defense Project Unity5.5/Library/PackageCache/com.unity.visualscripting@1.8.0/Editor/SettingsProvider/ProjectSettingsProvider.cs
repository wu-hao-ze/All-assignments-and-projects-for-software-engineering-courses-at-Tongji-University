using UnityEditor;

namespace Unity.VisualScripting
{
    public class ProjectSettingsProvider : Editor
    {
        [SettingsProvider]
        public static SettingsProvider CreateProjectSettingProvider()
        {
            return new ProjectSettingsProviderView();
        }
    }
}
