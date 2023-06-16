using UnityEditor;
using UnityEditor.VersionControl;

using Codice.CM.Common;
using Codice.Client.BaseCommands.EventTracking;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Tool;

namespace Unity.PlasticSCM.Editor.AssetMenu
{
    internal class AssetMenuItems
    {
        internal static void Enable(
            WorkspaceInfo wkInfo,
            IAssetStatusCache assetStatusCache)
        {
            if (mIsEnabled)
                return;

            mWkInfo = wkInfo;
            mAssetStatusCache = assetStatusCache;

            mIsEnabled = true;

            mAssetSelection = new ProjectViewAssetSelection(UpdateFilterMenuItems);

            mFilterMenuBuilder = new AssetFilesFilterPatternsMenuBuilder(
                IGNORE_MENU_ITEMS_PRIORITY,
                HIDDEN_MENU_ITEMS_PRIORITY);

            AddMenuItems();
        }

        internal static void Disable()
        {
            mIsEnabled = false;

            RemoveMenuItems();

            if (mAssetSelection != null)
                mAssetSelection.Dispose();

            mWkInfo = null;
            mAssetStatusCache = null;
            mAssetSelection = null;
            mFilterMenuBuilder = null;
            mOperations = null;
        }

        internal static void BuildOperations(
            WorkspaceInfo wkInfo,
            WorkspaceWindow workspaceWindow,
            IViewSwitcher viewSwitcher,
            IHistoryViewLauncher historyViewLauncher,
            GluonGui.ViewHost viewHost,
            PlasticGui.WorkspaceWindow.NewIncomingChangesUpdater incomingChangesUpdater,
            IAssetStatusCache assetStatusCache,
            IMergeViewLauncher mergeViewLauncher,
            PlasticGui.Gluon.IGluonViewSwitcher gluonViewSwitcher,
            LaunchTool.IShowDownloadPlasticExeWindow showDownloadPlasticExeWindow,
            EditorWindow parentWindow,
            bool isGluonMode)
        {
            if (!mIsEnabled)
                Enable(wkInfo, assetStatusCache);

            AssetOperations assetOperations = new AssetOperations(
                wkInfo,
                workspaceWindow,
                viewSwitcher,
                historyViewLauncher,
                viewHost,
                incomingChangesUpdater,
                mAssetStatusCache,
                mergeViewLauncher,
                gluonViewSwitcher,
                parentWindow,
                mAssetSelection,
                showDownloadPlasticExeWindow,
                isGluonMode);

            mOperations = assetOperations;
            mFilterMenuBuilder.SetOperations(assetOperations);
        }

        static void RemoveMenuItems()
        {
            mFilterMenuBuilder.RemoveMenuItems();

            HandleMenuItem.RemoveMenuItem(
                PlasticLocalization.GetString(PlasticLocalization.Name.PrefixUnityVersionControlMenu));

            HandleMenuItem.UpdateAllMenus();
        }

        static void UpdateFilterMenuItems()
        {
            AssetList assetList = ((AssetOperations.IAssetSelection)
                mAssetSelection).GetSelectedAssets();

            SelectedPathsGroupInfo info = AssetsSelection.GetSelectedPathsGroupInfo(
                mWkInfo.ClientPath, assetList, mAssetStatusCache);

            FilterMenuActions actions =
                assetList.Count != info.SelectedCount ?
                new FilterMenuActions() :
                FilterMenuUpdater.GetMenuActions(info);

            mFilterMenuBuilder.UpdateMenuItems(actions);
        }

        static void AddMenuItems()
        {
            // TODO: Try removing this
            // Somehow first item always disappears. So this is a filler item
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.PendingChangesPlasticMenu),
                PENDING_CHANGES_MENU_ITEM_PRIORITY,
                PendingChanges, ValidatePendingChanges);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.PendingChangesPlasticMenu),
                PENDING_CHANGES_MENU_ITEM_PRIORITY,
                PendingChanges, ValidatePendingChanges);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.AddPlasticMenu),
                ADD_MENU_ITEM_PRIORITY,
                Add, ValidateAdd);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.CheckoutPlasticMenu),
                CHECKOUT_MENU_ITEM_PRIORITY,
                Checkout, ValidateCheckout);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.CheckinPlasticMenu),
                CHECKIN_MENU_ITEM_PRIORITY,
                Checkin, ValidateCheckin);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.UndoPlasticMenu),
                UNDO_MENU_ITEM_PRIORITY,
                Undo, ValidateUndo);

            UpdateFilterMenuItems();

            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.DiffPlasticMenu),
                GetPlasticShortcut.ForAssetDiff(),
                DIFF_MENU_ITEM_PRIORITY,
                Diff, ValidateDiff);
            HandleMenuItem.AddMenuItem(
                GetPlasticMenuItemName(PlasticLocalization.Name.HistoryPlasticMenu),
                GetPlasticShortcut.ForHistory(),
                HISTORY_MENU_ITEM_PRIORITY,
                History, ValidateHistory);

            HandleMenuItem.UpdateAllMenus();
        }

        static void PendingChanges()
        {
            ShowWindow.Plastic();

            mOperations.ShowPendingChanges();
        }

        static bool ValidatePendingChanges()
        {
            return true;
        }

        static void Add()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.Add();
        }

        static bool ValidateAdd()
        {
            return ShouldMenuItemBeEnabled(
                mWkInfo.ClientPath, mAssetSelection, mAssetStatusCache,
                AssetMenuOperations.Add);
        }

        static void Checkout()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.Checkout();
        }

        static bool ValidateCheckout()
        {
            return ShouldMenuItemBeEnabled(
                mWkInfo.ClientPath, mAssetSelection, mAssetStatusCache,
                AssetMenuOperations.Checkout);
        }

        static void Checkin()
        {
            TrackFeatureUseEvent.For(
                PlasticGui.Plastic.API.GetRepositorySpec(mWkInfo),
                TrackFeatureUseEvent.Features.ContextMenuCheckinOption);

            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.Checkin();
        }

        static bool ValidateCheckin()
        {
            return ShouldMenuItemBeEnabled(
                mWkInfo.ClientPath, mAssetSelection, mAssetStatusCache,
                AssetMenuOperations.Checkin);
        }

        static void Undo()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.Undo();
        }

        static bool ValidateUndo()
        {
            return ShouldMenuItemBeEnabled(
                mWkInfo.ClientPath, mAssetSelection, mAssetStatusCache,
                AssetMenuOperations.Undo);
        }

        static void Diff()
        {
            if (mOperations == null)
                ShowWindow.Plastic();

            mOperations.ShowDiff();
        }

        static bool ValidateDiff()
        {
            return ShouldMenuItemBeEnabled(
                mWkInfo.ClientPath, mAssetSelection, mAssetStatusCache,
                AssetMenuOperations.Diff);
        }

        static void History()
        {
            ShowWindow.Plastic();

            mOperations.ShowHistory();
        }

        static bool ValidateHistory()
        {
            return ShouldMenuItemBeEnabled(
                mWkInfo.ClientPath, mAssetSelection, mAssetStatusCache,
                AssetMenuOperations.History);
        }

        static bool ShouldMenuItemBeEnabled(
            string wkPath,
            AssetOperations.IAssetSelection assetSelection,
            IAssetStatusCache statusCache,
            AssetMenuOperations operation)
        {
            AssetList assetList = assetSelection.GetSelectedAssets();

            if (assetList.Count == 0)
                return false;

            SelectedAssetGroupInfo selectedGroupInfo = SelectedAssetGroupInfo.
                BuildFromAssetList(wkPath, assetList, statusCache);

            if (assetList.Count != selectedGroupInfo.SelectedCount)
                return false;

            AssetMenuOperations operations = AssetMenuUpdater.
                GetAvailableMenuOperations(selectedGroupInfo);

            return operations.HasFlag(operation);
        }

        static string GetPlasticMenuItemName(PlasticLocalization.Name name)
        {
            return string.Format("{0}/{1}",
                PlasticLocalization.GetString(PlasticLocalization.Name.PrefixUnityVersionControlMenu),
                PlasticLocalization.GetString(name));
        }

        static IAssetMenuOperations mOperations;

        static ProjectViewAssetSelection mAssetSelection;
        static AssetFilesFilterPatternsMenuBuilder mFilterMenuBuilder;

        static bool mIsEnabled;
        static IAssetStatusCache mAssetStatusCache;
        static WorkspaceInfo mWkInfo;

        const int BASE_MENU_ITEM_PRIORITY = 19; // Puts Plastic SCM right below Create menu

        // incrementing the "order" param by 11 causes the menu system to add a separator
        const int PENDING_CHANGES_MENU_ITEM_PRIORITY = BASE_MENU_ITEM_PRIORITY;
        const int ADD_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 11;
        const int CHECKOUT_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 12;
        const int CHECKIN_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 13;
        const int UNDO_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 14;
        const int IGNORE_MENU_ITEMS_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 25;
        const int HIDDEN_MENU_ITEMS_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 26;
        const int DIFF_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 37;
        const int HISTORY_MENU_ITEM_PRIORITY = PENDING_CHANGES_MENU_ITEM_PRIORITY + 38;
    }
}