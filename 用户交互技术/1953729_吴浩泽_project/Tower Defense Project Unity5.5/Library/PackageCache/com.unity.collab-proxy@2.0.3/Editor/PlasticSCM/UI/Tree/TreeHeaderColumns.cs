using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Tree
{
    internal static class TreeHeaderColumns
    {
        internal static void SetTitles(
            MultiColumnHeaderState.Column[] columns, string[] headerTitles)
        {
            for (int i = 0; i < headerTitles.Length; i++)
                columns[i].headerContent = new GUIContent(headerTitles[i]);
        }

        internal static void SetVisibilities(
            MultiColumnHeaderState.Column[] columns, bool[] visibilities)
        {
            for (int i = 0; i < visibilities.Length; i++)
                columns[i].allowToggleVisibility = visibilities[i];
        }

        internal static void SetWidths(
            MultiColumnHeaderState.Column[] columns, float[] widths)
        {
            for (int i = 0; i < widths.Length; i++)
                columns[i].width = widths[i];
        }

        internal static string[] GetTitles(
            MultiColumnHeaderState.Column[] columns)
        {
            string[] titles = new string[columns.Length];

            for (int i = 0; i < columns.Length; i++)
                titles[i] = columns[i].headerContent.text;

            return titles;
        }

        internal static bool[] GetVisibilities(
            MultiColumnHeaderState.Column[] columns)
        {
            bool[] visibilities = new bool[columns.Length];

            for (int i = 0; i < columns.Length; i++)
                visibilities[i] = columns[i].allowToggleVisibility;

            return visibilities;
        }

        internal static float[] GetWidths(
            MultiColumnHeaderState.Column[] columns)
        {
            float[] widths = new float[columns.Length];

            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].width;

            return widths;
        }
    }
}
