using UnityEditor;

namespace Unity.VisualScripting
{
    public class EditorPreferencesProvider : Editor
    {
        [SettingsProvider]
        public static SettingsProvider CreateEditorPreferencesProvider()
        {
            return new EditorPreferencesProviderView();
        }
    }
}
