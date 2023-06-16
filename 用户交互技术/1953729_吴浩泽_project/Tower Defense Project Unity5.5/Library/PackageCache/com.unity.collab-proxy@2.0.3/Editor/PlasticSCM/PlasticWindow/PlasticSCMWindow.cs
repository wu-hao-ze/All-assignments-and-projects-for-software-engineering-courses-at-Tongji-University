using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.PlasticSCM.Editor
{
    internal class PlasticSCMWindow : EditorWindow
    {
        internal static void ShowWindow()
        {
            PlasticSCMWindow window = GetWindow<PlasticSCMWindow>();
            window.titleContent = new GUIContent(
                UnityConstants.PLASTIC_WINDOW_TITLE,
                Images.GetPlasticViewIcon());

            window.minSize= new Vector2(750, 260);

            window.Show();
        }

        void OnEnable()
        {
            BuildComponents();
        }

        void OnDestroy()
        {
            Dispose();
        }

        void Dispose()
        {
            mRefreshButton.clicked -= RefreshButton_Clicked;
            mSettingsButton.clicked -= SettingsButton_Clicked;
        }

        void BuildComponents()
        {
            VisualElement root = rootVisualElement;
            root.Clear();
            root.LoadStyle("PlasticWindow/PlasticWindow");

            BuildTabview(root);
            BuildStatusBar(root);
        }

        /*** Tabview ***/
        void BuildTabview(VisualElement root)
        {
            mTabView = new TabView();
            mTabView.LoadStyle("PlasticWindow/PlasticWindow");

            mTabView.AddTab(
                PlasticLocalization.GetString(PlasticLocalization.Name.PendingChangesViewTitle),
                new VisualElement()).clicked += () =>
                {
                    // TODO: Add view switch to Pending Changes here
                };
            mTabView.AddTab(
                PlasticLocalization.GetString(PlasticLocalization.Name.ChangesetsViewTitle),
                new VisualElement()).clicked += () =>
                {
                    // TODO: Add view switch to Changesets here
                };
            mTabView.AddTab(
                PlasticLocalization.GetString(PlasticLocalization.Name.IncomingChangesViewTitle),
                new VisualElement()).clicked += () =>
                {
                    // TODO: Add view switch to Incoming Changes here
                };

            VisualElement controlsContainer = new VisualElement() { name = "ControlsContainer" };
            controlsContainer.AddToClassList("row");

            mRefreshButton = new Button() { name = "RefreshButton" };
            mRefreshButton.Add(new Image() { image = Images.GetRefreshIcon() });
            mRefreshButton.clicked += RefreshButton_Clicked;
            controlsContainer.Add(mRefreshButton);

            mSettingsButton = new Button() { name = "SettingsButton" };
            mSettingsButton.Add(new Image() { image = EditorGUIUtility.IconContent("settings").image });
            mSettingsButton.clicked += SettingsButton_Clicked;
            controlsContainer.Add(mSettingsButton);

            var tabArea = mTabView.Q<VisualElement>("TabArea");
            tabArea.Add(controlsContainer);

            root.Add(mTabView);
        }

        void RefreshButton_Clicked()
        {
            // TODO
        }

        void SettingsButton_Clicked()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Invite Members to Workspace"),
                false,
                InviteMemberButton_clicked,
                null);
            menu.ShowAsContext();
        }

        static void InviteMemberButton_clicked(object obj)
        {
            Application.OpenURL("https://www.plasticscm.com/dashboard/cloud/unity_cloud/users-and-groups");
        }

        /*** Update Bar ***/
        void BuildStatusBar(VisualElement root)
        {
            VisualElement StatusBar = new VisualElement() { name = "StatusBar" };
            StatusBar.AddToClassList("row");
            StatusBar.LoadLayout("PlasticWindow/StatusBar");

            mUpdateNotification = StatusBar.Q<VisualElement>("UpdateNotificationContainer");

            mUpdateNotificaionImage = StatusBar.Q<Image>("UpdateNotificationImage");
            mUpdateNotificationLabel = StatusBar.Q<Label>("UpdateNotificationLabel");

            mUpdateButton = StatusBar.Q<Button>("UpdateButton");
            mUpdateButton.text = PlasticLocalization.GetString(PlasticLocalization.Name.UpdateButton);

            mBranchLabel = StatusBar.Q<Label>("BranchLabel");
            mBranchLabel.text = "Branch main @ codice @ codice@cloud";

            ShowUpdateNotification(false);

            root.Add(StatusBar);
        }

        // For the icon string, the name of unity icons can be found at
        // https://unitylist.com/p/5c3/Unity-editor-icons
        internal void ShowUpdateNotification(bool show, string icon = "", string notification = "")
        {
            if (!string.IsNullOrEmpty(icon))
                mUpdateNotificaionImage.image = EditorGUIUtility.IconContent(icon).image;
            if (!string.IsNullOrEmpty(notification))
                mUpdateNotificationLabel.text = notification;

            if (show)
                mUpdateNotification.Show();
            else
                mUpdateNotification.Collapse();
        }

        // Tabview variables
        internal TabView mTabView;
        Button mRefreshButton;
        Button mSettingsButton;

        // Update bar variables
        VisualElement mUpdateNotification;
        Image mUpdateNotificaionImage;
        Label mUpdateNotificationLabel;
        Button mUpdateButton;
        Label mBranchLabel;
    }
}