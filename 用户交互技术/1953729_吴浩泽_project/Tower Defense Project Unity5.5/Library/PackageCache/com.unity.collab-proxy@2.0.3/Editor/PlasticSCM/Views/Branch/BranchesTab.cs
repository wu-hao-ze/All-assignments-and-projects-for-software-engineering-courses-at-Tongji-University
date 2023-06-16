using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.CM.Common;
using Codice.Client.Common.Threading;

using PlasticGui;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.QueryViews;
using PlasticGui.WorkspaceWindow.QueryViews.Branches;
using PlasticGui.WorkspaceWindow.Update;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.UI.Tree;
using Unity.PlasticSCM.Editor.Views.Branches.Dialogs;
using Unity.PlasticSCM.Editor.Views.Changesets;

namespace Unity.PlasticSCM.Editor.Views.Branches
{
    internal class BranchesTab : 
        IRefreshableView,
        IBranchMenuOperations,
        IQueryRefreshableView
    {
        internal BranchesListView Table { get { return mBranchesListView; } }
        internal IBranchMenuOperations Operations { get { return this; } }

        internal BranchesTab(
            WorkspaceInfo wkInfo,
            IWorkspaceWindow workspaceWindow,
            IViewSwitcher viewSwitcher,
            IMergeViewLauncher mergeViewLauncher,
            IUpdateReport updateReport,
            NewIncomingChangesUpdater developerNewIncomingChangesUpdater,
            EditorWindow parentWindow)
        {
            mWkInfo = wkInfo;
            mParentWindow = parentWindow;
            mProgressControls = new ProgressControlsForViews();

            mProgressControls = new ProgressControlsForViews();

            BuildComponents(
                wkInfo,
                workspaceWindow,
                viewSwitcher,
                mergeViewLauncher,
                updateReport,
                developerNewIncomingChangesUpdater,
                parentWindow);

            ((IRefreshableView)this).Refresh();
        }

        internal void Update()
        {
            mProgressControls.UpdateProgress(mParentWindow);
        }

        internal void OnGUI()
        {
            DoActionsToolbar(mProgressControls);

            DoBranchesArea(
                mBranchesListView,
                mProgressControls.IsOperationRunning());
        }

        internal void SetWorkingObjectInfo(WorkingObjectInfo homeInfo)
        {
            lock(mLock)
            {
                mLoadedBranchId = homeInfo.BranchInfo.BranchId;
            }

            mBranchesListView.SetLoadedBranchId(mLoadedBranchId);
        }

        static void DoBranchesArea(
            BranchesListView branchesListView,
            bool isOperationRunning)
        {
            EditorGUILayout.BeginVertical();

            GUI.enabled = !isOperationRunning;

            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);

            branchesListView.OnGUI(rect);

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        internal void DrawSearchFieldForBranchesTab()
        {
            DrawSearchField.For(
                mSearchField,
                mBranchesListView,
                UnityConstants.SEARCH_FIELD_WIDTH);
        }

        internal void OnDisable()
        {
            mSearchField.downOrUpArrowKeyPressed -=
                SearchField_OnDownOrUpArrowKeyPressed;

            TreeHeaderSettings.Save(
                mBranchesListView.multiColumnHeader.state,
                UnityConstants.BRANCHES_TABLE_SETTINGS_NAME);
        }

        void IRefreshableView.Refresh()
        {
            string query = GetBranchesQuery(mDateFilter);

            FillBranches(mWkInfo,
                query,
                BranchesSelection.GetSelectedRepObjectInfos(mBranchesListView));
        }

        //IQueryRefreshableView
        public void RefreshAndSelect(RepObjectInfo repObj)
        {
            string query = GetBranchesQuery(mDateFilter);

            FillBranches(mWkInfo,
                query,
                new List<RepObjectInfo> { repObj });
        }
 
        void FillBranches(WorkspaceInfo wkInfo, string query, List<RepObjectInfo> branchesToSelect)
        {
            if (mIsRefreshing)
                return;

            mIsRefreshing = true;

            int defaultRow = TableViewOperations.
                GetFirstSelectedRow(mBranchesListView);

            ((IProgressControls)mProgressControls).ShowProgress(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.LoadingBranches));

            ViewQueryResult queryResult = null;

            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    long loadedBranchId = GetLoadedBranchId(wkInfo);
                    lock(mLock)
                    {
                        mLoadedBranchId = loadedBranchId;
                    }

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

                        UpdateBranchesList(
                            mBranchesListView,
                            queryResult,
                            mLoadedBranchId);

                        int branchesCount = GetBranchesCount(queryResult);

                        if (branchesCount == 0)
                        {
                            return;
                        }

                        BranchesSelection.SelectBranches(
                            mBranchesListView, branchesToSelect, defaultRow);
                    }
                    finally
                    {
                        ((IProgressControls)mProgressControls).HideProgress();
                        mIsRefreshing = false;
                    }
                });
        }

        static long GetLoadedBranchId(WorkspaceInfo wkInfo)
        {
            BranchInfo brInfo = PlasticGui.Plastic.API.GetWorkingBranch(wkInfo);

            if (brInfo != null)
                return brInfo.BranchId;

            return -1;
        }

        static void UpdateBranchesList(
             BranchesListView branchesListView,
             ViewQueryResult queryResult,
             long loadedBranchId)
        {
            branchesListView.BuildModel(
                queryResult, loadedBranchId);

            branchesListView.Refilter();

            branchesListView.Sort();

            branchesListView.Reload();
        }

        internal static int GetBranchesCount(
            ViewQueryResult queryResult)
        {
            if (queryResult == null)
                return 0;

           return queryResult.Count();
        }

        internal static string GetBranchesQuery(DateFilter dateFilter)
        {
            if (dateFilter.FilterType == DateFilter.Type.AllTime)
                return QueryConstants.BranchesBeginningQuery;

            string whereClause = QueryConstants.GetDateWhereClause(
                dateFilter.GetFilterDate(DateTime.UtcNow));

            return string.Format("{0} {1}",
                QueryConstants.BranchesBeginningQuery,
                whereClause);
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
                    UnityConstants.BRANCHES_DATE_FILTER_SETTING_NAME);

                ((IRefreshableView)this).Refresh();
            }

            GUI.enabled = true;
        }

        void SearchField_OnDownOrUpArrowKeyPressed()
        {
            mBranchesListView.SetFocusAndEnsureSelectedItem();
        }

        static void DoActionsToolbar(
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

        void BuildComponents(
            WorkspaceInfo wkInfo,
            IWorkspaceWindow workspaceWindow,
            IViewSwitcher viewSwitcher,
            IMergeViewLauncher mergeViewLauncher,
            IUpdateReport updateReport,
            NewIncomingChangesUpdater developerNewIncomingChangesUpdater,
            EditorWindow parentWindow)
        {
            mSearchField = new SearchField();
            mSearchField.downOrUpArrowKeyPressed += SearchField_OnDownOrUpArrowKeyPressed;

            DateFilter.Type dateFilterType =
                EnumPopupSetting<DateFilter.Type>.Load(
                    UnityConstants.BRANCHES_DATE_FILTER_SETTING_NAME,
                    DateFilter.Type.LastMonth);
            mDateFilter = new DateFilter(dateFilterType);

            BranchesListHeaderState headerState =
                BranchesListHeaderState.GetDefault();

            TreeHeaderSettings.Load(headerState,
                UnityConstants.BRANCHES_TABLE_SETTINGS_NAME,
                (int)BranchesListColumn.CreationDate, false);

            mBranchesListView = new BranchesListView(
                headerState,
                BranchesListHeaderState.GetColumnNames(),
                new BranchesViewMenu(this),
                sizeChangedAction: OnBranchesListViewSizeChanged);

            mBranchesListView.Reload();

            mBranchOperations = new BranchOperations(
                wkInfo,
                workspaceWindow,
                viewSwitcher,
                mergeViewLauncher,
                this,
                ViewType.BranchesView,
                mProgressControls,
                updateReport,
                new ContinueWithPendingChangesQuestionerBuilder(viewSwitcher, parentWindow),
                developerNewIncomingChangesUpdater);
        }

        void OnBranchesListViewSizeChanged()
        {
            if (!mShouldScrollToSelection)
                return;

            mShouldScrollToSelection = false;
            TableViewOperations.ScrollToSelection(mBranchesListView);
        }

        int IBranchMenuOperations.GetSelectedBranchesCount()
        {
            return BranchesSelection.GetSelectedBranchesCount(mBranchesListView);
        }

        void IBranchMenuOperations.CreateBranch()
        {
            RepositorySpec repSpec = BranchesSelection.GetSelectedRepository(mBranchesListView);
            BranchInfo branchInfo = BranchesSelection.GetSelectedBranch(mBranchesListView);

            BranchCreationData branchCreationData = CreateBranchDialog.CreateBranchFromLastParentBranchChangeset(
                mParentWindow,
                repSpec,
                branchInfo);

            mBranchOperations.CreateBranch(
                branchCreationData,
                RefreshAsset.BeforeLongAssetOperation,
                RefreshAsset.AfterLongAssetOperation);
        }
		
		void IBranchMenuOperations.CreateTopLevelBranch() { }

        void IBranchMenuOperations.SwitchToBranch()
        {
            RepositorySpec repSpec = BranchesSelection.GetSelectedRepository(mBranchesListView);
            BranchInfo branchInfo = BranchesSelection.GetSelectedBranch(mBranchesListView);

            mBranchOperations.SwitchToBranch(
                repSpec,
                branchInfo,
                RefreshAsset.BeforeLongAssetOperation,
                RefreshAsset.AfterLongAssetOperation);
        }

        void IBranchMenuOperations.MergeBranch() { }

        void IBranchMenuOperations.CherrypickBranch() { }

        void IBranchMenuOperations.MergeToBranch() { }

        void IBranchMenuOperations.PullBranch() { }

        void IBranchMenuOperations.PullRemoteBranch() { }

        void IBranchMenuOperations.SyncWithGit() { }

        void IBranchMenuOperations.PushBranch() { }

        void IBranchMenuOperations.DiffBranch() { }

        void IBranchMenuOperations.DiffWithAnotherBranch() { }

        void IBranchMenuOperations.ViewChangesets() { }

        void IBranchMenuOperations.RenameBranch()
        {
            RepositorySpec repSpec = BranchesSelection.GetSelectedRepository(mBranchesListView);
            BranchInfo branchInfo = BranchesSelection.GetSelectedBranch(mBranchesListView);

            BranchRenameData branchRenameData = RenameBranchDialog.GetBranchRenameData(
                repSpec,
                branchInfo,
                mParentWindow);

            mBranchOperations.RenameBranch(branchRenameData);
        }

        void IBranchMenuOperations.DeleteBranch()
        {
            RepositorySpec repSpec = BranchesSelection.GetSelectedRepository(mBranchesListView);
            List<RepositorySpec> repositories = BranchesSelection.GetSelectedRepositories(mBranchesListView);
            List<BranchInfo> branchesToDelete = BranchesSelection.GetSelectedBranches(mBranchesListView);

            mBranchOperations.DeleteBranch(repositories, branchesToDelete);
        }

        void IBranchMenuOperations.CreateCodeReview() { }

        void IBranchMenuOperations.ViewPermissions() { }

        SearchField mSearchField;
        bool mIsRefreshing;

        DateFilter mDateFilter;
        bool mShouldScrollToSelection;
        BranchesListView mBranchesListView;
        BranchOperations mBranchOperations;

        long mLoadedBranchId = -1;
        object mLock = new object();

        readonly WorkspaceInfo mWkInfo;
        readonly ProgressControlsForViews mProgressControls;
        readonly EditorWindow mParentWindow;
    }
}