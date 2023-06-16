using System.IO;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.VersionControl;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Commands;
using Codice.Client.Commands.WkTree;
using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using GluonGui;
using PlasticGui;
using PlasticGui.Gluon;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Diff;
using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.AssetMenu.Dialogs;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs;

using GluonCheckoutOperation = GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer.Operations.CheckoutOperation;
using GluonUndoCheckoutOperation = GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer.Operations.UndoCheckoutOperation;
using GluonAddoperation = GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer.Operations.AddOperation;

namespace Unity.PlasticSCM.Editor.AssetMenu
{
    internal class AssetOperations :
        IAssetMenuOperations,
        IAssetFilesFilterPatternsMenuOperations
    {
        internal interface IAssetSelection
        {
            AssetList GetSelectedAssets();
        }

        internal AssetOperations(
            WorkspaceInfo wkInfo,
            IWorkspaceWindow workspaceWindow,
            IViewSwitcher viewSwitcher,
            IHistoryViewLauncher historyViewLauncher,
            ViewHost viewHost,
            NewIncomingChangesUpdater newIncomingChangesUpdater,
            IAssetStatusCache assetStatusCache,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonViewSwitcher,
            EditorWindow parentWindow,
            IAssetSelection assetSelection,
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            bool isGluonMode)
        {
            mWkInfo = wkInfo;
            mWorkspaceWindow = workspaceWindow;
            mViewSwitcher = viewSwitcher;
            mHistoryViewLauncher = historyViewLauncher;
            mViewHost = viewHost;
            mNewIncomingChangesUpdater = newIncomingChangesUpdater;
            mAssetStatusCache = assetStatusCache;
            mMergeViewLauncher = mergeViewLauncher;
            mGluonViewSwitcher = gluonViewSwitcher;
            mAssetSelection = assetSelection;
            mShowDownloadPlasticExeWindow = showDownloadPlasticExeWindow;
            mIsGluonMode = isGluonMode;
            mParentWindow = parentWindow;

            mGuiMessage = new UnityPlasticGuiMessage();
            mProgressControls = new EditorProgressControls(mGuiMessage);
        }

        void IAssetMenuOperations.ShowPendingChanges()
        {
            mViewSwitcher.ShowPendingChanges();
        }

        void IAssetMenuOperations.Add()
        {
            List<string> selectedPaths = GetSelectedPaths.ForOperation(
                mWkInfo.ClientPath,
                mAssetSelection.GetSelectedAssets(),
                mAssetStatusCache,
                AssetMenuOperations.Add);

            if (mIsGluonMode)
            {
                GluonAddoperation.Add(
                    mViewHost,
                    mProgressControls,
                    mGuiMessage,
                    selectedPaths.ToArray(),
                    false,
                    RefreshAsset.VersionControlCache);
                return;
            }

            AddOperation.Run(
                mWorkspaceWindow,
                mProgressControls,
                null,
                null,
                selectedPaths,
                false,
                mNewIncomingChangesUpdater,
                RefreshAsset.VersionControlCache);
        }

        void IAssetMenuOperations.Checkout()
        {
            List<string> selectedPaths = GetSelectedPaths.ForOperation(
                mWkInfo.ClientPath,
                mAssetSelection.GetSelectedAssets(),
                mAssetStatusCache,
                AssetMenuOperations.Checkout);

            if (mIsGluonMode)
            {
                GluonCheckoutOperation.Checkout(
                    mViewHost,
                    mProgressControls,
                    mGuiMessage,
                    selectedPaths.ToArray(),
                    false,
                    RefreshAsset.VersionControlCache);
                return;
            }

            CheckoutOperation.Checkout(
                mWorkspaceWindow,
                null,
                mProgressControls,
                selectedPaths,
                mNewIncomingChangesUpdater,
                RefreshAsset.VersionControlCache);
        }

        void IAssetMenuOperations.Checkin()
        {
            List<string> selectedPaths = GetSelectedPaths.ForOperation(
                mWkInfo.ClientPath,
                mAssetSelection.GetSelectedAssets(),
                mAssetStatusCache,
                AssetMenuOperations.Checkin);

            if (!CheckinDialog.CheckinPaths(
                mWkInfo,
                selectedPaths,
                mAssetStatusCache,
                mIsGluonMode,
                mParentWindow,
                mWorkspaceWindow,
                mViewHost,
                mGuiMessage,
                mMergeViewLauncher,
                mGluonViewSwitcher))
                return;

            RefreshAsset.UnityAssetDatabase();
        }

        void IAssetMenuOperations.Undo()
        {
            List<string> selectedPaths = GetSelectedPaths.ForOperation(
                mWkInfo.ClientPath,
                mAssetSelection.GetSelectedAssets(),
                mAssetStatusCache,
                AssetMenuOperations.Undo);

            SaveAssets.ForPathsWithoutConfirmation(selectedPaths);

            if (mIsGluonMode)
            {
                GluonUndoCheckoutOperation.UndoCheckout(
                    mWkInfo,
                    mViewHost,
                    mProgressControls,
                    selectedPaths.ToArray(),
                    false,
                    RefreshAsset.UnityAssetDatabase);
                return;
            }

            UndoCheckoutOperation.Run(
                mWorkspaceWindow,
                null,
                mProgressControls,
                selectedPaths,
                mNewIncomingChangesUpdater,
                RefreshAsset.UnityAssetDatabase);
        }

        void IAssetMenuOperations.ShowDiff()
        {
            if (mShowDownloadPlasticExeWindow.Show(
                    mWkInfo,
                    mIsGluonMode,
                    TrackFeatureUseEvent.Features.InstallPlasticCloudFromShowDiff,
                    TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromFromShowDiff,
                    TrackFeatureUseEvent.Features.CancelPlasticInstallationFromFromShowDiff))
                return;

            string selectedPath = AssetsSelection.GetSelectedPath(
                mWkInfo.ClientPath, 
                mAssetSelection.GetSelectedAssets());

            DiffInfo diffInfo = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    string symbolicName = GetSymbolicName(selectedPath);
                    string extension = Path.GetExtension(selectedPath);

                    diffInfo = PlasticGui.Plastic.API.BuildDiffInfoForDiffWithPrevious(
                        selectedPath, symbolicName, selectedPath, extension, mWkInfo);
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.DisplayException(waiter.Exception);
                        return;
                    }

                    DiffOperation.DiffWithPrevious(
                        diffInfo,
                        null,
                        null);
                });
        }

        void IAssetMenuOperations.ShowHistory()
        {
            if (mShowDownloadPlasticExeWindow.Show(
                   mWkInfo,
                   mIsGluonMode,
                   TrackFeatureUseEvent.Features.InstallPlasticCloudFromShowHistory,
                   TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromShowHistory,
                   TrackFeatureUseEvent.Features.CancelPlasticInstallationFromShowHistory))
                return;

            Asset selectedAsset = AssetsSelection.GetSelectedAsset(
                mWkInfo.ClientPath,
                mAssetSelection.GetSelectedAssets());

            string selectedPath = Path.GetFullPath(selectedAsset.path);

            WorkspaceTreeNode node = PlasticGui.Plastic.API.
                GetWorkspaceTreeNode(selectedPath);

            mHistoryViewLauncher.ShowHistoryView(
                node.RepSpec,
                node.RevInfo.ItemId,
                selectedPath,
                selectedAsset.isFolder);
        }

        void IAssetFilesFilterPatternsMenuOperations.AddFilesFilterPatterns(
            FilterTypes type, 
            FilterActions action, 
            FilterOperationType operation)
        {
            List<string> selectedPaths = AssetsSelection.GetSelectedPaths(
                mWkInfo.ClientPath,
                mAssetSelection.GetSelectedAssets());

            string[] rules = FilterRulesGenerator.GenerateRules(
                selectedPaths, mWkInfo.ClientPath, action, operation);

            bool isApplicableToAllWorkspaces = !mIsGluonMode;
            bool isAddOperation = operation == FilterOperationType.Add;

            FilterRulesConfirmationData filterRulesConfirmationData = 
                FilterRulesConfirmationDialog.AskForConfirmation(
                    rules, isAddOperation, isApplicableToAllWorkspaces, mParentWindow);

            AddFilesFilterPatternsOperation.Run(
                mWkInfo, mWorkspaceWindow, type, operation, filterRulesConfirmationData);
        }

        static string GetSymbolicName(string selectedPath)
        {
            WorkspaceTreeNode node = PlasticGui.Plastic.API.
                GetWorkspaceTreeNode(selectedPath);

            string branchName = string.Empty;
            BranchInfoCache.TryGetBranchName(
                node.RepSpec, node.RevInfo.BranchId, out branchName);

            string userName = PlasticGui.Plastic.API.GetUserName(
                node.RepSpec.Server, node.RevInfo.Owner);

            string symbolicName = string.Format(
                "cs:{0}@{1} {2} {3}",
                node.RevInfo.Changeset,
                string.Format("br:{0}", branchName),
                userName,
                "Workspace Revision");

            return symbolicName;
        }

        readonly WorkspaceInfo mWkInfo;
        readonly IViewSwitcher mViewSwitcher;
        readonly IHistoryViewLauncher mHistoryViewLauncher;
        readonly IWorkspaceWindow mWorkspaceWindow;
        readonly ViewHost mViewHost;
        readonly NewIncomingChangesUpdater mNewIncomingChangesUpdater;
        readonly IAssetStatusCache mAssetStatusCache;
        readonly IMergeViewLauncher mMergeViewLauncher;
        readonly IGluonViewSwitcher mGluonViewSwitcher;
        readonly bool mIsGluonMode;
        readonly GuiMessage.IGuiMessage mGuiMessage;
        readonly EditorProgressControls mProgressControls;
        readonly EditorWindow mParentWindow;
        readonly IAssetSelection mAssetSelection;
        readonly LaunchTool.IShowDownloadPlasticExeWindow mShowDownloadPlasticExeWindow;
    }
}
