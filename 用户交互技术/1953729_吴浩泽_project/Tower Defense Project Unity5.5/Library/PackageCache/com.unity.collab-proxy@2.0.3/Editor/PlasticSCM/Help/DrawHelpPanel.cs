using System.Diagnostics;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Help
{
    internal static class DrawHelpPanel
    {
        internal static void For(
            HelpPanel helpPanel)
        {
            if (!helpPanel.Visible)
                return;

            DoHelpPanelToolbar(helpPanel);

            GUILayout.Space(10);

            DoHelpPanelContent(helpPanel);
        }

        static void DoHelpPanelToolbar(
            HelpPanel helpPanel)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.y = rect.yMax;
            rect.height = 22;

            GUILayout.Space(1);
            GUIStyle expandableToolbar = new GUIStyle(EditorStyles.toolbar);
            expandableToolbar.fixedHeight = 0;
            GUI.Label(rect, GUIContent.none, expandableToolbar);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("<", EditorStyles.miniButtonLeft))
                {
                    // TODO(codice): On Left Clicked
                }

                if (GUILayout.Button(">", EditorStyles.miniButtonRight))
                {
                    // TODO(codice): On Right Clicked
                }

                GUILayout.FlexibleSpace();

                // TODO(codice): The bool used here must be loaded and persisted by some means
                helpPanel.Data.ShouldShowAgain = EditorGUILayout.ToggleLeft(
                    PlasticLocalization.GetString(PlasticLocalization.Name.DontShowItAgain),
                    helpPanel.Data.ShouldShowAgain, UnityStyles.MiniToggle);
                bool okWasPressed = GUILayout.Button(
                    PlasticLocalization.GetString(PlasticLocalization.Name.OkButton),
                    EditorStyles.miniButton);

                if (okWasPressed)
                {
                    helpPanel.Hide();
                    // TODO(codice): Do on helppanel dismiss actions
                    return;
                }
            }
        }

        static void DoHelpPanelContent(
            HelpPanel helpPanel)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    GUIStyle helpParagraph = UnityStyles.Paragraph;

                    helpPanel.TextScroll = GUILayout.BeginScrollView(helpPanel.TextScroll);

                    GUILayout.Label(helpPanel.GUIContent, helpParagraph);

                    if (Event.current.type != EventType.Layout)
                        DoHelpPanelLinks(helpPanel, helpParagraph);

                    GUILayout.EndScrollView();

                    Rect scrollRect = GUILayoutUtility.GetLastRect();
                    if (Mouse.IsRightMouseButtonPressed(Event.current) &&
                        scrollRect.Contains(Event.current.mousePosition))
                    {
                        GenericMenu contextMenu = BuildHelpPanelMenu(helpPanel.Data.CleanText);
                        contextMenu.ShowAsContext();
                    }
                }
            }
        }

        static void DoHelpPanelLinks(
            HelpPanel helpPanel,
            GUIStyle helpParagraph)
        {
            var lastRect = GUILayoutUtility.GetLastRect();

            bool linkWasClicked = false;
            GUIContent charContent = new GUIContent();
            for (int charIdx = 0; charIdx < helpPanel.GUIContent.text.Length; charIdx++)
            {
                HelpLink link;
                if (!helpPanel.TryGetLinkAtChar(charIdx, out link))
                    continue;

                charContent.text = helpPanel.GUIContent.text[charIdx].ToString();

                var pos = helpParagraph.GetCursorPixelPosition(
                    lastRect, helpPanel.GUIContent, charIdx);

                float charWidth = helpParagraph.CalcSize(charContent).x;

                Rect charRect = new Rect(pos, new Vector2(
                    charWidth - 4, helpParagraph.lineHeight));

                if (!linkWasClicked &&
                    Mouse.IsLeftMouseButtonPressed(Event.current) &&
                    charRect.Contains(Event.current.mousePosition))
                {
                    linkWasClicked = true;
                    OnHelpLinkClicked(helpPanel, link);
                }

                // Underline for links
                charRect.y = charRect.yMax - 1;
                charRect.height = 1;
                GUI.DrawTexture(charRect, Images.GetLinkUnderlineImage());
            }
        }

        static void OnHelpLinkClicked(
            HelpPanel helpPanel,
            HelpLink helpLink)
        {
            HelpLink.LinkType linkType;
            string content;

            if (!HelpLinkData.TryGet(helpLink.Link, out linkType, out content))
                return;

            switch (linkType)
            {
                case HelpLink.LinkType.Action:
                    GuiMessage.ShowInformation(
                        "An ACTION link has been clicked:\n" + content);
                    break;
                case HelpLink.LinkType.Help:
                    helpPanel.Show(
                        content == "sample1" ?
                            TestingHelpData.GetSample1() :
                            TestingHelpData.GetSample2());
                    break;
                case HelpLink.LinkType.Link:
                    Process.Start(content);
                    break;
            }
        }

        static void CopyToClipboard(string data)
        {
            EditorGUIUtility.systemCopyBuffer = data;
        }

        static GenericMenu BuildHelpPanelMenu(string cleanText)
        {
            GenericMenu result = new GenericMenu();

            result.AddItem(
                new GUIContent(PlasticLocalization.GetString(PlasticLocalization.Name.Copy)),
                false,
                () => CopyToClipboard(cleanText)
            );

            return result;
        }
    }
}
