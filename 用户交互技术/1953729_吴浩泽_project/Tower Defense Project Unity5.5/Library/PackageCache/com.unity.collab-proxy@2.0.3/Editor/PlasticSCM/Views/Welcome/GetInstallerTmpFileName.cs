using System;
using System.IO;

using Codice.Utils;

namespace Unity.PlasticSCM.Editor.Views.Welcome
{
    static class GetInstallerTmpFileName
    {
        internal static string ForPlatform()
        {
            string fileName = Guid.NewGuid().ToString();

            if (PlatformIdentifier.IsWindows())
                fileName += ".exe";

            if (PlatformIdentifier.IsMac())
                fileName += ".pkg.zip";

            return Path.Combine(
                Path.GetTempPath(),
                fileName);
        }
    }
}
