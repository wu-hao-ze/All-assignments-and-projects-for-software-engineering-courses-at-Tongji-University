using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.BaseCommands.Merge;
using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.Client.Common.FsNodeReaders;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using PlasticGui;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.BranchExplorer;
using PlasticGui.WorkspaceWindow.Diff;
using PlasticGui.WorkspaceWindow.IncomingChanges;
using PlasticGui.WorkspaceWindow.Merge;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.UI.Tree;
using Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer.DirectoryConflicts;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer
{
    internal class IncomingChangesTab :
        IIncomingChangesTab,
        IRefreshableView,
        MergeViewLogic.IMergeView,
        IIncomingChangesViewMenuOperations,
        IncomingChangesViewMenu.IMetaMenuOperations
    {
        internal IncomingChangesTab(
            WorkspaceInfo wkInfo,
            IWorkspaceWindow workspaceWindow,
            IViewSwitcher switcher,
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            NewIncomingChangesUpdater newIncomingChangesUpdater,
            EditorWindow parentWindow)
        {
            mWkInfo = wkInfo;
            mWorkspaceWindow = workspaceWindow;
            mSwitcher = switcher;
            mShowDownloadPlasticExeWindow = showDownloadPlasticExeWindow;
            mNewIncomingChangesUpdater = newIncomingChangesUpdater;
            mParentWindow = parentWindow;
            mGuiMessage = new UnityPlasticGuiMessage();

            BuildComponents(mWkInfo);

            mProgressControls = new ProgressControlsForViews();

            mCooldownClearUpdateSuccessAction = new CooldownWindowDelayer(
                DelayedClearUpdateSuccess,
                UnityConstants.NOTIFICATION_CLEAR_INTERVAL);

            PlasticNotifier plasticNotifier = new PlasticNotifier();

            mMergeController = new MergeController(
                mWkInfo,
                null,
                null,
                EnumMergeType.IncomingMerge,
                true,
                plasticNotifier,
                null);

            mMergeViewLogic = new MergeViewLogic(
                mWkInfo,
                EnumMergeType.IncomingMerge,
                true,
                mMergeController,
                plasticNotifier,
                ShowIncomingChangesFrom.NotificationBar,
                null,
                mNewIncomingChangesUpdater,
                null,
                this,
                NewChangesInWk.Build(mWkInfo, new BuildWorkspacekIsRelevantNewChange()),
                mProgressControls,
                null);

            ((IRefreshableView)this).Refresh();
        }

        bool IIncomingChangesTab.IsVisible{ get; set; }

        void IIncomingChangesTab.OnDisable()
        {
            TreeHeaderSettings.Save(
                mIncomingChangesTreeView.multiColumnHeader.state,
                UnityConstants.DEVELOPER_INCOMING_CHANGES_TABLE_SETTINGS_NAME);

            mResolveChangeset.Clear();
        }

        void IIncomingChangesTab.Update()
        {
            mProgressControls.UpdateProgress(mParentWindow);
        }

        void IIncomingChangesTab.OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                mHasPendingDirectoryConflicts = mMergeChangesTree != null &&
                    MergeChangesTreeParser.GetUnsolvedDirectoryConflictsCount(mMergeChangesTree) > 0;
                mIsOperationRunning = mProgressControls.IsOperationRunning();
            }

            DoConflictsTree(
                mIncomingChangesTreeView,
                mIsOperationRunning,
                mHasNothingToDownload,
                mIsUpdateSuccessful);

            List<MergeChangeInfo> selectedIncomingChanges =
                mIncomingChangesTreeView.GetSelectedIncomingChanges();

            if (GetSelectedIncomingChangesGroupInfo.For(
                    selectedIncomingChanges).IsDirectoryConflictsSelection &&
                !Mouse.IsRightMouseButtonPressed(Event.current))
            {
                DoDirectoryConflictResolutionPanel(
                    selectedIncomingChanges,
                    new Action<MergeChangeInfo>(ResolveDirectoryConflict),
                    mConflictResolutionStates);
            }

            DrawActionToolbar.Begin(mParentWindow);

            if (!mIsOperationRunning)
            {
                DoActionToolbarMessage(
                    mIsMessageLabelVisible,
                    mMessageLabelText,
                    mHasNothingToDownload,
                    mIsErrorMessageLabelVisible,
                    mErrorMessageLabelText,
                    mDirectoryConflictCount,
                    mFileConflictCount,
                    mChangesSummary);

                if (mIsProcessMergesButtonVisible)
                {
                    DoProcessMergesButton(
                        mIsProcessMergesButtonEnabled && !mHasPendingDirectoryConflicts,
                        mProcessMergesButtonText,
                        mSwitcher,
                        mWorkspaceWindow,
                        mGuiMessage,
                        mMergeViewLogic,
                        RefreshAsset.BeforeLongAssetOperation,
                        () => AfterProcessMerges(RefreshAsset.AfterLongAssetOperation));
                }

                if (mIsCancelMergesButtonVisible)
                {
                    mIsCancelMergesButtonEnabled = DoCancelMergesButton(
                        mIsCancelMergesButtonEnabled,
                        mMergeViewLogic);
                }

                if (mHasPendingDirectoryConflicts)
                {
                    GUILayout.Space(5);
                    DoWarningMessage();
                }
            }
            else
            {
                DrawProgressForViews.ForIndeterminateProgress(
                    mProgressControls.ProgressData);
            }

            DrawActionToolbar.End();

            if (mProgressControls.HasNotification())
            {
                DrawProgressForViews.ForNotificationArea(
                    mProgressControls.ProgressData);
            }
        }

        void IIncomingChangesTab.AutoRefresh()
        {
            BranchInfo workingBranch = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    workingBranch = OverlappedCalculator.GetWorkingBranch(
                        mWkInfo.ClientPath);
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.DisplayException(waiter.Exception);
                        return;
                    }

                    // No need for merge info if it's a label
                    if (workingBranch == null)
                        return;

                    mMergeController.UpdateMergeObjectInfoIfNeeded(workingBranch);
                    mMergeViewLogic.AutoRefresh();
                });
        }

        void IRefreshableView.Refresh()
        {
            BranchInfo workingBranch = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    workingBranch = OverlappedCalculator.GetWorkingBranch(
                        mWkInfo.ClientPath);
                },
                /*afterOperationDelegate*/ delegate
                {
                    if (waiter.Exception != null)
                    {
                        ExceptionsHandler.DisplayException(waiter.Exception);
                        return;
                    }

                    // No need for merge info if it's a label
                    if (workingBranch == null)
                        return;

                    mMergeController.UpdateMergeObjectInfoIfNeeded(workingBranch);
                    mMergeViewLogic.Refresh();
                });
        }

        void MergeViewLogic.IMergeView.UpdateData(
            MergeChangesTree mergeChangesTree,
            ExplainMergeData explainMergeData,
            MergeSolvedFileConflicts solvedFileConflicts,
            bool isIncomingMerge,
            bool isMergeTo,
            bool mergeHasFinished)
        {
            HideMessage();

            ShowProcessMergesButton(
                MergeViewTexts.GetProcessMergeButtonText(
                    MergeChangesTreeParser.HasFileConflicts(mergeChangesTree),
                    true));

            mMergeChangesTree = mergeChangesTree;

            mConflictResolutionStates.Clear();

            UpdateFileConflictsTree(
                mergeChangesTree,
                mIncomingChangesTreeView,
                mResolveChangeset);

            UpdateOverview(mergeChangesTree, solvedFileConflicts);
        }

        void MergeViewLogic.IMergeView.UpdateSolvedDirectoryConflicts()
        {
        }

        void MergeViewLogic.IMergeView.UpdateSolvedFileConflicts(
            MergeSolvedFileConflicts solvedFileConflicts)
        {
            mIncomingChangesTreeView.UpdateSolvedFileConflicts(
                solvedFileConflicts);
        }

        void MergeViewLogic.IMergeView.ShowMessage(
            string title,
            string message,
            bool isErrorMessage)
        {
            if (isErrorMessage)
            {
                mErrorMessageLabelText = message;
                mIsErrorMessageLabelVisible = true;
                return;
            }

            mMessageLabelText = message;
            mIsMessageLabelVisible = true;
            mHasNothingToDownload = message == PlasticLocalization.GetString(
                PlasticLocalization.Name.MergeNothingToDownloadForIncomingView);
        }

        string MergeViewLogic.IMergeView.GetComments(out bool bCancel)
        {
            bCancel = false;
            return string.Empty;
        }

        void MergeViewLogic.IMergeView.DisableProcessMergesButton()
        {
            mIsProcessMergesButtonEnabled = false;
        }

        void MergeViewLogic.IMergeView.ShowCancelButton()
        {
            mIsCancelMergesButtonEnabled = true;
            mIsCancelMergesButtonVisible = true;
        }

        void MergeViewLogic.IMergeView.HideCancelButton()
        {
            mIsCancelMergesButtonEnabled = false;
            mIsCancelMergesButtonVisible = false;
        }

        void MergeViewLogic.IMergeView.EnableMergeOptionButton()
        {
        }

        void MergeViewLogic.IMergeView.DisableMergeOptionsButton()
        {
        }

        SelectedIncomingChangesGroupInfo IIncomingChangesViewMenuOperations.GetSelectedIncomingChangesGroupInfo()
        {
            return IncomingChangesSelection.GetSelectedGroupInfo(mIncomingChangesTreeView);
        }

        void IIncomingChangesViewMenuOperations.MergeContributors()
        {
            if (mShowDownloadPlasticExeWindow.Show(
                mWkInfo,
                false,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromMergeSelectedFiles,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromMergeSelectedFiles,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromMergeSelectedFiles))
                return;

            List<string> selectedPaths = IncomingChangesSelection.
                GetPathsFromSelectedFileConflictsIncludingMeta(
                    mIncomingChangesTreeView);

            mMergeViewLogic.ProcessMerges(
                mWorkspaceWindow,
                mSwitcher,
                mGuiMessage,
                selectedPaths,
                null,
                MergeContributorType.MergeContributors,
                RefreshAsset.BeforeLongAssetOperation,
                () => AfterProcessMerges(RefreshAsset.AfterLongAssetOperation),
                null);
        }

        void IIncomingChangesViewMenuOperations.MergeKeepingSourceChanges()
        {
            List<string> selectedPaths = IncomingChangesSelection.
                GetPathsFromSelectedFileConflictsIncludingMeta(
                    mIncomingChangesTreeView);

            mMergeViewLogic.ProcessMerges(
                mWorkspaceWindow,
                mSwitcher,
                mGuiMessage,
                selectedPaths,
                null,
                MergeContributorType.KeepSource,
                RefreshAsset.BeforeLongAssetOperation,
                () => AfterProcessMerges(RefreshAsset.AfterLongAssetOperation),
                null);
        }

        void IIncomingChangesViewMenuOperations.MergeKeepingWorkspaceChanges()
        {
            List<string> selectedPaths = IncomingChangesSelection.
                GetPathsFromSelectedFileConflictsIncludingMeta(
                    mIncomingChangesTreeView);

            mMergeViewLogic.ProcessMerges(
                mWorkspaceWindow,
                mSwitcher,
                mGuiMessage,
                selectedPaths,
                null,
                MergeContributorType.KeepDestination,
                RefreshAsset.BeforeLongAssetOperation,
                () => AfterProcessMerges(RefreshAsset.AfterLongAssetOperation),
                null);
        }

        void IIncomingChangesViewMenuOperations.DiffYoursWithIncoming()
        {
            MergeChangeInfo incomingChange = IncomingChangesSelection.
                GetSingleSelectedIncomingChange(mIncomingChangesTreeView);

            if (incomingChange == null)
                return;

            DiffYoursWithIncoming(
                mShowDownloadPlasticExeWindow,
                incomingChange,
                mWkInfo);
        }

        void IIncomingChangesViewMenuOperations.DiffIncomingChanges()
        {
            MergeChangeInfo incomingChange = IncomingChangesSelection.
                GetSingleSelectedIncomingChange(mIncomingChangesTreeView);

            if (incomingChange == null)
                return;

            DiffIncomingChanges(
                mShowDownloadPlasticExeWindow,
                incomingChange,
                mWkInfo);
        }

        void IncomingChangesViewMenu.IMetaMenuOperations.DiffIncomingChanges()
        {
            MergeChangeInfo incomingChange = IncomingChangesSelection.
                GetSingleSelectedIncomingChange(mIncomingChangesTreeView);

            if (incomingChange == null)
                return;

            DiffIncomingChanges(
                mShowDownloadPlasticExeWindow,
                mIncomingChangesTreeView.GetMetaChange(incomingChange),
                mWkInfo);
        }

        void IncomingChangesViewMenu.IMetaMenuOperations.DiffYoursWithIncoming()
        {
            MergeChangeInfo incomingChange = IncomingChangesSelection.
                GetSingleSelectedIncomingChange(mIncomingChangesTreeView);

            if (incomingChange == null)
                return;

            DiffYoursWithIncoming(
                mShowDownloadPlasticExeWindow,
                mIncomingChangesTreeView.GetMetaChange(incomingChange),
                mWkInfo);
        }

        bool IncomingChangesViewMenu.IMetaMenuOperations.SelectionHasMeta()
        {
            return mIncomingChangesTreeView.SelectionHasMeta();
        }

        static void DiffYoursWithIncoming(
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            MergeChangeInfo incomingChange,
            WorkspaceInfo wkInfo)
        {
            if (showDownloadPlasticExeWindow.Show(
                wkInfo,
                false,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromDiffYoursWithIncoming,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromDiffYoursWithIncoming,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromDiffYoursWithIncoming))
                return;


            DiffOperation.DiffYoursWithIncoming(
                wkInfo,
                incomingChange.GetMount(),
                incomingChange.GetRevision(),
                incomingChange.GetPath(),
                xDiffLauncher: null,
                imageDiffLauncher: null);
        }

        static void DiffIncomingChanges(
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            MergeChangeInfo incomingChange,
            WorkspaceInfo wkInfo)
        {
            if (showDownloadPlasticExeWindow.Show(
                wkInfo,
                false,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromDiffIncomingChanges,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromDiffIncomingChanges,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromDiffIncomingChanges))
                return;

            DiffOperation.DiffRevisions(
                wkInfo,
                incomingChange.GetMount().RepSpec,
                incomingChange.GetBaseRevision(),
                incomingChange.GetRevision(),
                incomingChange.GetPath(),
                incomingChange.GetPath(),
                true,
                xDiffLauncher: null,
                imageDiffLauncher: null);
        }

        void ShowProcessMergesButton(string processMergesButtonText)
        {
            mProcessMergesButtonText = processMergesButtonText;
            mIsProcessMergesButtonEnabled = true;
            mIsProcessMergesButtonVisible = true;
        }

        void HideMessage()
        {
            mMessageLabelText = string.Empty;
            mIsMessageLabelVisible = false;
            mHasNothingToDownload = false;

            mErrorMessageLabelText = string.Empty;
            mIsErrorMessageLabelVisible = false;
        }

        void DelayedClearUpdateSuccess()
        {
            mIsUpdateSuccessful = false;
        }

        static void DoDirectoryConflictResolutionPanel(
            List<MergeChangeInfo> selectedChangeInfos,
            Action<MergeChangeInfo> resolveDirectoryConflictAction,
            Dictionary<DirectoryConflict, ConflictResolutionState> conflictResolutionStates)
        {
            MergeChangeInfo selectedDirectoryConflict = selectedChangeInfos[0];

            if (selectedDirectoryConflict.DirectoryConflict.IsResolved())
                return;

            DirectoryConflictUserInfo conflictUserInfo;
            DirectoryConflictAction[] conflictActions;

            DirectoryConflictResolutionInfo.FromDirectoryConflict(
                selectedDirectoryConflict.GetMount(),
                selectedDirectoryConflict.DirectoryConflict,
                false,
                out conflictUserInfo,
                out conflictActions);

            ConflictResolutionState conflictResolutionState = GetConflictResolutionState(
                selectedDirectoryConflict.DirectoryConflict,
                conflictActions,
                conflictResolutionStates);

            int pendingSelectedConflictsCount = GetPendingConflictsCount(
                selectedChangeInfos);

            DrawDirectoryResolutionPanel.ForConflict(
                selectedDirectoryConflict,
                (pendingSelectedConflictsCount <= 1) ? 0 : pendingSelectedConflictsCount - 1,
                conflictUserInfo,
                conflictActions,
                resolveDirectoryConflictAction,
                ref conflictResolutionState);
        }

        void ResolveDirectoryConflict(MergeChangeInfo conflict)
        {
            ConflictResolutionState state;

            if (!mConflictResolutionStates.TryGetValue(conflict.DirectoryConflict, out state))
                return;

            List<DirectoryConflictResolutionData> conflictResolutions =
                new List<DirectoryConflictResolutionData>();

            AddConflictResolution(
                conflict,
                state.ResolveAction,
                state.RenameValue,
                conflictResolutions);

            MergeChangeInfo metaConflict =
                mIncomingChangesTreeView.GetMetaChange(conflict);

            if (metaConflict != null)
            {
                AddConflictResolution(
                    metaConflict,
                    state.ResolveAction,
                    MetaPath.GetMetaPath(state.RenameValue),
                    conflictResolutions);
            }

            if (state.IsApplyActionsForNextConflictsChecked)
            {
                foreach (MergeChangeInfo otherConflict in mIncomingChangesTreeView.GetSelectedIncomingChanges())
                {
                    AddConflictResolution(
                        otherConflict,
                        state.ResolveAction,
                        state.RenameValue,
                        conflictResolutions);
                }
            }

            mMergeViewLogic.ResolveDirectoryConflicts(conflictResolutions);
        }

        static void AddConflictResolution(
            MergeChangeInfo conflict,
            DirectoryConflictResolveActions resolveAction,
            string renameValue,
            List<DirectoryConflictResolutionData> conflictResolutions)
        {
            conflictResolutions.Add(new DirectoryConflictResolutionData(
                conflict.DirectoryConflict,
                conflict.Xlink,
                conflict.GetMount().Mount,
                resolveAction,
                renameValue));
        }

        static void DoConflictsTree(
            IncomingChangesTreeView incomingChangesTreeView,
            bool isOperationRunning,
            bool hasNothingToDownload,
            bool isUpdateSuccessful)
        {
            GUI.enabled = !isOperationRunning;

            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);

            incomingChangesTreeView.OnGUI(rect);

            if (hasNothingToDownload)
                DrawEmptyState(rect, isUpdateSuccessful);

            GUI.enabled = true;
        }

        static void DrawEmptyState(
            Rect rect,
            bool isUpdateSuccessful)
        {
            if (isUpdateSuccessful)
            {
                DrawTreeViewEmptyState.For(
                    rect,
                    PlasticLocalization.GetString(PlasticLocalization.Name.WorkspaceUpdateCompleted),
                    Images.GetStepOkIcon());

                return;
            }

            DrawTreeViewEmptyState.For(
                rect,
                PlasticLocalization.GetString(PlasticLocalization.Name.NoIncomingChanges));
        }

        static void DoActionToolbarMessage(
            bool isMessageLabelVisible,
            string messageLabelText,
            bool hasNothingToDownload,
            bool isErrorMessageLabelVisible,
            string errorMessageLabelText,
            int directoryConflictCount,
            int fileConflictCount,
            MergeViewTexts.ChangesToApplySummary changesSummary)
        {
            if (isMessageLabelVisible)
            {
                string message = messageLabelText;

                if (hasNothingToDownload)
                {
                    message = PlasticLocalization.GetString(
                        PlasticLocalization.Name.WorkspaceIsUpToDate);
                }

                DoInfoMessage(message);
            }
            
            if (isErrorMessageLabelVisible)
            {
                DoErrorMessage(errorMessageLabelText);
            }

            if (!isMessageLabelVisible && !isErrorMessageLabelVisible)
            {
                DrawIncomingChangesOverview.For(
                    directoryConflictCount,
                    fileConflictCount,
                    changesSummary);
            }
        }

        void AfterProcessMerges(Action afterAssetLongOperation)
        {
            mIsUpdateSuccessful = true;
            mCooldownClearUpdateSuccessAction.Ping();

            afterAssetLongOperation();
        }

        static void DoProcessMergesButton(
            bool isEnabled,
            string processMergesButtonText,
            IViewSwitcher switcher,
            IWorkspaceWindow workspaceWindow,
            GuiMessage.IGuiMessage guiMessage,
            MergeViewLogic mergeViewLogic,
            Action beforeProcessMergesAction,
            Action afterProcessMergesAction)
        {
            GUI.enabled = isEnabled;

            if (DrawActionButton.For(processMergesButtonText))
            {
                mergeViewLogic.ProcessMerges(
                    workspaceWindow,
                    switcher,
                    guiMessage,
                    new List<string>(),
                    null,
                    MergeContributorType.MergeContributors,
                    beforeProcessMergesAction,
                    afterProcessMergesAction,
                    null);
            }

            GUI.enabled = true;
        }

        static bool DoCancelMergesButton(
            bool isEnabled,
            MergeViewLogic mergeViewLogic)
        {
            bool shouldCancelMergesButtonEnabled = true;

            GUI.enabled = isEnabled;

            if (DrawActionButton.For(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CancelButton)))
            {
                mergeViewLogic.Cancel();

                shouldCancelMergesButtonEnabled = false;
            }

            GUI.enabled = true;

            return shouldCancelMergesButtonEnabled;
        }

        static void DoWarningMessage()
        {
            string label = PlasticLocalization.GetString(PlasticLocalization.Name.SolveConflictsInLable);

            GUILayout.Label(
                new GUIContent(label, Images.GetWarnIcon()),
                UnityStyles.IncomingChangesTab.HeaderWarningLabel);
        }

        void UpdateFileConflictsTree(
            MergeChangesTree incomingChangesTree,
            IncomingChangesTreeView incomingChangesTreeView,
            IResolveChangeset resolveChangeset)
        {
            UnityIncomingChangesTree unityIncomingChangesTree = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    unityIncomingChangesTree = UnityIncomingChangesTree.BuildIncomingChangeCategories(
                        incomingChangesTree);
                    incomingChangesTree.ResolveUserNames(
                        new MergeChangesTree.ResolveUserName());
                    incomingChangesTree.ResolveComments(resolveChangeset);
                },
                /*afterOperationDelegate*/ delegate
                {
                    incomingChangesTreeView.BuildModel(unityIncomingChangesTree);
                    incomingChangesTreeView.Sort();
                    incomingChangesTreeView.Reload();

                    incomingChangesTreeView.SelectFirstUnsolvedDirectoryConflict();
                });
        }

        void UpdateOverview(
            MergeChangesTree mergeChangesTree,
            MergeSolvedFileConflicts solvedFileConflicts)
        {
            mChangesSummary = MergeChangesTreeParser.
                GetChangesToApplySummary(mergeChangesTree);

            mFileConflictCount = MergeChangesTreeParser.GetUnsolvedFileConflictsCount(
                mergeChangesTree, solvedFileConflicts);

            mDirectoryConflictCount = MergeChangesTreeParser.GetUnsolvedDirectoryConflictsCount(
                mergeChangesTree);
        }

        static void DoInfoMessage(string message)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(message, UnityStyles.IncomingChangesTab.ChangesToApplySummaryLabel);

            EditorGUILayout.EndHorizontal();
        }

        static void DoErrorMessage(string message)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(message, UnityStyles.IncomingChangesTab.RedPendingConflictsOfTotalLabel);

            EditorGUILayout.EndHorizontal();
        }

        void BuildComponents(WorkspaceInfo wkInfo)
        {
            IncomingChangesTreeHeaderState incomingChangesHeaderState =
                IncomingChangesTreeHeaderState.GetDefault();

            TreeHeaderSettings.Load(incomingChangesHeaderState,
                UnityConstants.DEVELOPER_INCOMING_CHANGES_TABLE_SETTINGS_NAME,
                (int)IncomingChangesTreeColumn.Path, true);

            mIncomingChangesTreeView = new IncomingChangesTreeView(
                wkInfo, incomingChangesHeaderState,
                IncomingChangesTreeHeaderState.GetColumnNames(),
                new IncomingChangesViewMenu(this, this));

            mIncomingChangesTreeView.Reload();
        }

        static ConflictResolutionState GetConflictResolutionState(
            DirectoryConflict directoryConflict,
            DirectoryConflictAction[] conflictActions,
            Dictionary<DirectoryConflict, ConflictResolutionState> conflictResoltionStates)
        {
            ConflictResolutionState result;

            if (conflictResoltionStates.TryGetValue(directoryConflict, out result))
                return result;

            result = ConflictResolutionState.Build(directoryConflict, conflictActions);

            conflictResoltionStates.Add(directoryConflict, result);
            return result;
        }

        static int GetPendingConflictsCount(
            List<MergeChangeInfo> selectedChangeInfos)
        {
            int result = 0;
            foreach (MergeChangeInfo changeInfo in selectedChangeInfos)
            {
                if (changeInfo.DirectoryConflict.IsResolved())
                    continue;

                result++;
            }

            return result;
        }
        bool mIsProcessMergesButtonVisible;
        bool mIsCancelMergesButtonVisible;
        bool mIsMessageLabelVisible;
        bool mIsErrorMessageLabelVisible;
        bool mHasNothingToDownload;

        bool mIsProcessMergesButtonEnabled;
        bool mIsCancelMergesButtonEnabled;
        bool mHasPendingDirectoryConflicts;
        bool mIsOperationRunning;
        bool mIsUpdateSuccessful;

        string mProcessMergesButtonText;
        string mMessageLabelText;
        string mErrorMessageLabelText;

        IncomingChangesTreeView mIncomingChangesTreeView;

        MergeChangesTree mMergeChangesTree;

        Dictionary<DirectoryConflict, ConflictResolutionState> mConflictResolutionStates =
            new Dictionary<DirectoryConflict, ConflictResolutionState>();

        int mDirectoryConflictCount;
        int mFileConflictCount;
        MergeViewTexts.ChangesToApplySummary mChangesSummary;

        readonly ProgressControlsForViews mProgressControls;
        readonly CooldownWindowDelayer mCooldownClearUpdateSuccessAction;
        readonly MergeViewLogic mMergeViewLogic;
        readonly MergeController mMergeController;
        readonly GuiMessage.IGuiMessage mGuiMessage;
        readonly EditorWindow mParentWindow;
        readonly NewIncomingChangesUpdater mNewIncomingChangesUpdater;
        readonly LaunchTool.IShowDownloadPlasticExeWindow mShowDownloadPlasticExeWindow;
        readonly IViewSwitcher mSwitcher;
        readonly IWorkspaceWindow mWorkspaceWindow;
        readonly WorkspaceInfo mWkInfo;
        readonly IResolveChangeset mResolveChangeset = new ResolveChangeset();
    }
}