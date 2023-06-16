using System.Collections.Generic;
using System.IO;

using UnityEditor.VersionControl;

using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetUtils;

namespace Unity.PlasticSCM.Editor.AssetMenu
{
    internal static class AssetsSelection
    {
        internal static Asset GetSelectedAsset(
            string wkPath,
            AssetList assetList)
        {
            if (assetList.Count == 0)
                return null;

            foreach (Asset asset in assetList)
            {
                if (AssetsPath.GetFullPathUnderWorkspace.
                        ForAsset(wkPath, asset.path) == null)
                    continue;

                return asset;
            }

            return null;
        }

        internal static string GetSelectedPath(
            string wkPath,
            AssetList assetList)
        {
            Asset result = GetSelectedAsset(wkPath, assetList);

            if (result == null)
                return null;

            return Path.GetFullPath(result.path);
        }

        internal static List<string> GetSelectedPaths(
            string wkPath,
            AssetList assetList)
        {
            List<string> result = new List<string>();

            foreach (Asset asset in assetList)
            {
                string fullPath = AssetsPath.GetFullPathUnderWorkspace.
                    ForAsset(wkPath, asset.path);

                if (fullPath == null)
                    continue;

                result.Add(fullPath);
            }

            return result;
        }

        internal static SelectedPathsGroupInfo GetSelectedPathsGroupInfo(
            string wkPath,
            AssetList assetList,
            IAssetStatusCache statusCache)
        {
            SelectedPathsGroupInfo result = new SelectedPathsGroupInfo();

            if (assetList.Count == 0)
                return result;

            result.IsRootSelected = false;
            result.IsCheckedoutEverySelected = true;
            result.IsDirectoryEverySelected = true;
            result.IsCheckedinEverySelected = true;
            result.IsChangedEverySelected = true;

            foreach (Asset asset in assetList)
            {
                string fullPath = AssetsPath.GetFullPathUnderWorkspace.
                    ForAsset(wkPath, asset.path);

                if (fullPath == null)
                    continue;

                if (MetaPath.IsMetaPath(fullPath))
                    fullPath = MetaPath.GetPathFromMetaPath(fullPath);

                AssetStatus status = statusCache.GetStatus(fullPath);
                string assetName = GetAssetName(asset);

                result.IsCheckedoutEverySelected &= ClassifyAssetStatus.IsCheckedOut(status);
                result.IsDirectoryEverySelected &= asset.isFolder;
                result.IsCheckedinEverySelected &= false; // TODO: not implemented yet
                result.IsChangedEverySelected &= false; // TODO: not implemented yet

                result.IsAnyDirectorySelected |= asset.isFolder;
                result.IsAnyPrivateSelected |= ClassifyAssetStatus.IsPrivate(status);

                result.FilterInfo.IsAnyIgnoredSelected |= ClassifyAssetStatus.IsIgnored(status);
                result.FilterInfo.IsAnyHiddenChangedSelected |= ClassifyAssetStatus.IsHiddenChanged(status);

                result.SelectedCount++;

                if (result.SelectedCount == 1)
                {
                    result.FirstIsControlled = ClassifyAssetStatus.IsControlled(status);
                    result.FirstIsDirectory = asset.isFolder;

                    result.FilterInfo.CommonName = assetName;
                    result.FilterInfo.CommonExtension = Path.GetExtension(assetName);
                    result.FilterInfo.CommonFullPath = asset.assetPath;
                    continue;
                }

                if (result.FilterInfo.CommonName != assetName)
                    result.FilterInfo.CommonName = null;

                if (result.FilterInfo.CommonExtension != Path.GetExtension(assetName))
                    result.FilterInfo.CommonExtension = null;

                if (result.FilterInfo.CommonFullPath != asset.assetPath)
                    result.FilterInfo.CommonFullPath = null;
            }

            return result;
        }

        static string GetAssetName(Asset asset)
        {
            if (asset.isFolder)
                return Path.GetFileName(Path.GetDirectoryName(asset.path));

            return asset.fullName;
        }
    }
}
