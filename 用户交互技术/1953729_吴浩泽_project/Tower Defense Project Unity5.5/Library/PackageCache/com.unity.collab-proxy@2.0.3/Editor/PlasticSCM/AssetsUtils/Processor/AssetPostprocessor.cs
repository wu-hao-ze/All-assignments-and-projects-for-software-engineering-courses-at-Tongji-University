using System.Collections.Generic;

using Codice.Client.Common.FsNodeReaders.Watcher;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    class AssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        internal struct PathToMove
        {
            internal readonly string SrcPath;
            internal readonly string DstPath;

            internal PathToMove(string srcPath, string dstPath)
            {
                SrcPath = srcPath;
                DstPath = dstPath;
            }
        }

        internal static void Enable(
            string wkPath,
            PlasticAssetsProcessor plasticAssetsProcessor)
        {
            mWkPath = wkPath;
            mPlasticAssetsProcessor = plasticAssetsProcessor;

            mIsEnabled = true;
        }

        internal static void Disable()
        {
            mIsEnabled = false;

            mWkPath = null;
            mPlasticAssetsProcessor = null;
        }

        internal static void SetIsRepaintInspectorNeededAfterAssetDatabaseRefresh()
        {
            mIsRepaintInspectorNeededAfterAssetDatabaseRefresh = true;
        }

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!mIsEnabled)
                return;

            if (mIsRepaintInspectorNeededAfterAssetDatabaseRefresh)
            {
                mIsRepaintInspectorNeededAfterAssetDatabaseRefresh = false;
                RepaintInspector.All();
            }

            // We need to ensure that the FSWatcher is enabled before processing Plastic operations
            // It fixes the following scenario: 
            // 1. Close PlasticSCM window
            // 2. Create an asset, it appears with the added overlay
            // 3. Open PlasticSCM window, the asset should appear as added instead of deleted locally
            MonoFileSystemWatcher.IsEnabled = true;

            mPlasticAssetsProcessor.MoveOnSourceControl(
                GetPathsToMoveContainedOnWorkspace(
                    mWkPath, movedAssets, movedFromAssetPaths));

            mPlasticAssetsProcessor.DeleteFromSourceControl(
                GetPathsContainedOnWorkspace(mWkPath, deletedAssets));

            mPlasticAssetsProcessor.AddToSourceControl(
                GetPathsContainedOnWorkspace(mWkPath, importedAssets));

            if (AssetModificationProcessor.ModifiedAssets == null)
                return;

            mPlasticAssetsProcessor.CheckoutOnSourceControl(
                GetPathsContainedOnWorkspace(
                    mWkPath, AssetModificationProcessor.ModifiedAssets));

            AssetModificationProcessor.ModifiedAssets = null;
        }

        static List<PathToMove> GetPathsToMoveContainedOnWorkspace(
            string wkPath,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            List<PathToMove> result = new List<PathToMove>(
                movedAssets.Length);

            for (int i = 0; i < movedAssets.Length; i++)
            {
                string fullSrcPath = AssetsPath.GetFullPathUnderWorkspace.
                    ForAsset(wkPath, movedFromAssetPaths[i]);

                if (fullSrcPath == null)
                    continue;

                string fullDstPath = AssetsPath.GetFullPathUnderWorkspace.
                    ForAsset(wkPath, movedAssets[i]);

                if (fullDstPath == null)
                    continue;

                result.Add(new PathToMove(
                    fullSrcPath, fullDstPath));
            }

            return result;
        }

        static List<string> GetPathsContainedOnWorkspace(
            string wkPath, string[] assets)
        {
            List<string> result = new List<string>(
                assets.Length);

            foreach (string asset in assets)
            {
                string fullPath = AssetsPath.GetFullPathUnderWorkspace.
                    ForAsset(wkPath, asset);

                if (fullPath == null)
                    continue;

                result.Add(fullPath);
            }

            return result;
        }

        static bool mIsEnabled;
        static bool mIsRepaintInspectorNeededAfterAssetDatabaseRefresh;

        static PlasticAssetsProcessor mPlasticAssetsProcessor;
        static string mWkPath;
    }
}
