using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal class PendingChangesMultiColumnHeader : MultiColumnHeader
    {
        internal PendingChangesMultiColumnHeader(
            PendingChangesTreeView treeView,
            MultiColumnHeaderState headerState, 
            UnityPendingChangesTree tree)
            : base(headerState)
        {
            mPendingChangesTreeView = treeView;
            mPendingChangesTree = tree;
        }

        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            if (columnIndex == 0)
            {
                bool checkAllWasMixed = IsMixedCheckedState();
                bool checkAllWasTrue = IsAllCheckedState();

                var checkRect = new Rect(
                    headerRect.x + UnityConstants.TREEVIEW_BASE_INDENT,
                    headerRect.y + 3 + UnityConstants.TREEVIEW_HEADER_CHECKBOX_Y_OFFSET,  // Custom offset because header labels are not centered
                    UnityConstants.TREEVIEW_CHECKBOX_SIZE,
                    headerRect.height);
                
                EditorGUI.showMixedValue = checkAllWasMixed;
                bool checkAllIsTrue = EditorGUI.Toggle(
                    checkRect,
                    checkAllWasTrue);
                EditorGUI.showMixedValue = false;

                if (checkAllWasTrue != checkAllIsTrue)
                {
                    UpdateCheckedState(checkAllIsTrue);
                    ((PendingChangesTreeHeaderState)state).UpdateItemColumnHeader(mPendingChangesTreeView);
                }

                headerRect.x = checkRect.xMax;
                headerRect.xMax = column.width;
            }
            base.ColumnHeaderGUI(column, headerRect, columnIndex);
        }

        internal bool IsAllCheckedState()
        {
            List<IPlasticTreeNode> nodes = mPendingChangesTree.GetNodes();

            if (nodes == null || nodes.Count == 0)
                return false;

            foreach (IPlasticTreeNode node in nodes)
            {
                if (!(CheckedItems.GetIsCheckedValue(node) ?? false))
                    return false;
            }

            return true;
        }

        protected bool IsMixedCheckedState()
        {
            List<IPlasticTreeNode> nodes = mPendingChangesTree.GetNodes();

            if (nodes == null)
                return false;

            bool hasCheckedNode = false;
            bool hasUncheckedNode = false;
            foreach (IPlasticTreeNode node in nodes)
            {
                if (CheckedItems.GetIsPartiallyCheckedValue(node))
                    return true;

                if (CheckedItems.GetIsCheckedValue(node) ?? false)
                    hasCheckedNode = true;
                else
                    hasUncheckedNode = true;

                if (hasCheckedNode && hasUncheckedNode)
                    return true;
            }

            return false;
        }

        internal void UpdateCheckedState(bool isChecked)
        {
            List<IPlasticTreeNode> nodes = mPendingChangesTree.GetNodes();

            if (nodes == null)
                return;

            foreach (IPlasticTreeNode node in nodes)
                CheckedItems.SetCheckedValue(node, isChecked);
        }

        readonly PendingChangesTreeView mPendingChangesTreeView;
        protected UnityPendingChangesTree mPendingChangesTree;
    }
}