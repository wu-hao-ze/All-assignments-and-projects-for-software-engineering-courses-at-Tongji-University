using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.Client.Common.Threading;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Update;
using PlasticGui.WorkspaceWindow.QueryViews;
using PlasticGui.WorkspaceWindow.QueryViews.Changesets;
using GluonGui;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.UI.Tree;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.Views.Diff;

using IGluonUpdateReport = PlasticGui.Gluon.IUpdateReport;

namespace Unity.PlasticSCM.Editor.Views.Changesets
{
    internal partial class ChangesetsTab :
        IRefreshableView,
        IQueryRefreshableView,
        IChangesetMenuOperations,
        ChangesetsViewMenu.IMenuOperations
    {
        internal ChangesetsListView Table { get { return mChangesetsListView; } }
        internal IChangesetMenuOperations Operations { get { return this; } }

        internal interface IRevertToChangesetListener
        {
            void OnSuccessOperation();
        }

        internal ChangesetsTab(
            WorkspaceInfo wkInfo,
            WorkspaceWindow workspaceWindow,
            IViewSwitcher viewSwitcher,
            IMergeViewLauncher mergeViewLauncher,
            IHistoryViewLauncher historyViewLauncher,
            ViewHost viewHost,
            IUpdateReport updateReport,
            IGluonUpdateReport gluonUpdateReport,
            NewIncomingChangesUpdater newIncomingChangesUpdater,
            IRevertToChangesetListener revertToChangesetListener,
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            LaunchTool.IProcessExecutor processExecutor,
            EditorWindow parentWindow,
            bool isGluonMode)
        {
            mWkInfo = wkInfo;
            mViewHost = viewHost;
            mWorkspaceWindow = workspaceWindow;
            mViewSwitcher = viewSwitcher;
            mRevertToChangesetListener = revertToChangesetListener;
            mShowDownloadPlasticExeWindow = showDownloadPlasticExeWindow;
            mProcessExecutor = processExecutor;
            mParentWindow = parentWindow;
            mIsGluonMode = isGluonMode;
            mGluonUpdateReport = gluonUpdateReport;

            BuildComponents(
                wkInfo,
                workspaceWindow,
                workspaceWindow,
                viewSwitcher,
                historyViewLauncher,
                parentWindow);

            mProgressControls = new ProgressControlsForViews();

            mSplitterState = PlasticSplitterGUILayout.InitSplitterState(
                new float[] { 0.50f, 0.50f },
                new int[] { 100, (int)UnityConstants.DIFF_PANEL_MIN_WIDTH },
                new int[] { 100000, 100000 }
            );

            mBorderColor = EditorGUIUtility.isProSkin
                ? (Color)new Color32(35, 35, 35, 255)
                : (Color)new Color32(153, 153, 153, 255);

            mChangesetOperations = new ChangesetOperations(
                wkInfo,
                workspaceWindow,
                viewSwitcher,
                mergeViewLauncher,
                this,
                ViewType.ChangesetsView,
                mProgressControls,
                updateReport,
                new ContinueWithPendingChangesQuestionerBuilder(viewSwitcher, parentWindow),
                newIncomingChangesUpdater,
                null,
                null);

            ((IRefreshableView)this).Refresh();
        }

        internal void OnDisable()
        {
            mDiffPanel.OnDisable();

            mSearchField.downOrUpArrowKeyPressed -=
                SearchField_OnDownOrUpArrowKeyPressed;

            TreeHeaderSettings.Save(
                mChangesetsListView.multiColumnHeader.state,
                UnityConstants.CHANGESETS_TABLE_SETTINGS_NAME);
        }

        internal void Update()
        {
            mDiffPanel.Update();

            mProgressControls.UpdateProgress(mParentWindow);
        }

        internal void OnGUI()
        {
            InitializeShowChangesButtonWidth();

            DoActionsToolbar(
                this,
                mProgressControls,
                mSearchField,
                mChangesetsListView,
                mDateFilter);

            PlasticSplitterGUILayout.BeginHorizontalSplit(mSplitterState);

            DoChangesetsArea(
                mChangesetsListView,
                mProgressControls.IsOperationRunning());

            EditorGUILayout.BeginHorizontal();

            Rect border = GUILayoutUtility.GetRect(1, 0, 1, 100000);
            EditorGUI.DrawRect(border, mBorderColor);

            DoChangesArea(mDiffPanel);

            EditorGUILayout.EndHorizontal();

            PlasticSplitterGUILayout.EndHorizontalSplit();

        }

        internal void DrawSearchFieldForChangesetsTab()
        {
            DrawSearchField.For(
                mSearchField,
                mChangesetsListView,
                UnityConstants.SEARCH_FIELD_WIDTH);

            VerifyIfSearchFieldIsRecentlyFocused(mSearchField);
        }

        internal void SetWorkingObjectInfo(WorkingObjectInfo homeInfo)
        {
            if (mIsGluonMode)
                return;

            lock (mLock)
            {
                mLoadedChangesetId = homeInfo.GetChangesetId();
            }

            mChangesetsListView.SetLoadedChangesetId(mLoadedChangesetId);
            mChangesetsViewMenu.SetLoadedBranchId(homeInfo.BranchInfo.BranchId);
        }

        internal void SetRevertToChangesetOperationInterfacesForTesting(
            RevertToChangesetOperation.IGetStatusForWorkspace getStatusForWorkspace,
            RevertToChangesetOperation.IUndoCheckoutOperation undoCheckoutOperation,
            RevertToChangesetOperation.IRevertToChangesetMergeController revertToChangesetMergeController)
        {
            mGetStatusForWorkspace = getStatusForWorkspace;
            mUndoCheckoutOperation = undoCheckoutOperation;
            mRevertToChangesetMergeController = revertToChangesetMergeController;
        }

        internal void SetLaunchToolForTesting(
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            LaunchTool.IProcessExecutor processExecutor)
        {
            mShowDownloadPlasticExeWindow = showDownloadPlasticExeWindow;
            mProcessExecutor = processExecutor;
        }

        void IRefreshableView.Refresh()
        {
            string query = GetChangesetsQuery(mDateFilter);

            FillChangesets(
                mWkInfo,
                query,
                ChangesetsSelection.GetSelectedRepObjectInfos(mChangesetsListView));
        }

        void IQueryRefreshableView.RefreshAndSelect(RepObjectInfo repObj)
        {
            string query = GetChangesetsQuery(mDateFilter);

            FillChangesets(
                mWkInfo,
                query,
                new List<RepObjectInfo> { repObj });
        }

        int IChangesetMenuOperations.GetSelectedChangesetsCount()
        {
            return ChangesetsSelection.GetSelectedChangesetsCount(mChangesetsListView);
        }

        void IChangesetMenuOperations.DiffChangeset()
        {
            LaunchDiffOperations.DiffChangeset(
                mShowDownloadPlasticExeWindow,
                mProcessExecutor,
                ChangesetsSelection.GetSelectedRepository(mChangesetsListView),
                ChangesetsSelection.GetSelectedChangeset(mChangesetsListView),
                mIsGluonMode);
        }

        void IChangesetMenuOperations.DiffSelectedChangesets()
        {
            List<RepObjectInfo> selectedChangesets = ChangesetsSelection.
                GetSelectedRepObjectInfos(mChangesetsListView);

            if (selectedChangesets.Count < 2)
                return;

            LaunchDiffOperations.DiffSelectedChangesets(
                mShowDownloadPlasticExeWindow,
                mProcessExecutor,
                ChangesetsSelection.GetSelectedRepository(mChangesetsListView),
                (ChangesetExtendedInfo)selectedChangesets[0],
                (ChangesetExtendedInfo)selectedChangesets[1],
                mIsGluonMode);
        }

        void IChangesetMenuOperations.SwitchToChangeset()
        {
            SwitchToChangesetForMode(mIsGluonMode);
        }

        void IChangesetMenuOperations.DiffWithAnotherChangeset() { }
        void IChangesetMenuOperations.CreateBranch() { }
        void IChangesetMenuOperations.LabelChangeset() { }
        void IChangesetMenuOperations.MergeChangeset() {}
        void IChangesetMenuOperations.CherryPickChangeset() { }
        void IChangesetMenuOperations.SubtractiveChangeset() { }
        void IChangesetMenuOperations.SubtractiveChangesetInterval() { }
        void IChangesetMenuOperations.CherryPickChangesetInterval() { }
        void IChangesetMenuOperations.MergeToChangeset() { }
        void IChangesetMenuOperations.MoveChangeset() { }
        void IChangesetMenuOperations.DeleteChangeset() { }
        void IChangesetMenuOperations.BrowseRepositoryOnChangeset() { }
        void IChangesetMenuOperations.CreateCodeReview() { }

        void IChangesetMenuOperations.RevertToChangeset()
        {
            if (((IChangesetMenuOperations)this).GetSelectedChangesetsCount() != 1)
                return;

            ChangesetExtendedInfo targetChangesetInfo = ((ChangesetsViewMenu.IMenuOperations)this).GetSelectedChangeset();

            RevertToChangesetOperation.RevertTo(
                mWkInfo,
                mViewSwitcher,
                mWorkspaceWindow,
                mProgressControls,
                mGetStatusForWorkspace,
                mUndoCheckoutOperation,
                mRevertToChangesetMergeController,
                GuiMessage.Get(),
                targetChangesetInfo,
                RefreshAsset.BeforeLongAssetOperation,
                RefreshAsset.AfterLongAssetOperation,
                mRevertToChangesetListener.OnSuccessOperation);
        }

        void SearchField_OnDownOrUpArrowKeyPressed()
        {
            mChangesetsListView.SetFocusAndEnsureSelectedItem();
        }

        void FillChangesets(WorkspaceInfo wkInfo, string query, List<RepObjectInfo> changesetsToSelect)
        {
            if (mIsRefreshing)
                return;

            mIsRefreshing = true;

            int defaultRow = TableViewOperations.
                GetFirstSelectedRow(mChangesetsListView);

            ((IProgressControls)mProgressControls).ShowProgress(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.LoadingChangesets));

            ViewQueryResult queryResult = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    long loadedChangesetId = GetLoadedChangesetId(
                        wkInfo, mIsGluonMode);

                    long loadedBranchId = GetLoadedBranchId(
                        wkInfo, loadedChangesetId, mIsGluonMode);

                    lock (mLock)
                    {
                        mLoadedChangesetId = loadedChangesetId;
                    }

                    mChangesetsViewMenu.SetLoadedBranchId(loadedBranchId);

                    queryResult = new ViewQueryResult(
                        PlasticGui.Plastic.API.FindQuery(wkInfo, query));
                },
                /*afterOperationDelegate*/ delegate
                {
                    try
                    {
                        if (waiter.Exception != null)
                        {
                            ExceptionsHandler.DisplayException(waiter.Exception);
                            return;
                        }

                        UpdateChangesetsList(
                            mChangesetsListView,
                            queryResult,
                            mLoadedChangesetId);

                        int changesetsCount = GetChangesetsCount(queryResult);

                        if (changesetsCount == 0)
                        {
                            mDiffPanel.ClearInfo();
                            return;
                        }

                        ChangesetsSelection.SelectChangesets(
                            mChangesetsListView, changesetsToSelect, defaultRow);
                    }
                    finally
                    {
                        ((IProgressControls)mProgressControls).HideProgress();
                        mIsRefreshing = false;
                    }
                });
        }

        void ChangesetsViewMenu.IMenuOperations.DiffBranch()
        {
            LaunchDiffOperations.DiffBranch(
                mShowDownloadPlasticExeWindow,
                mProcessExecutor,
                ChangesetsSelection.GetSelectedRepository(mChangesetsListView),
                ChangesetsSelection.GetSelectedChangeset(mChangesetsListView),
                mIsGluonMode);
        }

        ChangesetExtendedInfo ChangesetsViewMenu.IMenuOperations.GetSelectedChangeset()
        {
            return ChangesetsSelection.GetSelectedChangeset(
                mChangesetsListView);
        }

        void OnChangesetsListViewSizeChanged()
        {
            if (!mShouldScrollToSelection)
                return;

            mShouldScrollToSelection = false;
            TableViewOperations.ScrollToSelection(mChangesetsListView);
        }

        void OnSelectionChanged()
        {
            List<RepObjectInfo> selectedChangesets = ChangesetsSelection.
                GetSelectedRepObjectInfos(mChangesetsListView);

            if (selectedChangesets.Count != 1)
                return;

            mDiffPanel.UpdateInfo(
                MountPointWithPath.BuildWorkspaceRootMountPoint(
                    ChangesetsSelection.GetSelectedRepository(mChangesetsListView)),
                (ChangesetExtendedInfo)selectedChangesets[0]);
        }

        static void UpdateChangesetsList(
            ChangesetsListView changesetsListView,
            ViewQueryResult queryResult,
            long loadedChangesetId)
        {
            changesetsListView.BuildModel(
                queryResult,
                loadedChangesetId);

            changesetsListView.Refilter();

            changesetsListView.Sort();

            changesetsListView.Reload();
        }

        static long GetLoadedChangesetId(
            WorkspaceInfo wkInfo,
            bool isGluonMode)
        {
            if (isGluonMode)
                return -1;

            return PlasticGui.Plastic.API.GetLoadedChangeset(wkInfo);
        }

        static long GetLoadedBranchId(
            WorkspaceInfo wkInfo,
            long loadedChangesetId,
            bool isGluonMode)
        {
            if (isGluonMode)
                return -1;

            return PlasticGui.Plastic.API.GetChangesetInfoFromId(
                        wkInfo, loadedChangesetId).BranchId;
        }

        static string GetChangesetsQuery(DateFilter dateFilter)
        {
            if (dateFilter.FilterType == DateFilter.Type.AllTime)
                return QueryConstants.ChangesetsBeginningQuery;

            string whereClause = QueryConstants.GetDateWhereClause(
                dateFilter.GetFilterDate(DateTime.UtcNow));

            return string.Format("{0} {1}",
                QueryConstants.ChangesetsBeginningQuery,
                whereClause);
        }

        static int GetChangesetsCount(
            ViewQueryResult queryResult)
        {
            if (queryResult == null)
                return 0;

           return queryResult.Count();
        }

        void VerifyIfSearchFieldIsRecentlyFocused(SearchField searchField)
        {
            if (searchField.HasFocus() != mIsSearchFieldFocused)
            {
                mIsSearchFieldFocused = !mIsSearchFieldFocused;

                if (mIsSearchFieldFocused)
                {
                    TrackFeatureUseEvent.For(
                        PlasticGui.Plastic.API.GetRepositorySpec(mWkInfo),
                        TrackFeatureUseEvent.Features.ChangesetsViewChangesetsSearchBox);
                }
            }
        }

        void DoActionsToolbar(
            IRefreshableView refreshableView,
            ProgressControlsForViews progressControls,
            SearchField searchField,
            ChangesetsListView changesetsListView,
            DateFilter dateFilter)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (progressControls.IsOperationRunning())
            {
                DrawProgressForViews.ForIndeterminateProgress(
                    progressControls.ProgressData);
            }

            GUILayout.FlexibleSpace();

            GUILayout.Space(2);

            EditorGUILayout.EndHorizontal();
        }

        static void DoChangesetsArea(
            ChangesetsListView changesetsListView,
            bool isOperationRunning)
        {
            EditorGUILayout.BeginVertical();

            GUI.enabled = !isOperationRunning;

            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);

            changesetsListView.OnGUI(rect);

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        static void DoChangesArea(DiffPanel diffPanel)
        {
            EditorGUILayout.BeginVertical();

            diffPanel.OnGUI();

            EditorGUILayout.EndVertical();
        }

        internal void DrawDateFilter()
        {
            GUI.enabled = !mProgressControls.IsOperationRunning();

            EditorGUI.BeginChangeCheck();

            mDateFilter.FilterType = (DateFilter.Type)
                EditorGUILayout.EnumPopup(
                    mDateFilter.FilterType,
                    EditorStyles.toolbarDropDown,
                    GUILayout.Width(100));

            if (EditorGUI.EndChangeCheck())
            {
                EnumPopupSetting<DateFilter.Type>.Save(
                    mDateFilter.FilterType,
                    UnityConstants.CHANGESETS_DATE_FILTER_SETTING_NAME);

                ((IRefreshableView)this).Refresh();
            }

            GUI.enabled = true;
        }

        void InitializeShowChangesButtonWidth()
        {
            if (mShowChangesButtonWidth != -1)
                return;

            mShowChangesButtonWidth = MeasureMaxWidth.ForTexts(
                EditorStyles.toolbarButton,
                PlasticLocalization.GetString(PlasticLocalization.Name.HideChanges),
                PlasticLocalization.GetString(PlasticLocalization.Name.ShowChanges));
        }

        void BuildComponents(
            WorkspaceInfo wkInfo,
            IWorkspaceWindow workspaceWindow,
            IRefreshView refreshView,
            IViewSwitcher viewSwitcher,
            IHistoryViewLauncher historyViewLauncher,
            EditorWindow parentWindow)
        {
            mSearchField = new SearchField();
            mSearchField.downOrUpArrowKeyPressed += SearchField_OnDownOrUpArrowKeyPressed;

            DateFilter.Type dateFilterType =
                EnumPopupSetting<DateFilter.Type>.Load(
                    UnityConstants.CHANGESETS_DATE_FILTER_SETTING_NAME,
                    DateFilter.Type.LastMonth);
            mDateFilter = new DateFilter(dateFilterType);

            ChangesetsListHeaderState headerState =
                ChangesetsListHeaderState.GetDefault();
            TreeHeaderSettings.Load(headerState,
                UnityConstants.CHANGESETS_TABLE_SETTINGS_NAME,
                (int)ChangesetsListColumn.CreationDate, false);

            mChangesetsViewMenu = new ChangesetsViewMenu(
                wkInfo, 
                this, 
                this,
                mShowDownloadPlasticExeWindow,
                mIsGluonMode);

            mChangesetsListView = new ChangesetsListView(
                headerState,
                ChangesetsListHeaderState.GetColumnNames(),
                mChangesetsViewMenu,
                sizeChangedAction: OnChangesetsListViewSizeChanged,
                selectionChangedAction: OnSelectionChanged,
                doubleClickAction: ((IChangesetMenuOperations)this).DiffChangeset);
            mChangesetsListView.Reload();

            mDiffPanel = new DiffPanel(
                wkInfo, workspaceWindow, refreshView, viewSwitcher,
                historyViewLauncher, mShowDownloadPlasticExeWindow,
                parentWindow, mIsGluonMode);
        }

        bool mIsRefreshing;

        bool mShouldScrollToSelection;

        float mShowChangesButtonWidth = -1;

        object mSplitterState;
        Color mBorderColor;

        DateFilter mDateFilter;

        SearchField mSearchField;
        bool mIsSearchFieldFocused = false;

        ChangesetsListView mChangesetsListView;
        ChangesetOperations mChangesetOperations;
        DiffPanel mDiffPanel;
        ChangesetsViewMenu mChangesetsViewMenu;

        RevertToChangesetOperation.IGetStatusForWorkspace mGetStatusForWorkspace =
            new RevertToChangesetOperation.GetStatusFromWorkspace();
        RevertToChangesetOperation.IUndoCheckoutOperation mUndoCheckoutOperation =
            new RevertToChangesetOperation.UndoCheckout();
        RevertToChangesetOperation.IRevertToChangesetMergeController mRevertToChangesetMergeController =
            new RevertToChangesetOperation.RevertToChangesetMergeController();

        long mLoadedChangesetId = -1;
        object mLock = new object();

        LaunchTool.IProcessExecutor mProcessExecutor;
        LaunchTool.IShowDownloadPlasticExeWindow mShowDownloadPlasticExeWindow;

        readonly bool mIsGluonMode;
        readonly ViewHost mViewHost;
        readonly IGluonUpdateReport mGluonUpdateReport;
        readonly WorkspaceWindow mWorkspaceWindow;
        readonly IViewSwitcher mViewSwitcher;
        readonly ProgressControlsForViews mProgressControls;
        readonly IRevertToChangesetListener mRevertToChangesetListener;
        readonly EditorWindow mParentWindow;
        readonly WorkspaceInfo mWkInfo;
    }
}
