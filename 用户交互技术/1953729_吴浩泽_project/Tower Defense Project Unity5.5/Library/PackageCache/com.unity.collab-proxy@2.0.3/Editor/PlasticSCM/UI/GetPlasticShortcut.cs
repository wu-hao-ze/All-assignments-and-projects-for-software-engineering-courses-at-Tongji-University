using Codice.Utils;
using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class GetPlasticShortcut
    {
        internal static string ForOpen()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.UnityOpenShortcut);
        }

        internal static string ForDelete()
        {
            if (PlatformIdentifier.IsWindows())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.UnityDeleteShortcutForWindows);

            if (PlatformIdentifier.IsMac())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.UnityDeleteShortcutForMacOS);

            return string.Empty;
        }

        internal static string ForDiff()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.UnityDiffShortcut);
        }

        internal static string ForAssetDiff()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.UnityAssetDiffShortcut);
        }

        internal static string ForHistory()
        {
            if (PlatformIdentifier.IsWindows())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.UnityHistoryShortcutForWindows);

            if (PlatformIdentifier.IsMac())
                return PlasticLocalization.GetString(
                    PlasticLocalization.Name.UnityHistoryShortcutForMacOS);

            return string.Empty;
        }
    }
}
