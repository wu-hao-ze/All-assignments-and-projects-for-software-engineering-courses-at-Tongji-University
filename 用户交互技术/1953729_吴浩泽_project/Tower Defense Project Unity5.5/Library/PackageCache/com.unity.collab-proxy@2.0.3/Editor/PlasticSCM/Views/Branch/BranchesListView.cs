using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.QueryViews;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Avatar;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.Branches
{
    internal class BranchesListView : TreeView
    {
        internal GenericMenu Menu { get { return mMenu.Menu; } }

        internal BranchesListView(
            BranchesListHeaderState headerState,
            List<string> columnNames,
            BranchesViewMenu menu,
            Action sizeChangedAction)
            : base(new TreeViewState())
        {
            mColumnNames = columnNames;
            mMenu = menu;
            mSizeChangedAction = sizeChangedAction;

            multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.canSort = true;
            multiColumnHeader.sortingChanged += SortingChanged;

            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = false;

            mCooldownFilterAction = new CooldownWindowDelayer(
                DelayedSearchChanged, UnityConstants.SEARCH_DELAYED_INPUT_ACTION_INTERVAL);
        }

        public override IList<TreeViewItem> GetRows()
        {
            return mRows;
        }

        internal void SetLoadedBranchId(long loadedBranchId)
        {
            mLoadedBranchId = loadedBranchId;
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1, string.Empty);
        }

        protected override IList<TreeViewItem> BuildRows(
            TreeViewItem rootItem)
        {
            if (mQueryResult == null)
            {
                ClearRows(rootItem, mRows);

                return mRows;
            }

            RegenerateRows(
                mListViewItemIds,
                mQueryResult.GetObjects(),
                rootItem, mRows);

            return mRows;
        }

        protected override void SearchChanged(string newSearch)
        {
            mCooldownFilterAction.Ping();
        }

        protected override void ContextClickedItem(int id)
        {
            mMenu.Popup();
            Repaint();
        }

        public override void OnGUI(Rect rect)
        {
            if (Event.current.type == EventType.Layout)
            {
                if (IsSizeChanged(treeViewRect, mLastRect))
                    mSizeChangedAction();
            }

            mLastRect = treeViewRect;

            base.OnGUI(rect);

            Event e = Event.current;

            if (e.type != EventType.KeyDown)
                return;

            bool isProcessed = mMenu.ProcessKeyActionIfNeeded(e);

            if (isProcessed)
                e.Use();
        }

        protected override void BeforeRowsGUI()
        {
            int firstRowVisible;
            int lastRowVisible;
            GetFirstAndLastVisibleRows(out firstRowVisible, out lastRowVisible);

            GUI.DrawTexture(new Rect(0,
                firstRowVisible * rowHeight,
                GetRowRect(0).width,
                (lastRowVisible * rowHeight) + 1000),
                Images.GetTreeviewBackgroundTexture());

            DrawTreeViewItem.InitializeStyles();
            base.BeforeRowsGUI();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.item is BranchListViewItem)
            {
                BranchListViewItem branchListViewItem = (BranchListViewItem)args.item;
                BranchInfo branchInfo = (BranchInfo)branchListViewItem.ObjectInfo;

                BranchesListViewItemGUI(
                    mQueryResult,
                    rowHeight,
                    branchListViewItem,
                    args,
                    branchInfo.BranchId == mLoadedBranchId,
                    Repaint);
                return;
            }

            base.RowGUI(args);
        }

        internal void BuildModel(
            ViewQueryResult queryResult,
            long loadedBranchId)
        {
            mListViewItemIds.Clear();

            mQueryResult = queryResult;
            mLoadedBranchId = loadedBranchId;
        }

        internal void Refilter()
        {
            if (mQueryResult == null)
                return;

            Filter filter = new Filter(searchString);
            mQueryResult.ApplyFilter(filter, mColumnNames);
        }

        internal void Sort()
        {
            if (mQueryResult == null)
                return;

            int sortedColumnIdx = multiColumnHeader.state.sortedColumnIndex;
            bool sortAscending = multiColumnHeader.IsSortedAscending(sortedColumnIdx);

            mQueryResult.Sort(
                mColumnNames[sortedColumnIdx],
                sortAscending);
        }

        internal List<RepositorySpec> GetSelectedRepositories()
        {
            List<RepositorySpec> result = new List<RepositorySpec>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<object, int> item
                in mListViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                RepositorySpec repSpec =
                    mQueryResult.GetRepositorySpec(item.Key);
                result.Add(repSpec);
            }

            return result;
        }

        internal List<RepObjectInfo> GetSelectedRepObjectInfos()
        {
            List<RepObjectInfo> result = new List<RepObjectInfo>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<object, int> item
                in mListViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                RepObjectInfo repObjectInfo =
                    mQueryResult.GetRepObjectInfo(item.Key);
                result.Add(repObjectInfo);
            }

            return result;
        }

        internal void SelectRepObjectInfos(
            List<RepObjectInfo> repObjectsToSelect)
        {
            List<int> idsToSelect = new List<int>();

            foreach (RepObjectInfo repObjectInfo in repObjectsToSelect)
            {
                int repObjectInfoId = GetTreeIdForItem(repObjectInfo);

                if (repObjectInfoId == -1)
                    continue;

                idsToSelect.Add(repObjectInfoId);
            }

            TableViewOperations.SetSelectionAndScroll(this, idsToSelect);
        }

        int GetTreeIdForItem(RepObjectInfo repObjectInfo)
        {
            foreach (KeyValuePair<object, int> item in mListViewItemIds.GetInfoItems())
            {
                RepObjectInfo currentRepObjectInfo =
                    mQueryResult.GetRepObjectInfo(item.Key);

                if (!currentRepObjectInfo.Equals(repObjectInfo))
                    continue;

                if (!currentRepObjectInfo.GUID.Equals(repObjectInfo.GUID))
                    continue;

                return item.Value;
            }

            return -1;
        }

        void DelayedSearchChanged()
        {
            Refilter();

            Sort();

            Reload();

            TableViewOperations.ScrollToSelection(this);
        }

        void SortingChanged(MultiColumnHeader multiColumnHeader)
        {
            Sort();

            Reload();
        }

        static void RegenerateRows(
            ListViewItemIds<object> listViewItemIds,
            List<object> objectInfos,
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            ClearRows(rootItem, rows);

            if (objectInfos.Count == 0)
                return;

            foreach (object objectInfo in objectInfos)
            {
                int objectId;
                if (!listViewItemIds.TryGetInfoItemId(objectInfo, out objectId))
                    objectId = listViewItemIds.AddInfoItem(objectInfo);

                BranchListViewItem branchListViewItem =
                    new BranchListViewItem(objectId, objectInfo);

                rootItem.AddChild(branchListViewItem);
                rows.Add(branchListViewItem);
            }
        }

        static void ClearRows(
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            if (rootItem.hasChildren)
                rootItem.children.Clear();

            rows.Clear();
        }

        static void BranchesListViewItemGUI(
            ViewQueryResult queryResult,
            float rowHeight,
            BranchListViewItem item,
            RowGUIArgs args,
            bool isBoldText,
            Action avatarLoadedAction)
        {
            for (int visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                Rect cellRect = args.GetCellRect(visibleColumnIdx);

                if (visibleColumnIdx == 0)
                {
                    cellRect.x += UnityConstants.FIRST_COLUMN_WITHOUT_ICON_INDENT;
                    cellRect.width -= UnityConstants.FIRST_COLUMN_WITHOUT_ICON_INDENT;
                }

                BranchesListColumn column =
                    (BranchesListColumn)args.GetColumn(visibleColumnIdx);

                BranchesListViewItemCellGUI(
                    cellRect,
                    rowHeight,
                    queryResult,
                    item,
                    column,
                    avatarLoadedAction,
                    args.selected,
                    args.focused,
                    isBoldText);
            }
        }

        static void BranchesListViewItemCellGUI(
            Rect rect,
            float rowHeight,
            ViewQueryResult queryResult,
            BranchListViewItem item,
            BranchesListColumn column,
            Action avatarLoadedAction,
            bool isSelected,
            bool isFocused,
            bool isBoldText)
        {
            string columnText = RepObjectInfoView.GetColumnText(
                queryResult.GetRepositorySpec(item.ObjectInfo),
                queryResult.GetRepObjectInfo(item.ObjectInfo),
                BranchesListHeaderState.GetColumnName(column));

            if (column == BranchesListColumn.CreatedBy)
            {
                DrawTreeViewItem.ForItemCell(
                    rect,
                    rowHeight,
                    -1,
                    GetAvatar.ForEmail(columnText, avatarLoadedAction),
                    null,
                    columnText,
                    isSelected,
                    isFocused,
                    isBoldText,
                    false);
                return;
            }

            if (column == BranchesListColumn.Branch ||
                column == BranchesListColumn.Repository ||
                column == BranchesListColumn.Guid)
            {
                DrawTreeViewItem.ForSecondaryLabel(
                    rect, columnText, isSelected, isFocused, isBoldText);
                return;
            }

            DrawTreeViewItem.ForLabel(
                rect, columnText, isSelected, isFocused, isBoldText);
        }

        static bool IsSizeChanged(
            Rect currentRect, Rect lastRect)
        {
            if (currentRect.width != lastRect.width)
                return true;

            if (currentRect.height != lastRect.height)
                return true;

            return false;
        }

        Rect mLastRect;

        ListViewItemIds<object> mListViewItemIds = new ListViewItemIds<object>();
        List<TreeViewItem> mRows = new List<TreeViewItem>();

        ViewQueryResult mQueryResult;
        long mLoadedBranchId;

        readonly CooldownWindowDelayer mCooldownFilterAction;
        readonly Action mSizeChangedAction;
        readonly BranchesViewMenu mMenu;
        readonly List<string> mColumnNames;
    }
}
