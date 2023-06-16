using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Gluon
{
    internal enum IncomingChangesTreeColumn
    {
        Path,
        LastEditedBy,
        Size,
        DateModififed,
        Comment
    }

    [Serializable]
    internal class IncomingChangesTreeHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static IncomingChangesTreeHeaderState GetDefault()
        {
            return new IncomingChangesTreeHeaderState(BuildColumns());
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.PathColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.LastEditedByColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.SizeColumn));
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
                case IncomingChangesTreeColumn.LastEditedBy:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.LastEditedByColumn);
                case IncomingChangesTreeColumn.Size:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.SizeColumn);
                case IncomingChangesTreeColumn.DateModififed:
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

        static Column[] BuildColumns()
        {
            return new Column[]
                {
                    new Column()
                    {
                        width = 440,
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
                            GetColumnName(IncomingChangesTreeColumn.LastEditedBy)),
                        minWidth = 80,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 80,
                        headerContent = new GUIContent(
                            GetColumnName(IncomingChangesTreeColumn.Size)),
                        minWidth = 45,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 260,
                        headerContent = new GUIContent(
                            GetColumnName(IncomingChangesTreeColumn.DateModififed)),
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
