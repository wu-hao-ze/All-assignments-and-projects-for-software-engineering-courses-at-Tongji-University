using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges
{
    internal interface IFilesFilterPatternsMenuOperations
    {
        void AddFilesFilterPatterns(
            FilterTypes type, FilterActions action, FilterOperationType operation);
    }

    internal class FilesFilterPatternsMenuBuilder
    {
        internal FilesFilterPatternsMenuBuilder(IFilesFilterPatternsMenuOperations operations)
        {
            mOperations = operations;
        }

        internal void BuildIgnoredSubmenuItem()
        {
            mIgnoredSubmenuItem = new GUIContent(string.Empty);
            mIgnoredByNameMenuItemContent = new GUIContent(string.Empty);
            mIgnoredByExtensionMenuItemContent = new GUIContent(string.Empty);
            mIgnoredByFullPathMenuItemContent = new GUIContent(string.Empty);
        }

        internal void BuildHiddenChangesSubmenuItem()
        {
            mHiddenChangesSubmenuItem = new GUIContent(string.Empty);
            mHiddenChangesByNameMenuItemContent = new GUIContent(string.Empty);
            mHiddenChangesByExtensionMenuItemContent = new GUIContent(string.Empty);
            mHiddenChangesByFullPathMenuItemContent = new GUIContent(string.Empty);
        }

        internal void UpdateMenuItems(GenericMenu menu, FilterMenuActions actions)
        {
            if (mIgnoredSubmenuItem != null)
                UpdateIgnoredMenuItems(menu, actions.Operations);

            if (mHiddenChangesSubmenuItem != null)
                UpdateHiddenChangesMenuItems(menu, actions.Operations);

            SetFilterMenuItemsLabels(actions);
        }

        void UpdateIgnoredMenuItems(GenericMenu menu, FilterMenuOperations operations)
        {
            if (!operations.HasFlag(FilterMenuOperations.Ignore))
            {
                menu.AddDisabledItem(mIgnoredSubmenuItem);
                return;
            }

            menu.AddItem(mIgnoredByNameMenuItemContent, false, IgnoredByName_Click);
            menu.AddItem(mIgnoredByExtensionMenuItemContent, false, IgnoredByExtension_Click);

            if (!operations.HasFlag(FilterMenuOperations.IgnoreByExtension))
                return;

            menu.AddItem(mIgnoredByFullPathMenuItemContent, false, IgnoredByFullPath_Click);
        }

        void UpdateHiddenChangesMenuItems(GenericMenu menu, FilterMenuOperations operations)
        {
            if (!operations.HasFlag(FilterMenuOperations.HideChanged))
            {
                menu.AddDisabledItem(mHiddenChangesSubmenuItem);
                return;
            }

            menu.AddItem(mHiddenChangesByNameMenuItemContent, false, HiddenChangesByName_Click);
            menu.AddItem(mHiddenChangesByExtensionMenuItemContent, false, HiddenChangesByExtension_Click);

            if (!operations.HasFlag(FilterMenuOperations.HideChangedByExtension))
                return;

            menu.AddItem(mHiddenChangesByFullPathMenuItemContent, false, HiddenChangesByFullPath_Click);
        }

        void IgnoredByName_Click()
        {
            mOperations.AddFilesFilterPatterns(
                FilterTypes.Ignored, FilterActions.ByName,
                GetIgnoredFilterOperationType());
        }

        void IgnoredByExtension_Click()
        {
            mOperations.AddFilesFilterPatterns(
                FilterTypes.Ignored, FilterActions.ByExtension,
                GetIgnoredFilterOperationType());
        }

        void IgnoredByFullPath_Click()
        {
            mOperations.AddFilesFilterPatterns(
                FilterTypes.Ignored, FilterActions.ByFullPath,
                GetIgnoredFilterOperationType());
        }

        void HiddenChangesByName_Click()
        {
            mOperations.AddFilesFilterPatterns(
                FilterTypes.HiddenChanges, FilterActions.ByName,
                GetHiddenChangesFilterOperationType());
        }

        void HiddenChangesByExtension_Click()
        {
            mOperations.AddFilesFilterPatterns(
                FilterTypes.HiddenChanges, FilterActions.ByExtension,
                GetHiddenChangesFilterOperationType());
        }

        void HiddenChangesByFullPath_Click()
        {
            mOperations.AddFilesFilterPatterns(
                FilterTypes.HiddenChanges, FilterActions.ByFullPath,
                GetHiddenChangesFilterOperationType());
        }

        FilterOperationType GetIgnoredFilterOperationType()
        {
            if (mIgnoredByNameMenuItemContent.text.StartsWith(PlasticLocalization.GetString(
                    PlasticLocalization.Name.MenuAddToIgnoreList)))
            {
                return FilterOperationType.Add;
            }

            return FilterOperationType.Remove;
        }

        FilterOperationType GetHiddenChangesFilterOperationType()
        {
            if (mHiddenChangesByNameMenuItemContent.text.StartsWith(PlasticLocalization.GetString(
                    PlasticLocalization.Name.MenuAddToHiddenChangesList)))
            {
                return FilterOperationType.Add;
            }

            return FilterOperationType.Remove;
        }

        void SetFilterMenuItemsLabels(FilterMenuActions actions)
        {
            if (mIgnoredSubmenuItem != null)
            {
                mIgnoredSubmenuItem.text = actions.IgnoredTitle;
                mIgnoredByNameMenuItemContent.text = GetSubMenuText(
                    actions.IgnoredTitle, actions.FilterByName);
                mIgnoredByExtensionMenuItemContent.text = GetSubMenuText(
                    actions.IgnoredTitle, actions.FilterByExtension);
                mIgnoredByFullPathMenuItemContent.text = GetSubMenuText(
                    actions.IgnoredTitle, actions.FilterByFullPath);
            }

            if (mHiddenChangesSubmenuItem != null)
            {
                mHiddenChangesSubmenuItem.text = actions.HiddenChangesTitle;
                mHiddenChangesByNameMenuItemContent.text = GetSubMenuText(
                    actions.HiddenChangesTitle, actions.FilterByName);
                mHiddenChangesByExtensionMenuItemContent.text = GetSubMenuText(
                    actions.HiddenChangesTitle, actions.FilterByExtension);
                mHiddenChangesByFullPathMenuItemContent.text = GetSubMenuText(
                    actions.HiddenChangesTitle, actions.FilterByFullPath);
            }
        }

        static string GetSubMenuText(string menuName, string subMenuName)
        {
            return UnityMenuItem.GetText(
                menuName,
                UnityMenuItem.EscapedText(subMenuName));
        }

        GUIContent mIgnoredSubmenuItem;
        GUIContent mHiddenChangesSubmenuItem;

        GUIContent mIgnoredByNameMenuItemContent;
        GUIContent mHiddenChangesByNameMenuItemContent;

        GUIContent mIgnoredByExtensionMenuItemContent;
        GUIContent mHiddenChangesByExtensionMenuItemContent;

        GUIContent mIgnoredByFullPathMenuItemContent;        
        GUIContent mHiddenChangesByFullPathMenuItemContent;

        IFilesFilterPatternsMenuOperations mOperations;
    }
}
