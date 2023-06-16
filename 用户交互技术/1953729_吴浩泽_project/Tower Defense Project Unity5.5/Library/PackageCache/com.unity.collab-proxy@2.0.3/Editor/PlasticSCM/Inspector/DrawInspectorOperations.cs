using System.IO;

using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;

using Codice.CM.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Tool;

namespace Unity.PlasticSCM.Editor.Inspector
{
    static class DrawInspectorOperations
    {
        internal static void Enable(
            string wkPath,
            IAssetStatusCache assetStatusCache)
        {
            if (mIsEnabled)
                return;

            mWkPath = wkPath;
            mAssetStatusCache = assetStatusCache;

            mIsEnabled = true;

            mAssetSelection = new InspectorAssetSelection();

            UnityEditor.Editor.finishedDefaultHeaderGUI +=
                Editor_finishedDefaultHeaderGUI;

            RepaintInspector.All();
        }

        internal static void Disable()
        {
            mIsEnabled = false;

            UnityEditor.Editor.finishedDefaultHeaderGUI -=
                Editor_finishedDefaultHeaderGUI;

            RepaintInspector.All();

            mWkPath = null;
            mAssetStatusCache = null;
            mAssetSelection = null;
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
                Enable(wkInfo.ClientPath, assetStatusCache);

            mOperations = new AssetOperations(
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
        }

        static void Editor_finishedDefaultHeaderGUI(UnityEditor.Editor inspector)
        {
            if (!mIsEnabled)
                return;

            if (!FindWorkspace.HasWorkspace(ApplicationDataPath.Get()))
            {
                Disable();
                return;
            }

            mAssetSelection.SetActiveInspector(inspector);

            AssetList assetList = ((AssetOperations.IAssetSelection)
                mAssetSelection).GetSelectedAssets();

            if (assetList.Count == 0)
                return;

            SelectedAssetGroupInfo selectedGroupInfo = SelectedAssetGroupInfo.
                BuildFromAssetList(mWkPath, assetList, mAssetStatusCache);

            if (assetList.Count != selectedGroupInfo.SelectedCount)
                return;

            AssetsOverlays.AssetStatus assetStatus;
            LockStatusData lockStatusData;
            GetAssetStatusToDraw(
                assetList[0].path, assetList.Count,
                mAssetStatusCache,
                out assetStatus,
                out lockStatusData);

            AssetMenuOperations assetOperations = AssetMenuUpdater.
                GetAvailableMenuOperations(selectedGroupInfo);

            bool guiEnabledBck = GUI.enabled;
            GUI.enabled = true;
            try
            {
                DrawBackRectangle(guiEnabledBck);

                GUILayout.BeginHorizontal();
                
                DrawStatusLabel(assetStatus, lockStatusData);
                
                GUILayout.FlexibleSpace();

                DrawButtons(assetOperations);

                GUILayout.EndHorizontal();
            }
            finally
            {
                GUI.enabled = guiEnabledBck;
            }
        }

        static void DrawBackRectangle(bool isEnabled)
        {
            // when the inspector is disabled, there is a separator line
            // that breaks the visual style. Draw an empty rectangle
            // matching the background color to cover it

            GUILayout.Space(UnityConstants.INSPECTOR_ACTIONS_BACK_RECTANGLE_TOP_MARGIN);

            GUIStyle targetStyle = (isEnabled) ?
                UnityStyles.Inspector.HeaderBackgroundStyle :
                UnityStyles.Inspector.DisabledHeaderBackgroundStyle;

            Rect rect = GUILayoutUtility.GetRect(
                GUIContent.none, targetStyle);

            // extra space to cover the inspector full width
            rect.x -= 20;
            rect.width += 80;

            GUI.Box(rect, GUIContent.none, targetStyle);

            // now reset the space used by the rectangle
            GUILayout.Space(
                -UnityConstants.INSPECTOR_ACTIONS_HEADER_BACK_RECTANGLE_HEIGHT
                - UnityConstants.INSPECTOR_ACTIONS_BACK_RECTANGLE_TOP_MARGIN);
        }

        static void DrawButtons(
            AssetMenuOperations selectedGroupInfo)
        {
            if (selectedGroupInfo.HasFlag(AssetMenuOperations.Add))
                DoAddButton();

            if (selectedGroupInfo.HasFlag(AssetMenuOperations.Checkout))
                DoCheckoutButton();

            if (selectedGroupInfo.HasFlag(AssetMenuOperations.Checkin))
                DoCheckinButton();

            if (selectedGroupInfo.HasFlag(AssetMenuOperations.Undo))
                DoUndoButton();
        }

        static void DrawStatusLabel(
            AssetsOverlays.AssetStatus assetStatus,
            LockStatusData lockStatusData)
        {
            Texture overlayIcon = DrawAssetOverlay.DrawOverlayIcon.
                GetOverlayIcon(assetStatus);

            if (overlayIcon == null)
                return;

            string statusText = DrawAssetOverlay.
                GetStatusString(assetStatus);

            string tooltipText = DrawAssetOverlay.GetTooltipText(
                assetStatus, lockStatusData);

            Rect selectionRect = GUILayoutUtility.GetRect(
                new GUIContent(statusText + EXTRA_SPACE, overlayIcon),
                GUIStyle.none);

            selectionRect.height = UnityConstants.OVERLAY_STATUS_ICON_SIZE;

            Rect overlayRect = OverlayRect.GetCenteredRect(selectionRect);

            GUI.DrawTexture(
                overlayRect,
                overlayIcon,
                ScaleMode.ScaleToFit);

            selectionRect.x += UnityConstants.OVERLAY_STATUS_ICON_SIZE;
            selectionRect.width -= UnityConstants.OVERLAY_STATUS_ICON_SIZE;

            GUI.Label(
                selectionRect,
                new GUIContent(statusText, tooltipText));
        }

        static void DoAddButton()
        {
            string buttonText = PlasticLocalization.GetString(PlasticLocalization.Name.AddButton);
            if (GUILayout.Button(string.Format("{0}", buttonText), EditorStyles.miniButton))
            {
                if (mOperations == null)
                    ShowWindow.Plastic();

                mOperations.Add();
            }
        }

        static void DoCheckoutButton()
        {
            string buttonText = PlasticLocalization.GetString(PlasticLocalization.Name.CheckoutButton);
            if (GUILayout.Button(string.Format("{0}", buttonText), EditorStyles.miniButton))
            {
                if (mOperations == null)
                    ShowWindow.Plastic();

                mOperations.Checkout();
            }
        }

        static void DoCheckinButton()
        {
            string buttonText = PlasticLocalization.GetString(PlasticLocalization.Name.CheckinButton);
            if (GUILayout.Button(string.Format("{0}", buttonText), EditorStyles.miniButton))
            {
                if (mOperations == null)
                    ShowWindow.Plastic();

                mOperations.Checkin();
                EditorGUIUtility.ExitGUI();
            }
        }

        static void DoUndoButton()
        {
            string buttonText = PlasticLocalization.GetString(PlasticLocalization.Name.UndoButton);
            if (GUILayout.Button(string.Format("{0}", buttonText), EditorStyles.miniButton))
            {
                if (mOperations == null)
                    ShowWindow.Plastic();

                mOperations.Undo();
                EditorGUIUtility.ExitGUI();
            }
        }

        static void GetAssetStatusToDraw(
            string selectedPath,
            int selectedCount,
            IAssetStatusCache statusCache,
            out AssetsOverlays.AssetStatus assetStatus,
            out LockStatusData lockStatusData)
        {
            assetStatus = AssetsOverlays.AssetStatus.None;
            lockStatusData = null;

            if (selectedCount > 1)
                return;

            string selectedFullPath = Path.GetFullPath(selectedPath);

            assetStatus = statusCache.GetStatus(selectedFullPath);
            lockStatusData = statusCache.GetLockStatusData(selectedFullPath);
        }

        static IAssetMenuOperations mOperations;
        static InspectorAssetSelection mAssetSelection;

        static bool mIsEnabled;
        static IAssetStatusCache mAssetStatusCache;
        static string mWkPath;

        const string EXTRA_SPACE = "    ";
    }
}