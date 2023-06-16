using UnityEditor;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.Cloud.Collaborate
{
    [InitializeOnLoad]
    internal static class ToolbarBootstrap
    {
        static ToolbarBootstrap()
        {
            CooldownWindowDelayer cooldownInitializeAction = new CooldownWindowDelayer(
                ToolbarButton.InitializeIfNeeded, UnityConstants.PLUGIN_DELAYED_INITIALIZE_INTERVAL);
            cooldownInitializeAction.Ping();
        }
    }

    internal class ToolbarButton : SubToolbar
    {
        internal static void InitializeIfNeeded()
        {
            if (CollabPlugin.IsEnabled())
                return;

            ToolbarButton toolbar = new ToolbarButton { Width = 32f };
            Toolbar.AddSubToolbar(toolbar);
        }

        ToolbarButton()
        {
            PlasticPlugin.OnNotificationUpdated += OnPlasticNotificationUpdated;
        }

        ~ToolbarButton()
        {
            PlasticPlugin.OnNotificationUpdated -= OnPlasticNotificationUpdated;
        }

        void OnPlasticNotificationUpdated()
        {
            Toolbar.RepaintToolbar();
        }

        public override void OnGUI(Rect rect)
        {
            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
            {
                Texture icon = PlasticPlugin.GetPluginStatusIcon();
                EditorGUIUtility.SetIconSize(new Vector2(16, 16));

                mButtonGUIContent.image = icon;

                if (GUI.Button(rect, mButtonGUIContent, "AppCommand"))
                {
                    PlasticPlugin.OpenPlasticWindowDisablingOfflineModeIfNeeded();
                }

                EditorGUIUtility.SetIconSize(Vector2.zero);
            }
        }

        static GUIContent mButtonGUIContent = new GUIContent(
            string.Empty, PlasticLocalization.GetString(
                PlasticLocalization.Name.UnityVersionControl));
    }
}
