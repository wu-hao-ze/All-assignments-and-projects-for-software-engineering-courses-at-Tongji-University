using System.IO;

using Codice.Utils;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal static class ToolConfig
    {
        internal static string GetUnityPlasticLogConfigFile()
        {
            return GetConfigFilePath(LOG_CONFIG_FILE);
        }

        static string GetConfigFilePath(string configfile)
        {
            string file = Path.Combine(ApplicationLocation.GetAppPath(), configfile);

            if (File.Exists(file))
                return file;

            return UserConfigFolder.GetConfigFile(configfile);
        }

        const string LOG_CONFIG_FILE = "unityplastic.log.conf";
    }
}
