using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Tree
{
    internal static class DrawTreeViewEmptyState
    {
        internal static void For(
            Rect rect,
            string text)
        {
            GUIContent content = new GUIContent(text);
            Vector2 contentSize = GetContentSize(content);

            GUI.BeginGroup(rect);

            DrawLabel(
                content,
                contentSize,
                (rect.width - contentSize.x) / 2,
                rect.height / 2);

            GUI.EndGroup();
        }

        internal static void For(
            Rect rect,
            string text,
            Texture2D icon)
        {
            GUIContent content = new GUIContent(text);
            Vector2 contentSize = GetContentSize(content);

            GUI.BeginGroup(rect);

            DrawLabelWithIcon(
                content,
                contentSize,
                (rect.width - contentSize.x) / 2,
                rect.height / 2,
                icon);

            GUI.EndGroup();
        }

        static void DrawLabel(
            GUIContent content,
            Vector2 contentSize,
            float offsetX,
            float offsetY)
        {
            GUI.Label(
                new Rect(offsetX, offsetY, contentSize.x, contentSize.y),
                content,
                UnityStyles.Tree.StatusLabel);
        }

        static void DrawLabelWithIcon(
            GUIContent content,
            Vector2 contentSize,
            float offsetX,
            float offsetY,
            Texture2D icon)
        {
            int iconSize = UnityConstants.TREEVIEW_STATUS_ICON_SIZE;
            int padding = UnityConstants.TREEVIEW_STATUS_CONTENT_PADDING;

            float iconOffsetX = offsetX - iconSize + padding;
            float contentOffsetX = offsetX + iconSize - padding;

            GUI.DrawTexture(
                new Rect(iconOffsetX, offsetY + padding, iconSize, iconSize),
                icon,
                ScaleMode.ScaleToFit);

            DrawLabel(
                content,
                contentSize,
                contentOffsetX,
                offsetY);
        }

        static Vector2 GetContentSize(GUIContent content)
        {
            return ((GUIStyle)UnityStyles.Tree.StatusLabel).CalcSize(content);
        }
    }
}