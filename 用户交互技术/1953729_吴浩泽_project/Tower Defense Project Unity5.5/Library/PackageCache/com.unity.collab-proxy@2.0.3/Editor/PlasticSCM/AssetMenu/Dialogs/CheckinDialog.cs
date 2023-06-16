using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

using Codice.Client.BaseCommands.EventTracking;
using Codice.Client.Common;
using Codice.CM.Common;
using GluonGui;
using PlasticGui;
using PlasticGui.Gluon;

using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.UI.Progress;
using Unity.PlasticSCM.Editor.UI.Tree;

namespace Unity.PlasticSCM.Editor.AssetMenu.Dialogs
{
    internal class CheckinDialog : PlasticDialog
    {
        protected override Rect DefaultRect
        {
            get
            {
                var baseRect = base.DefaultRect;
                return new Rect(baseRect.x, baseRect.y, 700, 450);
            }
        }

        protected override string GetTitle()
        {
            return PlasticLocalization.GetString(
                PlasticLocalization.Name.CheckinChanges);
        }

        internal static bool CheckinPaths(
            WorkspaceInfo wkInfo,
            List<string> paths,
            IAssetStatusCache assetStatusCache,
            bool isGluonMode,
            EditorWindow parentWindow,
            IWorkspaceWindow workspaceWindow,
            ViewHost viewHost,
            GuiMessage.IGuiMessage guiMessage,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonViewSwitcher)
        {
            MetaCache metaCache = new MetaCache();
            metaCache.Build(paths);

            CheckinDialog dialog = Create(
                wkInfo,
                paths,
                assetStatusCache,
                metaCache,
                isGluonMode,
                new ProgressControlsForDialogs(),
                workspaceWindow,
                viewHost,
                guiMessage,
                mergeViewLauncher,
                gluonViewSwitcher);

            return dialog.RunModal(parentWindow) == ResponseType.Ok;
        }

        protected override void OnModalGUI()
        {
            Title(PlasticLocalization.GetString(
                PlasticLocalization.Name.CheckinComment));

            GUI.SetNextControlName(CHECKIN_TEXTAREA_NAME);

            mComment = GUILayout.TextArea(
                mComment,
                EditorStyles.textArea,
                GUILayout.MinHeight(120));

            if (!mTextAreaFocused)
            {
                EditorGUI.FocusTextInControl(CHECKIN_TEXTAREA_NAME);
                mTextAreaFocused = true;
            }

            Title(PlasticLocalization.GetString(PlasticLocalization.Name.Files));

            DoFileList(
                mWkInfo,
                mPaths,
                mAssetStatusCache,
                mMetaCache);

            DrawProgressForDialogs.For(
                mProgressControls.ProgressData);

            DoButtonsArea();

            mProgressControls.ForcedUpdateProgress(this);
        }

        void DoFileList(
            WorkspaceInfo wkInfo,
            List<string> paths,
            IAssetStatusCache assetStatusCache,
            MetaCache metaCache)
        {
            mFileListScrollPosition = GUILayout.BeginScrollView(
                mFileListScrollPosition,
                EditorStyles.helpBox,
                GUILayout.ExpandHeight(true));

            foreach (string path in paths)
            {
                if (MetaPath.IsMetaPath(path))
                    continue;

                Texture fileIcon = Directory.Exists(path) ?
                    Images.GetDirectoryIcon() :
                    Images.GetFileIcon(path);

                string label = WorkspacePath.GetWorkspaceRelativePath(
                    wkInfo.ClientPath, path);

                if (metaCache.HasMeta(path))
                    label = string.Concat(label, UnityConstants.TREEVIEW_META_LABEL);

                AssetsOverlays.AssetStatus assetStatus = 
                    assetStatusCache.GetStatus(path);

                Rect selectionRect = EditorGUILayout.GetControlRect();

                DoListViewItem(selectionRect, fileIcon, label, assetStatus);
            }

            GUILayout.EndScrollView();
        }

        void DoListViewItem(
            Rect itemRect,
            Texture fileIcon,
            string label,
            AssetsOverlays.AssetStatus statusToDraw)
        {
            Texture overlayIcon = DrawAssetOverlay.DrawOverlayIcon.
                GetOverlayIcon(statusToDraw);

            itemRect = DrawTreeViewItem.DrawIconLeft(
                itemRect,
                UnityConstants.TREEVIEW_ROW_HEIGHT,
                fileIcon,
                overlayIcon);

            GUI.Label(itemRect, label);
        }

        void DoButtonsArea()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    DoCheckinButton();
                    DoCancelButton();
                    return;
                }

                DoCancelButton();
                DoCheckinButton();
            }
        }

        void DoCheckinButton()
        {
            GUI.enabled = !string.IsNullOrEmpty(mComment) && !mIsRunningCheckin;

            try
            {
                if (!AcceptButton(PlasticLocalization.GetString(
                    PlasticLocalization.Name.CheckinButton)))
                    return;
            }
            finally
            {
                if (!mSentCheckinTrackEvent)
                {
                    TrackFeatureUseEvent.For(
                      PlasticGui.Plastic.API.GetRepositorySpec(mWkInfo),
                      TrackFeatureUseEvent.Features.ContextMenuCheckinDialogCheckin);

                    mSentCheckinTrackEvent = true;
                }

                GUI.enabled = true;
            }

            OkButtonWithCheckinAction();
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
                    TrackFeatureUseEvent.Features.ContextMenuCheckinDialogCancel);

                mSentCancelTrackEvent = true;
            }

            CancelButtonAction();
        }

        void OkButtonWithCheckinAction()
        {
            bool isCancelled;
            SaveAssets.ForPathsWithConfirmation(mPaths, out isCancelled);

            if (isCancelled)
                return;

            mIsRunningCheckin = true;

            mPaths.AddRange(mMetaCache.GetExistingMeta(mPaths));

            if (mIsGluonMode)
            {
                CheckinDialogOperations.CheckinPathsPartial(
                    mWkInfo,
                    mPaths,
                    mComment,
                    mViewHost,
                    this,
                    mGuiMessage,
                    mProgressControls,
                    mGluonViewSwitcher);
                return;
            }

            CheckinDialogOperations.CheckinPaths(
                mWkInfo,
                mPaths,
                mComment,
                mWorkspaceWindow,
                this,
                mGuiMessage,
                mProgressControls,
                mMergeViewLauncher);
        }

        static CheckinDialog Create(
            WorkspaceInfo wkInfo,
            List<string> paths,
            IAssetStatusCache assetStatusCache,
            MetaCache metaCache,
            bool isGluonMode,
            ProgressControlsForDialogs progressControls,
            IWorkspaceWindow workspaceWindow,
            ViewHost viewHost,
            GuiMessage.IGuiMessage guiMessage,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonViewSwitcher)
        {
            var instance = CreateInstance<CheckinDialog>();
            instance.IsResizable = true;
            instance.minSize = new Vector2(520, 370);
            instance.mWkInfo = wkInfo;
            instance.mPaths = paths;
            instance.mAssetStatusCache = assetStatusCache;
            instance.mMetaCache = metaCache;
            instance.mIsGluonMode = isGluonMode;
            instance.mProgressControls = progressControls;
            instance.mWorkspaceWindow = workspaceWindow;
            instance.mViewHost = viewHost;
            instance.mGuiMessage = guiMessage;
            instance.mMergeViewLauncher = mergeViewLauncher;
            instance.mGluonViewSwitcher = gluonViewSwitcher;
            instance.mEscapeKeyAction = instance.CancelButtonAction;
            return instance;
        }

        WorkspaceInfo mWkInfo;
        List<string> mPaths;
        IAssetStatusCache mAssetStatusCache;
        MetaCache mMetaCache;
        bool mIsGluonMode;
        bool mTextAreaFocused;
        string mComment;

        bool mIsRunningCheckin;
        Vector2 mFileListScrollPosition;

        // IMGUI evaluates every frame, need to make sure feature tracks get sent only once
        bool mSentCheckinTrackEvent = false;
        bool mSentCancelTrackEvent = false;

        ProgressControlsForDialogs mProgressControls;

        IWorkspaceWindow mWorkspaceWindow;
        ViewHost mViewHost;
        IMergeViewLauncher mMergeViewLauncher;
        IGluonViewSwitcher mGluonViewSwitcher;
        GuiMessage.IGuiMessage mGuiMessage;

        const string CHECKIN_TEXTAREA_NAME = "checkin_textarea";

        class MetaCache
        {
            internal bool HasMeta(string path)
            {
                return mCache.Contains(MetaPath.GetMetaPath(path));
            }

            internal List<string> GetExistingMeta(List<string> paths)
            {
                List<string> result = new List<string>();

                foreach (string path in paths)
                {
                    string metaPath = MetaPath.GetMetaPath(path);

                    if (!mCache.Contains(metaPath))
                        continue;

                    result.Add(metaPath);
                }

                return result;
            }

            internal void Build(List<string> paths)
            {
                HashSet<string> indexedKeys = BuildIndexedKeys(paths);

                for (int i = paths.Count - 1; i >= 0; i--)
                {
                    string currentPath = paths[i];

                    if (!MetaPath.IsMetaPath(currentPath))
                        continue;

                    string realPath = MetaPath.GetPathFromMetaPath(currentPath);

                    if (!indexedKeys.Contains(realPath))
                        continue;

                    // found foo.c and foo.c.meta
                    // with the same chage types - move .meta to cache
                    mCache.Add(currentPath);
                    paths.RemoveAt(i);
                }
            }

            static HashSet<string> BuildIndexedKeys(List<string> paths)
            {
                HashSet<string> result = new HashSet<string>();

                foreach (string path in paths)
                {
                    if (MetaPath.IsMetaPath(path))
                        continue;

                    result.Add(path);
                }

                return result;
            }

            HashSet<string> mCache =
                new HashSet<string>();
        }
    }
}
