using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Unity.PlasticSCM.Editor.Views.PendingChanges;

namespace Unity.PlasticSCM.Editor
{
    internal static class DrawGuiModeSwitcher
    {
        internal static void ForMode(
            bool isGluonMode,
            WorkspaceWindow workspaceWindow,
            TreeView changesTreeView,
            EditorWindow editorWindow)
        {
            GUI.enabled = !workspaceWindow.IsOperationInProgress();

            EditorGUI.BeginChangeCheck();

            GuiMode currentMode = isGluonMode ?
                GuiMode.GluonMode : GuiMode.DeveloperMode;

            GuiMode selectedMode = (GuiMode)EditorGUILayout.EnumPopup(
                currentMode,
                EditorStyles.toolbarDropDown,
                GUILayout.Width(100));

            if (EditorGUI.EndChangeCheck())
            {
                SwitchGuiModeIfUserWants(
                    workspaceWindow, currentMode, selectedMode,
                    changesTreeView, editorWindow);
            }

            GUI.enabled = true;
        }

        static void SwitchGuiModeIfUserWants(
            WorkspaceWindow workspaceWindow,
            GuiMode currentMode, GuiMode selectedMode,
            TreeView changesTreeView,
            EditorWindow editorWindow)
        {
            if (currentMode == selectedMode)
                return;

            bool userConfirmed = SwitchModeConfirmationDialog.SwitchMode(
                currentMode == GuiMode.GluonMode, editorWindow);

            if (!userConfirmed)
                return;

            bool isGluonMode = selectedMode == GuiMode.GluonMode;

            workspaceWindow.UpdateWorkspaceForMode(
                isGluonMode, workspaceWindow);

            PendingChangesTreeHeaderState.SetMode(
                changesTreeView.multiColumnHeader.state,
                isGluonMode);
        }

        enum GuiMode
        {
            DeveloperMode,
            GluonMode
        }
    }
}
