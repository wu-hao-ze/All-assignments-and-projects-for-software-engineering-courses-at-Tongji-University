using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.History;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Avatar;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.History
{
    internal class HistoryListView : TreeView
    {
        internal HistoryListView(
            string wkPath,
            RepositorySpec repSpec,
            HistoryListHeaderState headerState,
            HistoryListViewMenu menu,
            List<string> columnNames) :
            base(new TreeViewState())
        {
            mWkPath = wkPath;
            mRepSpec = repSpec;
            mMenu = menu;
            mColumnNames = columnNames;

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

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);

            Event e = Event.current;

            if (e.type != EventType.KeyDown)
                return;

            bool isProcessed = mMenu.ProcessKeyActionIfNeeded(e);

            if (isProcessed)
                e.Use();
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1, string.Empty);
        }

        protected override IList<TreeViewItem> BuildRows(
            TreeViewItem rootItem)
        {
            if (mRevisionsList == null)
            {
                ClearRows(rootItem, mRows);

                return mRows;
            }

            RegenerateRows(
                mListViewItemIds,
                mRevisionsList,
                rootItem,
                mRows);

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
            if (args.item is HistoryListViewItem)
            {
                HistoryListViewItem historyListViewItem = (HistoryListViewItem)args.item;

                HistoryListViewItemGUI(
                    mWkPath,
                    mRepSpec,
                    rowHeight,
                    historyListViewItem,
                    args,
                    mLoadedRevisionId == historyListViewItem.Revision.Id,
                    Repaint);
                return;
            }

            base.RowGUI(args);
        }

        internal void BuildModel(
            HistoryRevisionList historyRevisionList,
            long loadedRevisionId)
        {
            mListViewItemIds.Clear();

            mRevisionsList = historyRevisionList;
            mLoadedRevisionId = loadedRevisionId;
        }

        internal void Refilter()
        {
            if (mRevisionsList == null)
                return;

            Filter filter = new Filter(searchString);
            mRevisionsList.Filter(filter, mColumnNames, null);
        }

        internal void Sort()
        {
            if (mRevisionsList == null)
                return;

            int sortedColumnIdx = multiColumnHeader.state.sortedColumnIndex;
            bool sortAscending = multiColumnHeader.IsSortedAscending(sortedColumnIdx);

            mRevisionsList.Sort(
                mColumnNames[sortedColumnIdx],
                sortAscending);
        }

        internal long GetLoadedRevisionId()
        {
            return mLoadedRevisionId;
        }

        internal List<RepObjectInfo> GetSelectedRepObjectInfos()
        {
            List<RepObjectInfo> result = new List<RepObjectInfo>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<RepObjectInfo, int> item
                in mListViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                result.Add(item.Key);
            }

            return result;
        }

        internal List<HistoryRevision> GetSelectedHistoryRevisions()
        {
            return GetSelectedRepObjectInfos().OfType<HistoryRevision>().ToList();
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

        int GetTreeIdForItem(RepObjectInfo currentRepObjectInfo)
        {
            foreach (KeyValuePair<RepObjectInfo, int> item in mListViewItemIds.GetInfoItems())
            {
                if (!currentRepObjectInfo.Equals(item.Key))
                    continue;

                if (!currentRepObjectInfo.GUID.Equals(item.Key.GUID))
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
            ListViewItemIds<RepObjectInfo> listViewItemIds,
            HistoryRevisionList revisions,
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            ClearRows(rootItem, rows);

            if (revisions.GetCount() == 0)
                return;

            foreach (RepObjectInfo objectInfo in revisions.GetRevisions())
            {
                int objectId;
                if (!listViewItemIds.TryGetInfoItemId(objectInfo, out objectId))
                    objectId = listViewItemIds.AddInfoItem(objectInfo);

                HistoryListViewItem changesetListViewItem =
                    new HistoryListViewItem(objectId, objectInfo);

                rootItem.AddChild(changesetListViewItem);
                rows.Add(changesetListViewItem);
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

        static void HistoryListViewItemGUI(
            string wkPath,
            RepositorySpec repSpec,
            float rowHeight,
            HistoryListViewItem item,
            RowGUIArgs args,
            bool isBoldText,
            Action avatarLoadedAction)
        {
            for (int visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                Rect cellRect = args.GetCellRect(visibleColumnIdx);

                HistoryListColumn column =
                    (HistoryListColumn)args.GetColumn(visibleColumnIdx);

                HistoryListViewItemCellGUI(
                    cellRect,
                    rowHeight,
                    wkPath,
                    repSpec,
                    item,
                    column,
                    avatarLoadedAction,
                    args.selected,
                    args.focused,
                    isBoldText);
            }
        }

        static void HistoryListViewItemCellGUI(
            Rect rect,
            float rowHeight,
            string wkPath,
            RepositorySpec repSpec,
            HistoryListViewItem item,
            HistoryListColumn column,
            Action avatarLoadedAction,
            bool isSelected,
            bool isFocused,
            bool isBoldText)
        {
            string columnText = HistoryInfoView.GetColumnText(
                wkPath, repSpec, item.Revision,
                HistoryListHeaderState.GetColumnName(column));

            if (column == HistoryListColumn.Changeset)
            {
                DrawTreeViewItem.ForItemCell(
                    rect,
                    rowHeight,
                    0,
                    GetRevisionIcon(item.Revision),
                    null,
                    columnText,
                    isSelected,
                    isFocused,
                    isBoldText,
                    false);

                return;
            }

            if (column == HistoryListColumn.CreatedBy)
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

            if (column == HistoryListColumn.Branch)
            {
                DrawTreeViewItem.ForSecondaryLabel(
                    rect, columnText, isSelected, isFocused, isBoldText);
                return;
            }

            DrawTreeViewItem.ForLabel(
                rect, columnText, isSelected, isFocused, isBoldText);
        }

        static Texture GetRevisionIcon(RepObjectInfo revision)
        {
            if (revision is MoveRealizationInfo)
                return Images.GetMovedIcon();

            if (revision is RemovedRealizationInfo)
                return Images.GetDeletedIcon();

            return Images.GetFileIcon();
        }

        ListViewItemIds<RepObjectInfo> mListViewItemIds = new ListViewItemIds<RepObjectInfo>();
        List<TreeViewItem> mRows = new List<TreeViewItem>();

        HistoryRevisionList mRevisionsList;
        long mLoadedRevisionId;

        readonly CooldownWindowDelayer mCooldownFilterAction;
        readonly HistoryListViewMenu mMenu;
        readonly List<string> mColumnNames;
        readonly RepositorySpec mRepSpec;
        readonly string mWkPath;
    }
}
