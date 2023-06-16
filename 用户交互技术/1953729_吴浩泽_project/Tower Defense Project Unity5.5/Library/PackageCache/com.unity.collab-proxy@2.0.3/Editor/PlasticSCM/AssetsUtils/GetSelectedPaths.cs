using System.Collections.Generic;
using System.IO;

using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using UnityEditor.VersionControl;

namespace Unity.PlasticSCM.Editor.AssetUtils
{
    internal static class GetSelectedPaths
    {
        internal static List<string> ForOperation(
            string wkPath,
            AssetList assetList,
            IAssetStatusCache assetStatusCache,
            AssetMenuOperations operation)
        {
            List<string> selectedPaths = AssetsSelection.
                GetSelectedPaths(wkPath, assetList);

            List<string> result = new List<string>(selectedPaths);

            foreach (string path in selectedPaths)
            {
                if (MetaPath.IsMetaPath(path))
                    continue;

                string metaPath = MetaPath.GetMetaPath(path);

                if (!File.Exists(metaPath))
                    continue;

                if (result.Contains(metaPath))
                    continue;

                if (!IsApplicableForOperation(
                        metaPath, false, operation, assetStatusCache))
                    continue;

                result.Add(metaPath);
            }

            return result;
        }

        static bool IsApplicableForOperation(
            string path,
            bool isDirectory,
            AssetMenuOperations operation,
            IAssetStatusCache assetStatusCache)
        {
            SelectedAssetGroupInfo info = SelectedAssetGroupInfo.BuildFromSingleFile(
                path, isDirectory, assetStatusCache);

            return AssetMenuUpdater.GetAvailableMenuOperations(info).HasFlag(operation);
        }
    }
}
