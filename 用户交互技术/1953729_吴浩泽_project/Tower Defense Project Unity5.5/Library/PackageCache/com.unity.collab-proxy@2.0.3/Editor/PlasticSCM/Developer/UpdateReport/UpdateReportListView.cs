using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.Common;
using Codice.CM.Common;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Developer.UpdateReport
{
    internal class UpdateReportListView : TreeView
    {
        internal UpdateReportListView(
            WorkspaceInfo wkInfo,
            UpdateReportListHeaderState headerState,
            Action onCheckedReportLineChanged)
            : base(new TreeViewState())
        {
            mWkInfo = wkInfo;
            mOnCheckedReportLineChanged = onCheckedReportLineChanged;

            multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.canSort = false;

            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = false;
        }

        internal List<ReportLine> GetCheckedLines()
        {
            List<ReportLine> result = new List<ReportLine>();

            foreach (UpdateReportLineListViewItem item in mCheckedLines)
                result.Add(item.ReportLine);

            return result;
        }

        internal bool IsAnyLineChecked()
        {
            return mCheckedLines.Count > 0;
        }

        internal bool AreAllLinesChecked()
        {
            if (mReportLines.Count == 0)
                return false;

            return mCheckedLines.Count == mReportLines.Count;
        }

        internal void CheckAllLines()
        {
            mCheckedLines.Clear();

            foreach (UpdateReportLineListViewItem row in mRows)
            {
                mCheckedLines.Add(row);
            }

            mOnCheckedReportLineChanged();
        }

        internal void UnCheckAllLines()
        {
            mCheckedLines.Clear();
            mOnCheckedReportLineChanged();
        }

        internal void BuildModel(IList reportLines)
        {
            mReportLines = reportLines;
            mCheckedLines.Clear();
            mOnCheckedReportLineChanged();
        }

        internal ReportLine GetSelectedError()
        {
            List<ReportLine> selectedErrors = GetSelectedErrors(this);

            if (selectedErrors.Count != 1)
                return null;

            return selectedErrors[0];
        }

        public override IList<TreeViewItem> GetRows()
        {
            return mRows;
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1, string.Empty);
        }

        protected override IList<TreeViewItem> BuildRows(
            TreeViewItem rootItem)
        {
            RegenerateRows(
                this, mReportLines, rootItem, mRows);

            return mRows;
        }

        protected override void BeforeRowsGUI()
        {
            int firstRowVisible;
            int lastRowVisible;
            GetFirstAndLastVisibleRows(out firstRowVisible, out lastRowVisible);

            GUI.DrawTexture(new Rect(0,
                firstRowVisible * rowHeight,
                GetRowRect(0).width + 500,
                (lastRowVisible * rowHeight) + 1000),
                Images.GetTreeviewBackgroundTexture());

            DrawTreeViewItem.InitializeStyles();
            base.BeforeRowsGUI();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.item is UpdateReportLineListViewItem)
            {
                UpdateReportListViewItemGUI(
                    mWkInfo.ClientPath,
                    (UpdateReportLineListViewItem)args.item,
                    args,
                    rowHeight,
                    mReportLines.Count,
                    mOnCheckedReportLineChanged,
                    mCheckedLines);
                return;
            }

            base.RowGUI(args);
        }

        static List<ReportLine> GetSelectedErrors(
            UpdateReportListView listView)
        {
            List<ReportLine> result = new List<ReportLine>();

            IList<int> selectedIds = listView.GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (UpdateReportLineListViewItem treeViewItem in
                listView.FindRows(selectedIds))
            {
                result.Add(treeViewItem.ReportLine);
            }

            return result;
        }

        static void RegenerateRows(
            UpdateReportListView listView,
            IList reportLines,
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            ClearRows(rootItem, rows);

            if (reportLines.Count == 0)
                return;

            for (int i = 0; i < reportLines.Count; i++)
            {
                UpdateReportLineListViewItem errorListViewItem =
                    new UpdateReportLineListViewItem(i + 1, (ReportLine)reportLines[i]);

                rootItem.AddChild(errorListViewItem);
                rows.Add(errorListViewItem);
            }

            listView.SetSelection(new List<int> { 1 });
        }

        static void ClearRows(
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            if (rootItem.hasChildren)
                rootItem.children.Clear();

            rows.Clear();
        }

        static void UpdateReportListViewItemGUI(
            string wkPath,
            UpdateReportLineListViewItem item,
            RowGUIArgs args,
            float rowHeight,
            int totalLinesCount,
            Action onCheckedReportLineChanged,
            HashSet<UpdateReportLineListViewItem> checkedLines)
        {
            for (int visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                Rect cellRect = args.GetCellRect(visibleColumnIdx);

                ErrorsListColumn column =
                    (ErrorsListColumn)args.GetColumn(visibleColumnIdx);

                UpdateReportListViewItemCellGUI(
                    cellRect,
                    wkPath,
                    item,
                    column,
                    rowHeight,
                    args.selected,
                    args.focused,
                    totalLinesCount,
                    onCheckedReportLineChanged,
                    checkedLines);
            }
        }

        static void UpdateReportListViewItemCellGUI(
            Rect rect,
            string wkPath,
            UpdateReportLineListViewItem item,
            ErrorsListColumn column,
            float rowHeight,
            bool isSelected,
            bool isFocused,
            int totalLinesCount,
            Action onCheckedReportLineChanged,
            HashSet<UpdateReportLineListViewItem> checkedLines)
        {
            string label = WorkspacePath.GetWorkspaceRelativePath(
                wkPath,
                item.ReportLine.ItemPath);

            bool wasChecked = checkedLines.Contains(item);
            bool isChecked = DrawTreeViewItem.ForCheckableItemCell(
                rect,
                rowHeight,
                0,
                null,
                null,
                label,
                isSelected,
                isFocused,
                false,
                wasChecked);

            if (wasChecked != isChecked)
            {
                UpdateCheckedState(checkedLines, item, isChecked);
                onCheckedReportLineChanged();
            }
        }

        static void UpdateCheckedState(
            HashSet<UpdateReportLineListViewItem> checkedLines,
            UpdateReportLineListViewItem item,
            bool isChecked)
        {
            if (isChecked)
            {
                checkedLines.Add(item);
                return;
            }

            checkedLines.Remove(item);
        }

        List<TreeViewItem> mRows = new List<TreeViewItem>();
        IList mReportLines = new ArrayList();

        HashSet<UpdateReportLineListViewItem> mCheckedLines =
            new HashSet<UpdateReportLineListViewItem>();

        readonly WorkspaceInfo mWkInfo;
        readonly Action mOnCheckedReportLineChanged;
    }
}
