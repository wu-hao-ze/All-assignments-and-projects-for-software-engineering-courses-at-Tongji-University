#if UNITY_2020_2_OR_NEWER
[assembly: UnityEditor.Localization]
#else
using UnityEngine;
using UnityEditor;

namespace UnityEditor.Timeline
{
    // dummy functions
    internal static class L10n
    {
        public static string Tr(string str)
        {
            return str;
        }

        public static string[] Tr(string[] str_list)
        {
            return str_list;
        }

        public static string Tr(string str, string groupName)
        {
            return str;
        }

        public static string TrPath(string path)
        {
            return path;
        }

        public static GUIContent TextContent(string text, string tooltip = null, Texture icon = null)
        {
            return EditorGUIUtility.TrTextContent(text, tooltip, icon);
        }

        public static GUIContent TextContent(string text, string tooltip, string iconName)
        {
            return EditorGUIUtility.TrTextContent(text, tooltip, iconName);
        }

        public static GUIContent TextContent(string text, Texture icon)
        {
            return EditorGUIUtility.TrTextContent(text, icon);
        }

        public static GUIContent TextContentWithIcon(string text, Texture icon)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text, icon);
        }

        public static GUIContent TextContentWithIcon(string text, string iconName)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text, iconName);
        }

        public static GUIContent TextContentWithIcon(string text, string tooltip, string iconName)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, iconName);
        }

        public static GUIContent TextContentWithIcon(string text, string tooltip, Texture icon)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, icon);
        }

        public static GUIContent TextContentWithIcon(string text, string tooltip, MessageType messageType)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, messageType);
        }

        public static GUIContent TextContentWithIcon(string text, MessageType messageType)
        {
            return EditorGUIUtility.TrTextContentWithIcon(text, messageType);
        }

        public static GUIContent IconContent(string iconName, string tooltip = null)
        {
            return EditorGUIUtility.TrIconContent(iconName, tooltip);
        }

        public static GUIContent IconContent(Texture icon, string tooltip = null)
        {
            return EditorGUIUtility.TrIconContent(icon, tooltip);
        }

        public static GUIContent TempContent(string t)
        {
            return EditorGUIUtility.TrTempContent(t);
        }

        public static GUIContent[] TempContent(string[] texts)
        {
            return EditorGUIUtility.TrTempContent(texts);
        }

        public static GUIContent[] TempContent(string[] texts, string[] tooltips)
        {
            return EditorGUIUtility.TrTempContent(texts, tooltips);
        }
    }
}
#endif
