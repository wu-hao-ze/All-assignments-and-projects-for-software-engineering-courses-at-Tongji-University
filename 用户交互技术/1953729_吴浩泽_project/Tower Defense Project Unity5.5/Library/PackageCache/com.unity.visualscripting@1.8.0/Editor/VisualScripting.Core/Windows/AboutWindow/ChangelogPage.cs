using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class ChangelogPage : Page
    {
        public ChangelogPage(PluginChangelog changelog, bool showPluginName)
        {
            if (showPluginName)
            {
                title = shortTitle = changelog.plugin.manifest.name;
                subtitle = $"v.{changelog.version}";
            }
            else
            {
                title = subtitle = $"Version {changelog.version}";
            }

            icon = BoltCore.Resources.LoadIcon("ChangelogPage.png");

            this.changelog = changelog;

            changes = changelog.changes.NotNull().Select(FormatChange).ToList();
        }

        private readonly List<Change> changes;

        private readonly PluginChangelog changelog;

        private struct Change
        {
            public GUIContent type { get; set; }
            public GUIContent content { get; set; }
        }

        protected override void OnContentGUI()
        {
            GUILayout.BeginVertical(Styles.background);

            GUILayout.Label(new GUIContent(changelog.plugin.manifest.name), Styles.plugin);
            LudiqGUI.Space(2);
            GUILayout.Label(new GUIContent($"Version {changelog.version}"), Styles.version);

            if (!StringUtility.IsNullOrWhiteSpace(changelog.description))
            {
                LudiqGUI.Space(5);
                GUILayout.Label(new GUIContent(changelog.description.Trim()), Styles.description);
            }

            foreach (var change in changes)
            {
                LudiqGUI.Space(10);
                GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
                GUILayout.Label(GUIContent.none, Styles.bullet);
                GUILayout.Label(change.type, Styles.changeType);
                GUILayout.Label(change.content, Styles.changeContent);
                LudiqGUI.EndHorizontal();
            }

            LudiqGUI.EndVertical();
        }

        private static Change FormatChange(string change)
        {
            change = change.Trim();

            if (change.StartsWith('[') && change.Contains(']'))
            {
                var typeEnd = change.IndexOf(']');
                var type = change.Substring(1, typeEnd - 1).Trim();
                var content = change.Substring(typeEnd + 2).Trim();

                var color = ColorPalette.unitySelectionHighlight.color;

                if (changeTypesColors.ContainsKey(type))
                {
                    color = changeTypesColors[type];
                }

                return new Change() { type = new GUIContent($"<color=#{color.ToHexString()}>{type}</color>"), content = new GUIContent(content) };
            }
            else
            {
                return new Change() { content = new GUIContent(change) };
            }
        }

        // Important to keep that out of Styles because FormatChange can be called before OnGUI.
        // If it does then EditorStyles won't be initialized an an exception will be thrown by the Styles initializer.
        // https://support.ludiq.io/communities/5/topics/2164-c
        public static readonly Dictionary<string, Color> changeTypesColors = new Dictionary<string, Color>()
        {
            { "Added", new Color(0.23f, 0.85f, 0.09f) },
            { "Improved", new Color(0.23f, 0.85f, 0.09f) },
            { "Fixed", new Color(0.33f, 0.73f, 1.00f) },
            { "Optimized", new Color(0.33f, 0.73f, 1.00f) },
            { "Refactored", new Color(0.33f, 0.73f, 1.00f) },
            { "Changed", new Color(1.00f, 0.75f, 0.00f) },
            { "Deprecated", new Color(1.00f, 0.75f, 0.00f) },
            { "Removed", new Color(1.00f, 0.12f, 0.12f) },
        };

        public static class Styles
        {
            static Styles()
            {
                background = ColorPalette.unityBackgroundMid.CreateBackground();
                background.padding = new RectOffset(10, 10, 10, 10);

                plugin = new GUIStyle(EditorStyles.largeLabel);
                plugin.margin = new RectOffset(0, 0, 0, 0);
                plugin.fontSize = 15;

                version = new GUIStyle(EditorStyles.label);
                version.margin = new RectOffset(0, 0, 0, 0);
                version.fontSize = 13;

                description = new GUIStyle(EditorStyles.label);
                description.margin = new RectOffset(0, 0, 0, 0);
                description.wordWrap = true;
                description.richText = true;

                changeType = new GUIStyle("ProfilerBadge");
                changeType.padding = new RectOffset(5, 5, 2, 2);
                changeType.fontSize = 9;
                changeType.richText = true;
                changeType.margin = new RectOffset(0, 3, 1, 0);
                changeType.fixedHeight = 0;
                changeType.border = new RectOffset(6, 6, 6, 6);
                changeType.stretchWidth = false;

                changeContent = new GUIStyle(EditorStyles.label);
                changeContent.wordWrap = true;
                changeContent.richText = true;
                changeContent.margin = new RectOffset(0, 0, 0, 0);

                bullet = new GUIStyle("AC RightArrow");
                bullet.fixedWidth = 13;
                bullet.fixedHeight = 13;
                bullet.margin = new RectOffset(8, 0, 1, 0);
            }

            public static readonly GUIStyle background;
            public static readonly GUIStyle plugin;
            public static readonly GUIStyle version;
            public static readonly GUIStyle description;
            public static readonly GUIStyle changeType;
            public static readonly GUIStyle changeContent;
            public static readonly GUIStyle bullet;
        }
    }
}
