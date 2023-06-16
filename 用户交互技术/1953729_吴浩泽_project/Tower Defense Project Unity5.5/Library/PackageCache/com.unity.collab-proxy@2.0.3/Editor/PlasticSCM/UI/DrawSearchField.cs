using System;
using System.Reflection;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawSearchField
    {
        internal static void For(
            SearchField searchField,
            TreeView treeView,
            float width)
        {
            Rect searchFieldRect = GUILayoutUtility.GetRect(
                width / 2f, EditorGUIUtility.singleLineHeight);
            searchFieldRect.y += 2f;

            treeView.searchString = Draw(
                searchField,
                searchFieldRect,
                treeView.searchString);

            if (!string.IsNullOrEmpty(treeView.searchString))
                return;

            GUI.Label(searchFieldRect, PlasticLocalization.GetString(
                PlasticLocalization.Name.SearchTooltip), UnityStyles.Search);
        }

        static string Draw(
            SearchField searchField,
            Rect searchFieldRect,
            string searchString)
        {
#if UNITY_2019
            if (!mIsToolbarSearchFieldSearched)
            {
                mIsToolbarSearchFieldSearched = true;
                InternalToolbarSearchField = FindToolbarSearchField();
            }

            if (InternalToolbarSearchField != null)
            {
                return (string)InternalToolbarSearchField.Invoke(
                    null,
                    new object[] { searchFieldRect, searchString, false });
            }
#endif
            return searchField.OnToolbarGUI(
                    searchFieldRect, searchString);
        }

#if UNITY_2019
        static MethodInfo FindToolbarSearchField()
        {
            return EditorGUIType.GetMethod(
                "ToolbarSearchField",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(Rect), typeof(string), typeof(bool) },
                null);
        }

        static bool mIsToolbarSearchFieldSearched;
        static MethodInfo InternalToolbarSearchField;

        static readonly Type EditorGUIType = typeof(EditorGUI);
#endif
    }
}
