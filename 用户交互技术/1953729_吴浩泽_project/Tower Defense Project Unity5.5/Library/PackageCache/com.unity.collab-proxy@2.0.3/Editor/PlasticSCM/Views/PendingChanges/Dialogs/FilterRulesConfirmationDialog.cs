using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using PlasticGui;
using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs
{
    internal class FilterRulesConfirmationDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 520, 350);
            }
        }

        internal static FilterRulesConfirmationData AskForConfirmation(
            string[] rules,
            bool isAddOperation,
            bool isApplicableToAllWorkspaces,
            EditorWindow parentWindow)
        {
            string explanation = PlasticLocalization.GetString(isAddOperation ?
                PlasticLocalization.Name.FilterRulesConfirmationAddMessage :
                PlasticLocalization.Name.FilterRulesConfirmationRemoveMessage);

            FilterRulesConfirmationDialog dialog = Create(
                explanation, GetRulesText(rules), isApplicableToAllWorkspaces);

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            FilterRulesConfirmationData result = new FilterRulesConfirmationData(
                dialog.mApplyRulesToAllWorkspace, dialog.GetRules());

            result.Result = dialogResult == ResponseType.Ok;
            return result;
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
                PlasticLocalization.Name.FilterRulesConfirmationTitle));

            Paragraph(mDialogExplanation);

            RulesArea();

            GUILayout.Space(20);

            DoButtonsArea();
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.FilterRulesConfirmationTitle);
        }

        void RulesArea()
        {
            mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition);

            mRulesText = EditorGUILayout.TextArea(
                mRulesText, GUILayout.ExpandHeight(true));

            mIsTextAreaFocused = FixTextAreaSelectionIfNeeded(mIsTextAreaFocused);

            EditorGUILayout.EndScrollView();

            if (!mIsApplicableToAllWorkspaces)
                return;

            mApplyRulesToAllWorkspace = EditorGUILayout.ToggleLeft(
                PlasticLocalization.GetString(PlasticLocalization.Name.ApplyRulesToAllWorkspaceCheckButton),
                mApplyRulesToAllWorkspace, GUILayout.Width(200));
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

        string[] GetRules()
        {
            if (string.IsNullOrEmpty(mRulesText))
                return new string[0];

            return mRulesText.Split(
                Environment.NewLine.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries);
        }

        static bool FixTextAreaSelectionIfNeeded(bool isTextAreaFocused)
        {
            TextEditor textEditor = typeof(EditorGUI)
              .GetField("activeEditor", BindingFlags.Static | BindingFlags.NonPublic)
              .GetValue(null) as TextEditor;

            // text editor is null when it is not focused
            if (textEditor == null)
                return false;

            // restore the selection the first time that has selected text
            // because it is done automatically by Unity
            if (isTextAreaFocused)
                return true;

            if (string.IsNullOrEmpty(textEditor.SelectedText))
                return false;

            textEditor.SelectNone();
            textEditor.MoveTextEnd();
            return true;
        }

        static string GetRulesText(string[] rules)
        {
            if (rules == null)
                return string.Empty;

            return string.Join(Environment.NewLine, rules)
                 + Environment.NewLine;
        }

        static FilterRulesConfirmationDialog Create(
            string explanation,
            string rulesText,
            bool isApplicableToAllWorkspaces)
        {
            var instance = CreateInstance<FilterRulesConfirmationDialog>();
            instance.mDialogExplanation = explanation;
            instance.mRulesText = rulesText;
            instance.mIsApplicableToAllWorkspaces = isApplicableToAllWorkspaces;
            instance.mEnterKeyAction = instance.OkButtonAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            return instance;
        }

        bool mIsTextAreaFocused;
        Vector2 mScrollPosition;

        bool mApplyRulesToAllWorkspace;

        bool mIsApplicableToAllWorkspaces;
        string mRulesText;
        string mDialogExplanation;
    }
}
