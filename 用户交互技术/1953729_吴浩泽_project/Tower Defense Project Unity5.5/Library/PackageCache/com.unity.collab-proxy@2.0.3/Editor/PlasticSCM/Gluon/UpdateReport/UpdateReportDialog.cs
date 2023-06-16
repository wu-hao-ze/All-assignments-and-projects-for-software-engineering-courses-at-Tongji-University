using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.CM.Common;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Gluon.UpdateReport
{
    internal class UpdateReportDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 800, 400);
            }
        }

        internal static UpdateReportResult ShowUpdateReport(
            WorkspaceInfo wkInfo,
            List<ErrorMessage> errors,
            EditorWindow parentWindow)
        {
            UpdateReportDialog dialog = Create(wkInfo, errors);

            ResponseType dialogResult = dialog.RunModal(parentWindow);

            UpdateReportResult result = dialog.GetUpdateReportResult();

            result.Result = dialogResult == ResponseType.Ok;
            return result;
        }

        protected override void SaveSettings()
        {
            TreeHeaderSettings.Save(mUpdateReportListView.multiColumnHeader.state,
                UnityConstants.GLUON_UPDATE_REPORT_TABLE_SETTINGS_NAME);
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
                PlasticLocalization.Name.UpdateResultsTitle));

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.UpdateResultsExplanation));

            DoUpdateReportArea(
                mUpdateReportListView, mErrorDetailsSplitterState);

            GUILayout.Space(10);

            DoSelectAllArea();

            GUILayout.Space(20);

            DoButtonsArea(mIsUpdateForcedButtonEnabled);
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.UpdateResultsTitle);
        }

        void OnCheckedErrorChanged()
        {
            mIsUpdateForcedButtonEnabled =
                mUpdateReportListView.IsAnyErrorChecked();
            mIsSelectAllToggleChecked =
                mUpdateReportListView.AreAllErrorsChecked();
        }

        UpdateReportResult GetUpdateReportResult()
        {
            return new UpdateReportResult
            {
                UpdateForcedPaths = mUpdateReportListView.GetCheckedPaths(),
                UnaffectedErrors = mUpdateReportListView.GetUncheckedErrors()
            };
        }

        static void UpdateUpdateReportList(
            UpdateReportListView updateReportListView,
            List<ErrorMessage> errorMessages)
        {
            updateReportListView.BuildModel(errorMessages);

            updateReportListView.Reload();
        }

        static string GetErrorDetailsText(
            ErrorMessage selectedErrorMessage)
        {
            if (selectedErrorMessage == null)
                return string.Empty;

            return string.Format("{0}:{1}{2}",
                selectedErrorMessage.Path,
                Environment.NewLine,
                selectedErrorMessage.Error);
        }

        void DoUpdateReportArea(
            UpdateReportListView updateReportListView,
            object splitterState)
        {
            PlasticSplitterGUILayout.BeginHorizontalSplit(splitterState);

            DoUpdateReportViewArea(updateReportListView);

            DoErrorDetailsTextArea(updateReportListView.GetSelectedError());

            PlasticSplitterGUILayout.EndHorizontalSplit();
        }

        static void DoUpdateReportViewArea(
            UpdateReportListView updateReportListView)
        {
            Rect treeRect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);

            updateReportListView.OnGUI(treeRect);
        }

        void DoErrorDetailsTextArea(
            ErrorMessage selectedErrorMessage)
        {
            string errorDetailsText =
                GetErrorDetailsText(selectedErrorMessage);

            mErrorDetailsScrollPosition = GUILayout.BeginScrollView(
                mErrorDetailsScrollPosition);

            GUILayout.TextArea(
                errorDetailsText, UnityStyles.TextFieldWithWrapping,
                GUILayout.ExpandHeight(true));

            GUILayout.EndScrollView();
        }

        void DoSelectAllArea()
        {
            bool wasChecked = mIsSelectAllToggleChecked;

            bool isChecked = EditorGUILayout.ToggleLeft(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.SelectAll),
                wasChecked);

            if (!wasChecked && isChecked)
            {
                mIsSelectAllToggleChecked = true;
                mUpdateReportListView.CheckAllLines();
                return;
            }

            if (wasChecked && !isChecked)
            {
                mIsSelectAllToggleChecked = false;
                mUpdateReportListView.UncheckAllLines();
                return;
            }
        }

        void DoButtonsArea(bool isUpdateForcedButtonEnabled)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoUpdateForcedButton(isUpdateForcedButtonEnabled);
                    DoCloseButton();
                    return;
                }

                DoCloseButton();
                DoUpdateForcedButton(isUpdateForcedButtonEnabled);
            }
        }

        void DoUpdateForcedButton(bool isEnabled)
        {
            GUI.enabled = isEnabled;

            mEnterKeyAction = GetEnterKeyAction(isEnabled);

            bool pressed = AcceptButton(PlasticLocalization.GetString(
                PlasticLocalization.Name.UpdateForced));

            GUI.enabled = true;

            if (!pressed)
                return;

            OkButtonAction();
        }

        void DoCloseButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CloseButton)))
                return;

            CloseButtonAction();
        }

        Action GetEnterKeyAction(bool isEnabled)
        {
            if (isEnabled)
                return OkButtonAction;

            return null;
        }

        void BuildComponents(WorkspaceInfo wkInfo)
        {
            UpdateReportListHeaderState updateReportListHeaderState =
                UpdateReportListHeaderState.GetDefault();
            TreeHeaderSettings.Load(updateReportListHeaderState,
                UnityConstants.GLUON_UPDATE_REPORT_TABLE_SETTINGS_NAME,
                UnityConstants.UNSORT_COLUMN_ID);

            mUpdateReportListView = new UpdateReportListView(
                wkInfo, updateReportListHeaderState,
                OnCheckedErrorChanged);
            mUpdateReportListView.Reload();
        }

        static UpdateReportDialog Create(
            WorkspaceInfo wkInfo, 
            List<ErrorMessage> errors)
        {
            var instance = CreateInstance<UpdateReportDialog>();
            instance.mWkInfo = wkInfo;
            instance.mErrors = errors;
            instance.mEscapeKeyAction = instance.CloseButtonAction;

            instance.BuildComponents(instance.mWkInfo);

            instance.mErrorDetailsSplitterState = PlasticSplitterGUILayout.InitSplitterState(
                new float[] { 0.50f, 0.50f },
                new int[] { 100, 100 },
                new int[] { 100000, 100000 }
            );

            UpdateUpdateReportList(
                instance.mUpdateReportListView,
                instance.mErrors);

            return instance;
        }

        bool mIsSelectAllToggleChecked;
        bool mIsUpdateForcedButtonEnabled;
        object mErrorDetailsSplitterState;
        Vector2 mErrorDetailsScrollPosition;

        UpdateReportListView mUpdateReportListView;

        List<ErrorMessage> mErrors;
        WorkspaceInfo mWkInfo;
    }
}
