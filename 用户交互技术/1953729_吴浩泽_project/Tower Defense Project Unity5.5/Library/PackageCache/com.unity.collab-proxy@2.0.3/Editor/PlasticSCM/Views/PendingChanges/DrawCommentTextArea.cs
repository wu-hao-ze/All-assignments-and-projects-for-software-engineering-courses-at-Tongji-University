using System.Reflection;

using UnityEditor;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal static class DrawCommentTextArea
    {
        internal static void For(
            PendingChangesTab pendingChangesTab,
            float width,
            bool isOperationRunning)
        {
            using (new GuiEnabled(!isOperationRunning))
            {
                EditorGUILayout.BeginHorizontal();

                Rect textAreaRect = BuildTextAreaRect(
                    pendingChangesTab.CommentText,
                    width);

                EditorGUI.BeginChangeCheck();

                pendingChangesTab.CommentText = DoTextArea(
                    pendingChangesTab.CommentText ?? string.Empty,
                    pendingChangesTab.ForceToShowComment,
                    textAreaRect);

                pendingChangesTab.ForceToShowComment = false;

                if (EditorGUI.EndChangeCheck())
                    OnTextAreaChanged(pendingChangesTab);

                if (string.IsNullOrEmpty(pendingChangesTab.CommentText))
                {
                    DoPlaceholderIfNeeded(PlasticLocalization.GetString(
                        PlasticLocalization.Name.CheckinComment),
                        textAreaRect);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        static void OnTextAreaChanged(PendingChangesTab pendingChangesTab)
        {
            pendingChangesTab.ClearIsCommentWarningNeeded();
        }

        static string DoTextArea(
            string text,
            bool forceToShowText,
            Rect textAreaRect)
        {
            // while the text area has the focus, the changes to 
            // the source string will not be picked up by the text editor. 
            // so, when we want to change the text programmatically
            // we have to remove the focus, set the text and then reset the focus.

            TextEditor textEditor = typeof(EditorGUI)
                .GetField("activeEditor", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null) as TextEditor;

            bool shouldBeFocusFixed = forceToShowText && textEditor != null;

            if (shouldBeFocusFixed)
                EditorGUIUtility.keyboardControl = 0;

            var modifiedTextAreaStyle = new GUIStyle(EditorStyles.textArea);
            modifiedTextAreaStyle.padding.left = 7;
            modifiedTextAreaStyle.padding.top = 5;
            modifiedTextAreaStyle.stretchWidth = false;
            modifiedTextAreaStyle.stretchHeight = false;

            text = EditorGUI.TextArea(textAreaRect, text, modifiedTextAreaStyle);

            if (shouldBeFocusFixed)
                EditorGUIUtility.keyboardControl = textEditor.controlID;

            return text;
        }

        static void DoPlaceholderIfNeeded(string placeholder, Rect textAreaRect)
        {
            int textAreaControlId = GUIUtility.GetControlID(FocusType.Passive) - 1;

            if (EditorGUIUtility.keyboardControl == textAreaControlId)
                return;

            Rect hintRect = textAreaRect;
            hintRect.height = EditorStyles.textArea.lineHeight;

            GUI.Label(hintRect, placeholder, UnityStyles.PendingChangesTab.CommentPlaceHolder);
        }

        static Rect BuildTextAreaRect(string text, float width)
        {
            GUIStyle commentTextAreaStyle = UnityStyles.PendingChangesTab.CommentTextArea;
            commentTextAreaStyle.stretchWidth = false;

            // The number here (230) controls how much the right side buttons are pushed off the
            // screen when window is at min width
            float contentWidth = width - 230f;

            Rect result = GUILayoutUtility.GetRect(
                contentWidth,
                UnityConstants.PLASTIC_WINDOW_COMMENT_SECTION_HEIGHT);

            result.width = contentWidth;
            result.height = UnityConstants.PLASTIC_WINDOW_COMMENT_SECTION_HEIGHT;
            result.xMin = 50f;

            return result;
        }
    }
}
