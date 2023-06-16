using System.Collections.Generic;

using UnityEngine;
using UnityEditor.IMGUI.Controls;

using Codice.Client.BaseCommands;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Gluon.Errors
{
    internal class ErrorsListView : TreeView
    {
        internal ErrorsListView(ErrorsListHeaderState headerState)
            : base(new TreeViewState())
        {
            multiColumnHeader = new MultiColumnHeader(headerState);
            multiColumnHeader.canSort = false;

            rowHeight = UnityConstants.TREEVIEW_ROW_HEIGHT;
            showAlternatingRowBackgrounds = true;
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
                GetRowRect(0).width,
                (lastRowVisible * rowHeight) + 1000),
                Images.GetTreeviewBackgroundTexture());

            DrawTreeViewItem.InitializeStyles();
            base.BeforeRowsGUI();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.item is ErrorListViewItem)
            {
                ErrorListViewItemGUI((ErrorListViewItem)args.item, args);
                return;
            }

            base.RowGUI(args);
        }

        internal void BuildModel(List<ErrorMessage> errorMessages)
        {
            mErrorMessages = errorMessages;
        }

        internal ErrorMessage GetSelectedError()
        {
            List<ErrorMessage> selectedErrors = GetSelectedErrors(this);

            if (selectedErrors.Count != 1)
                return null;

            return selectedErrors[0];
        }

        static List<ErrorMessage> GetSelectedErrors(
            ErrorsListView listView)
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
            ErrorsListView listView,
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
            ErrorListViewItem item,
            RowGUIArgs args)
        {
            for (int visibleColumnIdx = 0; visibleColumnIdx < args.GetNumVisibleColumns(); visibleColumnIdx++)
            {
                Rect cellRect = args.GetCellRect(visibleColumnIdx);

                ErrorsListColumn column =
                    (ErrorsListColumn)args.GetColumn(visibleColumnIdx);

                ErrorListViewItemCellGUI(
                    cellRect, item, column, args.selected, args.focused);
            }
        }

        static void ErrorListViewItemCellGUI(
            Rect rect,
            ErrorListViewItem item,
            ErrorsListColumn column,
            bool isSelected,
            bool isFocused)
        {
            ErrorMessage errorMessage = item.ErrorMessage;

            string label = column == ErrorsListColumn.Path ?
                errorMessage.Path : errorMessage.Error;

            if (column == ErrorsListColumn.Path)
            {
                DrawTreeViewItem.ForLabel(
                    rect, label, isSelected, isFocused, false);
                return;
            }

            DrawTreeViewItem.ForSecondaryLabel(
                rect, label, isSelected, isFocused, false);
        }

        List<TreeViewItem> mRows = new List<TreeViewItem>();

        List<ErrorMessage> mErrorMessages = new List<ErrorMessage>();
    }
}
