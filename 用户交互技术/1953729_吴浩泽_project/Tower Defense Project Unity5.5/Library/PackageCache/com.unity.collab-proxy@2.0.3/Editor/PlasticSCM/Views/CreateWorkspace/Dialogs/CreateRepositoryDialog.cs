using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Codice.Client.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Home.Repositories;
using PlasticGui.WebApi;
using PlasticGui.WorkspaceWindow.Servers;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace.Dialogs
{
    internal class CreateRepositoryDialog :
        PlasticDialog,
        KnownServersListOperations.IKnownServersList
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 600, 275);
            }
        }

        internal static RepositoryCreationData CreateRepository(
            EditorWindow parentWindow,
            IPlasticWebRestApi plasticWebRestApi,
            string proposedRepositoryName,
            string proposedServer,
            string lastUsedRepServer,
            string clientConfServer)
        {
            string server = CreateRepositoryDialogUserAssistant.GetProposedServer(
                proposedServer, lastUsedRepServer, clientConfServer);

            CreateRepositoryDialog dialog = Create(
                plasticWebRestApi,
                new ProgressControlsForDialogs(),
                proposedRepositoryName,
                server);

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            RepositoryCreationData result = dialog.BuildCreationData();

            result.Result = dialogResult == ResponseType.Ok;
            return result;
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
                PlasticLocalization.Name.NewRepositoryTitle));

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.NewRepositoryExplanation));

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.CreateRepositoryDialogDetailedExplanation));

            if (Event.current.type == EventType.Layout)
            {
                mProgressControls.ProgressData.CopyInto(mProgressData);
            }

            GUILayout.Space(5);

            DoEntriesArea();

            GUILayout.Space(10);

            DrawProgressForDialogs.For(
                mProgressControls.ProgressData);

            DoButtonsArea();

            mProgressControls.ForcedUpdateProgress(this);
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.NewRepositoryTitle);
        }

        void KnownServersListOperations.IKnownServersList.FillValues(List<string> values)
        {
            mKnownServers = values;
        }

        void DoEntriesArea()
        {
            mName = TextEntry(
                PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryNameShortLabel),
                mName,
                REPONAME_CONTROL_NAME,
                ENTRY_WIDTH,
                ENTRY_X);

            if (!mFocusIsAreadySet)
            {
                mFocusIsAreadySet = true;
                GUI.FocusControl(REPONAME_CONTROL_NAME);
            }

            GUILayout.Space(5);

            mServer = ComboBox(
                PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryExplorerServerLabel),
                mServer,
                DROPDOWN_CONTROL_NAME,
                mKnownServers,
                new GenericMenu.MenuFunction2(SelectServer),
                ENTRY_WIDTH,
                ENTRY_X);
        }

        void SelectServer(object userData)
        {
            mServer = userData.ToString();

            Repaint();
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoOkButton();
                    DoCancelButton();
                    return;
                }

                DoCancelButton();
                DoOkButton();
            }
        }

        void DoOkButton()
        {
            if (!AcceptButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.OkButton)))
                return;

            OkButtonWithValidationAction();
        }

        void DoCancelButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CancelButton)))
                return;

            CancelButtonAction();
        }

        void OkButtonWithValidationAction()
        {
            RepositoryCreationValidation.AsyncValidation(
                BuildCreationData(),
                this,
                mProgressControls);
        }

        RepositoryCreationData BuildCreationData()
        {
            return new RepositoryCreationData(mName, mServer);
        }

        static CreateRepositoryDialog Create(
            IPlasticWebRestApi plasticWebRestApi,
            ProgressControlsForDialogs progressControls,
            string proposedRepositoryName,
            string proposedServer)
        {
            var instance = CreateInstance<CreateRepositoryDialog>();
            instance.mEnterKeyAction = instance.OkButtonWithValidationAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            instance.mPlasticWebRestApi = plasticWebRestApi;
            instance.mProgressControls = progressControls;
            instance.BuildComponents(proposedRepositoryName, proposedServer);
            return instance;
        }

        void BuildComponents(string proposedRepositoryName, string proposedServer)
        {
            mName = proposedRepositoryName;
            mServer = proposedServer;

            KnownServersListOperations.GetCombinedServers(
                true,
                GetExtraServers(proposedServer),
                mProgressControls,
                this,
                mPlasticWebRestApi,
                CmConnection.Get().GetProfileManager());
        }

        static List<string> GetExtraServers(string proposedServer)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(proposedServer))
                result.Add(proposedServer);

            return result;
        }

        IPlasticWebRestApi mPlasticWebRestApi;
        bool mFocusIsAreadySet;

        string mName;
        string mServer;
        List<string> mKnownServers = new List<string>();
        ProgressControlsForDialogs.Data mProgressData = new ProgressControlsForDialogs.Data();

        ProgressControlsForDialogs mProgressControls;

        const float ENTRY_WIDTH = 400;
        const float ENTRY_X = 175f;
        const string REPONAME_CONTROL_NAME = "CreateRepositoryDialog.RepositoryName";
        const string DROPDOWN_CONTROL_NAME = "CreateRepositoryDialog.ServerDropdown";
    }
}
