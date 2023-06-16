using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using Codice.Client.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Home.Repositories;
using PlasticGui.WebApi;
using PlasticGui.WorkspaceWindow.Servers;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Views.CreateWorkspace.Dialogs
{
    internal class RepositoryExplorerDialog :
        PlasticDialog,
        KnownServersListOperations.IKnownServersList,
        FillRepositoriesTable.IGetFilterText
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 750, 450);
            }
        }

        internal static string BrowseRepository(
            EditorWindow parentWindow,
            IPlasticWebRestApi plasticWebRestApi,
            string defaultServer)
        {
            RepositoryExplorerDialog dialog = Create(
                plasticWebRestApi,
                new ProgressControlsForDialogs(),
                defaultServer,
                new UnityPlasticGuiMessage());

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            if (dialogResult != ResponseType.Ok)
                return null;

            return dialog.mRepositoriesListView.GetSelectedRepository();
        }

        void OnDisable()
        {
            mSearchField.downOrUpArrowKeyPressed -=
                SearchField_OnDownOrUpArrowKeyPressed;
        }

        protected override void SaveSettings()
        {
            TreeHeaderSettings.Save(
                mRepositoriesListView.multiColumnHeader.state,
                UnityConstants.REPOSITORIES_TABLE_SETTINGS_NAME);
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(PlasticLocalization.Name.ChooseRepositoryTitle));

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.SelectRepositoryBelow));

            if (Event.current.type == EventType.Layout)
            {
                mProgressControls.ProgressData.CopyInto(
                    mState.ProgressData);
            }

            bool isEnabled = !mProgressControls.ProgressData.IsWaitingAsyncResult;

            DoToolbarArea(
                mSearchField,
                mRepositoriesListView,
                isEnabled,
                Refresh,
                OnServerSelected,
                ref mState);

            GUILayout.Space(10);

            DoListArea(
                mRepositoriesListView,
                isEnabled);

            DrawProgressForDialogs.For(
                mProgressControls.ProgressData);

            DoButtonsArea();

            mProgressControls.ForcedUpdateProgress(this);
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(PlasticLocalization.Name.ExploreRepositories);
        }

        void SearchField_OnDownOrUpArrowKeyPressed()
        {
            mRepositoriesListView.SetFocusAndEnsureSelectedItem();
        }

        void Refresh()
        {
            mFillRepositoriesTable.FillTable(
                mRepositoriesListView,
                null,
                mProgressControls,
                null,
                new FillRepositoriesTable.SaveLastUsedServer(true),
                mGuiMessage,
                null,
                null,
                this,
                mState.Server,
                false,
                false,
                true);
        }

        void KnownServersListOperations.IKnownServersList.FillValues(
            List<string> values)
        {
            mState.AvailableServers = values;

            Refresh();
        }

        string FillRepositoriesTable.IGetFilterText.Get()
        {
            return mRepositoriesListView.searchString;
        }

        void OnServerSelected(object server)
        {
            mState.Server = server.ToString();
            Repaint();
            Refresh();
        }

        static void DoToolbarArea(
            SearchField searchField,
            RepositoriesListView listView,
            bool isEnabled,
            Action refreshAction,
            GenericMenu.MenuFunction2 selectServerAction,
            ref State state)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(
                PlasticLocalization.GetString(PlasticLocalization.Name.RepositoryExplorerServerLabel));

            GUI.enabled = isEnabled;

            state.Server = DoDropDownTextField(
                state.Server,
                state.AvailableServers,
                selectServerAction,
                refreshAction);

            var refreshText = PlasticLocalization.GetString(PlasticLocalization.Name.RefreshButton);
            if (GUILayout.Button(refreshText, EditorStyles.miniButton))
                refreshAction();

            GUILayout.FlexibleSpace();

            DrawSearchField.For(
                searchField, listView, SEARCH_FIELD_WIDTH);

            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        static void DoListArea(
            RepositoriesListView listView,
            bool isEnabled)
        {
            GUI.enabled = isEnabled;

            Rect treeRect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);

            listView.OnGUI(treeRect);

            GUI.enabled = true;
        }

        static string DoDropDownTextField(
            string text,
            List<string> options,
            GenericMenu.MenuFunction2 selectServerAction,
            Action enterKeyAction)
        {
            bool isEnterKeyPressed = false;

            Event e = Event.current;

            if (Keyboard.IsReturnOrEnterKeyPressed(e))
            {
                isEnterKeyPressed = true;
            }

            string result = DropDownTextField.DoDropDownTextField(
                text,
                DROPDOWN_CONTROL_NAME,
                options,
                selectServerAction,

                GUILayout.Width(DROPDOWN_WIDTH));

            if (isEnterKeyPressed &&
                GUI.GetNameOfFocusedControl() == DROPDOWN_CONTROL_NAME)
            {
                e.Use();
                enterKeyAction();
            }

            return result;
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

            OkButtonAction();
        }

        void DoCancelButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CancelButton)))
                return;

            CancelButtonAction();
        }

        static RepositoryExplorerDialog Create(
            IPlasticWebRestApi plasticWebRestApi,
            ProgressControlsForDialogs progressControls,
            string defaultServer,
            GuiMessage.IGuiMessage guiMessage)
        {
            var instance = CreateInstance<RepositoryExplorerDialog>();
            instance.mGuiMessage = guiMessage;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            instance.mProgressControls = progressControls;
            instance.BuildComponents(defaultServer, plasticWebRestApi);
            return instance;
        }

        void BuildComponents(
            string defaultServer,
            IPlasticWebRestApi plasticWebRestApi)
        {
            mSearchField = new SearchField();
            mSearchField.downOrUpArrowKeyPressed += SearchField_OnDownOrUpArrowKeyPressed;

            RepositoriesListHeaderState headerState =
                RepositoriesListHeaderState.GetDefault();
            TreeHeaderSettings.Load(headerState,
                UnityConstants.REPOSITORIES_TABLE_SETTINGS_NAME,
                (int)RepositoriesListColumn.Name);

            mRepositoriesListView = new RepositoriesListView(
                headerState,
                RepositoriesListHeaderState.GetColumnNames(),
                OkButtonAction);
            mRepositoriesListView.Reload();

            mFillRepositoriesTable = new FillRepositoriesTable(
                new LocalRepositoriesProvider());

            mState = new State()
            {
                Server = defaultServer,
                ProgressData = new ProgressControlsForDialogs.Data()
            };

            KnownServersListOperations.GetCombinedServers(
                true,
                new List<string>(),
                mProgressControls,
                this,
                plasticWebRestApi,
                CmConnection.Get().GetProfileManager());
        }

        SearchField mSearchField;
        RepositoriesListView mRepositoriesListView;
        ProgressControlsForDialogs mProgressControls;
        FillRepositoriesTable mFillRepositoriesTable;
        State mState;
        GuiMessage.IGuiMessage mGuiMessage;

        const string DROPDOWN_CONTROL_NAME = "RepositoryExplorerDialog.ServerDropdown";
        const float DROPDOWN_WIDTH = 250;
        const float SEARCH_FIELD_WIDTH = 450;

        class State
        {
            internal List<string> AvailableServers { get; set; }
            internal string Server { get; set; }
            internal ProgressControlsForDialogs.Data ProgressData { get; set; }
        }
    }
}
