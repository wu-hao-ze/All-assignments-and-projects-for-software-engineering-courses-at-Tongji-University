using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.CM.Common;

using PlasticGui;
using PlasticGui.WorkspaceWindow.PendingChanges;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;

using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetsOverlays;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal class PendingChangesTreeView : TreeView
    {
        internal PendingChangesTreeView(
            WorkspaceInfo wkInfo,
            bool isGluonMode,
            PendingChangesTreeHeaderState headerState,
            List<string> columnNames,
            PendingChangesViewMenu menu,
            IAssetStatusCache assetStatusCache)
            : base(new TreeViewState())
        {
            mWkInfo = wkInfo;
            mIsGluonMode = isGluonMode;
            mColumnNames = columnNames;
            mHeaderState = headerState;
            mMenu = menu;
            mAssetStatusCache = assetStatusCache;

            mPendingChangesTree = new UnityPendingChangesTree();

            multiColumnHeader = new PendingChangesMultiColumnHeader(
                this,
                headerState,
                mPendingChangesTree);
            multiColumnHeader.canSort = true;
            multiColumnHeader.sortingChanged += SortingChanged;

            customFoldoutYOffset = UnityConstants.TREEVIEW_FOLDOUT_Y_OFFSET;
            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = false;

            mCooldownFilterAction = new CooldownWindowDelayer(
                DelayedSearchChanged, UnityConstants.SEARCH_DELAYED_INPUT_ACTION_INTERVAL);
        }

        protected override void SingleClickedItem(int id)
        {
            SelectionChanged(new [] { id });
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            mHeaderState.UpdateItemColumnHeader(this);

            if (mIsSelectionChangedEventDisabled)
                return;

            List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

            foreach (ChangeInfo changeInfo in GetSelectedChanges(false))
            {
                UnityEngine.Object asset = LoadAsset.FromChangeInfo(changeInfo);

                if (asset == null)
                    continue;

                assets.Add(asset);
            }

            UnityEditor.Selection.objects = assets.ToArray();

            if (assets.Count == 1)
                EditorGUIUtility.PingObject(assets[0]);
        }

        protected void SelectionChanged()
        {
            SelectionChanged(GetSelection());
        }

        public override IList<TreeViewItem> GetRows()
        {
            return mRows;
        }

        public override void OnGUI(Rect rect)
        {
            MultiColumnHeader.DefaultStyles.background =
                UnityStyles.Tree.Columns;

            try
            {
                base.OnGUI(rect);

                if (!base.HasFocus())
                    return;

                Event e = Event.current;

                if (e.type != EventType.KeyDown)
                    return;

                bool isProcessed = mMenu.ProcessKeyActionIfNeeded(e);

                if (isProcessed)
                    e.Use();
            }
            finally
            {
                MultiColumnHeader.DefaultStyles.background =
                    UnityStyles.PendingChangesTab.DefaultMultiColumHeader;
            }
        }

        protected override bool CanChangeExpandedState(TreeViewItem item)
        {
            return item is ChangeCategoryTreeViewItem || item is ChangelistTreeViewItem;
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
                    mPendingChangesTree, mTreeViewItemIds, this,
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
            if (args.item is ChangelistTreeViewItem)
            {
                ChangelistTreeViewItemGUI(
                    this,
                    args.rowRect, rowHeight,
                    (ChangelistTreeViewItem)args.item,
                    args.selected, args.focused);
                return;
            }

            if (args.item is ChangeCategoryTreeViewItem)
            {
                CategoryTreeViewItemGUI(
                    this,
                    args.rowRect, rowHeight,
                    (ChangeCategoryTreeViewItem)args.item,
                    args.selected, args.focused);
                return;
            }

            if (args.item is ChangeTreeViewItem)
            {
                ChangeTreeViewItemGUI(
                    mWkInfo.ClientPath,
                    mIsGluonMode,
                    mAssetStatusCache,
                    this,
                    mPendingChangesTree,
                    (ChangeTreeViewItem)args.item,
                    args);
                return;
            }

            base.RowGUI(args);
        }

        internal void BuildModel(List<ChangeInfo> changes, PendingChangesViewCheckedStateManager checkedStateManager)
        {
            mTreeViewItemIds.Clear();

            mPendingChangesTree.BuildChangeCategories(
                mWkInfo,
                changes,
                checkedStateManager);
        }

        internal ChangeInfo GetChangedForMoved(ChangeInfo moved)
        {
            return mPendingChangesTree.GetChangedForMoved(moved);
        }

        internal void Refilter()
        {
            Filter filter = new Filter(searchString);
            mPendingChangesTree.Filter(filter, mColumnNames);

            mExpandCategories = true;
        }

        internal void Sort()
        {
            int sortedColumnIdx = multiColumnHeader.state.sortedColumnIndex;
            bool sortAscending = multiColumnHeader.IsSortedAscending(sortedColumnIdx);

            mPendingChangesTree.Sort(
                mColumnNames[sortedColumnIdx],
                sortAscending);
        }

        internal bool GetSelectedPathsToDelete(
            out List<string> privateDirectories,
            out List<string> privateFiles)
        {
            privateDirectories = new List<string>();
            privateFiles = new List<string>();

            List<ChangeInfo> dirChanges = new List<ChangeInfo>();
            List<ChangeInfo> fileChanges = new List<ChangeInfo>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return false;

            foreach (KeyValuePair<PendingChangeInfo, int> item
                in mTreeViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                ChangeInfo changeInfo = item.Key.ChangeInfo;

                if (ChangeInfoType.IsControlled(changeInfo))
                    continue;

                if (changeInfo.IsDirectory)
                {
                    dirChanges.Add(changeInfo);
                    continue;
                }

                fileChanges.Add(changeInfo);
            }

            mPendingChangesTree.FillWithMeta(fileChanges);
            mPendingChangesTree.FillWithMeta(dirChanges);

            privateDirectories = dirChanges.Select(
                d => d.GetFullPath()).ToList();
            privateFiles = fileChanges.Select(
                f => f.GetFullPath()).ToList();

            return true;
        }

        internal void GetCheckedChanges(
            List<ChangelistNode> selectedChangelists,
            bool bExcludePrivates,
            out List<ChangeInfo> changes,
            out List<ChangeInfo> dependenciesCandidates)
        {
            mPendingChangesTree.GetCheckedChanges(
                selectedChangelists,
                bExcludePrivates,
                out changes,
                out dependenciesCandidates);
        }

        internal List<ChangeInfo> GetAllChanges()
        {
            return mPendingChangesTree.GetAllChanges();
        }

        internal ChangeInfo GetMetaChange(ChangeInfo change)
        {
            if (change == null)
                return null;

            return mPendingChangesTree.GetMetaChange(change);
        }

        internal List<ChangeInfo> GetDependenciesCandidates(
            List<ChangeInfo> selectedChanges, bool bExcludePrivates)
        {
            return mPendingChangesTree.GetDependenciesCandidates(
                selectedChanges, bExcludePrivates);
        }

        internal List<IPlasticTreeNode> GetSelectedNodes()
        {
            List<IPlasticTreeNode> result = new List<IPlasticTreeNode>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<IPlasticTreeNode, int> item
                in mTreeViewItemIds.GetCategoryItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                result.Add(item.Key);
            }

            foreach (KeyValuePair<PendingChangeInfo, int> item
                in mTreeViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                result.Add(item.Key);
            }

            return result;
        }

        internal List<ChangeInfo> GetSelectedChanges(bool includeMetaFiles)
        {
            List<ChangeInfo> result = new List<ChangeInfo>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<PendingChangeInfo, int> item
                in mTreeViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                result.Add(item.Key.ChangeInfo);
            }

            if (includeMetaFiles)
                mPendingChangesTree.FillWithMeta(result);

            return result;
        }

        internal List<PendingChangeInfo> GetSelectedPendingChangeInfos()
        {
            List<PendingChangeInfo> result = new List<PendingChangeInfo>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<PendingChangeInfo, int> item
                in mTreeViewItemIds.GetInfoItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                result.Add(item.Key);
            }

            return result;
        }

        internal List<ChangelistNode> GetSelectedChangelistNodes()
        {
            List<ChangelistNode> result = new List<ChangelistNode>();

            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (KeyValuePair<IPlasticTreeNode, int> item
                in mTreeViewItemIds.GetCategoryItems())
            {
                if (!selectedIds.Contains(item.Value))
                    continue;

                if (item.Key is ChangelistNode)
                    result.Add((ChangelistNode)item.Key);
            }

            return result;
        }

        internal bool SelectionHasMeta()
        {
            ChangeInfo selectedChangeInfo = GetSelectedRow();

            if (selectedChangeInfo == null)
                return false;

            return mPendingChangesTree.HasMeta(selectedChangeInfo);
        }

        internal ChangeInfo GetSelectedRow()
        {
            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return null;

            int selectedId = selectedIds[0];

            foreach (KeyValuePair<PendingChangeInfo, int> item
                in mTreeViewItemIds.GetInfoItems())
            {
                if (selectedId == item.Value)
                    return item.Key.ChangeInfo;
            }

            return null;
        }

        internal ChangeInfo GetNearestAddedChange()
        {
            IList<int> selectedIds = GetSelection();

            if (selectedIds.Count == 0)
                return null;

            int id = selectedIds[0];

            IList<TreeViewItem> treeViewItems =
                FindRows(new List<int>() { id });

            if (treeViewItems.Count == 0)
                return null;

            PendingChangeInfo changeInfo =
                ((ChangeTreeViewItem)treeViewItems[0]).ChangeInfo;
            PendingChangeCategory category =
                (PendingChangeCategory)changeInfo.GetParent();

            int itemIndex = category.GetChildPosition(changeInfo);

            ChangeInfo result = GetNextExistingAddedItem(category, itemIndex);

            if (result != null)
                return result;

            return GetPreviousExistingAddedItem(category, itemIndex);
        }

        internal void SelectFirstPendingChangeOnTree()
        {
            int treeIdFirstItem = GetTreeIdFirstItem();

            if (treeIdFirstItem == -1)
                return;

            mIsSelectionChangedEventDisabled = true;

            try
            {
                TableViewOperations.SetSelectionAndScroll(
                    this, new List<int> { treeIdFirstItem });
            }
            finally
            {
                mIsSelectionChangedEventDisabled = false;
            }
        }

        internal void SelectPreviouslySelectedPendingChanges(
            List<ChangeInfo> changesToSelect)
        {
            List<int> idsToSelect = new List<int>();

            foreach (ChangeInfo change in changesToSelect)
            {
                int changeId = GetTreeIdForItem(change);

                if (changeId == -1)
                    continue;

                idsToSelect.Add(changeId);
            }

            mIsSelectionChangedEventDisabled = true;
            try
            {
                TableViewOperations.SetSelectionAndScroll(
                    this, idsToSelect);
            }
            finally
            {
                mIsSelectionChangedEventDisabled = false;
            }
        }

        internal int GetCheckedItemCount()
        {
            return CheckedItems.GetCheckedChildNodeCount(mPendingChangesTree.GetNodes());
        }

        internal int GetTotalItemCount()
        {
            return CheckedItems.GetTotalChildNodeCount(mPendingChangesTree.GetNodes());
        }

        ChangeInfo GetNextExistingAddedItem(
            PendingChangeCategory addedCategory, int targetAddedItemIndex)
        {
            int addedItemsCount = addedCategory.GetChildrenCount();

            for (int i = targetAddedItemIndex + 1; i < addedItemsCount; i++)
            {
                ChangeInfo currentChangeInfo = GetExistingAddedItem(addedCategory, i);

                if (currentChangeInfo != null)
                    return currentChangeInfo;
            }

            return null;
        }

        ChangeInfo GetPreviousExistingAddedItem(
            PendingChangeCategory addedCategory, int targetAddedItemIndex)
        {
            for (int i = targetAddedItemIndex - 1; i >= 0; i--)
            {
                ChangeInfo currentChangeInfo = GetExistingAddedItem(addedCategory, i);

                if (currentChangeInfo != null)
                    return currentChangeInfo;
            }

            return null;
        }

        ChangeInfo GetExistingAddedItem(
            PendingChangeCategory addedCategory, int addedItemIndex)
        {
            ChangeInfo currentChangeInfo = ((PendingChangeInfo)addedCategory.
                GetChild(addedItemIndex)).ChangeInfo;

            if (Directory.Exists(currentChangeInfo.Path) ||
                File.Exists(currentChangeInfo.Path))
                return currentChangeInfo;

            return null;
        }

        int GetTreeIdFirstItem()
        {
            PendingChangeInfo firstChange = mPendingChangesTree.GetFirstPendingChange();

            if (firstChange == null)
                return -1;

            int changeId;
            if (mTreeViewItemIds.TryGetInfoItemId(firstChange, out changeId))
                return changeId;

            return -1;
        }

        int GetTreeIdForItem(ChangeInfo change)
        {
            foreach (KeyValuePair<PendingChangeInfo, int> item in mTreeViewItemIds.GetInfoItems())
            {
                ChangeInfo changeInfo = item.Key.ChangeInfo;

                if (changeInfo.ChangeTypes != change.ChangeTypes)
                    continue;

                if (changeInfo.GetFullPath() != change.GetFullPath())
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

        static void ChangelistTreeViewItemGUI(
            PendingChangesTreeView treeView,
            Rect rowRect,
            float rowHeight,
            ChangelistTreeViewItem item,
            bool isSelected,
            bool isFocused)
        {
            string label = item.Changelist.ChangelistInfo.Name;
            string secondaryLabel = item.Changelist.ChangelistInfo.Description;

            bool wasChecked = CheckedItems.GetIsCheckedValue(item.Changelist) ?? false;
            
            bool hadCheckedChildren = 
                ((ICheckablePlasticTreeCategoryGroup)item.Changelist).GetCheckedCategoriesCount() > 0;

            bool hadPartiallyCheckedChildren =
                ((ICheckablePlasticTreeCategoryGroup)item.Changelist).GetPartiallyCheckedCategoriesCount() > 0;

            bool isChecked = DrawTreeViewItem.ForCheckableCategoryItem(
                rowRect,
                rowHeight,
                item.depth,
                label,
                secondaryLabel,
                isSelected,
                isFocused,
                wasChecked,
                hadCheckedChildren,
                hadPartiallyCheckedChildren);

            if (wasChecked != isChecked)
            {
                CheckedItems.SetCheckedValue(item.Changelist, isChecked);
                treeView.SelectionChanged();
            }
        }

        static void CategoryTreeViewItemGUI(
            PendingChangesTreeView treeView,
            Rect rowRect,
            float rowHeight,
            ChangeCategoryTreeViewItem item,
            bool isSelected,
            bool isFocused)
        {
            string label = item.Category.CategoryName;
            string secondaryLabel = item.Category.GetCheckedChangesText();

            bool wasChecked = item.Category.IsChecked();
            bool hadCheckedChildren = 
                ((ICheckablePlasticTreeCategory)item.Category).GetCheckedChangesCount() > 0;

            bool isChecked = DrawTreeViewItem.ForCheckableCategoryItem(
                rowRect,
                rowHeight,
                item.depth,
                label,
                secondaryLabel,
                isSelected,
                isFocused,
                wasChecked,
                hadCheckedChildren,
                false);

            if (wasChecked != isChecked)
            {
                CheckedItems.SetCheckedValue(item.Category, isChecked);
                treeView.SelectionChanged();
            }
        }

        static void ChangeTreeViewItemGUI(
            string wkPath,
            bool isGluonMode,
            IAssetStatusCache assetStatusCache,
            PendingChangesTreeView treeView,
            UnityPendingChangesTree pendingChangesTree,
            ChangeTreeViewItem item,
            RowGUIArgs args)
        {
            for (int visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                Rect cellRect = args.GetCellRect(visibleColumnIdx);

                PendingChangesTreeColumn column =
                    (PendingChangesTreeColumn)args.GetColumn(visibleColumnIdx);

                ChangeTreeViewItemCellGUI(
                    isGluonMode,
                    assetStatusCache,
                    cellRect,
                    treeView.rowHeight,
                    treeView,
                    pendingChangesTree,
                    item,
                    column,
                    args.selected,
                    args.focused);
            }
        }

        static void ChangeTreeViewItemCellGUI(
            bool isGluonMode,
            IAssetStatusCache assetStatusCache,
            Rect rect,
            float rowHeight,
            PendingChangesTreeView treeView,
            UnityPendingChangesTree pendingChangesTree,
            ChangeTreeViewItem item,
            PendingChangesTreeColumn column,
            bool isSelected,
            bool isFocused)
        {
            PendingChangeInfo changeInfo = item.ChangeInfo;

            string label = changeInfo.GetColumnText(
                PendingChangesTreeHeaderState.GetColumnName(column));

            DefaultStyles.label.fontSize = UnityConstants.PENDING_CHANGES_FONT_SIZE;

            if (column == PendingChangesTreeColumn.Item)
            {

                if (pendingChangesTree.HasMeta(changeInfo.ChangeInfo))
                    label = string.Concat(label, UnityConstants.TREEVIEW_META_LABEL);

                Texture icon = GetIcon(changeInfo);

                bool isConflicted = IsConflicted(
                    isGluonMode, assetStatusCache,
                    changeInfo.ChangeInfo.GetFullPath());

                Texture overlayIcon =
                    GetChangesOverlayIcon.ForPendingChange(
                        changeInfo.ChangeInfo, isConflicted);

                bool wasChecked = ((ICheckablePlasticTreeNode)changeInfo).IsChecked();

                bool isChecked = DrawTreeViewItem.ForCheckableItemCell(
                    rect, rowHeight, item.depth,
                    icon, overlayIcon, label,
                    isSelected, isFocused, false,
                    wasChecked);

                ((ICheckablePlasticTreeNode)changeInfo).UpdateCheckedState(isChecked);

                if (wasChecked != isChecked)
                {
                    UpdateCheckStateForSelection(treeView, item);
                    treeView.SelectionChanged();
                }

                return;
            }

            if (column == PendingChangesTreeColumn.Size)
            {
                DrawTreeViewItem.ForSecondaryLabelRightAligned(
                    rect, label, isSelected, isFocused, false);
                return;
            }

            DrawTreeViewItem.ForSecondaryLabel(
                rect, label, isSelected, isFocused, false);
        }

        static void UpdateCheckStateForSelection(
            PendingChangesTreeView treeView,
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

        static void RegenerateRows(
            UnityPendingChangesTree pendingChangesTree,
            TreeViewItemIds<IPlasticTreeNode, PendingChangeInfo> treeViewItemIds,
            PendingChangesTreeView treeView,
            TreeViewItem rootItem,
            List<TreeViewItem> rows,
            bool expandCategories)
        {
            ClearRows(rootItem, rows);

            List<IPlasticTreeNode> nodes = pendingChangesTree.GetNodes();

            if (nodes == null)
                return;

            foreach (IPlasticTreeNode node in nodes)
            {
                if (node is ChangelistNode)
                {
                    AddChangelistNode(
                        (ChangelistNode)node, 
                        treeViewItemIds,
                        treeView,
                        rootItem,
                        rows,
                        expandCategories);
                    continue;
                }

                if (node is PendingChangeCategory)
                    AddPendingChangeCategory(
                        (PendingChangeCategory)node,
                        treeViewItemIds,
                        treeView,
                        rootItem,
                        rows,
                        expandCategories,
                        0);
            }

            if (!expandCategories)
                return;

            treeView.state.expandedIDs = treeViewItemIds.GetCategoryIds();
        }

        static void AddChangelistNode(
            ChangelistNode changelist,
            TreeViewItemIds<IPlasticTreeNode, PendingChangeInfo> treeViewItemIds,
            PendingChangesTreeView treeView,
            TreeViewItem rootItem,
            List<TreeViewItem> rows,
            bool expandCategories)
        {
            int changelistCategoryId;
            if (!treeViewItemIds.TryGetCategoryItemId(changelist, out changelistCategoryId))
                changelistCategoryId = treeViewItemIds.AddCategoryItem(changelist);

            ChangelistTreeViewItem changelistTreeViewItem =
                new ChangelistTreeViewItem(changelistCategoryId, changelist);

            rootItem.AddChild(changelistTreeViewItem);
            rows.Add(changelistTreeViewItem);

            if (!expandCategories &&
                !treeView.IsExpanded(changelistTreeViewItem.id))
                return;

            IList<IPlasticTreeNode> categories = ((IPlasticTreeNode)changelist).GetChildren();

            foreach (IPlasticTreeNode category in categories)
            {
                AddPendingChangeCategory(
                    (PendingChangeCategory)category,
                    treeViewItemIds,
                    treeView,
                    changelistTreeViewItem,
                    rows,
                    expandCategories,
                    1);
            }
        }

        static void AddPendingChangeCategory(
            PendingChangeCategory category,
            TreeViewItemIds<IPlasticTreeNode, PendingChangeInfo> treeViewItemIds,
            PendingChangesTreeView treeView,
            TreeViewItem rootItem,
            List<TreeViewItem> rows,
            bool expandCategories,
            int depth)
        {
            int categoryId;
            if (!treeViewItemIds.TryGetCategoryItemId(category, out categoryId))
                categoryId = treeViewItemIds.AddCategoryItem(category);

            ChangeCategoryTreeViewItem categoryTreeViewItem =
                new ChangeCategoryTreeViewItem(categoryId, category, depth);

            rootItem.AddChild(categoryTreeViewItem);
            rows.Add(categoryTreeViewItem);

            if (!expandCategories &&
                !treeView.IsExpanded(categoryTreeViewItem.id))
                return;

            foreach (PendingChangeInfo change in category.GetCurrentChanges())
            {
                int changeId;
                if (!treeViewItemIds.TryGetInfoItemId(change, out changeId))
                    changeId = treeViewItemIds.AddInfoItem(change);

                TreeViewItem changeTreeViewItem =
                    new ChangeTreeViewItem(changeId, change, depth + 1);

                categoryTreeViewItem.AddChild(changeTreeViewItem);
                rows.Add(changeTreeViewItem);
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

        static Texture GetIcon(PendingChangeInfo change)
        {
            if (change.ChangeInfo.IsDirectory)
                return Images.GetDirectoryIcon();

            string fullPath = change.ChangeInfo.GetFullPath();
            return Images.GetFileIcon(fullPath);
        }

        static bool IsConflicted(
            bool isGluonMode,
            IAssetStatusCache assetStatusCache,
            string fullPath)
        {
            if (!isGluonMode)
                return false;

            return ClassifyAssetStatus.IsConflicted(
                assetStatusCache.GetStatus(fullPath));
        }

        bool mExpandCategories;
        bool mIsSelectionChangedEventDisabled;

        TreeViewItemIds<IPlasticTreeNode, PendingChangeInfo> mTreeViewItemIds =
            new TreeViewItemIds<IPlasticTreeNode, PendingChangeInfo>();
        List<TreeViewItem> mRows = new List<TreeViewItem>();

        UnityPendingChangesTree mPendingChangesTree;
        CooldownWindowDelayer mCooldownFilterAction;

        readonly PendingChangesTreeHeaderState mHeaderState;
        readonly PendingChangesViewMenu mMenu;
        readonly IAssetStatusCache mAssetStatusCache;
        readonly List<string> mColumnNames;
        readonly bool mIsGluonMode;
        readonly WorkspaceInfo mWkInfo;
    }
}