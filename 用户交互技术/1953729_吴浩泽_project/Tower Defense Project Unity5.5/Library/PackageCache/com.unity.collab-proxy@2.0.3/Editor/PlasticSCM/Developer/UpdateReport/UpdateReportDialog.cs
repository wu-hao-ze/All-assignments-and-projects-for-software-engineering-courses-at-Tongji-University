using System.Collections;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.Client.Commands;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.Update;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.Developer.UpdateReport
{
    internal class UpdateReportDialog :
        PlasticDialog,
        IUpdateReportDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 810, 385);
            }
        }

        internal static ResponseType ShowReportDialog(
            WorkspaceInfo wkInfo,
            IList reportLines,
            EditorWindow parentWindow)
        {
            UpdateReportDialog dialog = Create(
                wkInfo,
                reportLines,
                new ProgressControlsForDialogs());

            return dialog.RunModal(parentWindow);
        }

        protected override void SaveSettings()
        {
            TreeHeaderSettings.Save(mPathsListView.multiColumnHeader.state,
                UnityConstants.DEVELOPER_UPDATE_REPORT_TABLE_SETTINGS_NAME);
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(PlasticLocalization.Name.UpdateResultsTitle));

            Paragraph(PlasticLocalization.GetString(PlasticLocalization.Name.UpdateResultsError));

            DoListArea(
                mPathsListView,
                mErrorDetailsSplitterState);

            GUILayout.Space(10);

            DoSelectAllArea();

            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            DrawProgressForDialogs.For(
                mProgressControls.ProgressData);
            GUILayout.Space(4);
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            DoButtonsArea(mIsAnyLineChecked);

            mProgressControls.ForcedUpdateProgress(this);
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.UpdateResultsTitle);
        }

        void IUpdateReportDialog.Reload()
        {
            SetReportLines(mReportLines);
        }

        void OnCheckedReportLineChanged()
        {
            mIsAnyLineChecked =
                mPathsListView.IsAnyLineChecked();
            mAreAllLinesChecked =
                mPathsListView.AreAllLinesChecked();
        }

        void DoListArea(
            UpdateReportListView errorsListView,
            object splitterState)
        {
            EditorGUILayout.BeginVertical();
            PlasticSplitterGUILayout.BeginHorizontalSplit(splitterState);

            DoErrorsListViewArea(errorsListView);
            DoErrorDetailsTextArea(errorsListView.GetSelectedError());

            PlasticSplitterGUILayout.EndHorizontalSplit();
            EditorGUILayout.EndVertical();
        }

        static void DoErrorsListViewArea(
            UpdateReportListView errorsListView)
        {
            Rect treeRect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);

            errorsListView.OnGUI(treeRect);
        }

        void DoErrorDetailsTextArea(ReportLine selectedReportLine)
        {
            string errorDetailsText = selectedReportLine == null ?
                string.Empty : selectedReportLine.Message;

            EditorGUILayout.BeginVertical();

            GUILayout.Space(8);
            GUILayout.Label(PlasticLocalization.GetString(PlasticLocalization.Name.ProblemColumn));

            mErrorDetailsScrollPosition = GUILayout.BeginScrollView(
                mErrorDetailsScrollPosition);

            GUILayout.TextArea(
                errorDetailsText, UnityStyles.TextFieldWithWrapping,
                GUILayout.ExpandHeight(true));

            GUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        void DoSelectAllArea()
        {
            bool toggleValue = GUILayout.Toggle(
                mAreAllLinesChecked,
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.SelectAll));

            if (toggleValue != mAreAllLinesChecked && toggleValue)
            {
                mPathsListView.CheckAllLines();
                return;
            }

            if (toggleValue != mAreAllLinesChecked && !toggleValue)
            {
                mPathsListView.UnCheckAllLines();
                return;
            }
        }

        void DoButtonsArea(bool areUpdateButtonsEneabled)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DoRetryUpdateButton(areUpdateButtonsEneabled);
                DoUpdateForcedButton(areUpdateButtonsEneabled);

                GUILayout.FlexibleSpace();

                DoCloseButton();
            }
        }

        void DoRetryUpdateButton(bool isEnabled)
        {
            GUI.enabled = isEnabled;

            bool pressed = NormalButton(PlasticLocalization.GetString(
                PlasticLocalization.Name.RetryUpdate));

            GUI.enabled = true;

            if (!pressed)
                return;

            SelectiveUpdateOperation.SelectiveUpdate(
                mWkInfo, mReportLines, mPathsListView.GetCheckedLines(),
                UpdateFlags.None, this, mProgressControls);
        }

        void DoUpdateForcedButton(bool isEnabled)
        {
            GUI.enabled = isEnabled;

            bool pressed = NormalButton(PlasticLocalization.GetString(
                PlasticLocalization.Name.UpdateForced));

            GUI.enabled = true;

            if (!pressed)
                return;

            SelectiveUpdateOperation.SelectiveUpdate(
                mWkInfo, mReportLines, mPathsListView.GetCheckedLines(),
                UpdateFlags.Forced, this, mProgressControls);
        }

        void DoCloseButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                PlasticLocalization.Name.CloseButton)))
                return;

            CancelButtonAction();
        }

        static UpdateReportDialog Create(
            WorkspaceInfo wkInfo,
            IList reportLines,
            ProgressControlsForDialogs progressControls)
        {
            var instance = CreateInstance<UpdateReportDialog>();
            instance.mWkInfo = wkInfo;
            instance.mReportLines = reportLines;
            instance.mProgressControls = progressControls;
            instance.mEscapeKeyAction = instance.CloseButtonAction;
            instance.BuildComponents(wkInfo);
            instance.SetReportLines(reportLines);
            return instance;
        }

        void SetReportLines(IList reportLines)
        {
            mReportLines = reportLines;

            mPathsListView.BuildModel(reportLines);
            mPathsListView.Reload();
            mAreAllLinesChecked = false;
        }

        void BuildComponents(WorkspaceInfo wkInfo)
        {
            mErrorDetailsSplitterState = PlasticSplitterGUILayout.InitSplitterState(
                new float[] { 0.50f, 0.50f },
                new int[] { 100, 100 },
                new int[] { 100000, 100000 }
            );

            UpdateReportListHeaderState errorsListHeaderState =
                UpdateReportListHeaderState.GetDefault();
            TreeHeaderSettings.Load(errorsListHeaderState,
                UnityConstants.DEVELOPER_UPDATE_REPORT_TABLE_SETTINGS_NAME,
                UnityConstants.UNSORT_COLUMN_ID);

            mPathsListView = new UpdateReportListView(
                wkInfo,
                errorsListHeaderState,
                OnCheckedReportLineChanged);

            mPathsListView.Reload();
        }

        bool mIsAnyLineChecked = false;
        bool mAreAllLinesChecked = false;
        UpdateReportListView mPathsListView;

        object mErrorDetailsSplitterState;
        Vector2 mErrorDetailsScrollPosition;

        WorkspaceInfo mWkInfo;
        IList mReportLines;
        ProgressControlsForDialogs mProgressControls;
    }
}
