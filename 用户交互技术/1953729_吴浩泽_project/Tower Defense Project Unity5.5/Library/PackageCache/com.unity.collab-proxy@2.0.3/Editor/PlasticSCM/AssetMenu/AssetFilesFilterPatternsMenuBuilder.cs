using Codice.Client.BaseCommands;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.AssetMenu
{
    internal interface IAssetFilesFilterPatternsMenuOperations
    {
        void AddFilesFilterPatterns(
            FilterTypes type, FilterActions action, FilterOperationType operation);
    }

    internal class AssetFilesFilterPatternsMenuBuilder
    {
        internal AssetFilesFilterPatternsMenuBuilder(
            int ignoredMenuItemsPriority,
            int hiddenChangesMenuItemsPriority)
        {
            mIgnoredMenuItemsPriority = ignoredMenuItemsPriority;
            mHiddenChangesMenuItemsPriority = hiddenChangesMenuItemsPriority;

            mIgnoredSubmenuItem = string.Format(
                "{0}/{1}",
                PlasticLocalization.GetString(PlasticLocalization.Name.PrefixUnityVersionControlMenu),
                PlasticLocalization.GetString(PlasticLocalization.Name.MenuAddToIgnoreList));

            mHiddenChangesSubmenuItem = string.Format(
                "{0}/{1}",
                PlasticLocalization.GetString(PlasticLocalization.Name.PrefixUnityVersionControlMenu),
                PlasticLocalization.GetString(PlasticLocalization.Name.MenuAddToHiddenChangesList));
        }

        internal void SetOperations(
            IAssetFilesFilterPatternsMenuOperations operations)
        {
            mOperations = operations;
        }

        internal void UpdateMenuItems(FilterMenuActions actions)
        {
            UpdateIgnoredMenuItems(actions);
            UpdateHiddenChangesMenuItems(actions);

            HandleMenuItem.UpdateAllMenus();
        }

        internal void RemoveMenuItems()
        {
            RemoveIgnoredMenuItems();
            RemoveHiddenChangesMenuItems();
        }

        void IgnoredByName_Click()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.AddFilesFilterPatterns(
                FilterTypes.Ignored, FilterActions.ByName,
                GetIgnoredFilterOperationType());
        }

        void IgnoredByExtension_Click()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.AddFilesFilterPatterns(
                FilterTypes.Ignored, FilterActions.ByExtension,
                GetIgnoredFilterOperationType());
        }

        void IgnoredByFullPath_Click()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.AddFilesFilterPatterns(
                FilterTypes.Ignored, FilterActions.ByFullPath,
                GetIgnoredFilterOperationType());
        }

        void HiddenChangesByName_Click()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.AddFilesFilterPatterns(
                FilterTypes.HiddenChanges, FilterActions.ByName,
                GetHiddenChangesFilterOperationType());
        }

        void HiddenChangesByExtension_Click()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.AddFilesFilterPatterns(
                FilterTypes.HiddenChanges, FilterActions.ByExtension,
                GetHiddenChangesFilterOperationType());
        }

        void HiddenChangesByFullPath_Click()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.AddFilesFilterPatterns(
                FilterTypes.HiddenChanges, FilterActions.ByFullPath,
                GetHiddenChangesFilterOperationType());
        }

        void UpdateIgnoredMenuItems(FilterMenuActions actions)
        {
            RemoveIgnoredMenuItems();

            if (!actions.Operations.HasFlag(FilterMenuOperations.Ignore))
            {
                HandleMenuItem.AddMenuItem(
                    mIgnoredSubmenuItem,
                    mIgnoredMenuItemsPriority,
                    DisabledMenuItem_Click, ValidateDisabledMenuItem);
                return;
            }

            mIgnoredByNameMenuItem = GetIgnoredMenuItemName(actions.FilterByName);
            mIgnoredByExtensionMenuItem = GetIgnoredMenuItemName(actions.FilterByExtension);
            mIgnoredByFullPathMenuItem = GetIgnoredMenuItemName(actions.FilterByFullPath);

            HandleMenuItem.AddMenuItem(
                mIgnoredByNameMenuItem,
                mIgnoredMenuItemsPriority,
                IgnoredByName_Click, ValidateEnabledMenuItem);

            if (!actions.Operations.HasFlag(FilterMenuOperations.IgnoreByExtension))
                HandleMenuItem.AddMenuItem(
                    mIgnoredByExtensionMenuItem,
                    mIgnoredMenuItemsPriority,
                    IgnoredByExtension_Click, ValidateEnabledMenuItem);

            HandleMenuItem.AddMenuItem(
                mIgnoredByFullPathMenuItem,
                mIgnoredMenuItemsPriority,
                IgnoredByFullPath_Click, ValidateEnabledMenuItem);
        }

        void UpdateHiddenChangesMenuItems(FilterMenuActions actions)
        {
            RemoveHiddenChangesMenuItems();

            if (!actions.Operations.HasFlag(FilterMenuOperations.HideChanged))
            {
                HandleMenuItem.AddMenuItem(
                    mHiddenChangesSubmenuItem,
                    mHiddenChangesMenuItemsPriority,
                    DisabledMenuItem_Click, ValidateDisabledMenuItem);
                return;
            }

            mHiddenChangesByNameMenuItem = GetHiddenChangesMenuItemName(actions.FilterByName);
            mHiddenChangesByExtensionMenuItem = GetHiddenChangesMenuItemName(actions.FilterByExtension);
            mHiddenChangesByFullPathMenuItem = GetHiddenChangesMenuItemName(actions.FilterByFullPath);

            HandleMenuItem.AddMenuItem(
                mHiddenChangesByNameMenuItem,
                mIgnoredMenuItemsPriority,
                HiddenChangesByName_Click, ValidateEnabledMenuItem);

            if (actions.Operations.HasFlag(FilterMenuOperations.HideChangedByExtension))
                HandleMenuItem.AddMenuItem(
                    mHiddenChangesByExtensionMenuItem,
                    mIgnoredMenuItemsPriority,
                    HiddenChangesByExtension_Click, ValidateEnabledMenuItem);

            HandleMenuItem.AddMenuItem(
                mHiddenChangesByFullPathMenuItem,
                mIgnoredMenuItemsPriority,
                HiddenChangesByFullPath_Click, ValidateEnabledMenuItem);
        }

        void RemoveIgnoredMenuItems()
        {
            HandleMenuItem.RemoveMenuItem(mIgnoredSubmenuItem);
            HandleMenuItem.RemoveMenuItem(mIgnoredByNameMenuItem);
            HandleMenuItem.RemoveMenuItem(mIgnoredByExtensionMenuItem);
            HandleMenuItem.RemoveMenuItem(mIgnoredByFullPathMenuItem);
        }

        void RemoveHiddenChangesMenuItems()
        {
            HandleMenuItem.RemoveMenuItem(mHiddenChangesSubmenuItem);
            HandleMenuItem.RemoveMenuItem(mHiddenChangesByNameMenuItem);
            HandleMenuItem.RemoveMenuItem(mHiddenChangesByExtensionMenuItem);
            HandleMenuItem.RemoveMenuItem(mHiddenChangesByFullPathMenuItem);
        }

        FilterOperationType GetIgnoredFilterOperationType()
        {
            if (mIgnoredByNameMenuItem.StartsWith(PlasticLocalization.GetString(
                    PlasticLocalization.Name.MenuAddToIgnoreList)))
            {
                return FilterOperationType.Add;
            }

            return FilterOperationType.Remove;
        }

        FilterOperationType GetHiddenChangesFilterOperationType()
        {
            if (mHiddenChangesByNameMenuItem.StartsWith(PlasticLocalization.GetString(
                    PlasticLocalization.Name.MenuAddToHiddenChangesList)))
            {
                return FilterOperationType.Add;
            }

            return FilterOperationType.Remove;
        }

        void DisabledMenuItem_Click() { }

        bool ValidateEnabledMenuItem() { return true; }

        bool ValidateDisabledMenuItem() { return false; }

        string GetIgnoredMenuItemName(string filterPattern)
        {
            return UnityMenuItem.GetText(
                mIgnoredSubmenuItem,
                UnityMenuItem.EscapedText(filterPattern));
        }

        string GetHiddenChangesMenuItemName(string filterPattern)
        {
            return UnityMenuItem.GetText(
                mHiddenChangesSubmenuItem,
                UnityMenuItem.EscapedText(filterPattern));
        }

        IAssetFilesFilterPatternsMenuOperations mOperations;

        string mIgnoredSubmenuItem;
        string mHiddenChangesSubmenuItem;

        string mIgnoredByNameMenuItem;
        string mHiddenChangesByNameMenuItem;

        string mIgnoredByExtensionMenuItem;
        string mHiddenChangesByExtensionMenuItem;

        string mIgnoredByFullPathMenuItem;
        string mHiddenChangesByFullPathMenuItem;

        readonly int mIgnoredMenuItemsPriority;
        readonly int mHiddenChangesMenuItemsPriority;
    }
}