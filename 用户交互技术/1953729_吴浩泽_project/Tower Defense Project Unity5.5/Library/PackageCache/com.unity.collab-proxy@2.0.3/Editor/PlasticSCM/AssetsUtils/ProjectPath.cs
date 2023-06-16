using System.IO;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class ProjectPath
    {
        internal static string FromApplicationDataPath(string dataPath)
        {
            return Path.GetDirectoryName(Path.GetFullPath(dataPath));
        }
    }
}
