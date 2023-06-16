using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.Branches
{
    internal enum BranchesListColumn
    {
        Name,
        Repository,
        CreatedBy,
        CreationDate,
        Comment,
        Branch,
        Guid
    }

    [Serializable]
    internal class BranchesListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static BranchesListHeaderState GetDefault()
        {
            return new BranchesListHeaderState(BuildColumns());
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CreationDateColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.CommentColumn));
            return result;
        }

        internal static string GetColumnName(BranchesListColumn column)
        {
            switch (column)
            {
                case BranchesListColumn.Name:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn);
                case BranchesListColumn.Repository:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryColumn);
                case BranchesListColumn.CreatedBy:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CreatedByColumn);
                case BranchesListColumn.CreationDate:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.CreationDateColumn);
                case BranchesListColumn.Comment:
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

        static Column[] BuildColumns()
        {
            return new Column[]
            {
                new Column()
                {
                    width = UnityConstants.BranchesColumns.BRANCHES_NAME_WIDTH,
                    minWidth = UnityConstants.BranchesColumns.BRANCHES_NAME_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(BranchesListColumn.Name)),
                    allowToggleVisibility = false,
                    sortingArrowAlignment = TextAlignment.Right
                },
                 new Column()
                {
                    width = UnityConstants.BranchesColumns.REPOSITORY_WIDTH,
                    minWidth = UnityConstants.BranchesColumns.REPOSITORY_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(BranchesListColumn.Repository)),
                    allowToggleVisibility = true,
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = UnityConstants.BranchesColumns.CREATEDBY_WIDTH,
                    minWidth = UnityConstants.BranchesColumns.CREATEDBY_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(BranchesListColumn.CreatedBy)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = UnityConstants.BranchesColumns.CREATION_DATE_WIDTH,
                    minWidth = UnityConstants.BranchesColumns.CREATION_DATE_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(BranchesListColumn.CreationDate)),
                    sortingArrowAlignment = TextAlignment.Right
                },
                new Column()
                {
                    width = UnityConstants.BranchesColumns.COMMENT_WIDTH,
                    minWidth = UnityConstants.BranchesColumns.COMMENT_MIN_WIDTH,
                    headerContent = new GUIContent(
                        GetColumnName(BranchesListColumn.Comment)),
                    sortingArrowAlignment = TextAlignment.Right
                }
            };
        }

        BranchesListHeaderState(Column[] columns)
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
