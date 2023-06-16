using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    internal static class PlasticPluginIsEnabledPreference
    {
        internal static bool IsEnabled()
        {
            return BoolSetting.Load(
                UnityConstants.PLASTIC_PLUGIN_IS_ENABLED_KEY_NAME,
                true);
        }

        internal static void Enable()
        {
            BoolSetting.Save(
                true,
                UnityConstants.PLASTIC_PLUGIN_IS_ENABLED_KEY_NAME);
        }

        internal static void Disable()
        {
            BoolSetting.Save(
                false,
                UnityConstants.PLASTIC_PLUGIN_IS_ENABLED_KEY_NAME);
        }
    }
}
