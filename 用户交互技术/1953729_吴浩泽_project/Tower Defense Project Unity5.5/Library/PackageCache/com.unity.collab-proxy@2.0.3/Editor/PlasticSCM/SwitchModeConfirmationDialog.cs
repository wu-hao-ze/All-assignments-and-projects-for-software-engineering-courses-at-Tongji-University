using UnityEditor;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor
{
    internal class SwitchModeConfirmationDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 560, 180);
            }
        }

        internal static bool SwitchMode(
            bool isGluonMode,
            EditorWindow parentWindow)
        {
            SwitchModeConfirmationDialog dialog = Create(isGluonMode);
            return dialog.RunModal(parentWindow) == ResponseType.Ok;
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
                PlasticLocalization.Name.SwitchModeConfirmationDialogTitle));

            DoExplanationArea(mIsGluonMode);

            GUILayout.Space(20);

            DoButtonsArea();
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.SwitchModeConfirmationDialogTitle);
        }

        void DoExplanationArea(bool isGluonMode)
        {
            PlasticLocalization.Name currentMode = isGluonMode ?
                PlasticLocalization.Name.GluonMode :
                PlasticLocalization.Name.DeveloperMode;

            PlasticLocalization.Name selectedMode = isGluonMode ?
                PlasticLocalization.Name.DeveloperMode :
                PlasticLocalization.Name.GluonMode;

            string formattedExplanation = PlasticLocalization.GetString(
                PlasticLocalization.Name.SwitchModeConfirmationDialogExplanation,
                PlasticLocalization.GetString(currentMode),
                PlasticLocalization.GetString(selectedMode),
                "{0}");

            TextBlockWithEndLink(
                GLUON_HELP_URL, formattedExplanation, UnityStyles.Paragraph);
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoSwitchButton();
                    DoCancelButton();
                    return;
                }

                DoCancelButton();
                DoSwitchButton();
            }
        }

        void DoSwitchButton()
        {
            if (!AcceptButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.SwitchButton)))
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

        static SwitchModeConfirmationDialog Create(
            bool isGluonMode)
        {
            var instance = CreateInstance<SwitchModeConfirmationDialog>();
            instance.mIsGluonMode = isGluonMode;
            instance.mEnterKeyAction = instance.OkButtonAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            return instance;
        }

        bool mIsGluonMode;

        const string GLUON_HELP_URL = "https://www.plasticscm.com/gluon";
    }
}
