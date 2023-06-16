using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityEditor.Timeline
{
    partial class TimelineWindow
    {
        public TimelineTreeViewGUI treeView { get; private set; }

        void TracksGUI(Rect clientRect, WindowState state, TimelineModeGUIState trackState)
        {
            if (Event.current.type == EventType.Repaint && treeView != null)
            {
                state.headerSpacePartitioner.Clear();
                state.spacePartitioner.Clear();
            }

            if (state.IsEditingASubTimeline() && !state.IsEditingAnEmptyTimeline())
            {
                var headerRect = clientRect;
                headerRect.width = state.sequencerHeaderWidth;
                Graphics.DrawBackgroundRect(state, headerRect);

                var clipRect = clientRect;
                clipRect.xMin = headerRect.xMax;
                Graphics.DrawBackgroundRect(state, clipRect, subSequenceMode: true);
            }
            else
            {
                Graphics.DrawBackgroundRect(state, clientRect);
            }

            if (!state.IsEditingAnEmptyTimeline())
                m_TimeArea.DrawMajorTicks(sequenceContentRect, (float)state.referenceSequence.frameRate);

            GUILayout.BeginVertical();
            {
                GUILayout.Space(5.0f);
                GUILayout.BeginHorizontal();

                if (this.state.editSequence.asset == null)
                    DrawNoSequenceGUI(state);
                else
                    DrawTracksGUI(clientRect, trackState);

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            Graphics.DrawShadow(clientRect);
        }

        void DrawNoSequenceGUI(WindowState windowState)
        {
            bool showCreateButton = false;
            var currentlySelectedGo = UnityEditor.Selection.activeObject != null ? UnityEditor.Selection.activeObject as GameObject : null;
            var textContent = DirectorStyles.noTimelineAssetSelected;
            var existingDirector = currentlySelectedGo != null ? currentlySelectedGo.GetComponent<PlayableDirector>() : null;
            var existingAsset = existingDirector != null ? existingDirector.playableAsset : null;

            if (currentlySelectedGo != null && !TimelineUtility.IsPrefabOrAsset(currentlySelectedGo) && existingAsset == null)
            {
                showCreateButton = true;
                textContent = new GUIContent(String.Format(DirectorStyles.createTimelineOnSelection.text, currentlySelectedGo.name, L10n.Tr("a Director component and a Timeline asset")));
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label(textContent);

            if (showCreateButton)
            {
                GUILayout.BeginHorizontal();
                var textSize = GUI.skin.label.CalcSize(textContent);
                GUILayout.Space((textSize.x / 2.0f) - (WindowConstants.createButtonWidth / 2.0f));
                if (GUILayout.Button(L10n.Tr("Create"), GUILayout.Width(WindowConstants.createButtonWidth)))
                {
                    var message = DirectorStyles.createNewTimelineText.text + " '" + currentlySelectedGo.name + "'";
                    var defaultName = currentlySelectedGo.name.EndsWith(DirectorStyles.newTimelineDefaultNameSuffix, StringComparison.OrdinalIgnoreCase)
                        ? currentlySelectedGo.name
                        : currentlySelectedGo.name + DirectorStyles.newTimelineDefaultNameSuffix;

                    // Use the project window path by default only if it's under the asset folder.
                    // Otherwise the saveFilePanel will reject the save (case 1289923)
                    var defaultPath = ProjectWindowUtil.GetActiveFolderPath();
                    if (!defaultPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                        defaultPath = "Assets";

                    string newSequencePath = EditorUtility.SaveFilePanelInProject(DirectorStyles.createNewTimelineText.text, defaultName, "playable", message, defaultPath);
                    if (!string.IsNullOrEmpty(newSequencePath))
                    {
                        var newAsset = TimelineUtility.CreateAndSaveTimelineAsset(newSequencePath);

                        Undo.IncrementCurrentGroup();

                        if (existingDirector == null)
                        {
                            existingDirector = Undo.AddComponent<PlayableDirector>(currentlySelectedGo);
                        }

                        existingDirector.playableAsset = newAsset;
                        SetTimeline(existingDirector);
                        windowState.previewMode = false;
                    }

                    // If we reach this point, the state of the panel has changed; skip the rest of this GUI phase
                    // Fixes: case 955831 - [OSX] NullReferenceException when creating a timeline on a selected object
                    GUIUtility.ExitGUI();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        internal List<OverlayDrawer> OverlayDrawData = new List<OverlayDrawer>();

        void DrawTracksGUI(Rect clientRect, TimelineModeGUIState trackState)
        {
            GUILayout.BeginVertical(GUILayout.Height(clientRect.height));
            if (treeView != null)
            {
                if (Event.current.type == EventType.Layout)
                {
                    OverlayDrawData.Clear();
                }

                treeView.OnGUI(clientRect);

                if (Event.current.type == EventType.Repaint)
                {
                    foreach (var overlayData in OverlayDrawData)
                    {
                        using (new GUIViewportScope(sequenceContentRect))
                            overlayData.Draw();
                    }
                }
            }
            GUILayout.EndVertical();
        }
    }
}
