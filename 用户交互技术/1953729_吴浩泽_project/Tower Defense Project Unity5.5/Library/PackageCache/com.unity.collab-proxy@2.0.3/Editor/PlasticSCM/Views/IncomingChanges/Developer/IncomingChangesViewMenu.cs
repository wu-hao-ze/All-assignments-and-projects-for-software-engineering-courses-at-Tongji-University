using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands.EventTracking;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.IncomingChanges;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.IncomingChanges.Developer
{
    internal class IncomingChangesViewMenu
    {
        internal interface IMetaMenuOperations
        {
            void DiffIncomingChanges();
            void DiffYoursWithIncoming();
            bool SelectionHasMeta();
        }

        internal IncomingChangesViewMenu(
            IIncomingChangesViewMenuOperations incomingChangesViewMenuOperations,
            IMetaMenuOperations incomingChangesMetaMenuOperations)
        {
            mIncomingChangesViewMenuOperations = incomingChangesViewMenuOperations;
            mIncomingChangesMetaMenuOperations = incomingChangesMetaMenuOperations;
            
            BuildComponents();
        }

        internal void Popup()
        {
            GenericMenu menu = new GenericMenu();

            UpdateMenuItems(menu);

            menu.ShowAsContext();
        }

        void MergeSelectedFilesMenuItem_Click()
        {
            mIncomingChangesViewMenuOperations.MergeContributors();
        }

        void MergeKeepingSourceChangesMenuItem_Click()
        {
            mIncomingChangesViewMenuOperations.MergeKeepingSourceChanges();
        }

        void MergeKeepingWorkspaceChangesMenuItem_Click()
        {
            mIncomingChangesViewMenuOperations.MergeKeepingWorkspaceChanges();
        }

        void DiffYoursWithIncomingMenuItem_Click()
        {
            mIncomingChangesViewMenuOperations.DiffYoursWithIncoming();
        }

        void DiffIncomingChangesMenuItem_Click()
        {
            mIncomingChangesViewMenuOperations.DiffIncomingChanges();
        }

        void DiffMetaYoursWithIncomingMenuItem_Click()
        {
            mIncomingChangesMetaMenuOperations.DiffYoursWithIncoming();
        }

        void DiffMetaIncomingChangesMenuItem_Click()
        {
            mIncomingChangesMetaMenuOperations.DiffIncomingChanges();
        }

        void UpdateMenuItems(GenericMenu menu)
        {
            SelectedIncomingChangesGroupInfo info =
                mIncomingChangesViewMenuOperations.GetSelectedIncomingChangesGroupInfo();
            IncomingChangesMenuOperations operations =
                UpdateIncomingChangesMenu.GetAvailableMenuOperations(info);

            if (operations == IncomingChangesMenuOperations.None)
            {
                menu.AddDisabledItem(GetNoActionMenuItemContent());
                return;
            }

            AddMergeActions(menu, operations);

            menu.AddSeparator(string.Empty);

            AddDiffActions(menu, operations);

            if (!mIncomingChangesMetaMenuOperations.SelectionHasMeta())
                return;

            menu.AddSeparator(string.Empty);

            AddMetaActions(menu, operations);
        }

        void AddMergeActions(
            GenericMenu menu,
            IncomingChangesMenuOperations operations)
        {
            if (operations.HasFlag(IncomingChangesMenuOperations.MergeContributors))
                menu.AddItem(mMergeSelectedFilesMenuItemContent, false,
                    MergeSelectedFilesMenuItem_Click);
            else
                menu.AddDisabledItem(mMergeSelectedFilesMenuItemContent);

            if (operations.HasFlag(IncomingChangesMenuOperations.MergeKeepingSourceChanges))
                menu.AddItem(mMergeKeepingSourceChangesMenuItemContent, false,
                    MergeKeepingSourceChangesMenuItem_Click);
            else
                menu.AddDisabledItem(mMergeKeepingSourceChangesMenuItemContent);

            if (operations.HasFlag(IncomingChangesMenuOperations.MergeKeepingWorkspaceChanges))
                menu.AddItem(mMergeKeepingWorkspaceChangesMenuItemContent, false,
                    MergeKeepingWorkspaceChangesMenuItem_Click);
            else
                menu.AddDisabledItem(mMergeKeepingWorkspaceChangesMenuItemContent);
        }

        void AddDiffActions(GenericMenu menu, IncomingChangesMenuOperations operations)
        {
            if (operations.HasFlag(IncomingChangesMenuOperations.DiffYoursWithIncoming))
                menu.AddItem(mDiffYoursWithIncomingMenuItemContent, false,
                    DiffYoursWithIncomingMenuItem_Click);
            else
                menu.AddDisabledItem(mDiffYoursWithIncomingMenuItemContent);

            if (operations.HasFlag(IncomingChangesMenuOperations.DiffIncomingChanges))
                menu.AddItem(mDiffIncomingChangesMenuItemContent, false,
                    DiffIncomingChangesMenuItem_Click);
            else
                menu.AddDisabledItem(mDiffIncomingChangesMenuItemContent);
        }

        void AddMetaActions(GenericMenu menu, IncomingChangesMenuOperations operations)
        {
            if (operations.HasFlag(IncomingChangesMenuOperations.DiffYoursWithIncoming))
                menu.AddItem(mDiffMetaYoursWithIncomingMenuItemContent, false,
                    DiffMetaYoursWithIncomingMenuItem_Click);
            else
                menu.AddDisabledItem(mDiffMetaYoursWithIncomingMenuItemContent);

            if (operations.HasFlag(IncomingChangesMenuOperations.DiffIncomingChanges))
                menu.AddItem(mDiffMetaIncomingChangesMenuItemContent, false,
                    DiffMetaIncomingChangesMenuItem_Click);
            else
                menu.AddDisabledItem(mDiffMetaIncomingChangesMenuItemContent);
        }

        GUIContent GetNoActionMenuItemContent()
        {
            if (mNoActionMenuItemContent == null)
            {
                mNoActionMenuItemContent = new GUIContent(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.NoActionMenuItem));
            }

            return mNoActionMenuItemContent;
        }

        void BuildComponents()
        {
            mMergeSelectedFilesMenuItemContent = new GUIContent(
                PlasticLocalization.GetString(PlasticLocalization.
                    Name.MergeSelectedFiles));

            mMergeKeepingSourceChangesMenuItemContent = new GUIContent(
                UnityMenuItem.EscapedText(
                    PlasticLocalization.GetString(PlasticLocalization.
                        Name.IncomingChangesMenuItemMergeKeepingSourceChanges)));

            mMergeKeepingWorkspaceChangesMenuItemContent = new GUIContent(
                UnityMenuItem.EscapedText(
                    PlasticLocalization.GetString(PlasticLocalization.
                        Name.IncomingChangesMenuItemMergeKeepingWorkspaceChanges)));

            string diffYoursWithIncomingText = UnityMenuItem.EscapedText(
                    PlasticLocalization.GetString(PlasticLocalization.
                        Name.IncomingChangesMenuItemDiffYoursWithIncoming));

            string diffIncomingChangesText = UnityMenuItem.EscapedText(
                    PlasticLocalization.GetString(PlasticLocalization.
                        Name.IncomingChangesMenuItemDiffIncomingChanges));

            mDiffYoursWithIncomingMenuItemContent = new GUIContent(
                diffYoursWithIncomingText);

            mDiffIncomingChangesMenuItemContent = new GUIContent(
                diffIncomingChangesText);

            mDiffMetaYoursWithIncomingMenuItemContent = new GUIContent(
                string.Format(
                    "{0}/{1}",
                    MetaPath.META_EXTENSION,
                    diffYoursWithIncomingText));
            mDiffMetaIncomingChangesMenuItemContent = new GUIContent(
                string.Format(
                    "{0}/{1}",
                    MetaPath.META_EXTENSION,
                    diffIncomingChangesText));
        }

        GUIContent mNoActionMenuItemContent;

        GUIContent mMergeSelectedFilesMenuItemContent;
        GUIContent mMergeKeepingSourceChangesMenuItemContent;
        GUIContent mMergeKeepingWorkspaceChangesMenuItemContent;
        GUIContent mDiffYoursWithIncomingMenuItemContent;
        GUIContent mDiffIncomingChangesMenuItemContent;
        GUIContent mDiffMetaYoursWithIncomingMenuItemContent;
        GUIContent mDiffMetaIncomingChangesMenuItemContent;

        readonly IIncomingChangesViewMenuOperations mIncomingChangesViewMenuOperations;
        readonly IMetaMenuOperations mIncomingChangesMetaMenuOperations;
    }
}