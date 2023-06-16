using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using Codice.Client.Common.Threading;
using PlasticGui;
using PlasticGui.WebApi;
using PlasticGui.WebApi.Responses;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.UIElements;

namespace Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome
{
    internal class OrganizationPanel : VisualElement
    {
        internal OrganizationPanel(
            CloudEditionWelcomeWindow parentWindow,
            IPlasticWebRestApi restApi,
            string title,
            List<string> organizations)
        {
            mParentWindow = parentWindow;
            mRestApi = restApi;

            InitializeLayoutAndStyles();

            BuildComponents(title, organizations);

            EditorWindowFocus.OnApplicationActivated += OnEditorActivated;
        }

        internal void Dispose()
        {
            EditorWindowFocus.OnApplicationActivated -= OnEditorActivated;

            mParentWindow.CancelJoinOrganization();

            if (mJoinSingleOrganizationButton != null)
                mJoinSingleOrganizationButton.clicked -= JoinOrganizationButton_clicked;

            if (mJoinMultipleOrganizationsButton != null)
                mJoinMultipleOrganizationsButton.clicked -= JoinOrganizationButton_clicked;

            if (mOpenUnityDashboardButton != null)
                mOpenUnityDashboardButton.clicked -= OpenUnityDashboardButton_clicked;
        }

        void OnEditorActivated()
        {
            if (!mReloadOrganizationsNeeded)
                return;

            mReloadOrganizationsNeeded = false;
            mProgressControls.ShowProgress(PlasticLocalization.Name.LoadingOrganizations.GetString());

            OrganizationsResponse organizationResponse = null;
            IThreadWaiter waiter = ThreadWaiter.GetWaiter();
            waiter.Execute(
                /*threadOperationDelegate*/ delegate
                {
                    organizationResponse = mRestApi.GetCloudServers();
                },
                /*afterOperationDelegate*/ delegate
                {
                    mProgressControls.HideProgress();

                    if (waiter.Exception != null)
                    {
                        mProgressControls.ShowError(PlasticLocalization.Name.UnexpectedError.GetString());
                        ExceptionsHandler.LogException(typeof(OrganizationPanel).Name, waiter.Exception);
                        return;
                    }

                    if (organizationResponse.Error != null)
                    {
                        mReloadOrganizationsNeeded = true;
                        mProgressControls.ShowError(organizationResponse.Error.Message);
                        return;
                    }

                    ProcessOrganizations(organizationResponse.CloudServers);
                });
        }

        void ProcessOrganizations(List<string> organizations)
        {
            this.Query<VisualElement>("noOrganization").Collapse();
            this.Query<VisualElement>("joinSingleOrganization").Collapse();
            this.Query<VisualElement>("joinMultipleOrganizations").Collapse();

            if (organizations.Count == 0)
            {
                mReloadOrganizationsNeeded = true;
                BuildNoOrganizationSection();

                mOpenUnityDashboardButton = this.Q<Button>("openUnityDashboardButton");
                mOpenUnityDashboardButton.clicked += OpenUnityDashboardButton_clicked;

                return;
            }

            mReloadOrganizationsNeeded = false;

            if (organizations.Count == 1)
            {
                BuildSingleOrganizationSection(organizations.First());

                mJoinSingleOrganizationButton = this.Q<Button>("joinSingleOrganizationButton");
                mJoinSingleOrganizationButton.clicked += JoinOrganizationButton_clicked;

                return;
            }

            BuildMultipleOrganizationsSection(organizations);

            mJoinMultipleOrganizationsButton = this.Q<Button>("joinMultipleOrganizationsButton");
            mJoinMultipleOrganizationsButton.clicked += JoinOrganizationButton_clicked;
            mOrganizationToJoin = organizations.First();
        }

        void InitializeLayoutAndStyles()
        {
            this.LoadLayout(typeof(OrganizationPanel).Name);
            this.LoadStyle(typeof(OrganizationPanel).Name);
        }

        void JoinOrganizationButton_clicked()
        {
            mParentWindow.JoinOrganizationAndWelcomePage(mOrganizationToJoin);

            // TODO: Closing the window for now. Need to connect this event to the main on boarding
            //       workflow.
            mParentWindow.Close();
        }

        void OpenUnityDashboardButton_clicked()
        {
            Application.OpenURL(UnityUrl.UnityDashboard.Get());
        }

        void BuildComponents(string title, List<string> organizations)
        {
            mParentWindow.titleContent = new UnityEngine.GUIContent(title);

            mProgressControls = new ProgressControlsForDialogs(null);

            mProgressContainer = this.Q<VisualElement>("progressContainer");
            mProgressContainer.Add((VisualElement)mProgressControls);

            ProcessOrganizations(organizations);
        }

        void BuildSingleOrganizationSection(string organizationName)
        {
            this.SetControlText<Label>("confirmationMessage",
                PlasticLocalization.Name.JoinOrganizationTitle);

            mOrganizationToJoin = organizationName;

            this.Query<VisualElement>("joinSingleOrganization").Show();

            this.SetControlText<Label>("joinSingleOrganizationLabel",
                PlasticLocalization.Name.YouBelongToOrganization, organizationName);

            this.SetControlText<Button>("joinSingleOrganizationButton",
                PlasticLocalization.Name.JoinButton);
        }

        void BuildMultipleOrganizationsSection(List<string> organizationNames)
        {
            this.SetControlText<Label>("confirmationMessage",
                PlasticLocalization.Name.JoinOrganizationTitle);

            this.Query<VisualElement>("joinMultipleOrganizations").Show();

            this.SetControlText<Label>("joinMultipleOrganizationsLabel",
                PlasticLocalization.Name.YouBelongToSeveralOrganizations);

            VisualElement organizationDropdown = this.Query<VisualElement>("organizationDropdown");
            ToolbarMenu toolbarMenu = new ToolbarMenu
            {
                text = organizationNames.FirstOrDefault(),
            };

            foreach (string name in organizationNames)
            {
                toolbarMenu.menu.AppendAction(name, x => 
                {
                    toolbarMenu.text = name;
                    mOrganizationToJoin = name;
                }, DropdownMenuAction.AlwaysEnabled);
                organizationDropdown.Add(toolbarMenu);
            }

            this.SetControlText<Button>("joinMultipleOrganizationsButton",
                PlasticLocalization.Name.JoinButton);
        }

        void BuildNoOrganizationSection()
        {
            this.SetControlText<Label>("confirmationMessage",
                PlasticLocalization.Name.CreateOrganizationTitle);

            this.Query<VisualElement>("noOrganization").Show();

            this.SetControlImage("iconUnity",
                Images.Name.ButtonSsoSignInUnity);

            this.SetControlText<Label>("noOrganizationLabel",
                PlasticLocalization.Name.ClickButtonBelowToCreateOrg);

            this.SetControlText<Button>("openUnityDashboardButton",
                PlasticLocalization.Name.MainSidebarOpenUnityDashboardItem);
        }

        string mOrganizationToJoin = "";
        bool mReloadOrganizationsNeeded;

        Button mJoinSingleOrganizationButton;
        Button mJoinMultipleOrganizationsButton;
        Button mOpenUnityDashboardButton;
        VisualElement mProgressContainer;

        IProgressControls mProgressControls;

        readonly CloudEditionWelcomeWindow mParentWindow;
        readonly IPlasticWebRestApi mRestApi;
    }
}