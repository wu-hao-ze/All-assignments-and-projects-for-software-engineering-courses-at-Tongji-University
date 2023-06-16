using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.PendingChanges.Changelists;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Changelists
{
    internal class MoveToChangelistMenuBuilder
    {
        internal MoveToChangelistMenuBuilder(
            WorkspaceInfo wkInfo,
            IChangelistMenuOperations operations)
        {
            mWkInfo = wkInfo;
            mOperations = operations;
        }

        internal void BuildComponents()
        {
            mMoveToChangelistMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.Name.MoveToChangelist));
            mNewChangelistMenuItemContent = new GUIContent(GetSubMenuText(
                PlasticLocalization.GetString(PlasticLocalization.Name.New)));
        }

        internal void UpdateMenuItems(
            GenericMenu menu, 
            ChangelistMenuOperations operations,
            List<ChangeInfo> changes,
            List<ChangeListInfo> involvedChangelists)
        {
            if (!operations.HasFlag(ChangelistMenuOperations.MoveToChangelist))
            {
                menu.AddDisabledItem(mMoveToChangelistMenuItemContent);
                return;
            }

            menu.AddItem(
                mNewChangelistMenuItemContent,
                false, 
                () => NewChangelist_Click(changes));

            List<string> targetChangelists = GetTargetChangelists.
                ForInvolvedChangelists(mWkInfo, involvedChangelists);

            if (targetChangelists.Count == 0)
                return;

            menu.AddSeparator(GetSubMenuText(string.Empty));

            foreach (string changelist in targetChangelists)
            {
                menu.AddItem(
                    new GUIContent(GetSubMenuText(changelist)),
                    false,
                    () => MoveToChangelist_Click(changes, changelist));
            }
        }

        void NewChangelist_Click(List<ChangeInfo> changes)
        {
            mOperations.MoveToNewChangelist(changes);
        }

        void MoveToChangelist_Click(List<ChangeInfo> changes, string targetChangelist)
        {
            mOperations.MoveToChangelist(changes, targetChangelist);
        }

        static string GetSubMenuText(string subMenuName)
        {
            return UnityMenuItem.GetText(
                PlasticLocalization.GetString(PlasticLocalization.Name.MoveToChangelist),
                UnityMenuItem.EscapedText(subMenuName));
        }

        GUIContent mMoveToChangelistMenuItemContent;
        GUIContent mNewChangelistMenuItemContent;

        readonly WorkspaceInfo mWkInfo;
        readonly IChangelistMenuOperations mOperations;
    }
}
