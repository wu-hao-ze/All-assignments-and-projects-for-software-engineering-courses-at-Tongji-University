using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer
{
    internal enum IncomingChangesTreeColumn
    {
        Path,
        Size,
        Author,
        Details,
        Resolution,
        DateModified,
        Comment
    }

    [Serializable]
    internal class IncomingChangesTreeHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static IncomingChangesTreeHeaderState GetDefault()
        {
            IncomingChangesTreeHeaderState headerState =
                new IncomingChangesTreeHeaderState(BuildColumns());

            headerState.visibleColumns = GetDefaultVisibleColumns();

            return headerState;
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.PathColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.SizeColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.DetailsColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.ResolutionMethodColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.DateModifiedColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn));
            return result;
        }

        internal static string GetColumnName(IncomingChangesTreeColumn column)
        {
            switch (column)
            {
                case IncomingChangesTreeColumn.Path:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.PathColumn);
                case IncomingChangesTreeColumn.Size:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.SizeColumn);
                case IncomingChangesTreeColumn.Author:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.AuthorColumn);
                case IncomingChangesTreeColumn.Details:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.DetailsColumn);
                case IncomingChangesTreeColumn.Resolution:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.ResolutionMethodColumn);
                case IncomingChangesTreeColumn.DateModified:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.DateModifiedColumn);
                case IncomingChangesTreeColumn.Comment:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn);
                default:
                    return null;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (mHeaderTitles != null)
                TreeHeaderColumns.SetTitles(columns, mHeaderTitles);

            if (mColumsAllowedToggleVisibility != null)
                TreeHeaderColumns.SetVisibilities(columns, mColumsAllowedToggleVisibility);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        static int[] GetDefaultVisibleColumns()
        {
            List<int> result = new List<int>();
            result.Add((int)IncomingChangesTreeColumn.Path);
            result.Add((int)IncomingChangesTreeColumn.Size);
            result.Add((int)IncomingChangesTreeColumn.Author);
            result.Add((int)IncomingChangesTreeColumn.DateModified);
            result.Add((int)IncomingChangesTreeColumn.Comment);
            return result.ToArray();
        }

        static Column[] BuildColumns()
        {
            return new Column[]
            {
                new Column()
                {
                    width = 450,
                    headerContent = new GUIContent(
                        GetColumnName(IncomingChangesTreeColumn.Path)),
                    minWidth = 200,
                    allowToggleVisibility = false,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = 150,
                    headerContent = new GUIContent(
                        GetColumnName(IncomingChangesTreeColumn.Size)),
                    minWidth = 45,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = 150,
                    headerContent = new GUIContent(
                        GetColumnName(IncomingChangesTreeColumn.Author)),
                    minWidth = 80,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = 200,
                    headerContent = new GUIContent(
                        GetColumnName(IncomingChangesTreeColumn.Details)),
                    minWidth = 100,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = 250,
                    headerContent = new GUIContent(
                        GetColumnName(IncomingChangesTreeColumn.Resolution)),
                    minWidth = 120,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = 330,
                    headerContent = new GUIContent(
                        GetColumnName(IncomingChangesTreeColumn.DateModified)),
                    minWidth = 100,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = 400,
                    headerContent = new GUIContent(
                        GetColumnName(IncomingChangesTreeColumn.Comment)),
                    minWidth = 100,
                    sortingArrowAlignment = TextAlignment.Right
                }
            };
        }

        IncomingChangesTreeHeaderState(Column[] columns)
            : base(columns)
        {
            if (mHeaderTitles == null)
                mHeaderTitles = TreeHeaderColumns.GetTitles(columns);

            if (mColumsAllowedToggleVisibility == null)
                mColumsAllowedToggleVisibility = TreeHeaderColumns.GetVisibilities(columns);
        }

        [SerializeField]
        string[] mHeaderTitles;

        [SerializeField]
        bool[] mColumsAllowedToggleVisibility;
    }
}
