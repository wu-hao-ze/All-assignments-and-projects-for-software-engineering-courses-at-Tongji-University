using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class ConfigurationPanel
    {
        public ConfigurationPanel(Product product)
        {
            Ensure.That(nameof(product)).IsNotNull(product);

            this.product = product;
            configurations = product.plugins.Select(plugin => plugin.configuration).ToList();
        }

        private readonly Product product;
        private readonly List<PluginConfiguration> configurations;
        private string label => product.configurationPanelLabel;

        public void PreferenceItem()
        {
            EditorGUIUtility.labelWidth = 220;
            OnGUI();
        }

        public void Show()
        {
            Show(label);
        }

        public IEnumerable<string> GetSearchKeywords()
        {
            List<string> keywords = new List<string>();
            foreach (var configuration in configurations)
            {
                if (configuration.Any(i => i.visible))
                {
                    foreach (var item in configuration.Where(i => i.visible))
                    {
                        keywords.Add(item.member.HumanName());
                    }
                }
            }

            return keywords;
        }

        static ConfigurationPanel()
        {
            if (EditorApplicationUtility.unityVersion >= "2018.3.0")
            {
                return;
            }

            try
            {
                PreferencesWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.PreferencesWindow", true);
                PreferencesWindow_ShowPreferencesWindow = PreferencesWindowType.GetMethod("ShowPreferencesWindow", BindingFlags.Static | BindingFlags.NonPublic);
                PreferencesWindow_selectedSectionIndex = PreferencesWindowType.GetProperty("selectedSectionIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                PreferencesWindow_m_Sections = PreferencesWindowType.GetField("m_Sections", BindingFlags.Instance | BindingFlags.NonPublic);
                PreferencesWindow_m_RefreshCustomPreferences = PreferencesWindowType.GetField("m_RefreshCustomPreferences", BindingFlags.Instance | BindingFlags.NonPublic);
                PreferencesWindow_AddCustomSections = PreferencesWindowType.GetMethod("AddCustomSections", BindingFlags.Instance | BindingFlags.NonPublic);
                PreferencesWindow_s_ScrollPosition = PreferencesWindowType.GetField("s_ScrollPosition", BindingFlags.Static | BindingFlags.NonPublic);

                if (PreferencesWindow_ShowPreferencesWindow == null)
                {
                    throw new MissingMemberException(PreferencesWindowType.FullName, "ShowPreferencesWindow");
                }

                if (PreferencesWindow_selectedSectionIndex == null)
                {
                    throw new MissingMemberException(PreferencesWindowType.FullName, "selectedSectionIndex");
                }

                if (PreferencesWindow_m_Sections == null)
                {
                    throw new MissingMemberException(PreferencesWindowType.FullName, "m_Sections");
                }

                if (PreferencesWindow_m_RefreshCustomPreferences == null)
                {
                    throw new MissingMemberException(PreferencesWindowType.FullName, "m_RefreshCustomPreferences");
                }

                if (PreferencesWindow_AddCustomSections == null)
                {
                    throw new MissingMemberException(PreferencesWindowType.FullName, "AddCustomSections");
                }

                PreferencesWindow_SectionType = typeof(EditorWindow).Assembly.GetType("UnityEditor.PreferencesWindow+Section", true);
                PreferencesWindow_Section_content = PreferencesWindow_SectionType.GetField("content", BindingFlags.Instance | BindingFlags.Public);

                if (PreferencesWindow_Section_content == null)
                {
                    throw new MissingMemberException(PreferencesWindow_SectionType.FullName, "content");
                }

                if (PreferencesWindow_s_ScrollPosition == null)
                {
                    throw new MissingMemberException(PreferencesWindowType.FullName, "s_ScrollPosition");
                }

                internalHooksAvailable = true;
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        #region Internal Hooks

        private static bool internalHooksAvailable;
        private static Type PreferencesWindowType; // typeof(PreferencesWindow);
        private static Type PreferencesWindow_SectionType; // typeof(PreferencesWindow.Section);
        private static FieldInfo PreferencesWindow_m_Sections; // List<PreferencesWindow.Section> PreferencesWindow.m_Sections
        private static PropertyInfo PreferencesWindow_selectedSectionIndex; // PreferencesWindow.selectedSectionIndex;
        private static MethodInfo PreferencesWindow_ShowPreferencesWindow; // PreferencesWindow.ShowPreferencesWindow()
        private static FieldInfo PreferencesWindow_Section_content; // GUIContent PreferencesWindow.Section.content;
        private static FieldInfo PreferencesWindow_m_RefreshCustomPreferences; // bool PreferencesWindow.m_refreshCustomPreferences;
        private static MethodInfo PreferencesWindow_AddCustomSections; // void PreferencesWindow.AddCustomSections();
        private static FieldInfo PreferencesWindow_s_ScrollPosition; // private static Vector2 s_ScrollPosition = Vector2.zero;

        #endregion

        #region Showing

        private static void Show(string label)
        {
            Ensure.That(nameof(label)).IsNotNull(label);

            if (!internalHooksAvailable)
            {
                EditorUtility.DisplayDialog("Preferences Window Moved", $"Use the new Unity Preferences Window to access {label} editor preferences and project settings.", "OK");
                return;
            }

            try
            {
                if (PreferencesWindow_ShowPreferencesWindow == null)
                {
                    PreferencesWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.PreferencesWindow", true);
                    PreferencesWindow_ShowPreferencesWindow = PreferencesWindowType.GetMethod("ShowPreferencesWindow", BindingFlags.Static | BindingFlags.NonPublic);
                    PreferencesWindow_selectedSectionIndex = PreferencesWindowType.GetProperty("selectedSectionIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                    PreferencesWindow_m_Sections = PreferencesWindowType.GetField("m_Sections", BindingFlags.Instance | BindingFlags.NonPublic);
                    PreferencesWindow_m_RefreshCustomPreferences = PreferencesWindowType.GetField("m_RefreshCustomPreferences", BindingFlags.Instance | BindingFlags.NonPublic);
                    PreferencesWindow_AddCustomSections = PreferencesWindowType.GetMethod("AddCustomSections", BindingFlags.Instance | BindingFlags.NonPublic);

                    if (PreferencesWindow_ShowPreferencesWindow == null)
                    {
                        throw new MissingMemberException(PreferencesWindowType.FullName, "ShowPreferencesWindow");
                    }

                    if (PreferencesWindow_selectedSectionIndex == null)
                    {
                        throw new MissingMemberException(PreferencesWindowType.FullName, "selectedSectionIndex");
                    }

                    if (PreferencesWindow_m_Sections == null)
                    {
                        throw new MissingMemberException(PreferencesWindowType.FullName, "m_Sections");
                    }

                    if (PreferencesWindow_m_RefreshCustomPreferences == null)
                    {
                        throw new MissingMemberException(PreferencesWindowType.FullName, "m_RefreshCustomPreferences");
                    }

                    if (PreferencesWindow_AddCustomSections == null)
                    {
                        throw new MissingMemberException(PreferencesWindowType.FullName, "AddCustomSections");
                    }

                    PreferencesWindow_SectionType = typeof(EditorWindow).Assembly.GetType("UnityEditor.PreferencesWindow+Section", true);
                    PreferencesWindow_Section_content = PreferencesWindow_SectionType.GetField("content", BindingFlags.Instance | BindingFlags.Public);

                    if (PreferencesWindow_Section_content == null)
                    {
                        throw new MissingMemberException(PreferencesWindow_SectionType.FullName, "content");
                    }
                }

                PreferencesWindow_ShowPreferencesWindow.Invoke(null, new object[0]);
                var window = EditorWindow.GetWindow(PreferencesWindowType, true);
                window.Center();

                if ((bool)PreferencesWindow_m_RefreshCustomPreferences.GetValue(window))
                {
                    PreferencesWindow_AddCustomSections.Invoke(window, new object[0]);
                    PreferencesWindow_m_RefreshCustomPreferences.SetValue(window, false);
                }

                var sections = ((IList)PreferencesWindow_m_Sections.GetValue(window)).Cast<object>().ToList();
                var section = sections.Single(s => ((GUIContent)PreferencesWindow_Section_content.GetValue(s)).text == label);
                var sectionIndex = sections.IndexOf(section);
                PreferencesWindow_selectedSectionIndex.SetValue(window, sectionIndex, null);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        #endregion

        public static class Styles
        {
            static Styles()
            {
                header = new GUIStyle(EditorStyles.boldLabel);
                header.fontSize = 14;
                header.margin = new RectOffset(2, 0, 15, 6);
                header.padding = new RectOffset(0, 0, 0, 0);

                tabBackground = new GUIStyle("ButtonMid");
                tabBackground.alignment = TextAnchor.UpperLeft;
                tabBackground.margin = new RectOffset(0, 0, 0, 0);
                tabBackground.padding = new RectOffset(7, 7, 7, 7);
                tabBackground.fixedHeight = 54;

                tabIcon = new GUIStyle();
                tabIcon.fixedWidth = tabIcon.fixedHeight = 24;
                tabIcon.margin = new RectOffset(0, 7, 2, 0);

                tabTitle = new GUIStyle(EditorStyles.label);
                tabTitle.padding = new RectOffset(0, 0, 0, 0);
                tabTitle.margin = new RectOffset(0, 0, 0, 0);
                tabTitle.normal.background = ColorPalette.transparent.GetPixel();
                tabTitle.onNormal.background = ColorPalette.transparent.GetPixel();
                tabTitle.normal.textColor = ColorPalette.unityForeground;
                tabTitle.onNormal.textColor = ColorPalette.unityForegroundSelected;

                tabDescription = new GUIStyle();
                tabDescription.wordWrap = true;
                tabDescription.fontSize = 10;
                tabDescription.margin = new RectOffset(0, 0, 0, 0);
                tabDescription.normal.background = ColorPalette.transparent.GetPixel();
                tabDescription.onNormal.background = ColorPalette.transparent.GetPixel();
                tabDescription.normal.textColor = ColorPalette.unityForegroundDim;
                tabDescription.onNormal.textColor = ColorPalette.unityForegroundSelected;
            }

            public const int iconSize = 12;

            public static readonly GUIStyle header;
            public static readonly GUIStyle tabBackground;
            public static readonly GUIStyle tabIcon;
            public static readonly GUIStyle tabTitle;
            public static readonly GUIStyle tabDescription;
        }

        #region Drawing

        private static Vector2 _scroll;

        private static Vector2 scroll
        {
            get
            {
                return _scroll;
                //return (Vector2)PreferencesWindow_s_ScrollPosition.GetValue(null);
            }
            set
            {
                _scroll = value;

                if (internalHooksAvailable)
                {
                    PreferencesWindow_s_ScrollPosition.SetValue(null, value);
                }
            }
        }

        private static void Header(string text)
        {
            GUILayout.Label(text, Styles.header);
            LudiqGUI.Space(4);
        }

        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

            scroll = GUILayout.BeginScrollView(scroll);

            LudiqGUI.BeginHorizontal();
            LudiqGUI.BeginVertical();

            foreach (var configuration in configurations)
            {
                if (configuration.Any(i => i.visible))
                {
                    if (configurations.Count > 1)
                    {
                        Header(configuration.header.Replace(label + " ", ""));
                    }

                    EditorGUI.BeginChangeCheck();

                    using (Inspector.expandTooltip.Override(true))
                    {
                        foreach (var item in configuration.Where(i => i.visible))
                        {
                            LudiqGUI.Space(2);

                            LudiqGUI.BeginHorizontal();

                            LudiqGUI.Space(4);

                            var iconPosition = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(Styles.iconSize), GUILayout.Height(Styles.iconSize), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false));

                            EditorTexture icon = null;
                            string tooltip = null;

                            if (item is ProjectSettingMetadata)
                            {
                                icon = BoltCore.Icons.projectSetting;
                                tooltip = "Project Setting: Shared across users, local to this project. Included in version control.";
                            }
                            else if (item is EditorPrefMetadata)
                            {
                                icon = BoltCore.Icons.editorPref;
                                tooltip = "Editor Pref: Local to this user, shared across projects. Excluded from version control.";
                            }

                            if (icon != null)
                            {
                                using (LudiqGUI.color.Override(GUI.color.WithAlpha(0.6f)))
                                {
                                    GUI.Label(iconPosition, new GUIContent(icon[Styles.iconSize], tooltip), GUIStyle.none);
                                }
                            }

                            LudiqGUI.Space(6);

                            LudiqGUI.BeginVertical();

                            LudiqGUI.Space(-3);

                            LudiqGUI.InspectorLayout(item);

                            LudiqGUI.EndVertical();

                            LudiqGUI.EndHorizontal();
                        }
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        configuration.Save();
                        InternalEditorUtility.RepaintAllViews();
                    }
                }
            }

            LudiqGUI.Space(8);

            if (GUILayout.Button("Reset to Defaults"))
            {
                if (EditorUtility.DisplayDialog("Reset to Defaults", "Are you sure you want to reset your preferences and project settings to default?", "Reset", "Cancel"))
                {
                    foreach (var configuration in configurations)
                    {
                        configuration.Reset();
                        configuration.Save();
                    }

                    InternalEditorUtility.RepaintAllViews();
                }
            }

            LudiqGUI.Space(8);
            LudiqGUI.EndVertical();
            LudiqGUI.Space(8);
            LudiqGUI.EndHorizontal();
            GUILayout.EndScrollView();
            EditorGUI.EndDisabledGroup();
        }

        #endregion
    }
}
