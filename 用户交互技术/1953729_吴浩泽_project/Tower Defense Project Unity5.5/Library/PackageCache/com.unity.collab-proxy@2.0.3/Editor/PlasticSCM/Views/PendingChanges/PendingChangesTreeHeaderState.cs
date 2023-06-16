using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.IMGUI.Controls;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal enum PendingChangesTreeColumn
    {
        Item,
        Status,
        Size,
        Extension,
        Type,
        DateModififed,
        Repository
    }

    [Serializable]
    internal class PendingChangesTreeHeaderState : MultiColumnHeaderState, ISerializationCallbackReceiver
    {
        internal static PendingChangesTreeHeaderState GetDefault(bool isGluonMode)
        {
            PendingChangesTreeHeaderState headerState =
                new PendingChangesTreeHeaderState(BuildColumns());

            headerState.visibleColumns = GetDefaultVisibleColumns();

            SetMode(headerState, isGluonMode);

            return headerState;
        }

        internal static List<string> GetColumnNames()
        {
            List<string> result = new List<string>();
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.ItemColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.StatusColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.SizeColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.ExtensionColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.TypeColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.DateModifiedColumn));
            result.Add(PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryColumn));
            return result;
        }

        internal static string GetColumnName(PendingChangesTreeColumn column)
        {
            switch (column)
            {
                case PendingChangesTreeColumn.Item:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.ItemColumn);
                case PendingChangesTreeColumn.Status:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.StatusColumn);
                case PendingChangesTreeColumn.Size:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.SizeColumn);
                case PendingChangesTreeColumn.Extension:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.ExtensionColumn);
                case PendingChangesTreeColumn.Type:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.TypeColumn);
                case PendingChangesTreeColumn.DateModififed:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.DateModifiedColumn);
                case PendingChangesTreeColumn.Repository:
                    return PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryColumn);
                default:
                    return null;
            }
        }

        internal static void SetMode(MultiColumnHeaderState state, bool isGluonMode)
        {
            List<int> result = state.visibleColumns.ToList();

            if (!result.Contains((int)PendingChangesTreeColumn.Item))
                result.Add((int)PendingChangesTreeColumn.Item);

            if (isGluonMode)
                result.Remove((int)PendingChangesTreeColumn.Type);

            state.columns[(int)PendingChangesTreeColumn.Item].allowToggleVisibility = false;
            state.columns[(int)PendingChangesTreeColumn.Type].allowToggleVisibility = !isGluonMode;

            state.visibleColumns = result.ToArray();
        }

        internal void UpdateItemColumnHeader(PendingChangesTreeView treeView)
        {
            Column itemColumn = columns[(int)PendingChangesTreeColumn.Item];
            string columnName = GetColumnName(PendingChangesTreeColumn.Item);
            int totalItemCount = treeView.GetTotalItemCount();

            if (totalItemCount > 0)
            {
                string columnStatus = string.Format(
                    PlasticLocalization.GetString(PlasticLocalization.Name.ItemsSelected),
                    treeView.GetCheckedItemCount(),
                    totalItemCount);

                itemColumn.headerContent.text = string.Format("{0}  {1}", columnName, columnStatus);
            }
            else
            {
                itemColumn.headerContent.text = columnName;
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
            result.Add((int)PendingChangesTreeColumn.Item);
            result.Add((int)PendingChangesTreeColumn.Status);
            result.Add((int)PendingChangesTreeColumn.DateModififed);
            return result.ToArray();
        }

        static Column[] BuildColumns()
        {
            return new Column[]
                {
                    new Column()
                    {
                        width = 800,
                        headerContent = new GUIContent(
                            GetColumnName(PendingChangesTreeColumn.Item)),
                        minWidth = 200,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 200,
                        headerContent = new GUIContent(
                            GetColumnName(PendingChangesTreeColumn.Status)),
                        minWidth = 80,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 80,
                        headerContent = new GUIContent(
                            GetColumnName(PendingChangesTreeColumn.Size)),
                        minWidth = 45,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 70,
                        headerContent = new GUIContent(
                            GetColumnName(PendingChangesTreeColumn.Extension)),
                        minWidth = 50,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 60,
                        headerContent = new GUIContent(
                            GetColumnName(PendingChangesTreeColumn.Type)),
                        minWidth = 45,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 330,
                        headerContent = new GUIContent(
                            GetColumnName(PendingChangesTreeColumn.DateModififed)),
                        minWidth = 100,
                        sortingArrowAlignment = TextAlignment.Right
                    },
                    new Column()
                    {
                        width = 210,
                        headerContent = new GUIContent(
                            GetColumnName(PendingChangesTreeColumn.Repository)),
                        minWidth = 90,
                        sortingArrowAlignment = TextAlignment.Right
                    }
                };
        }

        PendingChangesTreeHeaderState(Column[] columns)
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
