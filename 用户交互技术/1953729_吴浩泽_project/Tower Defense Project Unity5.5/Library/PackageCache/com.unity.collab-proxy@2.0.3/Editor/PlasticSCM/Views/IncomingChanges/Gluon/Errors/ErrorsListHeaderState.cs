using System;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Gluon.Errors
{
    internal enum ErrorsListColumn
    {
        Path,
        Reason
    }

    [Serializable]
    internal class ErrorsListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static ErrorsListHeaderState GetDefault()
        {
            return new ErrorsListHeaderState(BuildColumns());
        }

        static string GetColumnName(ErrorsListColumn column)
        {
            switch (column)
            {
                case ErrorsListColumn.Path:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.PathColumn);
                case ErrorsListColumn.Reason:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.Reason);
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
                        width = 300,
                        headerContent = new GUIContent(
                            GetColumnName(ErrorsListColumn.Path)),
                        minWidth = 200,
                        allowToggleVisibility = false,
                        canSort = false,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 600,
                        headerContent = new GUIContent(
                            GetColumnName(ErrorsListColumn.Reason)),
                        minWidth = 200,
                        canSort = false,
                        sortingArrowAlignment = TextAlignment.Right
                    }
                };
        }

        ErrorsListHeaderState(Column[] columns)
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
