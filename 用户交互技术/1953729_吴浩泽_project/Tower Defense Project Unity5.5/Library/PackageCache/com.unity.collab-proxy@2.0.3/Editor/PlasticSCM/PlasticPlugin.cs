using System;
using System.Threading.Tasks;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common.Connection;
using Codice.CM.Common;
using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetUtils.Processor;
using Unity.PlasticSCM.Editor.CollabMigration;
using Unity.PlasticSCM.Editor.Inspector;
using Unity.PlasticSCM.Editor.ProjectDownloader;
using Unity.PlasticSCM.Editor.SceneView;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    /// <summary>
    /// The Plastic SCM plugin for Unity editor.
    /// </summary>
    [InitializeOnLoad]
    public static class PlasticPlugin
    {
        /// <summary>
        /// Invoked when notification status changed.
        /// </summary>
        public static event Action OnNotificationUpdated = delegate { };

        internal static IAssetStatusCache AssetStatusCache 
        { 
            get { return mAssetStatusCache; } 
        }

        internal static WorkspaceOperationsMonitor WorkspaceOperationsMonitor 
        { 
            get { return mWorkspaceOperationsMonitor; } 
        }

        internal static PlasticConnectionMonitor ConnectionMonitor
        {
            get { return mPlasticConnectionMonitor; }
        }

        static PlasticPlugin()
        {
            CloudProjectDownloader.Initialize();
            MigrateCollabProject.Initialize();
            EditorDispatcher.Initialize();

            if (!FindWorkspace.HasWorkspace(ApplicationDataPath.Get()))
                return;

            if (!PlasticPluginIsEnabledPreference.IsEnabled())
                return;

            CooldownWindowDelayer cooldownInitializeAction = new CooldownWindowDelayer(
                Enable, UnityConstants.PLUGIN_DELAYED_INITIALIZE_INTERVAL);
            cooldownInitializeAction.Ping();
        }

        /// <summary>
        /// Open the Plastic SCM window.
        /// Also, it enables the plugin IsEnabled preference if it is disabled.
        /// </summary>
        public static void OpenPlasticWindowDisablingOfflineModeIfNeeded()
        {
            // It's pending to rename the OpenPlasticWindowDisablingOfflineModeIfNeeded
            // method to OpenPlasticWindowAndEnablePluginIfNeeded. We cannot do it now
            // because it's a public method and this rename breaks the API validation
            // check. We will do it when we change the major version number to v3.0.0.

            if (!PlasticPluginIsEnabledPreference.IsEnabled())
            {
                PlasticPluginIsEnabledPreference.Enable();
                Enable();
            }

            ShowWindow.Plastic();
        }

        /// <summary>
        /// Get the plugin status icon.
        /// </summary>
        public static Texture GetPluginStatusIcon()
        {
            return PlasticNotification.GetIcon(mNotificationStatus);
        }

        internal static void Enable()
        {
            if (mIsEnabled)
                return;

            mIsEnabled = true;

            PlasticApp.InitializeIfNeeded();

            if (!FindWorkspace.HasWorkspace(ApplicationDataPath.Get()))
                return;

            EnableForWorkspace();
        }

        internal static void EnableForWorkspace()
        {
            if (mIsEnabledForWorkspace)
                return;

            WorkspaceInfo wkInfo = FindWorkspace.InfoForApplicationPath(
                ApplicationDataPath.Get(), PlasticGui.Plastic.API);

            if (wkInfo == null)
                return;

            mIsEnabledForWorkspace = true;

            PlasticApp.SetWorkspace(wkInfo);

            HandleCredsAliasAndServerCert.InitializeHostUnreachableExceptionListener(
                mPlasticConnectionMonitor);

            bool isGluonMode = PlasticGui.Plastic.API.IsGluonWorkspace(wkInfo);

            mAssetStatusCache = new AssetStatusCache(wkInfo, isGluonMode);

            PlasticAssetsProcessor plasticAssetsProcessor = new PlasticAssetsProcessor();

            mWorkspaceOperationsMonitor = BuildWorkspaceOperationsMonitor(
                plasticAssetsProcessor, isGluonMode);
            mWorkspaceOperationsMonitor.Start();

            AssetsProcessors.Enable(
                wkInfo.ClientPath, plasticAssetsProcessor, mAssetStatusCache);
            AssetMenuItems.Enable(
                wkInfo, mAssetStatusCache);
            DrawAssetOverlay.Enable(
                wkInfo.ClientPath, mAssetStatusCache);
            DrawInspectorOperations.Enable(
                wkInfo.ClientPath, mAssetStatusCache);
            DrawSceneOperations.Enable(
                wkInfo.ClientPath, mWorkspaceOperationsMonitor, mAssetStatusCache);

            Task.Run(() => EnsureServerConnection(wkInfo, mPlasticConnectionMonitor));
        }

        internal static void Disable()
        {
            if (!mIsEnabled)
                return;

            try
            {
                PlasticApp.Dispose();

                if (!mIsEnabledForWorkspace)
                    return;

                mWorkspaceOperationsMonitor.Stop();

                AssetsProcessors.Disable();
                AssetMenuItems.Disable();
                DrawAssetOverlay.Disable();
                DrawInspectorOperations.Disable();
                DrawSceneOperations.Disable();
            }
            finally
            {
                mIsEnabled = false;
                mIsEnabledForWorkspace = false;
            }
        }

        internal static void SetNotificationStatus(
            PlasticWindow plasticWindow,
            PlasticNotification.Status status)
        {
            mNotificationStatus = status;

            plasticWindow.SetupWindowTitle(status);

            if (OnNotificationUpdated != null) 
                OnNotificationUpdated.Invoke();
        }

        static WorkspaceOperationsMonitor BuildWorkspaceOperationsMonitor(
            PlasticAssetsProcessor plasticAssetsProcessor,
            bool isGluonMode)
        {
            WorkspaceOperationsMonitor result = new WorkspaceOperationsMonitor(
                PlasticGui.Plastic.API, plasticAssetsProcessor, isGluonMode);
            plasticAssetsProcessor.SetWorkspaceOperationsMonitor(result);
            return result;
        }

        static void EnsureServerConnection(
            WorkspaceInfo wkInfo,
            PlasticConnectionMonitor plasticConnectionMonitor)
        {
            RepositorySpec repSpec = PlasticGui.Plastic.API.GetRepositorySpec(wkInfo);

            plasticConnectionMonitor.SetRepositorySpecForEventTracking(repSpec);

            try
            {
                // set the PlasticConnectionMonitor initially to have a valid connection
                // then check that the server connection is valid. If failed, we call
                // PlasticConnectionMonitor.OnConnectionError that fires the Plugin disable
                // and the reconnection mechanism

                plasticConnectionMonitor.SetAsConnected();

                if (!PlasticGui.Plastic.API.CheckServerConnection(repSpec.Server))
                    throw new Exception(string.Format("Failed to connect to {0}", repSpec.Server));
            }
            catch (Exception ex)
            {
                plasticConnectionMonitor.OnConnectionError(ex, repSpec.Server);
            }
        }

        static PlasticNotification.Status mNotificationStatus;
        static AssetStatusCache mAssetStatusCache;
        static WorkspaceOperationsMonitor mWorkspaceOperationsMonitor;
        static PlasticConnectionMonitor mPlasticConnectionMonitor = new PlasticConnectionMonitor();
        static bool mIsEnabled;
        static bool mIsEnabledForWorkspace;
    }
}
