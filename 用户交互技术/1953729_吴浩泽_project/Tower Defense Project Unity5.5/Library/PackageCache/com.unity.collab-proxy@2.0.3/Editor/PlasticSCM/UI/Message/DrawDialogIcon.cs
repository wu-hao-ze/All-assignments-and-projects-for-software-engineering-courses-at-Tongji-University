using UnityEditor;
using UnityEngine;

using Codice.Client.Common;

namespace Unity.PlasticSCM.Editor.UI.Message
{
    internal static class DrawDialogIcon
    {
        internal static void ForMessage(GuiMessage.GuiMessageType alertType)
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(80)))
            {
                Rect iconRect = GUILayoutUtility.GetRect(
                    GUIContent.none, EditorStyles.label,
                    GUILayout.Width(60), GUILayout.Height(60));

                GUI.DrawTexture(
                    iconRect,
                    Images.GetPlasticIcon(),
                    ScaleMode.ScaleToFit);

                Rect overlayIconRect = new Rect(
                    iconRect.xMax - 30, iconRect.yMax - 24, 32, 32);

                GUI.DrawTexture(
                    overlayIconRect,
                    GetHelpIcon(alertType),
                    ScaleMode.ScaleToFit);
            }
        }

        static Texture GetHelpIcon(GuiMessage.GuiMessageType alertType)
        {
            switch (alertType)
            {
                case GuiMessage.GuiMessageType.Critical:
                    return Images.GetErrorDialogIcon();
                case GuiMessage.GuiMessageType.Warning:
                    return Images.GetWarnDialogIcon();
                default:
                    return Images.GetInfoDialogIcon();
            }
        }
    }
}
