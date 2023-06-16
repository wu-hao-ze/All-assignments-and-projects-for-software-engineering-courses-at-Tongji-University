using System;
using System.IO;

using UnityEditor;

using Codice.Client.BaseCommands;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class LoadAsset
    {
        internal static UnityEngine.Object FromChangeInfo(ChangeInfo changeInfo)
        {
            string changeFullPath = changeInfo.GetFullPath();

            if (MetaPath.IsMetaPath(changeFullPath))
                changeFullPath = MetaPath.GetPathFromMetaPath(changeFullPath);

            return FromFullPath(changeFullPath);
        }

        static UnityEngine.Object FromFullPath(string fullPath)
        {
            if (!IsPathUnderProject(fullPath))
                return null;

            return AssetDatabase.LoadMainAssetAtPath(
                AssetsPath.GetRelativePath(fullPath));
        }

        static bool IsPathUnderProject(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            var fullPath = Path.GetFullPath(path).Replace('\\', '/');

            return fullPath.StartsWith(
                mProjectRelativePath,
                StringComparison.OrdinalIgnoreCase);
        }

        static string mProjectRelativePath = 
            Directory.GetCurrentDirectory().Replace('\\', '/') + '/';
    }
}
