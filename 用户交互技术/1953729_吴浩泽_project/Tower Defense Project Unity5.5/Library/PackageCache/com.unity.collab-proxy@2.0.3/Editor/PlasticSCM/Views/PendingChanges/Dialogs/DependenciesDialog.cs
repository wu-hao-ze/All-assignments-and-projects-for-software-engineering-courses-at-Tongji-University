using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands;
using Codice.CM.Common;
using PlasticGui;
using PlasticGui.WorkspaceWindow.PendingChanges;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Views.PendingChanges.Dialogs
{
    internal class DependenciesDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 650, 430);
            }
        }

        internal static bool IncludeDependencies(
            WorkspaceInfo wkInfo,
            IList<ChangeDependencies<ChangeInfo>> changesDependencies,
            string operation,
            EditorWindow parentWindow)
        {
            DependenciesDialog dialog = Create(wkInfo, changesDependencies, operation);
            return dialog.RunModal(parentWindow) == ResponseType.Ok;
        }

        protected override void OnModalGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Title(PlasticLocalization.GetString(
                    PlasticLocalization.Name.DependenciesDialogTitle));
            }

            Paragraph(PlasticLocalization.GetString(
                PlasticLocalization.Name.DependenciesExplanation, mOperation));

            Title(PlasticLocalization.GetString(PlasticLocalization.Name.ItemColumn));

            Rect scrollWidth = GUILayoutUtility.GetRect(0, position.width, 1, 1);
            GUI.DrawTexture(
                new Rect(scrollWidth.x, scrollWidth.y, scrollWidth.width, 200),
                Texture2D.whiteTexture);

            DoDependenciesArea();

            GUILayout.Space(20);

            DoButtonsArea();
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.DependenciesDialogTitle);
        }

        void DoDependenciesArea()
        {
            // NOTE(rafa): We cannot use a tree view here because it misbehaves with the way we create the modals
            mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition, GUILayout.Height(200));

            for (int i = 0; i < mChangesDependencies.Count; i++)
            {
                var dependant = mChangesDependencies[i];
                bool isExpanded = mExpandedDependencies[i];

                isExpanded = EditorGUILayout.Foldout(
                    isExpanded,
                    ChangeInfoView.GetPathDescription(
                        mWkInfo.ClientPath, dependant.Change),
                    UnityStyles.Dialog.Foldout);

                mExpandedDependencies[i] = isExpanded;

                if (isExpanded)
                {
                    for (int j = 0; j < dependant.Dependencies.Count; j++)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(20);
                            GUILayout.Label(
                                ChangeInfoView.GetPathDescription(
                                    mWkInfo.ClientPath, dependant.Dependencies[j]),
                                UnityStyles.Paragraph);
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
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
            if (!AcceptButton(mOperation))
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

        static DependenciesDialog Create(
            WorkspaceInfo wkInfo,
            IList<ChangeDependencies<ChangeInfo>> changesDependencies,
            string operation)
        {
            var instance = CreateInstance<DependenciesDialog>();

            instance.mWkInfo = wkInfo;
            instance.mChangesDependencies = changesDependencies;
            instance.mOperation = operation;
            instance.mEnterKeyAction = instance.OkButtonAction;
            instance.mEscapeKeyAction = instance.CancelButtonAction;

            instance.mExpandedDependencies = new bool[changesDependencies.Count];
            for (int i = 0; i < changesDependencies.Count; i++)
                instance.mExpandedDependencies[i] = true;

            return instance;
        }

        bool[] mExpandedDependencies;
        Vector2 mScrollPosition;

        string mOperation;
        IList<ChangeDependencies<ChangeInfo>> mChangesDependencies;
        WorkspaceInfo mWkInfo;
    }
}

