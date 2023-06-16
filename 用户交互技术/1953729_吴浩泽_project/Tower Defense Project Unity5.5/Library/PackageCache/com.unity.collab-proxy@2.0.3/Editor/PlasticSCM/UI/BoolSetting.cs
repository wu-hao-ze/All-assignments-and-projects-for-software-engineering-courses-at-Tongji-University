using UnityEditor;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class BoolSetting
    {
        internal static bool Load(
            string boolSettingName,
            bool defaultValue)
        {
            return EditorPrefs.GetBool(
                GetSettingKey(boolSettingName),
                defaultValue);
        }

        internal static void Save(
            bool value,
            string boolSettingName)
        {
            EditorPrefs.SetBool(
                GetSettingKey(boolSettingName), value);
        }

        internal static void Clear(
            string boolSettingName)
        {
            EditorPrefs.DeleteKey(
                GetSettingKey(boolSettingName));
        }

        static string GetSettingKey(string boolSettingName)
        {
            return string.Format(
                boolSettingName, PlayerSettings.productGUID);
        }
    }
}
