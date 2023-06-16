using UnityEditor;
using UnityEngine;

using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Codice.CM.Client.Gui;
using Codice.Client.BaseCommands.EventTracking;
using Codice.CM.Common;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs
{
    internal class EmptyCheckinMessageDialog : PlasticDialog
    {
        internal bool UserChoseToNotDisplayWarningAgain { get; private set; }

        protected override string GetTitle()
        {
            return string.Empty;
        }

        protected override void OnModalGUI()
        {
            DoMainContentSection();

            DoCheckboxSection();

            DoButtonsArea();
        }

        void DoMainContentSection()
        {
            using (new EditorGUILayout.HorizontalScope())
            {

               DoIconArea();

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label(
                        PlasticLocalization.GetString(
                            PlasticLocalization.Name.EmptyCommentsDialogTitle),
                        UnityStyles.Dialog.MessageTitle);

                    GUILayout.Space(3f);

                    GUILayout.Label(
                        PlasticLocalization.GetString(
                            PlasticLocalization.Name.EmptyCommentsDialogContent),
                        UnityStyles.Dialog.MessageText);

                    GUILayout.Space(15f);
                }
            }
        }

        void DoCheckboxSection()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(70f);

                UserChoseToNotDisplayWarningAgain = TitleToggle(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.DoNotShowMessageAgain),
                    UserChoseToNotDisplayWarningAgain);
            }
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Space(25f);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (Application.platform == RuntimePlatform.WindowsEditor)
                    {
                        DoCheckInAnywayButton();                        
                        GUILayout.Space(13f);                        
                        DoCancelButton();
                        return;
                    }

                    DoCancelButton();                    
                    GUILayout.Space(13f);                    
                    DoCheckInAnywayButton();
                }
            }
        }

        void DoCheckInAnywayButton()
        {
            if (!AcceptButton(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinAnyway),
                30))
                return;

            if (!mSentCheckinAnywayTrackEvent)
            {
                TrackFeatureUseEvent.For(
                  PlasticGui.Plastic.API.GetRepositorySpec(mWkInfo),
                  TrackFeatureUseEvent.Features.PendingChangesCheckinDialogCheckinAnyway);

                mSentCheckinAnywayTrackEvent = true;
            }

            if (UserChoseToNotDisplayWarningAgain && !mSentCheckboxTrackEvent)
            {
                TrackFeatureUseEvent.For(
                    PlasticGui.Plastic.API.GetRepositorySpec(mWkInfo),
                    TrackFeatureUseEvent.Features.PendingChangesCheckinDialogDoNotShowMessageAgain);

                mSentCheckboxTrackEvent = true;
            }

            ApplyButtonAction();
        }

        void DoCancelButton()
        {
            if (!NormalButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CancelButton)))
                return;

            if (!mSentCancelTrackEvent)
            {
                TrackFeatureUseEvent.For(
                    PlasticGui.Plastic.API.GetRepositorySpec(mWkInfo),
                    TrackFeatureUseEvent.Features.PendingChangesCheckinDialogCancel);

                mSentCancelTrackEvent = true;
            }

            CancelButtonAction();
        }

        void DoIconArea()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);

            Rect iconRect = GUILayoutUtility.GetRect(
                GUIContent.none, GUIStyle.none,
                GUILayout.Width(60), GUILayout.Height(60));
            iconRect.x -= 10;

            GUI.DrawTexture(
                iconRect,
                Images.GetPlasticIcon(),
                ScaleMode.ScaleToFit);

            GUILayout.EndVertical();
        }

        internal static bool ShouldContinueWithCheckin(
            EditorWindow parentWindow,
            WorkspaceInfo wkInfo)
        {
            var dialog = Create(wkInfo);

            // using the apply response as the 'Check In Anyway' button click
            if (dialog.RunModal(parentWindow) != ResponseType.Apply)
                return false;

            if (dialog.UserChoseToNotDisplayWarningAgain)
            {
                var guiClientConfig = GuiClientConfig.Get();

                guiClientConfig.Configuration.ShowEmptyCommentWarning = false;

                guiClientConfig.Save();
            }

            return true;
        }

        static EmptyCheckinMessageDialog Create(WorkspaceInfo wkInfo)
        {
            var instance = CreateInstance<EmptyCheckinMessageDialog>();
            instance.mEnterKeyAction = instance.OkButtonAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            instance.mWkInfo = wkInfo;

            return instance;
        }

        WorkspaceInfo mWkInfo;

        // IMGUI evaluates every frame, need to make sure feature tracks get sent only once
        bool mSentCheckinAnywayTrackEvent = false;
        bool mSentCancelTrackEvent = false;
        bool mSentCheckboxTrackEvent = false;
    }
}