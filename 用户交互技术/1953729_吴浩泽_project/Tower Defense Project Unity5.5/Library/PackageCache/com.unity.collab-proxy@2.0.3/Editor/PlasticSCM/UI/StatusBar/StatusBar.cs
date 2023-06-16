using UnityEditor;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.Gluon;
using PlasticGui.WorkspaceWindow.Topbar;
using PlasticGui.WorkspaceWindow.PendingChanges;

using GluonShowIncomingChanges = PlasticGui.Gluon.WorkspaceWindow.ShowIncomingChanges;

namespace Unity.PlasticSCM.Editor.UI.StatusBar
{
    interface IIncomingChangesNotifier
    {
        bool HasNotification { get; }
        IncomingChangesNotification Notification { get; }
    }

    internal class StatusBar
    {
        internal NotificationBar NotificationBar { get; private set; }

        internal StatusBar()
        {
            mCooldownNotificationClearAction = new CooldownWindowDelayer(
                DelayedClearNotification,
                UnityConstants.NOTIFICATION_CLEAR_INTERVAL);

            NotificationBar = new NotificationBar();
        }

        internal void Notify(string message, MessageType type, Texture2D image)
        {
            mNotification = new Notification(message, type, image);
            mCooldownNotificationClearAction.Ping();
        }

        internal void OnGUI(
            WorkspaceInfo wkInfo,
            WorkspaceWindow workspaceWindow,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonViewSwitcher,
            IIncomingChangesNotifier incomingChangesNotifier,
            bool isGluonMode)
        {
            if (NotificationBar.HasNotification &&
                NotificationBar.IsVisible)
            {
                BeginDrawBar();
                NotificationBar.OnGUI();
                EndDrawBar();
            }

            BeginDrawBar();

            if (NotificationBar.HasNotification)
            {
                DrawNotificationAvailablePanel(NotificationBar);
            }

            if (incomingChangesNotifier.HasNotification)
            {
                DrawIncomingChangesNotification(
                    wkInfo,
                    workspaceWindow,
                    mergeViewLauncher,
                    gluonViewSwitcher,
                    incomingChangesNotifier.Notification,
                    isGluonMode);
            }

            if (mNotification != null)
                DrawNotification(mNotification);

            GUILayout.FlexibleSpace();

            DrawWorkspaceStatus(workspaceWindow.WorkspaceStatus);

            EndDrawBar();
        }

        void DelayedClearNotification()
        {
            mNotification = null;
        }

        static void DrawNotificationAvailablePanel(
            NotificationBar notificationBar)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(PlasticLocalization.GetString(
                    notificationBar.IsVisible ?
                        PlasticLocalization.Name.HideNotification :
                        PlasticLocalization.Name.ShowNotification)))
            {
                notificationBar.SetVisibility(!notificationBar.IsVisible);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        static void DrawIncomingChangesNotification(
            WorkspaceInfo wkInfo,
            WorkspaceWindow workspaceWindow,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonViewSwitcher,
            IncomingChangesNotification notification,
            bool isGluonMode)
        {
            Texture2D icon = notification.Status == PlasticNotification.Status.Conflicts ?
                Images.GetConflictedIcon() :
                Images.GetOutOfSyncIcon();

            DrawIcon(icon);

            DrawNotificationLabel(notification.InfoText);

            if (DrawButton(notification.ActionText, notification.TooltipText))
            {
                if (notification.HasUpdateAction)
                {
                    workspaceWindow.UpdateWorkspace();
                    return;
                }

                ShowIncomingChangesForMode(
                    wkInfo,
                    mergeViewLauncher,
                    gluonViewSwitcher,
                    isGluonMode);
            }
        }

        static void DrawNotification(Notification notification)
        {
            DrawIcon(notification.Image);
            DrawNotificationLabel(notification.Message);
        }

        static void DrawWorkspaceStatus(WorkspaceStatusString.Data status)
        {
            DrawIcon(Images.GetBranchIcon());

            if (status == null)
                return;

            DrawLabel(string.Format(
                "{0}@{1}@{2}",
                status.ObjectSpec,
                status.RepositoryName,
                status.Server));
        }

        static void DrawIcon(Texture2D icon)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label(
                icon,
                UnityStyles.StatusBar.Icon,
                GUILayout.Height(UnityConstants.STATUS_BAR_ICON_SIZE),
                GUILayout.Width(UnityConstants.STATUS_BAR_ICON_SIZE));

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        static void DrawLabel(string label)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label(
                label,
                UnityStyles.StatusBar.Label);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        static void DrawNotificationLabel(string label)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label(
                label,
                UnityStyles.StatusBar.NotificationLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        static bool DrawButton(string label, string tooltip)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            bool buttonClicked = GUILayout.Button(
                new GUIContent(label, tooltip),
                UnityStyles.StatusBar.Button);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            return buttonClicked;
        }

        static void ShowIncomingChangesForMode(
            WorkspaceInfo workspaceInfo,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonSwitcher,
            bool isGluonMode)
        {
            if (isGluonMode)
            {
                GluonShowIncomingChanges.FromNotificationBar(
                    workspaceInfo, gluonSwitcher);
                return;
            }

            ShowIncomingChanges.FromNotificationBar(
                workspaceInfo, mergeViewLauncher);
        }

        static void BeginDrawBar()
        {
            EditorGUILayout.BeginVertical(
                GetBarStyle(),
                GUILayout.Height(UnityConstants.STATUS_BAR_HEIGHT));
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
        }

        static void EndDrawBar()
        {
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        static GUIStyle GetBarStyle()
        {
            if (sBarTexture == null)
                sBarTexture = new Texture2D(1, 1);

            if (sBarStyle == null)
                sBarStyle = new GUIStyle();

            sBarTexture.SetPixel(0, 0, UnityStyles.Colors.BackgroundBar);
            sBarTexture.Apply();
            sBarStyle.normal.background = sBarTexture;

            return sBarStyle;
        }

        class Notification
        {
            internal string Message { get; private set; }
            internal MessageType MessageType { get; private set; }
            internal Texture2D Image { get; private set; }

            internal Notification(string message, MessageType messageType, Texture2D image)
            {
                Message = message;
                MessageType = messageType;
                Image = image;
            }
        }

        Notification mNotification;

        readonly CooldownWindowDelayer mCooldownNotificationClearAction;

        static Texture2D sBarTexture;
        static GUIStyle sBarStyle;
    }
}