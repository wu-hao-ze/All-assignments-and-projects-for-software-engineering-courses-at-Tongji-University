using System.Collections.Generic;

using UnityEditor.VersionControl;

using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetUtils.Processor;

namespace Unity.PlasticSCM.Editor.SceneView
{
    static class DrawSceneOperations
    {
        internal static void Enable(
            string wkPath,
            WorkspaceOperationsMonitor workspaceOperationsMonitor,
            IAssetStatusCache assetStatusCache)
        {
            if (mIsEnabled)
                return;

            mWkPath = wkPath;
            mWorkspaceOperationsMonitor = workspaceOperationsMonitor;
            mAssetStatusCache = assetStatusCache;

            mIsEnabled = true;

            Provider.preCheckoutCallback += Provider_preCheckoutCallback;
        }

        internal static void Disable()
        {
            mIsEnabled = false;

            Provider.preCheckoutCallback -= Provider_preCheckoutCallback;

            mWkPath = null;
            mWorkspaceOperationsMonitor = null;
            mAssetStatusCache = null;
        }

        static bool Provider_preCheckoutCallback(
            AssetList list,
            ref string changesetID,
            ref string changesetDescription)
        {
            if (!mIsEnabled)
                return true;

            if (!FindWorkspace.HasWorkspace(ApplicationDataPath.Get()))
            { 
                Disable();
                return true;
            }

            List<string> selectedPaths = GetSelectedPaths.ForOperation(
                mWkPath, list, mAssetStatusCache,
                AssetMenuOperations.Checkout);

            if (selectedPaths.Count == 0)
                return true;

            mWorkspaceOperationsMonitor.AddPathsToCheckout(selectedPaths);
            return true;
        }

        static bool mIsEnabled;
        static IAssetStatusCache mAssetStatusCache;
        static WorkspaceOperationsMonitor mWorkspaceOperationsMonitor;
        static string mWkPath;
    }
}
