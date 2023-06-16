using System.Diagnostics;

using Codice.Utils;

namespace Unity.PlasticSCM.Editor.Tool
{
    internal static class LaunchInstaller
    {
        internal static Process ForPlatform(string installerPath)
        {
            if (PlatformIdentifier.IsMac())
            {
                return Process.Start(
                    ToolConstants.Installer.INSTALLER_MACOS_OPEN,
                    string.Format(ToolConstants.Installer.INSTALLER_MACOS_OPEN_ARGS, installerPath));
            }

            return Process.Start(
                installerPath,
                ToolConstants.Installer.INSTALLER_WINDOWS_ARGS);
        }
    }
}
