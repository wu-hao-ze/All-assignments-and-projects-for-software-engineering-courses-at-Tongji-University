using System;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class AcknowledgementPage : Page
    {
        public AcknowledgementPage(PluginAcknowledgement acknowledgement)
        {
            this.acknowledgement = acknowledgement;
            title = shortTitle = acknowledgement.title;
            icon = BoltCore.Resources.LoadIcon("AcknowledgementPage.png");

            // Remove single newlines but keep multiple newlines.
            licenseText = acknowledgement.licenseText == null ? null : string.Join("\n\n", acknowledgement.licenseText.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Replace("\r\n", "").Replace("\n", "")).ToArray());
        }

        private readonly PluginAcknowledgement acknowledgement;
        private readonly string licenseText;

        private Vector2 licenseScroll;

        protected override void OnContentGUI()
        {
            GUILayout.BeginVertical(Styles.background);

            GUILayout.Label(acknowledgement.title, Styles.title);

            var hasAuthor = !StringUtility.IsNullOrWhiteSpace(acknowledgement.author);
            var hasCopyright = acknowledgement.copyrightYear != null;

            if (hasAuthor && hasCopyright)
            {
                GUILayout.Label($"Copyright \u00a9 {acknowledgement.copyrightYear} {acknowledgement.author}", Styles.property);
            }
            else if (hasAuthor)
            {
                GUILayout.Label($"Author: {acknowledgement.author}", Styles.property);
            }
            else if (hasCopyright)
            {
                GUILayout.Label($"Copyright \u00a9 {acknowledgement.copyrightYear}", Styles.property);
            }

            if (!StringUtility.IsNullOrWhiteSpace(acknowledgement.url))
            {
                if (GUILayout.Button(acknowledgement.url, Styles.url))
                {
                    Process.Start(acknowledgement.url);
                }
            }

            if (!StringUtility.IsNullOrWhiteSpace(acknowledgement.licenseName))
            {
                GUILayout.Label("License: " + acknowledgement.licenseName.Trim(), Styles.property);
            }

            LudiqGUI.EndVertical();

            if (!StringUtility.IsNullOrWhiteSpace(acknowledgement.licenseText))
            {
                GUILayout.Box(GUIContent.none, LudiqStyles.horizontalSeparator);

                licenseScroll = GUILayout.BeginScrollView(licenseScroll, Styles.licenseBackground);

                GUILayout.Label(licenseText, Styles.licenseText);

                GUILayout.EndScrollView();

                LudiqGUI.Space(-1);
            }
        }

        public static class Styles
        {
            static Styles()
            {
                title = new GUIStyle(EditorStyles.largeLabel);
                title.margin = new RectOffset(0, 0, 0, 0);
                title.fontSize = 15;
                title.margin.bottom = 4;

                property = new GUIStyle(EditorStyles.label);
                property.wordWrap = true;

                background = new GUIStyle(LudiqStyles.windowBackground);
                background.padding = new RectOffset(10, 10, 10, 10);

                url = new GUIStyle(property);
                url.normal.textColor = ColorPalette.hyperlink;
                url.active.textColor = ColorPalette.hyperlinkActive;
                url.wordWrap = true;

                licenseBackground = new GUIStyle(LudiqStyles.windowBackground);
                licenseBackground.margin = new RectOffset(0, 0, 0, 0);
                licenseBackground.padding = new RectOffset(10, 10, 10, 10);

                licenseText = new GUIStyle(EditorStyles.label);
                licenseText.font = Font.CreateDynamicFontFromOSFont(new[] { "Courier", "Courier New" }, 12);
                licenseText.wordWrap = true;
                licenseText.fontSize = 12;
            }

            public static readonly GUIStyle title;
            public static readonly GUIStyle property;
            public static readonly GUIStyle url;
            public static readonly GUIStyle background;
            public static readonly GUIStyle licenseBackground;
            public static readonly GUIStyle licenseText;
        }
    }
}
