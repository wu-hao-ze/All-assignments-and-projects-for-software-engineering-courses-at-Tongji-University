using System.IO;
using Codice.Client.Common;
using Codice.Utils;

namespace Unity.PlasticSCM.Editor.Views.Welcome
{
    class MacOSConfigWorkaround
    {
        /* In macOS there is no way to pass a parameter
         * to the PKG installer to avoid launching
         * Plastic at the end of the installation process.

         * As a workaround, we can create an empty client.conf in
         * the user config folder. This way the installer skips
         * launching Plastic at the end of the installation process.

         * see /01plastic/install/mac/macplastic/Scripts/postinstall

         * Then, we delete the client.conf file if we created it */

        internal void CreateClientConfigIfNeeded()
        {
            if (!PlatformIdentifier.IsMac())
                return;

            string clientConfFile = ConfigFileLocation.GetConfigFilePath(
                ClientConfig.CLIENT_CONFIG_FILE_NAME);

            if (File.Exists(clientConfFile))
                return;

            File.Create(clientConfFile).Close();
            mClientConfigCreated = true;
        }

        internal void DeleteClientConfigIfNeeded()
        {
            if (!mClientConfigCreated)
                return;

            string clientConfFile = ConfigFileLocation.GetConfigFilePath(
                ClientConfig.CLIENT_CONFIG_FILE_NAME);

            File.Delete(clientConfFile);
        }

        bool mClientConfigCreated;
    }
}
