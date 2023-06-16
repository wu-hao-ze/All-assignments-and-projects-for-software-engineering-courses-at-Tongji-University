using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class AboutablePage : Page
    {
        public AboutablePage(IAboutable aboutable)
        {
            Ensure.That(nameof(aboutable)).IsNotNull(aboutable);

            title = $"About {aboutable.name}";
            shortTitle = aboutable.name;
            subtitle = $"v.{aboutable.version}";
            icon = BoltCore.Resources.LoadIcon("AboutPage.png");

            this.aboutable = aboutable;
        }

        private readonly IAboutable aboutable;

        protected override void OnContentGUI()
        {
            GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

            LudiqGUI.FlexibleSpace();

            EditorGUILayout.LabelField($"Version {aboutable.version}", LudiqStyles.centeredLabel);

            LudiqGUI.FlexibleSpace();

            if (!StringUtility.IsNullOrWhiteSpace(aboutable.description))
            {
                EditorGUILayout.LabelField(aboutable.description.Trim(), LudiqStyles.centeredLabel);
            }

            LudiqGUI.FlexibleSpace();

            if (!StringUtility.IsNullOrWhiteSpace(aboutable.author))
            {
                EditorGUILayout.LabelField($"{aboutable.authorLabel.Trim()} {aboutable.author.Trim()}", LudiqStyles.centeredLabel);
            }

            if (!StringUtility.IsNullOrWhiteSpace(aboutable.copyrightHolder))
            {
                EditorGUILayout.LabelField($"Copyright \u00a9 {aboutable.copyrightYear} {aboutable.copyrightHolder.Trim()}. All Rights Reserved.", LudiqStyles.centeredLabel);
            }

            if (aboutable.authorLogo != null)
            {
                LudiqGUI.FlexibleSpace();

                LudiqGUI.BeginHorizontal();
                LudiqGUI.FlexibleSpace();
                var logoHeight = Styles.authorLogoHeight;
                var logoWidth = (float)aboutable.authorLogo.width / aboutable.authorLogo.height * logoHeight;
                var logoPosition = GUILayoutUtility.GetRect(logoWidth, logoHeight);

                if (!string.IsNullOrEmpty(aboutable.authorUrl))
                {
                    if (GUI.Button(logoPosition, aboutable.authorLogo, GUIStyle.none))
                    {
                        Process.Start(aboutable.authorUrl);
                    }
                }
                else if (e.type == EventType.Repaint)
                {
                    GUI.DrawTexture(logoPosition, aboutable.authorLogo);
                }

                LudiqGUI.FlexibleSpace();
                LudiqGUI.EndHorizontal();
            }

            LudiqGUI.FlexibleSpace();
            LudiqGUI.EndVertical();
        }

        public static class Styles
        {
            static Styles()
            {
                background = new GUIStyle(LudiqStyles.windowBackground);
                background.padding = new RectOffset(10, 10, 10, 10);
            }

            public static readonly GUIStyle background;
            public static readonly float authorLogoHeight = 50;
        }
    }
}
