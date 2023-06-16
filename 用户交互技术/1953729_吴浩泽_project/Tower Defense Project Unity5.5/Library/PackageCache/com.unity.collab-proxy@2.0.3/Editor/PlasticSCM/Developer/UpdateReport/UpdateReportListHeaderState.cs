using System;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Developer.UpdateReport
{
    internal enum ErrorsListColumn
    {
        Path,
    }

    [Serializable]
    internal class UpdateReportListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static UpdateReportListHeaderState GetDefault()
        {
            return new UpdateReportListHeaderState(BuildColumns());
        }

        static string GetColumnName(ErrorsListColumn column)
        {
            switch (column)
            {
                case ErrorsListColumn.Path:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.PathColumn);
                default:
                    return null;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (mHeaderTitles != null)
                TreeHeaderColumns.SetTitles(columns, mHeaderTitles);
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
                        width = 605,
                        headerContent = new GUIContent(
                            GetColumnName(ErrorsListColumn.Path)),
                        minWidth = 200,
                        allowToggleVisibility = false,
                        canSort = false,
                        sortingArrowAlignment = TextAlignment.Right
                    }
                };
        }

        UpdateReportListHeaderState(Column[] columns)
            : base(columns)
        {
            if (mHeaderTitles == null)
                mHeaderTitles = TreeHeaderColumns.GetTitles(columns);
        }

        [SerializeField]
        string[] mHeaderTitles;
    }
}
