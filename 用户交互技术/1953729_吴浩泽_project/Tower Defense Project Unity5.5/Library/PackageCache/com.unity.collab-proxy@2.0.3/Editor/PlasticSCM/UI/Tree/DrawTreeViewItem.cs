using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Tree
{
    internal static class DrawTreeViewItem
    {
        internal static void InitializeStyles()
        {
            if (EditorStyles.label == null)
                return;

            TreeView.DefaultStyles.label = UnityStyles.Tree.Label;
            TreeView.DefaultStyles.boldLabel = UnityStyles.Tree.BoldLabel;
        }

        internal static void ForCategoryItem(
            Rect rowRect,
            int depth,
            string label,
            string infoLabel,
            bool isSelected,
            bool isFocused)
        {
            float indent = GetIndent(depth);

            rowRect.x += indent;
            rowRect.width -= indent;

            //add a little indentation
            rowRect.x += 5;
            rowRect.width -= 5;
            
            TreeView.DefaultGUI.Label(rowRect, label, isSelected, isFocused);

            if (!string.IsNullOrEmpty(infoLabel))
                DrawInfolabel(rowRect, label, infoLabel);
        }

        internal static bool ForCheckableCategoryItem(
            Rect rowRect,
            float rowHeight,
            int depth,
            string label,
            string infoLabel,
            bool isSelected,
            bool isFocused,
            bool wasChecked,
            bool hadCheckedChildren,
            bool hadPartiallyCheckedChildren)
        {
            float indent = GetIndent(depth);

            rowRect.x += indent;
            rowRect.width -= indent;

            Rect checkRect = GetCheckboxRect(rowRect, rowHeight);

            if (!wasChecked && (hadCheckedChildren || hadPartiallyCheckedChildren))
                EditorGUI.showMixedValue = true;

            bool isChecked = EditorGUI.Toggle(checkRect, wasChecked);
            EditorGUI.showMixedValue = false;

            rowRect.x = checkRect.xMax - 4;
            rowRect.width -= checkRect.width;

            //add a little indentation
            rowRect.x += 5;
            rowRect.width -= 5;
          
            TreeView.DefaultGUI.Label(rowRect, label, isSelected, isFocused);

            if (!string.IsNullOrEmpty(infoLabel))
                DrawInfolabel(rowRect, label, infoLabel);

            return isChecked;
        }

        internal static void ForItemCell(
            Rect rect,
            float rowHeight,
            int depth,
            Texture icon,
            Texture overlayIcon,
            string label,
            bool isSelected,
            bool isFocused,
            bool isBoldText,
            bool isSecondaryLabel)
        {
            float indent = GetIndent(depth);

            rect.x += indent;
            rect.width -= indent;

            rect = DrawIconLeft(
               rect, rowHeight, icon, overlayIcon);
                  
            if (isSecondaryLabel)
            {
                ForSecondaryLabel(rect, label, isSelected, isFocused, isBoldText);
                return;
            }

            ForLabel(rect, label, isSelected, isFocused, isBoldText);
        }

        internal static bool ForCheckableItemCell(
            Rect rect,
            float rowHeight,
            int depth,
            Texture icon,
            Texture overlayIcon,
            string label,
            bool isSelected,
            bool isFocused,
            bool isHighlighted,
            bool wasChecked)
        {
            float indent = GetIndent(depth);

            rect.x += indent;
            rect.width -= indent;

            Rect checkRect = GetCheckboxRect(rect, rowHeight);

            bool isChecked = EditorGUI.Toggle(checkRect, wasChecked);

            rect.x = checkRect.xMax;
            rect.width -= checkRect.width;

            rect = DrawIconLeft(
                rect, rowHeight, icon, overlayIcon);

            if (isHighlighted)
                TreeView.DefaultGUI.BoldLabel(rect, label, isSelected, isFocused);
            else
                TreeView.DefaultGUI.Label(rect, label, isSelected, isFocused);

            return isChecked;
        }

        internal static Rect DrawIconLeft(
            Rect rect,
            float rowHeight,
            Texture icon,
            Texture overlayIcon)
        {
            if (icon == null)
                return rect;

            float iconWidth = rowHeight * ((float)icon.width / icon.height);

            Rect iconRect = new Rect(rect.x, rect.y, iconWidth, rowHeight);

            EditorGUI.LabelField(iconRect, new GUIContent(icon), UnityStyles.Tree.IconStyle);

            if (overlayIcon != null)
            {
                Rect overlayIconRect = OverlayRect.GetOverlayRect(
                    iconRect,
                    OVERLAY_ICON_OFFSET);

                GUI.DrawTexture(
                    overlayIconRect, overlayIcon,
                    ScaleMode.ScaleToFit);
            }

            rect.x += iconRect.width;
            rect.width -= iconRect.width;

            return rect;
        }

        static void DrawInfolabel(
            Rect rect,
            string label,
            string infoLabel)
        {
            Vector2 labelSize = ((GUIStyle)UnityStyles.Tree.Label)
                .CalcSize(new GUIContent(label));

            rect.x += labelSize.x;

            GUI.Label(rect, infoLabel, UnityStyles.Tree.InfoLabel);
        }

        static Rect GetCheckboxRect(Rect rect, float rowHeight)
        {
            return new Rect(
                rect.x,
                rect.y + UnityConstants.TREEVIEW_CHECKBOX_Y_OFFSET,
                UnityConstants.TREEVIEW_CHECKBOX_SIZE,
                rect.height);
        }

        static float GetIndent(int depth)
        {
            if (depth == -1)
                return 0;

            return 16 + (depth * 16);
        }

        internal static void ForSecondaryLabelRightAligned(
            Rect rect,
            string label,
            bool isSelected,
            bool isFocused,
            bool isBoldText)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (isBoldText)
            {
                GUIStyle secondaryBoldRightAligned =
                    UnityStyles.Tree.SecondaryLabelBoldRightAligned;
                secondaryBoldRightAligned.Draw(
                    rect, label, false, true, isSelected, isFocused);
                return;
            }

            GUIStyle secondaryLabelRightAligned =
                UnityStyles.Tree.SecondaryLabelRightAligned;
            secondaryLabelRightAligned.Draw(
                rect, label, false, true, isSelected, isFocused);
        }

        internal static void ForSecondaryLabel(
            Rect rect,
            string label,
            bool isSelected,
            bool isFocused,
            bool isBoldText)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (isBoldText)
            {
                GUIStyle secondaryBoldLabel =
                    UnityStyles.Tree.SecondaryBoldLabel;

                secondaryBoldLabel.fontSize = UnityConstants.PENDING_CHANGES_FONT_SIZE;
                secondaryBoldLabel.normal.textColor = Color.red;

                secondaryBoldLabel.Draw(
                    rect, label, false, true, isSelected, isFocused);
                return;
            }

            GUIStyle secondaryLabel =
                UnityStyles.Tree.SecondaryLabel;

            secondaryLabel.fontSize = UnityConstants.PENDING_CHANGES_FONT_SIZE;

            secondaryLabel.Draw(
                rect, label, false, true, isSelected, isFocused);
        }

        internal static void ForLabel(
            Rect rect,
            string label,
            bool isSelected,
            bool isFocused,
            bool isBoldText)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (isBoldText)
            {
                TreeView.DefaultStyles.boldLabel.Draw(
                    rect, label, false, true, isSelected, isFocused);
                return;
            }

            TreeView.DefaultStyles.label.Draw(
                rect, label, false, true, isSelected, isFocused);
        }

        const float OVERLAY_ICON_OFFSET = 16f;
    }
}
