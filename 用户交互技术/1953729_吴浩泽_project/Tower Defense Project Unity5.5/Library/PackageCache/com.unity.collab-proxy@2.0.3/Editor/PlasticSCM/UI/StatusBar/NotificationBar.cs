using UnityEditor;
using UnityEngine;

using PlasticGui.WebApi.Responses;
using PlasticGui.WorkspaceWindow.NotificationBar;

namespace Unity.PlasticSCM.Editor.UI.StatusBar
{
    class NotificationBar : INotificationBar
    {
        internal bool HasNotification { get; private set; }
        internal bool IsVisible { get; private set; }

        internal NotificationBar()
        {
            mSubscriptionPanel = new ActionPanel();
            mContactPanel = new ActionPanel();

            IsVisible = EditorPrefs.GetBool(
                UnityConstants.SHOW_NOTIFICATION_KEY_NAME,
                true);
        }

        internal void SetVisibility(bool isVisible)
        {
            IsVisible = isVisible;

            EditorPrefs.SetBool(
                UnityConstants.SHOW_NOTIFICATION_KEY_NAME,
                isVisible);
        }

        internal void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(UnityStyles.StatusBar.NotificationPanel);

            if (mSubscriptionPanel.HasNotification)
                mSubscriptionPanel.OnGUI();
            
            GUILayout.FlexibleSpace();

            if (mContactPanel.HasNotification)
                mContactPanel.OnGUI();

            DrawCloseButton(this);

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }

        void INotificationBar.SetActions(
            CloudServerInfo cloudServerInfo,
            CloudOrganizationHelpActionsResponse.Action subscriptionAction,
            CloudOrganizationHelpActionsResponse.Action contactAction)
        {
            mSubscriptionPanel.SetAction(cloudServerInfo, subscriptionAction, false);
            mContactPanel.SetAction(cloudServerInfo, contactAction, true);

            HasNotification = mSubscriptionPanel.HasNotification || mContactPanel.HasNotification;
        }

        static void DrawCloseButton(NotificationBar notificationBar)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(
                    new GUIContent(Images.GetCloseIcon()),
                    UnityStyles.StatusBar.NotificationPanelCloseButton))
            {
                notificationBar.SetVisibility(false);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        class ActionPanel
        {
            internal bool HasNotification { get; private set; }

            internal void SetAction(
                CloudServerInfo cloudServerInfo,
                CloudOrganizationHelpActionsResponse.Action action,
                bool isContactSupportAction)
            {
                if (action == null)
                {
                    HasNotification = false;
                    return;
                }

                mCloudServerInfo = cloudServerInfo;
                mActionButton = action.Button;
                mIsContactSupportAction = isContactSupportAction;

                HasNotification = true;
                mLabelText = action.Message;
                SetButton(action.Button);
            }

            internal void OnGUI()
            {
                DrawLabel(mLabelText);

                if (!mIsButtonVisible)
                    return;

                DrawButton(
                    mCloudServerInfo, mActionButton.Url,
                    mIsContactSupportAction, mButtonText);
            }

            void SetButton(
                CloudOrganizationHelpActionsResponse.ActionButton actionButton)
            {
                if (actionButton == null)
                {
                    mButtonText = string.Empty;
                    mIsButtonVisible = false;
                    return;
                }

                mButtonText = actionButton.Caption;
                mIsButtonVisible = true;
            }

            static void DrawLabel(string text)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                GUILayout.Label(
                    text,
                    UnityStyles.StatusBar.Label);

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }

            static void DrawButton(
                CloudServerInfo cloudServerInfo,
                string actionButtonUrl,
                bool isContactSupportAction,
                string buttonText)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(
                        buttonText,
                        UnityStyles.StatusBar.LinkLabel))
                {
                    LaunchNotificationAction.For(
                        cloudServerInfo,
                        actionButtonUrl,
                        isContactSupportAction);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }

            bool mIsButtonVisible;
            string mButtonText;
            string mLabelText;

            bool mIsContactSupportAction;
            CloudOrganizationHelpActionsResponse.ActionButton mActionButton;
            CloudServerInfo mCloudServerInfo;
        }

        ActionPanel mSubscriptionPanel;
        ActionPanel mContactPanel;
    }
}