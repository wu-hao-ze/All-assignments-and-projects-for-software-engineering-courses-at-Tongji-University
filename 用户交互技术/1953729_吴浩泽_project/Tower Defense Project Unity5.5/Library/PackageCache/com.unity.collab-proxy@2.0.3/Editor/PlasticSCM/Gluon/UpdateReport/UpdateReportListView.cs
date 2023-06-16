using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Gluon.UpdateReport
{
    internal class UpdateReportListView : TreeView
    {
        internal UpdateReportListView(
            WorkspaceInfo wkInfo,
            UpdateReportListHeaderState headerState,
            Action onCheckedErrorChanged)
            : base(new TreeViewState())
        {
            mWkInfo = wkInfo;
            mOnCheckedErrorChanged = onCheckedErrorChanged;

            multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.canSort = false;

            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = false;
        }

        public override IList<TreeViewItem> GetRows()
        {
            return mRows;
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1, string.Empty);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem rootItem)
        {
            RegenerateRows(
                this, mErrorMessages, rootItem, mRows);

            return mRows;
        }

        protected override void BeforeRowsGUI()
        {
            int firstRowVisible;
            int lastRowVisible;
            GetFirstAndLastVisibleRows(out firstRowVisible, out lastRowVisible);

            GUI.DrawTexture(new Rect(0,
                firstRowVisible * rowHeight,
                GetRowRect(0).width+500,
                (lastRowVisible * rowHeight) + 1000),
                Images.GetTreeviewBackgroundTexture());

            DrawTreeViewItem.InitializeStyles();
            base.BeforeRowsGUI();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.item is ErrorListViewItem)
            {
                ErrorListViewItemGUI(
                    rowHeight, mWkInfo, mCheckedErrors,
                    (ErrorListViewItem)args.item,
                    mOnCheckedErrorChanged, args);
                return;
            }

            base.RowGUI(args);
        }

        internal void BuildModel(List<ErrorMessage> errorMessages)
        {
            mCheckedErrors.Clear();

            mErrorMessages = errorMessages;

            mOnCheckedErrorChanged();
        }

        internal void CheckAllLines()
        {
            mCheckedErrors.Clear();

            foreach (ErrorMessage error in mErrorMessages)
                mCheckedErrors.Add(error);

            mOnCheckedErrorChanged();
        }

        internal void UncheckAllLines()
        {
            mCheckedErrors.Clear();

            mOnCheckedErrorChanged();
        }

        internal bool AreAllErrorsChecked()
        {
            if (mErrorMessages.Count == 0)
                return false;

            return mCheckedErrors.Count == mErrorMessages.Count;
        }

        internal bool IsAnyErrorChecked()
        {
            return mCheckedErrors.Count > 0;
        }

        internal List<string> GetCheckedPaths()
        {
            return mCheckedErrors.Select(
                message => message.Path).ToList();
        }

        internal List<ErrorMessage> GetUncheckedErrors()
        {
            return mErrorMessages.Where(
                message => !mCheckedErrors.Contains(message)).ToList();
        }

        internal ErrorMessage GetSelectedError()
        {
            List<ErrorMessage> selectedErrors = GetSelectedErrors(this);

            if (selectedErrors.Count != 1)
                return null;

            return selectedErrors[0];
        }

        static void UpdateCheckState(
            HashSet<ErrorMessage> checkedErrors,
            ErrorMessage errorMessage,
            bool isChecked)
        {
            if (isChecked)
            {
                checkedErrors.Add(errorMessage);
                return;
            }

            checkedErrors.Remove(errorMessage);
        }

        static List<ErrorMessage> GetSelectedErrors(
            UpdateReportListView listView)
        {
            List<ErrorMessage> result = new List<ErrorMessage>();

            IList<int> selectedIds = listView.GetSelection();

            if (selectedIds.Count == 0)
                return result;

            foreach (ErrorListViewItem treeViewItem in
                listView.FindRows(selectedIds))
            {
                result.Add(treeViewItem.ErrorMessage);
            }

            return result;
        }

        static void RegenerateRows(
            UpdateReportListView listView,
            List<ErrorMessage> errorMessages,
            TreeViewItem rootItem,
            List<TreeViewItem> rows)
        {
            ClearRows(rootItem, rows);

            if (errorMessages.Count == 0)
                return;

            for (int i = 0; i < errorMessages.Count; i++)
            {
                ErrorListViewItem errorListViewItem =
                    new ErrorListViewItem(i + 1, errorMessages[i]);

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

        static void ErrorListViewItemGUI(
            float rowHeight,
            WorkspaceInfo wkInfo,
            HashSet<ErrorMessage> checkedErrors,
            ErrorListViewItem item,
            Action onCheckedErrorChanged,
            RowGUIArgs args)
        {
            for (int visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                Rect cellRect = args.GetCellRect(visibleColumnIdx);

                UpdateReportListColumn column =
                    (UpdateReportListColumn)args.GetColumn(visibleColumnIdx);

                ErrorListViewItemCellGUI(
                    cellRect, rowHeight, wkInfo, checkedErrors,
                    item, onCheckedErrorChanged, column,
                    args.selected, args.focused);
            }
        }

        static void ErrorListViewItemCellGUI(
            Rect rect,
            float rowHeight,
            WorkspaceInfo wkInfo,
            HashSet<ErrorMessage> checkedErrors,
            ErrorListViewItem item,
            Action onCheckedErrorChanged,
            UpdateReportListColumn column,
            bool isSelected,
            bool isFocused)
        {
            ErrorMessage errorMessage = item.ErrorMessage;

            string label = GetColumnText(
                wkInfo, errorMessage,
                UpdateReportListHeaderState.GetColumnName(column));

            bool wasChecked = checkedErrors.Contains(errorMessage);

            bool isChecked = DrawTreeViewItem.ForCheckableItemCell(
                rect, rowHeight, 0, null, null, label,
                isSelected, isFocused, false, wasChecked);

            if (wasChecked != isChecked)
            {
                UpdateCheckState(
                    checkedErrors, errorMessage, isChecked);

                onCheckedErrorChanged();
            }
        }

        static string GetColumnText(
            WorkspaceInfo wkInfo, ErrorMessage message, string columnName)
        {
            if (columnName != PlasticLocalization.GetString(
                    PlasticLocalization.Name.PathColumn))
            {
                return string.Empty;
            }

            return WorkspacePath.ClientToCM(
                message.Path, wkInfo.ClientPath);
        }

        List<TreeViewItem> mRows = new List<TreeViewItem>();

        HashSet<ErrorMessage> mCheckedErrors = new HashSet<ErrorMessage>();
        List<ErrorMessage> mErrorMessages = new List<ErrorMessage>();

        readonly Action mOnCheckedErrorChanged;
        readonly WorkspaceInfo mWkInfo;
    }
}
