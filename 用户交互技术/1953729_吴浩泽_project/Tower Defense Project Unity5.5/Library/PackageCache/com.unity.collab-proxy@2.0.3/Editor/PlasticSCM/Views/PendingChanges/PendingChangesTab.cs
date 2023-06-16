using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.Client.Common.FsNodeReaders;
using Codice.Client.Common.Threading;
using Codice.CM.Client.Gui;
using Codice.CM.Common;
using Codice.CM.Common.Merge;
using Codice.LogWrapper;
using GluonGui;
using GluonGui.WorkspaceWindow.Views.Checkin.Operations;
using PlasticGui;
using PlasticGui.Help.Actions;
using PlasticGui.Help.Conditions;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Diff;
using PlasticGui.WorkspaceWindow.Items;
using PlasticGui.WorkspaceWindow.Open;
using PlasticGui.WorkspaceWindow.PendingChanges;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.Help;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.UI.StatusBar;
using Unity.PlasticSCM.Editor.UI.Tree;
using Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs;
using Unity.PlasticSCM.Editor.Views.PendingChanges.PendingMergeLinks;
using Unity.PlasticSCM.Editor.Views.Changesets;

using GluonNewIncomingChangesUpdater = PlasticGui.Gluon.WorkspaceWindow.NewIncomingChangesUpdater;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal partial class PendingChangesTab :
        IRefreshableView,
        PlasticProjectSettingsProvider.IAutoRefreshView,
        IPendingChangesView,
        CheckinUIOperation.ICheckinView,
        PendingChangesViewPendingChangeMenu.IMetaMenuOperations,
        IPendingChangesMenuOperations,
        IChangelistMenuOperations,
        IOpenMenuOperations,
        IFilesFilterPatternsMenuOperations,
        PendingChangesViewMenu.IGetSelectedNodes,
        ChangesetsTab.IRevertToChangesetListener
    {
        internal IProgressControls ProgressControlsForTesting { get { return mProgressControls; } }
        internal HelpPanel HelpPanelForTesting { get { return mHelpPanel; } }

        internal void SetMergeLinksForTesting(
            IDictionary<MountPoint, IList<PendingMergeLink>> mergeLinks)
        {
            mPendingMergeLinks = mergeLinks;

            UpdateMergeLinksList();
        }

        internal string CommentText { get; set; }

        internal bool ForceToShowComment { get; set; }

        internal PendingChangesTab(
            WorkspaceInfo wkInfo,
            ViewHost viewHost,
            bool isGluonMode,
            WorkspaceWindow workspaceWindow,
            IViewSwitcher switcher,
            IMergeViewLauncher mergeViewLauncher,
            IHistoryViewLauncher historyViewLauncher,
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            NewIncomingChangesUpdater developerNewIncomingChangesUpdater,
            GluonNewIncomingChangesUpdater gluonNewIncomingChangesUpdater,
            IAssetStatusCache assetStatusCache,
            StatusBar statusBar,
            EditorWindow parentWindow)
        {
            mWkInfo = wkInfo;
            mViewHost = viewHost;
            mIsGluonMode = isGluonMode;
            mWorkspaceWindow = workspaceWindow;
            mHistoryViewLauncher = historyViewLauncher;
            mShowDownloadPlasticExeWindow = showDownloadPlasticExeWindow;
            mDeveloperNewIncomingChangesUpdater = developerNewIncomingChangesUpdater;
            mGluonNewIncomingChangesUpdater = gluonNewIncomingChangesUpdater;
            mAssetStatusCache = assetStatusCache;
            mStatusBar = statusBar;
            mParentWindow = parentWindow;
            mGuiMessage = new UnityPlasticGuiMessage();
            mCheckedStateManager = new PendingChangesViewCheckedStateManager();

            mNewChangesInWk = NewChangesInWk.Build(
                mWkInfo,
                new BuildWorkspacekIsRelevantNewChange());

            BuildComponents(isGluonMode, parentWindow);

            mBorderColor = EditorGUIUtility.isProSkin
                ? (Color)new Color32(35, 35, 35, 255)
                : (Color)new Color32(153, 153, 153, 255);

            mProgressControls = new ProgressControlsForViews();

            mCooldownClearCheckinSuccessAction = new CooldownWindowDelayer(
                DelayedClearCheckinSuccess,
                UnityConstants.NOTIFICATION_CLEAR_INTERVAL);

            workspaceWindow.RegisterPendingChangesProgressControls(mProgressControls);

            mPendingChangesOperations = new PendingChangesOperations(
                mWkInfo,
                workspaceWindow,
                switcher,
                mergeViewLauncher,
                this,
                mProgressControls,
                workspaceWindow,
                null,
                null,
                null);

            InitIgnoreRulesAndRefreshView(mWkInfo.ClientPath, this);
        }

        internal void AutoRefresh()
        {
            if (mIsAutoRefreshDisabled)
                return;

            if (!PlasticGuiConfig.Get().Configuration.CommitAutoRefresh)
                return;

            if (mIsRefreshing || mWorkspaceWindow.IsOperationInProgress())
                return;

            if (mNewChangesInWk != null && !mNewChangesInWk.Detected())
                return;

            ((IRefreshableView)this).Refresh();
        }

        internal void ClearIsCommentWarningNeeded()
        {
            mIsEmptyCheckinCommentWarningNeeded = false;
        }

        internal void UpdateIsCommentWarningNeeded(string comment)
        {
            mIsEmptyCheckinCommentWarningNeeded =
                string.IsNullOrEmpty(comment) &&
                GuiClientConfig.Get().Configuration.ShowEmptyCommentWarning;

            mNeedsToShowEmptyCommentDialog = mIsEmptyCheckinCommentWarningNeeded;
        }

        internal void OnDisable()
        {
            mSearchField.downOrUpArrowKeyPressed -=
                SearchField_OnDownOrUpArrowKeyPressed;

            TreeHeaderSettings.Save(
                mPendingChangesTreeView.multiColumnHeader.state,
                UnityConstants.PENDING_CHANGES_TABLE_SETTINGS_NAME);
        }

        internal void Update()
        {
            mProgressControls.UpdateProgress(mParentWindow);

            // Displaying the dialog here, because showing it during the window's OnGUI function
            // caused errors
            if(mNeedsToShowEmptyCommentDialog)
            {
                mNeedsToShowEmptyCommentDialog = false;

                mHasPendingCheckinFromPreviousUpdate =
                    EmptyCheckinMessageDialog.ShouldContinueWithCheckin(mParentWindow, mWkInfo);

                mIsEmptyCheckinCommentWarningNeeded = !mHasPendingCheckinFromPreviousUpdate;
            }
        }

        internal void OnGUI()
        {
            if (!string.IsNullOrEmpty(mGluonWarningMessage))
                DoWarningMessage(mGluonWarningMessage);

            DoActionsToolbar(
                mWkInfo,
                mIsGluonMode,
                mProgressControls);

            DoChangesArea(
                mWkInfo,
                mPendingChangesTreeView,
                mProgressControls.IsOperationRunning(),
                mIsGluonMode,
                mIsCheckedInSuccessful);

            if (HasPendingMergeLinks() && !mHasPendingMergeLinksFromRevert)
                DoMergeLinksArea(mMergeLinksListView, mParentWindow.position.width);

            // Border
            Rect result = GUILayoutUtility.GetRect(mParentWindow.position.width, 1);
            EditorGUI.DrawRect(result, mBorderColor);

            DoCommentsSection();

            if (mProgressControls.HasNotification())
            {
                DrawProgressForViews.ForNotificationArea(mProgressControls.ProgressData);
            }

            DrawHelpPanel.For(mHelpPanel);
        }

        internal void DrawSearchFieldForPendingChangesTab()
        {
            DrawSearchField.For(
                mSearchField,
                mPendingChangesTreeView,
                UnityConstants.SEARCH_FIELD_WIDTH);
        }

        void IPendingChangesView.SetDefaultComment(string defaultComment)
        {
        }

        void IPendingChangesView.ClearComments()
        {
            ClearComments();
        }

        void IRefreshableView.Refresh()
        {
            if (!mAreIgnoreRulesInitialized)
                return;

            if (mDeveloperNewIncomingChangesUpdater != null)
                mDeveloperNewIncomingChangesUpdater.Update(DateTime.Now);

            if (mGluonNewIncomingChangesUpdater != null)
                mGluonNewIncomingChangesUpdater.Update(DateTime.Now);

            FillPendingChanges(mNewChangesInWk);
        }

        void PlasticProjectSettingsProvider.IAutoRefreshView.DisableAutoRefresh()
        {
            mIsAutoRefreshDisabled = true;
        }

        void PlasticProjectSettingsProvider.IAutoRefreshView.EnableAutoRefresh()
        {
            mIsAutoRefreshDisabled = false;
        }

        void PlasticProjectSettingsProvider.IAutoRefreshView.ForceRefresh()
        {
            ((IRefreshableView)this).Refresh();
        }

        void IPendingChangesView.ClearChangesToCheck(List<string> changes)
        {
            mCheckedStateManager.ClearChangesToCheck(changes);

            mParentWindow.Repaint();
        }

        void IPendingChangesView.CleanCheckedElements(List<ChangeInfo> checkedChanges)
        {
            mCheckedStateManager.Clean(checkedChanges);

            mParentWindow.Repaint();
        }

        void IPendingChangesView.CheckChanges(List<string> changesToCheck)
        {
            mCheckedStateManager.SetChangesToCheck(changesToCheck);

            mParentWindow.Repaint();
        }

        bool IPendingChangesView.IncludeDependencies(
            IList<ChangeDependencies<ChangeInfo>> changesDependencies,
            string operation)
        {
            return DependenciesDialog.IncludeDependencies(
                mWkInfo, changesDependencies, operation, mParentWindow);
        }

        SearchMatchesData IPendingChangesView.AskForMatches(string changePath)
        {
            throw new NotImplementedException();
        }

        void IPendingChangesView.CleanLinkedTasks()
        {
        }

        void CheckinUIOperation.ICheckinView.CollapseWarningMessagePanel()
        {
            mGluonWarningMessage = string.Empty;

            mParentWindow.Repaint();
        }

        void CheckinUIOperation.ICheckinView.ExpandWarningMessagePanel(string text)
        {
            mGluonWarningMessage = text;

            mParentWindow.Repaint();
        }

        void CheckinUIOperation.ICheckinView.ClearComments()
        {
            ClearComments();
        }

        bool PendingChangesViewPendingChangeMenu.IMetaMenuOperations.SelectionHasMeta()
        {
            return mPendingChangesTreeView.SelectionHasMeta();
        }

        void PendingChangesViewPendingChangeMenu.IMetaMenuOperations.DiffMeta()
        {
            if (mShowDownloadPlasticExeWindow.Show(
                mWkInfo,
                mIsGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromDiffWorkspaceContent,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromDiffWorkspaceContent,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromDiffWorkspaceContent))
                return;

            ChangeInfo selectedChange = PendingChangesSelection
                .GetSelectedChange(mPendingChangesTreeView);
            ChangeInfo selectedChangeMeta = mPendingChangesTreeView.GetMetaChange(
                selectedChange);

            ChangeInfo changedForMoved = mPendingChangesTreeView.GetChangedForMoved(selectedChange);
            ChangeInfo changedForMovedMeta = (changedForMoved == null) ?
                null : mPendingChangesTreeView.GetMetaChange(changedForMoved);

            DiffOperation.DiffWorkspaceContent(
                mWkInfo,
                selectedChangeMeta,
                changedForMovedMeta,
                mProgressControls,
                null,
                null);
        }

        void PendingChangesViewPendingChangeMenu.IMetaMenuOperations.HistoryMeta()
        {
            ChangeInfo selectedChange = PendingChangesSelection
                .GetSelectedChange(mPendingChangesTreeView);
            ChangeInfo selectedChangeMeta = mPendingChangesTreeView.GetMetaChange(
                selectedChange);

            mHistoryViewLauncher.ShowHistoryView(
                selectedChangeMeta.RepositorySpec,
                selectedChangeMeta.RevInfo.ItemId,
                selectedChangeMeta.Path,
                selectedChangeMeta.IsDirectory);
        }

        void PendingChangesViewPendingChangeMenu.IMetaMenuOperations.OpenMeta()
        {
            List<string> selectedPaths = PendingChangesSelection
                .GetSelectedMetaPaths(mPendingChangesTreeView);

            FileSystemOperation.Open(selectedPaths);
        }

        void PendingChangesViewPendingChangeMenu.IMetaMenuOperations.OpenMetaWith()
        {
            List<string> selectedPaths = PendingChangesSelection
                .GetSelectedMetaPaths(mPendingChangesTreeView);

            OpenOperation.OpenWith(
                FileSystemOperation.GetExePath(),
                selectedPaths);
        }

        void PendingChangesViewPendingChangeMenu.IMetaMenuOperations.OpenMetaInExplorer()
        {
            List<string> selectedPaths = PendingChangesSelection
                .GetSelectedMetaPaths(mPendingChangesTreeView);

            if (selectedPaths.Count < 1)
                return;

            FileSystemOperation.OpenInExplorer(selectedPaths[0]);
        }

        SelectedChangesGroupInfo IPendingChangesMenuOperations.GetSelectedChangesGroupInfo()
        {
            return PendingChangesSelection.GetSelectedChangesGroupInfo(
                mWkInfo.ClientPath, mPendingChangesTreeView);
        }

        void IPendingChangesMenuOperations.Diff()
        {
            if (mShowDownloadPlasticExeWindow.Show(
                mWkInfo,
                mIsGluonMode,
                TrackFeatureUseEvent.Features.InstallPlasticCloudFromDiffWorkspaceContent,
                TrackFeatureUseEvent.Features.InstallPlasticEnterpriseFromDiffWorkspaceContent,
                TrackFeatureUseEvent.Features.CancelPlasticInstallationFromDiffWorkspaceContent))
                return;

            ChangeInfo selectedChange = PendingChangesSelection
                .GetSelectedChange(mPendingChangesTreeView);

            DiffOperation.DiffWorkspaceContent(
                mWkInfo,
                selectedChange,
                mPendingChangesTreeView.GetChangedForMoved(selectedChange),
                null,
                null,
                null);
        }

        void IPendingChangesMenuOperations.UndoChanges()
        {
            List<ChangeInfo> changesToUndo = PendingChangesSelection
                .GetSelectedChanges(mPendingChangesTreeView);

            List<ChangeInfo> dependenciesCandidates =
                mPendingChangesTreeView.GetDependenciesCandidates(changesToUndo, true);

            UndoChangesForMode(
                mWkInfo, mIsGluonMode,
                changesToUndo, dependenciesCandidates);
        }

        void IPendingChangesMenuOperations.SearchMatches()
        {
            ChangeInfo selectedChange = PendingChangesSelection
                .GetSelectedChange(mPendingChangesTreeView);

            if (selectedChange == null)
                return;

            SearchMatchesOperation operation = new SearchMatchesOperation(
                mWkInfo, mWorkspaceWindow, this,
                mProgressControls, mDeveloperNewIncomingChangesUpdater);

            operation.SearchMatches(
                selectedChange,
                PendingChangesSelection.GetAllChanges(mPendingChangesTreeView),
                null);
        }

        void IPendingChangesMenuOperations.ApplyLocalChanges()
        {
            List<ChangeInfo> selectedChanges = PendingChangesSelection
                .GetSelectedChanges(mPendingChangesTreeView);

            if (selectedChanges.Count == 0)
                return;

            ApplyLocalChangesOperation operation = new ApplyLocalChangesOperation(
                mWkInfo, mWorkspaceWindow, this,
                mProgressControls, mDeveloperNewIncomingChangesUpdater);

            operation.ApplyLocalChanges(
                selectedChanges,
                PendingChangesSelection.GetAllChanges(mPendingChangesTreeView),
                null);
        }

        void IPendingChangesMenuOperations.Delete()
        {
            List<string> privateDirectoriesToDelete;
            List<string> privateFilesToDelete;

            if (!mPendingChangesTreeView.GetSelectedPathsToDelete(
                    out privateDirectoriesToDelete,
                    out privateFilesToDelete))
                return;

            DeleteOperation.Delete(
                mWorkspaceWindow, mProgressControls,
                privateDirectoriesToDelete, privateFilesToDelete,
                mDeveloperNewIncomingChangesUpdater,
                RefreshAsset.UnityAssetDatabase);
        }

        void IPendingChangesMenuOperations.Annotate()
        {
            throw new NotImplementedException();
        }

        void IPendingChangesMenuOperations.History()
        {
            ChangeInfo selectedChange = PendingChangesSelection.
                GetSelectedChange(mPendingChangesTreeView);

            mHistoryViewLauncher.ShowHistoryView(
                selectedChange.RepositorySpec,
                selectedChange.RevInfo.ItemId,
                selectedChange.Path,
                selectedChange.IsDirectory);
        }

        SelectedChangesGroupInfo IChangelistMenuOperations.GetSelectedChangesGroupInfo()
        {
            return PendingChangesSelection.GetSelectedChangesGroupInfo(
                mWkInfo.ClientPath, mPendingChangesTreeView);
        }

        List<ChangeListInfo> IChangelistMenuOperations.GetSelectedChangelistInfos()
        {
            return PendingChangesSelection.GetSelectedChangeListInfos(
                mPendingChangesTreeView);
        }

        void IChangelistMenuOperations.Checkin()
        {
            List<ChangeInfo> changesToCheckin;
            List<ChangeInfo> dependenciesCandidates;

            mPendingChangesTreeView.GetCheckedChanges(
                PendingChangesSelection.GetSelectedChangelistNodes(mPendingChangesTreeView),
                false, out changesToCheckin, out dependenciesCandidates);

            CheckinChangesForMode(
                changesToCheckin, dependenciesCandidates,
                mWkInfo, mIsGluonMode, mKeepItemsLocked);
        }

        void IChangelistMenuOperations.Shelve()
        {
            if (mIsGluonMode)
                return;

            List<ChangeInfo> changesToShelve;
            List<ChangeInfo> dependenciesCandidates;

            mPendingChangesTreeView.GetCheckedChanges(
                PendingChangesSelection.GetSelectedChangelistNodes(mPendingChangesTreeView),
                false, out changesToShelve, out dependenciesCandidates);

            ShelveChanges(changesToShelve, dependenciesCandidates, mWkInfo);
        }

        void IChangelistMenuOperations.Undo()
        {
            List<ChangeInfo> changesToUndo;
            List<ChangeInfo> dependenciesCandidates;

            mPendingChangesTreeView.GetCheckedChanges(
                PendingChangesSelection.GetSelectedChangelistNodes(mPendingChangesTreeView),
                true, out changesToUndo, out dependenciesCandidates);

            UndoChangesForMode(
                mWkInfo, mIsGluonMode, 
                changesToUndo, dependenciesCandidates);
        }

        void IChangelistMenuOperations.CreateNew()
        {
            ChangelistCreationData changelistCreationData = 
                CreateChangelistDialog.CreateChangelist(mWkInfo, mParentWindow);

            ChangelistOperations.CreateNew(mWkInfo, this, changelistCreationData);
        }

        void IChangelistMenuOperations.MoveToNewChangelist(List<ChangeInfo> changes)
        {
            ChangelistCreationData changelistCreationData =
                CreateChangelistDialog.CreateChangelist(mWkInfo, mParentWindow);

            if (!changelistCreationData.Result)
                return;

            ChangelistOperations.CreateNew(mWkInfo, this, changelistCreationData);

            ChangelistOperations.MoveToChangelist(
                mWkInfo, this, changes, 
                changelistCreationData.ChangelistInfo.Name);
        }

        void IChangelistMenuOperations.Edit()
        {
            ChangeListInfo changelistToEdit = PendingChangesSelection.GetSelectedChangeListInfo(
                mPendingChangesTreeView);

            ChangelistCreationData changelistCreationData = CreateChangelistDialog.EditChangelist(
                mWkInfo,
                changelistToEdit,
                mParentWindow);

            ChangelistOperations.Edit(mWkInfo, this, changelistToEdit, changelistCreationData);
        }

        void IChangelistMenuOperations.Delete()
        {
            ChangelistOperations.Delete(
                mWkInfo,
                this,
                PendingChangesSelection.GetSelectedChangelistNodes(mPendingChangesTreeView));
        }

        void IChangelistMenuOperations.MoveToChangelist(
            List<ChangeInfo> changes,
            string targetChangelist)
        {
            ChangelistOperations.MoveToChangelist(
                mWkInfo,
                this,
                changes,
                targetChangelist);
        }

        void IOpenMenuOperations.Open()
        {
            List<string> selectedPaths = PendingChangesSelection.
                GetSelectedPathsWithoutMeta(mPendingChangesTreeView);

            FileSystemOperation.Open(selectedPaths);
        }

        void IOpenMenuOperations.OpenWith()
        {
            List<string> selectedPaths = PendingChangesSelection.
                GetSelectedPathsWithoutMeta(mPendingChangesTreeView);

            OpenOperation.OpenWith(
                FileSystemOperation.GetExePath(),
                selectedPaths);
        }

        void IOpenMenuOperations.OpenWithCustom(string exePath, string args)
        {
            List<string> selectedPaths = PendingChangesSelection.
                GetSelectedPathsWithoutMeta(mPendingChangesTreeView);

            OpenOperation.OpenWith(exePath, selectedPaths);
        }

        void IOpenMenuOperations.OpenInExplorer()
        {
            List<string> selectedPaths = PendingChangesSelection
                .GetSelectedPathsWithoutMeta(mPendingChangesTreeView);

            if (selectedPaths.Count < 1)
                return;

            FileSystemOperation.OpenInExplorer(selectedPaths[0]);
        }

        void IFilesFilterPatternsMenuOperations.AddFilesFilterPatterns(
            FilterTypes type, FilterActions action, FilterOperationType operation)
        {
            List<string> selectedPaths = PendingChangesSelection.GetSelectedPaths(
                mPendingChangesTreeView);

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

        List<IPlasticTreeNode> PendingChangesViewMenu.IGetSelectedNodes.GetSelectedNodes()
        {
            return mPendingChangesTreeView.GetSelectedNodes();
        }

        void ChangesetsTab.IRevertToChangesetListener.OnSuccessOperation()
        {
            mHasPendingMergeLinksFromRevert = true;
        }

        void SearchField_OnDownOrUpArrowKeyPressed()
        {
            mPendingChangesTreeView.SetFocusAndEnsureSelectedItem();
        }

        void InitIgnoreRulesAndRefreshView(
            string wkPath, IRefreshableView view)
        {
            IThreadWaiter waiter = ThreadWaiter.GetWaiter(10);
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    if (IsIgnoreConfigDefined.For(wkPath))
                        return;

                    AddIgnoreRules.WriteRules(
                        wkPath, UnityConditions.GetMissingIgnoredRules(wkPath));
                },
                /*afterOperationDelegate*/ delegate
                {
                    mAreIgnoreRulesInitialized = true;

                    view.Refresh();

                    if (waiter.Exception == null)
                        return;

                    mLog.ErrorFormat(
                        "Error adding ignore rules for Unity: {0}",
                        waiter.Exception);

                    mLog.DebugFormat(
                        "Stack trace: {0}",
                        waiter.Exception.StackTrace);
                });
        }

        void FillPendingChanges(INewChangesInWk newChangesInWk)
        {
            if (mIsRefreshing)
                return;

            mIsRefreshing = true;

            List<ChangeInfo> changesToSelect =
                PendingChangesSelection.GetChangesToFocus(mPendingChangesTreeView);

            ((IProgressControls)mProgressControls).ShowProgress(PlasticLocalization.
                GetString(PlasticLocalization.Name.LoadingPendingChanges));

            IDictionary<MountPoint, IList<PendingMergeLink>> mergeLinks = null;

            WorkspaceStatusResult status = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    FilterManager.Get().Reload(mWkInfo);

                    WorkspaceStatusOptions options = WorkspaceStatusOptions.None;
                    options |= WorkspaceStatusOptions.FindAdded;
                    options |= WorkspaceStatusOptions.FindDeleted;
                    options |= WorkspaceStatusOptions.FindMoved;
                    options |= WorkspaceStatusOptions.SplitModifiedMoved;
                    options |= PendingChangesOptions.GetWorkspaceStatusOptions();

                    if (newChangesInWk != null)
                        newChangesInWk.Detected();

                    status = GetStatus.ForWorkspace(
                        mWkInfo,
                        options,
                        PendingChangesOptions.GetMovedMatchingOptions());

                    mergeLinks = PlasticGui.Plastic.API.GetPendingMergeLinks(mWkInfo);
                },
                /*afterOperationDelegate*/ delegate
                {
                    mPendingMergeLinks = mergeLinks;

                    try
                    {
                        if (waiter.Exception != null)
                        {
                            ExceptionsHandler.DisplayException(waiter.Exception);
                            return;
                        }

                        UpdateChangesTree(status.Changes);

                        UpdateMergeLinksList();

                        PendingChangesSelection.SelectChanges(
                            mPendingChangesTreeView, changesToSelect);

                        DrawAssetOverlay.ClearCache();
                    }
                    finally
                    {
                        ((IProgressControls)mProgressControls).HideProgress();

                        //UpdateIsCommentWarningNeeded(CommentText);

                        UpdateNotificationPanel();

                        mIsRefreshing = false;
                    }
                });
        }

        void DoCommentsSection()
        {
            EditorGUILayout.BeginVertical(UnityStyles.PendingChangesTab.Comment);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            DrawUserIcon.ForPendingChangesTab(CommentText);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            float width = Mathf.Clamp(mParentWindow.position.width, 300f, 820f);

            DrawCommentTextArea.For(
               this,
               width,
               mProgressControls.IsOperationRunning());

            EditorGUILayout.Space(2);

            // To center the action buttons vertically
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            DoOperationsToolbar(
              mWkInfo,
              mIsGluonMode,
              mAdvancedDropdownMenu,
              mProgressControls.IsOperationRunning());
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        void DoOperationsToolbar(
            WorkspaceInfo wkInfo,
            bool isGluonMode,
            GenericMenu advancedDropdownMenu,
            bool isOperationRunning)
        {
            EditorGUILayout.BeginHorizontal();

            using (new GuiEnabled(!isOperationRunning))
            {
                if(mHasPendingCheckinFromPreviousUpdate)
                {
                    mHasPendingCheckinFromPreviousUpdate = false;
                    CheckinForMode(wkInfo, isGluonMode, mKeepItemsLocked);
                }

                else if (DrawActionButton.ForCommentSection(
                        PlasticLocalization.GetString(
                            PlasticLocalization.Name.CheckinChanges)))
                {
                    UpdateIsCommentWarningNeeded(CommentText);

                    if (!mIsEmptyCheckinCommentWarningNeeded &&
                        mPendingChangesTreeView.GetCheckedItemCount() > 0)
                    {
                        CheckinForMode(wkInfo, isGluonMode, mKeepItemsLocked);
                    }
                }

                GUILayout.Space(2);

                if (DrawActionButton.ForCommentSection(PlasticLocalization.GetString(
                        PlasticLocalization.Name.UndoChanges)))
                {
                    TrackFeatureUseEvent.For(PlasticGui.Plastic.API.GetRepositorySpec(wkInfo),
                        TrackFeatureUseEvent.Features.UndoTextButton);
                    UndoForMode(wkInfo, isGluonMode);
                }

                if (isGluonMode)
                {
                    mKeepItemsLocked = EditorGUILayout.ToggleLeft(
                        PlasticLocalization.GetString(PlasticLocalization.Name.KeepLocked),
                        mKeepItemsLocked,
                        GUILayout.Width(UnityConstants.EXTRA_LARGE_BUTTON_WIDTH));
                }
                //TODO: Codice - beta: hide the advanced menu until the behavior is implemented
                /*else
                {
                    var dropDownContent = new GUIContent(string.Empty);
                    var dropDownRect = GUILayoutUtility.GetRect(
                        dropDownContent, EditorStyles.toolbarDropDown);

                    if (EditorGUI.DropdownButton(dropDownRect, dropDownContent,
                            FocusType.Passive, EditorStyles.toolbarDropDown))
                        advancedDropdownMenu.DropDown(dropDownRect);
                }*/
            }

            EditorGUILayout.EndHorizontal();
        }

        void UpdateChangesTree(List<ChangeInfo> changes)
        {
            mPendingChangesTreeView.BuildModel(changes, mCheckedStateManager);

            mPendingChangesTreeView.Refilter();

            mPendingChangesTreeView.Sort();

            mPendingChangesTreeView.Reload();
        }

        static void DoWarningMessage(string message)
        {
            GUILayout.Label(message, UnityStyles.WarningMessage);
        }

        void UpdateMergeLinksList()
        {
            mMergeLinksListView.BuildModel(mPendingMergeLinks);

            mMergeLinksListView.Reload();

            if (!HasPendingMergeLinks())
                mHasPendingMergeLinksFromRevert = false;
        }

        void UpdateNotificationPanel()
        {
            if (PlasticGui.Plastic.API.IsFsReaderWatchLimitReached(mWkInfo))
            {
                ((IProgressControls)mProgressControls).ShowWarning(PlasticLocalization.
                    GetString(PlasticLocalization.Name.NotifyLinuxWatchLimitWarning));
                return;
            }
        }

        void DoActionsToolbar(
            WorkspaceInfo workspaceInfo,
            bool isGluonMode,
            ProgressControlsForViews progressControls)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (progressControls.IsOperationRunning())
            {
                DrawProgressForViews.ForIndeterminateProgress(
                    progressControls.ProgressData);
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        static void DoChangesArea(
            WorkspaceInfo wkInfo,
            PendingChangesTreeView changesTreeView,
            bool isOperationRunning,
            bool isGluonMode,
            bool isCheckedInSuccessful)
        {
            GUI.enabled = !isOperationRunning;

            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
            changesTreeView.OnGUI(rect);

            if (changesTreeView.GetTotalItemCount() == 0)
            {
                DrawEmptyState(
                    rect,
                    isCheckedInSuccessful);
            }

            GUI.enabled = true;
        }

        static void DrawEmptyState(
            Rect rect,
            bool isCheckedInSuccessful)
        {
            if (isCheckedInSuccessful)
            {
                DrawTreeViewEmptyState.For(
                    rect,
                    PlasticLocalization.GetString(PlasticLocalization.Name.CheckinCompleted),
                    Images.GetStepOkIcon());
                return;
            }

            DrawTreeViewEmptyState.For(
                rect,
                PlasticLocalization.GetString(PlasticLocalization.Name.NoPendingChanges));
        }

        bool HasPendingMergeLinks()
        {
            if (mPendingMergeLinks == null)
                return false;

            return mPendingMergeLinks.Count > 0;
        }

        static void DoMergeLinksArea(
            MergeLinksListView mergeLinksListView, float width)
        {
            GUILayout.Label(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.MergeLinkDescriptionColumn),
                EditorStyles.boldLabel);

            float desiredTreeHeight = mergeLinksListView.DesiredHeight;

            Rect treeRect = GUILayoutUtility.GetRect(
                0,
                width,
                desiredTreeHeight,
                desiredTreeHeight);

            mergeLinksListView.OnGUI(treeRect);
        }

        void BuildComponents(
            bool isGluonMode,
            EditorWindow plasticWindow)
        {
            mHelpPanel = new HelpPanel(plasticWindow);

            mAdvancedDropdownMenu = new GenericMenu();
            mAdvancedDropdownMenu.AddItem(new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.UndoUnchangedButton)),
                false, () => { });

            mSearchField = new SearchField();
            mSearchField.downOrUpArrowKeyPressed += SearchField_OnDownOrUpArrowKeyPressed;

            PendingChangesTreeHeaderState headerState =
                PendingChangesTreeHeaderState.GetDefault(isGluonMode);
            TreeHeaderSettings.Load(headerState,
                UnityConstants.PENDING_CHANGES_TABLE_SETTINGS_NAME,
                (int)PendingChangesTreeColumn.Item, true);

            mPendingChangesTreeView = new PendingChangesTreeView(
                mWkInfo, mIsGluonMode, headerState,
                PendingChangesTreeHeaderState.GetColumnNames(),
                new PendingChangesViewMenu(mWkInfo, this, this, this, this, this, this, mIsGluonMode),
                mAssetStatusCache);
            mPendingChangesTreeView.Reload();

            mMergeLinksListView = new MergeLinksListView();
            mMergeLinksListView.Reload();
        }
        INewChangesInWk mNewChangesInWk;
        GenericMenu mAdvancedDropdownMenu;

        void ClearComments()
        {
            CommentText = string.Empty;
            ForceToShowComment = true;

            mParentWindow.Repaint();
        }

        PendingChangesTreeView mPendingChangesTreeView;
        MergeLinksListView mMergeLinksListView;
        HelpPanel mHelpPanel;

        volatile bool mAreIgnoreRulesInitialized = false;
        bool mIsRefreshing;

        bool mIsAutoRefreshDisabled;
        bool mIsEmptyCheckinCommentWarningNeeded = false;
        bool mNeedsToShowEmptyCommentDialog = false;
        bool mHasPendingCheckinFromPreviousUpdate = false;
        bool mHasPendingMergeLinksFromRevert = false;
        bool mKeepItemsLocked;
        string mGluonWarningMessage;
        bool mIsCheckedInSuccessful;

        IDictionary<MountPoint, IList<PendingMergeLink>> mPendingMergeLinks;

        SearchField mSearchField;

        Color mBorderColor;

        readonly ProgressControlsForViews mProgressControls;
        readonly EditorWindow mParentWindow;
        readonly StatusBar mStatusBar;
        readonly CooldownWindowDelayer mCooldownClearCheckinSuccessAction;
        readonly IAssetStatusCache mAssetStatusCache;

        readonly PendingChangesOperations mPendingChangesOperations;
        readonly PendingChangesViewCheckedStateManager mCheckedStateManager;
        readonly GuiMessage.IGuiMessage mGuiMessage;
        readonly NewIncomingChangesUpdater mDeveloperNewIncomingChangesUpdater;
        readonly GluonNewIncomingChangesUpdater mGluonNewIncomingChangesUpdater;
        readonly bool mIsGluonMode;
        readonly LaunchTool.IShowDownloadPlasticExeWindow mShowDownloadPlasticExeWindow;
        readonly IHistoryViewLauncher mHistoryViewLauncher;
        readonly WorkspaceWindow mWorkspaceWindow;
        readonly ViewHost mViewHost;
        readonly WorkspaceInfo mWkInfo;

        static readonly ILog mLog = LogManager.GetLogger("PendingChangesTab");
    }
}