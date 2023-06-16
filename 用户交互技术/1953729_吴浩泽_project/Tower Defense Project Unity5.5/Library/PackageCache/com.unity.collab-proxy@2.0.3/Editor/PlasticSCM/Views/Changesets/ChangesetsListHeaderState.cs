using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.Changesets
{
    internal enum ChangesetsListColumn
    {
        Name,
        CreationDate,
        CreatedBy,
        Comment,
        Branch,
        Repository,
        Guid
    }

    [Serializable]
    internal class ChangesetsListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static ChangesetsListHeaderState GetDefault()
        {
            return new ChangesetsListHeaderState(BuildColumns());
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreationDateColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.BranchColumn));
            return result;
        }

        internal static string GetColumnName(ChangesetsListColumn column)
        {
            switch (column)
            {
                case ChangesetsListColumn.Name:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn);
                case ChangesetsListColumn.CreationDate:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CreationDateColumn);
                case ChangesetsListColumn.CreatedBy:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn);
                case ChangesetsListColumn.Comment:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn);
                case ChangesetsListColumn.Branch:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.BranchColumn);
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

        static Column[] BuildColumns()
        {
            return new Column[]
            {
                new Column()
                {
                    width = UnityConstants.ChangesetsColumns.CHANGESET_NUMBER_WIDTH,
                    minWidth = UnityConstants.ChangesetsColumns.CHANGESET_NUMBER_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(ChangesetsListColumn.Name)),
                    allowToggleVisibility = false,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = UnityConstants.ChangesetsColumns.CREATION_DATE_WIDTH,
                    minWidth = UnityConstants.ChangesetsColumns.CREATION_DATE_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(ChangesetsListColumn.CreationDate)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = UnityConstants.ChangesetsColumns.CREATED_BY_WIDTH,
                    minWidth = UnityConstants.ChangesetsColumns.CREATED_BY_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(ChangesetsListColumn.CreatedBy)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = UnityConstants.ChangesetsColumns.COMMENT_WIDTH,
                    minWidth = UnityConstants.ChangesetsColumns.COMMENT_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(ChangesetsListColumn.Comment)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = UnityConstants.ChangesetsColumns.BRANCH_WIDTH,
                    minWidth = UnityConstants.ChangesetsColumns.BRANCH_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(ChangesetsListColumn.Branch)),
                    sortingArrowAlignment = TextAlignment.Right
                }
            };
        }

        ChangesetsListHeaderState(Column[] columns)
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
