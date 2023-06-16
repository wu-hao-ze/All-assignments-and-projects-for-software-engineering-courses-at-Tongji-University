using UnityEditor;
using UnityEngine;

using Codice.Client.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Merge;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges
{
    internal static class DrawIncomingChangesOverview
    {
        internal static void For(
            int directoryConflictCount,
            int fileConflictCount,
            MergeViewTexts.ChangesToApplySummary changesSummary)
        {
            DrawItem(
                Images.GetConflictedIcon(),
                PlasticLocalization.Name.DirectoryConflictsTitleSingular,
                PlasticLocalization.Name.DirectoryConflictsTitlePlural,
                directoryConflictCount,
                0,
                false);

            DrawItem(
                Images.GetConflictedIcon(),
                PlasticLocalization.Name.FileConflictsTitleSingular,
                PlasticLocalization.Name.FileConflictsTitlePlural,
                fileConflictCount,
                0,
                false);

            DrawItem(
                Images.GetOutOfSyncIcon(),
                PlasticLocalization.Name.MergeChangesMadeInSourceOfMergeOverviewSingular,
                PlasticLocalization.Name.MergeChangesMadeInSourceOfMergeOverviewPlural,
                changesSummary.FilesToModify,
                changesSummary.SizeToModify,
                true);

            DrawItem(
                Images.GetAddedLocalIcon(),
                PlasticLocalization.Name.MergeNewItemsToDownloadOverviewSingular,
                PlasticLocalization.Name.MergeNewItemsToDownloadOverviewPlural,
                changesSummary.FilesToAdd,
                changesSummary.SizeToAdd,
                true);

            DrawItem(
                Images.GetDeletedRemoteIcon(),
                PlasticLocalization.Name.MergeDeletesToApplyOverviewSingular,
                PlasticLocalization.Name.MergeDeletesToApplyOverviewPlural,
                changesSummary.FilesToDelete,
                changesSummary.SizeToDelete,
                true);
        }

        static void DrawItem(
            Texture2D icon,
            PlasticLocalization.Name singularLabel,
            PlasticLocalization.Name pluralLabel,
            int count,
            long size,
            bool showSize)
        {
            if (count == 0)
                return;

            EditorGUILayout.BeginHorizontal();

            GUIContent iconContent = new GUIContent(icon);
            GUILayout.Label(iconContent, GUILayout.Width(20f), GUILayout.Height(20f));

            string label = PlasticLocalization.GetString(count > 1 ? pluralLabel : singularLabel);
            if (showSize)
                label = string.Format(label, count, SizeConverter.ConvertToSizeString(size));
            else
                label = string.Format(label, count);

            GUIContent content = new GUIContent(label);
            GUILayout.Label(content, UnityStyles.IncomingChangesTab.ChangesToApplySummaryLabel);

            GUILayout.Space(5);

            EditorGUILayout.EndHorizontal();
        }
    }
}
