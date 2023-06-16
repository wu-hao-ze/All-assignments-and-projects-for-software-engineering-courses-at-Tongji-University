using System;
using System.IO;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.CM.Common;
using Codice.Client.Common;
using PlasticGui;
using PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Gluon
{
    internal class IncomingChangesTreeView : TreeView
    {
        internal IncomingChangesTreeView(
            WorkspaceInfo wkInfo,
            IncomingChangesTreeHeaderState headerState,
            List<string> columnNames,
            IncomingChangesViewMenu menu,
            Action onCheckedNodeChanged)
            : base(new TreeViewState())
        {
            mWkInfo = wkInfo;
            mColumnNames = columnNames;
            mMenu = menu;
            mOnCheckedNodeChanged = onCheckedNodeChanged;

            multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.canSort = true;
            multiColumnHeader.sortingChanged += SortingChanged;

            customFoldoutYOffset = UnityConstants.TREEVIEW_FOLDOUT_Y_OFFSET;
            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = false;
        }

        public override IList<TreeViewItem> GetRows()
        {
            return mRows;
        }

        protected override bool CanChangeExpandedState(TreeViewItem item)
        {
            return item is ChangeCategoryTreeViewItem;
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1, string.Empty);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem rootItem)
        {
            try
            {
                RegenerateRows(
                    mIncomingChangesTree, mTreeViewItemIds, this,
                        rootItem, mRows, mExpandCategories);
            }
            finally
            {
                mExpandCategories = false;
            }

            return mRows;
        }

        protected override void CommandEventHandling()
        {
            // NOTE - empty override to prevent crash when pressing ctrl-a in the treeview
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
            if (args.item is ChangeCategoryTreeViewItem)
            {
                CategoryTreeViewItemGUI(
                    args.rowRect, rowHeight,
                    (ChangeCategoryTreeViewItem)args.item,
                    mOnCheckedNodeChanged,
                    mSolvedConflicts.Count,
                    args.selected, args.focused);
                return;
            }

            if (args.item is ChangeTreeViewItem)
            {
                ChangeTreeViewItem changeTreeViewItem =
                    (ChangeTreeViewItem)args.item;

                IncomingChangeInfo changeInfo = changeTreeViewItem.ChangeInfo;
                IncomingChangeInfo metaChangeInfo = mIncomingChangesTree.GetMetaChange(
                    changeInfo);

                bool isCurrentConflict = IsCurrentConflict(
                    changeInfo,
                    metaChangeInfo,
                    mCurrentConflict);

                bool isSolvedConflict = IsSolvedConflict(
                    changeInfo,
                    metaChangeInfo,
                    mSolvedConflicts);

                IncomingChangeTreeViewItemGUI(
                    mWkInfo.ClientPath,
                    mIncomingChangesTree,
                    this,
                    changeTreeViewItem,
                    mOnCheckedNodeChanged, args,
                    isCurrentConflict,
                    isSolvedConflict);
                return;
            }

            base.RowGUI(args);
        }

        internal void BuildModel(UnityIncomingChangesTree tree)
        {
            mTreeViewItemIds.Clear();

            mIncomingChangesTree = tree;
            mSolvedConflicts = new List<IncomingChangeInfo>();
            mCurrentConflict = null;

            mExpandCategories = true;
        }

        internal void UpdateSolvedFileConflicts(
            List<IncomingChangeInfo> solvedConflicts,
            IncomingChangeInfo currentConflict)
        {
            mSolvedConflicts = solvedConflicts;
            mCurrentConflict = currentConflict;
        }

        internal void Sort()
        {
            int sortedColumnIdx = multiColumnHeader.state.sortedColumnIndex;
            bool sortAscending = multiColumnHeader.IsSortedAscending(sortedColumnIdx);

            mIncomingChangesTree.Sort(
                mColumnNames[sortedColumnIdx],
                sortAscending);
        }

        internal IncomingChangeInfo GetMetaChange(IncomingChangeInfo change)
        {
            if (change == null)
                return null;

            return mIncomingChangesTree.GetMetaChange(change);
        }

        internal void FillWithMeta(List<IncomingChangeInfo> changes)
        {
            mIncomingChangesTree.FillWithMeta(changes);
        }

        internal bool SelectionHasMeta()
        {
            IncomingChangeInfo selectedChangeInfo = GetSelectedIncomingChange();

            if (selectedChangeInfo == null)
                return false;

            return mIncomingChangesTree.HasMeta(selectedChangeInfo);
        }

        internal IncomingChangeInfo GetSelectedIncomingChange()
        {
            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count != 1)
                return null;

            int selectedId = selectedIds[0];

            foreach (KeyValuePair<IncomingChangeInfo, int> item
                in mTreeViewItemIds.GetInfoItems())
            {
                if (selectedId == item.Value)
                    return item.Key;
            }

            return null;
        }

        internal List<IncomingChangeInfo> GetSelectedIncomingChanges()
        {
            List<IncomingChangeInfo> result = new List<IncomingChangeInfo>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<IncomingChangeInfo, int> item
                in mTreeViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                result.Add(item.Key);
            }

            return result;
        }

        internal List<IncomingChangeInfo> GetSelectedFileConflicts()
        {
            List<IncomingChangeInfo> result = new List<IncomingChangeInfo>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<IncomingChangeInfo, int> item
                in mTreeViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                if (item.Key.CategoryType != IncomingChangeCategory.Type.Conflicted)
                    continue;

                result.Add(item.Key);
            }

            return result;
        }

        internal int GetCheckedItemCount()
        {
            List<IncomingChangeCategory> categories = mIncomingChangesTree.GetNodes();

            if (categories == null)
                return 0;

            int checkedCount = 0;
            foreach (IncomingChangeCategory category in categories)
            {
                checkedCount += ((ICheckablePlasticTreeCategory)category).GetCheckedChangesCount();
            }
            return checkedCount;
        }

        internal int GetTotalItemCount()
        {
            List<IncomingChangeCategory> categories = mIncomingChangesTree.GetNodes();

            if (categories == null)
                return 0;

            int totalCount = 0;
            foreach (IncomingChangeCategory category in categories)
            {
                totalCount += category.GetChildrenCount();
            }
            return totalCount;
        }

        void SortingChanged(MultiColumnHeader multiColumnHeader)
        {
            Sort();

            Reload();
        }

        static bool IsCurrentConflict(
            IncomingChangeInfo changeInfo,
            IncomingChangeInfo metaChangeInfo,
            IncomingChangeInfo currentConflict)
        {
            if (metaChangeInfo == null)
                return currentConflict == changeInfo;

            return currentConflict == changeInfo ||
                   currentConflict == metaChangeInfo;
        }

        static bool IsSolvedConflict(
            IncomingChangeInfo changeInfo,
            IncomingChangeInfo metaChangeInfo,
            List<IncomingChangeInfo> solvedConflicts)
        {
            if (metaChangeInfo == null)
                return solvedConflicts.Contains(changeInfo);

            return solvedConflicts.Contains(changeInfo) &&
                   solvedConflicts.Contains(metaChangeInfo);
        }

        static void RegenerateRows(
            UnityIncomingChangesTree incomingChangesTree,
            TreeViewItemIds<IncomingChangeCategory, IncomingChangeInfo> treeViewItemIds,
            IncomingChangesTreeView treeView,
            TreeViewItem rootItem,
            List<TreeViewItem> rows,
            bool expandCategories)
        {
            if (incomingChangesTree == null)
                return;

            ClearRows(rootItem, rows);

            List<IncomingChangeCategory> categories = incomingChangesTree.GetNodes();

            if (categories == null)
                return;

            List<int> categoriesToExpand = new List<int>();

            foreach (IncomingChangeCategory category in categories)
            {
                int categoryId;
                if (!treeViewItemIds.TryGetCategoryItemId(category, out categoryId))
                    categoryId = treeViewItemIds.AddCategoryItem(category);

                ChangeCategoryTreeViewItem categoryTreeViewItem =
                    new ChangeCategoryTreeViewItem(categoryId, category);

                rootItem.AddChild(categoryTreeViewItem);
                rows.Add(categoryTreeViewItem);

                if (!ShouldExpandCategory(
                        treeView, categoryTreeViewItem,
                        categories.Count, expandCategories))
                    continue;

                categoriesToExpand.Add(categoryTreeViewItem.id);

                foreach (IncomingChangeInfo incomingChange in category.GetChanges())
                {
                    int changeId;
                    if (!treeViewItemIds.TryGetInfoItemId(incomingChange, out changeId))
                        changeId = treeViewItemIds.AddInfoItem(incomingChange);

                    TreeViewItem changeTreeViewItem =
                        new ChangeTreeViewItem(changeId, incomingChange);

                    categoryTreeViewItem.AddChild(changeTreeViewItem);
                    rows.Add(changeTreeViewItem);
                }
            }

            treeView.state.expandedIDs = categoriesToExpand;
        }

        static void ClearRows(
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            if (rootItem.hasChildren)
                rootItem.children.Clear();

            rows.Clear();
        }

        static void UpdateCheckStateForSelection(
            IncomingChangesTreeView treeView,
            ChangeTreeViewItem senderTreeViewItem)
        {
            IList<int> selectedIds = treeView.GetSelection();

            if (selectedIds.Count <= 1)
                return;

            if (!selectedIds.Contains(senderTreeViewItem.id))
                return;

            bool isChecked = ((ICheckablePlasticTreeNode)senderTreeViewItem.ChangeInfo).IsChecked();

            foreach (TreeViewItem treeViewItem in treeView.FindRows(selectedIds))
            {
                if (treeViewItem is ChangeCategoryTreeViewItem)
                {
                    ((ICheckablePlasticTreeCategory)((ChangeCategoryTreeViewItem)treeViewItem)
                        .Category).UpdateCheckedState(isChecked);
                    continue;
                }

                ((ICheckablePlasticTreeNode)((ChangeTreeViewItem)treeViewItem)
                    .ChangeInfo).UpdateCheckedState(isChecked);
            }
        }

        static void CategoryTreeViewItemGUI(
            Rect rowRect,
            float rowHeight,
            ChangeCategoryTreeViewItem item,
            Action onCheckedNodeChanged,
            int solvedConflictsCount,
            bool isSelected,
            bool isFocused)
        {
            string label = item.Category.CategoryName;
            string infoLabel = item.Category.GetCheckedChangesText();

            bool wasChecked = item.Category.IsChecked();
            bool hadCheckedChildren = ((ICheckablePlasticTreeCategory)item.Category).GetCheckedChangesCount() > 0;

            DefaultStyles.label = GetCategoryStyle(
                item.Category, solvedConflictsCount, isSelected);

            bool isChecked = DrawTreeViewItem.ForCheckableCategoryItem(
                rowRect,
                rowHeight,
                item.depth,
                label,
                infoLabel,
                isSelected,
                isFocused,
                wasChecked,
                hadCheckedChildren,
                hadPartiallyCheckedChildren: false);

            DefaultStyles.label = UnityStyles.Tree.Label;

            if (!wasChecked && isChecked)
            {
                ((ICheckablePlasticTreeCategory)item.Category).UpdateCheckedState(true);
                onCheckedNodeChanged();
                return;
            }

            if (wasChecked && !isChecked)
            {
                ((ICheckablePlasticTreeNode)item.Category).UpdateCheckedState(false);
                onCheckedNodeChanged();
                return;
            }
        }

        static void IncomingChangeTreeViewItemGUI(
            string wkPath,
            UnityIncomingChangesTree incomingChangesTree,
            IncomingChangesTreeView treeView,
            ChangeTreeViewItem item,
            Action onCheckedNodeChanged,
            RowGUIArgs args,
            bool isCurrentConflict,
            bool isSolvedConflict)
        {
            for (int visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                Rect cellRect = args.GetCellRect(visibleColumnIdx);

                IncomingChangesTreeColumn column =
                    (IncomingChangesTreeColumn)args.GetColumn(visibleColumnIdx);

                IncomingChangeTreeViewItemCellGUI(
                    wkPath,
                    cellRect,
                    treeView.rowHeight, 
                    incomingChangesTree,
                    treeView,
                    item,
                    onCheckedNodeChanged,
                    column,
                    args.selected,
                    args.focused,
                    isCurrentConflict,
                    isSolvedConflict);
            }
        }

        static void IncomingChangeTreeViewItemCellGUI(
            string wkPath,
            Rect rect,
            float rowHeight,
            UnityIncomingChangesTree incomingChangesTree,
            IncomingChangesTreeView treeView,
            ChangeTreeViewItem item,
            Action onCheckedNodeChanged,
            IncomingChangesTreeColumn column,
            bool isSelected,
            bool isFocused,
            bool isCurrentConflict,
            bool isSolvedConflict)
        {
            IncomingChangeInfo incomingChange = item.ChangeInfo;

            string label = incomingChange.GetColumnText(
                IncomingChangesTreeHeaderState.GetColumnName(column));

            if (column == IncomingChangesTreeColumn.Path)
            {
                if (incomingChangesTree.HasMeta(item.ChangeInfo))
                    label = string.Concat(label, UnityConstants.TREEVIEW_META_LABEL);

                Texture icon = GetIcon(wkPath, incomingChange);
                Texture overlayIcon =
                    GetChangesOverlayIcon.ForGluonIncomingChange(
                        incomingChange, isSolvedConflict);

                bool wasChecked = ((ICheckablePlasticTreeNode)incomingChange).IsChecked();

                bool isChecked = DrawTreeViewItem.ForCheckableItemCell(
                    rect, rowHeight, item.depth,
                    icon, overlayIcon, label,
                    isSelected, isFocused, isCurrentConflict,
                    wasChecked);

                ((ICheckablePlasticTreeNode)incomingChange).UpdateCheckedState(isChecked);

                if (wasChecked != isChecked)
                {
                    UpdateCheckStateForSelection(treeView, item);
                    onCheckedNodeChanged();
                }

                return;
            }

            if (column == IncomingChangesTreeColumn.Size)
            {
                // If there is a meta file, add the meta file to the file size so that it is consistent 
                // with the Incoming Changes overview
                if (incomingChangesTree.HasMeta(item.ChangeInfo))
                {
                    IncomingChangeInfo metaFileInfo = incomingChangesTree.GetMetaChange(incomingChange);
                    long metaFileSize = metaFileInfo.GetSize();
                    long fileSize = incomingChange.GetSize();

                    label = SizeConverter.ConvertToSizeString(fileSize + metaFileSize);
                }

                DrawTreeViewItem.ForSecondaryLabelRightAligned(
                    rect, label, isSelected, isFocused, isCurrentConflict);
                return;
            }

            DrawTreeViewItem.ForSecondaryLabel(
                rect, label, isSelected, isFocused, isCurrentConflict);
        }

        static Texture GetIcon(
            string wkPath,
            IncomingChangeInfo incomingChange)
        {
            bool isDirectory = incomingChange.GetRevision().
                Type == EnumRevisionType.enDirectory;

            if (isDirectory || incomingChange.IsXLink())
                return Images.GetDirectoryIcon();

            string fullPath = WorkspacePath.GetWorkspacePathFromCmPath(
                wkPath, incomingChange.GetPath(), Path.DirectorySeparatorChar);

            return Images.GetFileIcon(fullPath);
        }

        static GUIStyle GetCategoryStyle(
            IncomingChangeCategory category,
            int solvedConflictsCount,
            bool isSelected)
        {
            if (isSelected)
                return UnityStyles.Tree.Label;

            if (category.CategoryType != IncomingChangeCategory.Type.Conflicted)
                return UnityStyles.Tree.Label;

            return category.GetChildrenCount() > solvedConflictsCount ?
                UnityStyles.Tree.RedLabel : UnityStyles.Tree.GreenLabel;
        }

        static bool ShouldExpandCategory(
            IncomingChangesTreeView treeView,
            ChangeCategoryTreeViewItem categoryTreeViewItem,
            int categoriesCount,
            bool expandCategories)
        {
            if (expandCategories)
            {
                if (categoriesCount == 1)
                    return true;

                if (categoryTreeViewItem.Category.CategoryType
                        == IncomingChangeCategory.Type.Conflicted)
                    return true;

                if (categoryTreeViewItem.Category.GetChildrenCount()
                        > NODES_TO_EXPAND_CATEGORY)
                    return false;

                return true;
            }

            return treeView.IsExpanded(categoryTreeViewItem.id);
        }

        bool mExpandCategories;

        TreeViewItemIds<IncomingChangeCategory, IncomingChangeInfo> mTreeViewItemIds =
            new TreeViewItemIds<IncomingChangeCategory, IncomingChangeInfo>();
        List<TreeViewItem> mRows = new List<TreeViewItem>();

        IncomingChangeInfo mCurrentConflict;
        List<IncomingChangeInfo> mSolvedConflicts;
        UnityIncomingChangesTree mIncomingChangesTree;

        readonly Action mOnCheckedNodeChanged;
        readonly IncomingChangesViewMenu mMenu;
        readonly List<string> mColumnNames;
        readonly WorkspaceInfo mWkInfo;

        const int NODES_TO_EXPAND_CATEGORY = 10;
    }
}
