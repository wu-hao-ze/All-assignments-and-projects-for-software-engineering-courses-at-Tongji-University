using System;
using System.Collections.Generic;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace.Dialogs
{
    internal enum RepositoriesListColumn
    {
        Name,
        Server
    }

    [Serializable]
    internal class RepositoriesListHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static RepositoriesListHeaderState GetDefault()
        {
            return new RepositoriesListHeaderState(BuildColumns());
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.ServerColumn));
            return result;
        }

        static string GetColumnName(RepositoriesListColumn column)
        {
            switch (column)
            {
                case RepositoriesListColumn.Name:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.NameColumn);
                case RepositoriesListColumn.Server:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.ServerColumn);
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
                        width = 320,
                        headerContent = new GUIContent(
                            GetColumnName(RepositoriesListColumn.Name)),
                        minWidth = 200,
                        allowToggleVisibility = false,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 200,
                        headerContent = new GUIContent(
                            GetColumnName(RepositoriesListColumn.Server)),
                        minWidth = 200,
                        allowToggleVisibility = false,
                        sortingArrowAlignment = TextAlignment.Right
                    }
                };
        }

        RepositoriesListHeaderState(Column[] columns)
            : base(columns)
        {
            if (mHeaderTitles == null)
                mHeaderTitles = TreeHeaderColumns.GetTitles(columns);
        }

        [SerializeField]
        string[] mHeaderTitles;
    }
}
