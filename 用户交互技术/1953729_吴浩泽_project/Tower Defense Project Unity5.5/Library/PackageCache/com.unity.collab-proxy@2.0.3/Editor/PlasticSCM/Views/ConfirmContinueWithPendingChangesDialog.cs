using UnityEditor;
using UnityEngine;

using Codice.Client.Common;

using Codice.Client.Commands;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Update;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views
{
    internal class ContinueWithPendingChangesQuestionerBuilder :
        SwitchController.IContinueWithPendingChangesQuestionerBuilder
    {
        internal ContinueWithPendingChangesQuestionerBuilder(
            IViewSwitcher viewSwitcher,
            EditorWindow parentWindow)
        {
            mViewSwitcher = viewSwitcher;
            mParentWindow = parentWindow;
        }

        public IContinueWithPendingChangesQuestioner Get(string title, string explanation)
        {
            return new ContinueWithPendingChangesQuestioner(
                title,
                explanation,
                mViewSwitcher,
                mParentWindow);
        }

        IViewSwitcher mViewSwitcher;
        EditorWindow mParentWindow;
    }

    internal class ContinueWithPendingChangesQuestioner : IContinueWithPendingChangesQuestioner
    {
        internal ContinueWithPendingChangesQuestioner(
            string title,
            string explanation,
            IViewSwitcher viewSwitcher,
            EditorWindow parentWindow)
        {
            mTitle = title;
            mExplanation = explanation;
            mViewSwitcher = viewSwitcher;
            mParentWindow = parentWindow;
        }

        public bool ContinueWithPendingChanges()
        {
            bool result = false;

            GUIActionRunner.RunGUIAction(() =>
            {
                result = ConfirmContinueWithPendingChangesDialog.ConfirmContinue(
                    mTitle,
                    mExplanation,
                    mViewSwitcher,
                    mParentWindow);
            });

            return result;
        }

        string mTitle;
        string mExplanation;
        IViewSwitcher mViewSwitcher;
        EditorWindow mParentWindow;
    }
    
    internal class ConfirmContinueWithPendingChangesDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 500, 287);
            }
        }

        internal static bool ConfirmContinue(
            string title,
            string explanation,
            IViewSwitcher viewSwitcher,
            EditorWindow parentWindow)
        {
            ConfirmContinueWithPendingChangesDialog dialog = Create(
                title,
                explanation,
                viewSwitcher);

            if (dialog.RunModal(parentWindow) != ResponseType.Ok)
                return false;

            if (dialog.mIsSwitchToConfirmationChecked)
                SavePreference();

            return true;
        }

        static ConfirmContinueWithPendingChangesDialog Create(
            string title,
            string explanation,
            IViewSwitcher viewSwitcher)
        {
            var instance = CreateInstance<ConfirmContinueWithPendingChangesDialog>();
            instance.mTitle = title;
            instance.mExplanation = explanation;
            instance.mViewSwitcher = viewSwitcher;
            return instance;
        }

        static void SavePreference()
        {
            ClientConfigData data = ClientConfig.Get().GetClientConfigData();
            data.SetPendingChangesOnSwitchAction(UserAction.None);
            ClientConfig.Get().Save(data);
        }

        protected override string GetTitle()
        {
            return mTitle;
        }

        protected override void OnModalGUI()
        {
            Title(mTitle);

            Paragraph(mExplanation);

            DoSwitchToConfirmationCheckButton();

            GUILayout.Space(10);

            DoButtonsArea();
        }

        void DoSwitchToConfirmationCheckButton()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                mIsSwitchToConfirmationChecked = TitleToggle(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.SwitchToConfirmationCheckButton),
                    mIsSwitchToConfirmationChecked);
            }
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoContinueButton();
                    DoCancelAndViewPendingChangesButton();
                    DoCancelButton();
                    return;
                }

                DoCancelButton();
                DoCancelAndViewPendingChangesButton();
                DoContinueButton();
            }
        }

        void DoContinueButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.SwitchToConfirmationContinueButton)))
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

        void DoCancelAndViewPendingChangesButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.SwitchToConfirmationCancelViewChangesButton)))
                return;

            mViewSwitcher.ShowPendingChanges();
            CancelButtonAction();
        }

        string mTitle;
        string mExplanation;
        IViewSwitcher mViewSwitcher;

        bool mIsSwitchToConfirmationChecked;
    }
}