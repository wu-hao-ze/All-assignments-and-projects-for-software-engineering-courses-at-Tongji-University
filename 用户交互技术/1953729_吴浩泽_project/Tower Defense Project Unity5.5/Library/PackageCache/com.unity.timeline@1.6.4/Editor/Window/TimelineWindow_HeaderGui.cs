using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    partial class TimelineWindow
    {
        static readonly GUIContent[] k_TimeReferenceGUIContents =
        {
            L10n.TextContent("Local", "Display time based on the current timeline."),
            L10n.TextContent("Global", "Display time based on the master timeline.")
        };

        TimelineMarkerHeaderGUI m_MarkerHeaderGUI;

        void MarkerHeaderGUI()
        {
            var timelineAsset = state.editSequence.asset;
            if (timelineAsset == null)
                return;

            if (m_MarkerHeaderGUI == null)
                m_MarkerHeaderGUI = new TimelineMarkerHeaderGUI(timelineAsset, state);
            m_MarkerHeaderGUI.Draw(markerHeaderRect, markerContentRect, state);
        }

        void DrawTransportToolbar()
        {
            using (new EditorGUI.DisabledScope(currentMode.PreviewState(state) == TimelineModeGUIState.Disabled))
            {
                PreviewModeButtonGUI();
            }

            using (new EditorGUI.DisabledScope(currentMode.ToolbarState(state) == TimelineModeGUIState.Disabled))
            {
                GotoBeginingSequenceGUI();
                PreviousEventButtonGUI();
                PlayButtonGUI();
                NextEventButtonGUI();
                GotoEndSequenceGUI();
                PlayRangeButtonGUI();
                TimeCodeGUI();
                ReferenceTimeGUI();
            }
        }

        void PreviewModeButtonGUI()
        {
            if (state.ignorePreview && !Application.isPlaying)
            {
                GUILayout.Label(DirectorStyles.previewDisabledContent, DirectorStyles.Instance.previewButtonDisabled);
                return;
            }

            EditorGUI.BeginChangeCheck();
            var enabled = state.previewMode;
            enabled = GUILayout.Toggle(enabled, DirectorStyles.previewContent, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                // turn off auto play as well, so it doesn't auto reenable
                if (!enabled)
                {
                    state.SetPlaying(false);
                    state.recording = false;
                }

                state.previewMode = enabled;

                // if we are successfully enabled, rebuild the graph so initial states work correctly
                // Note: testing both values because previewMode setter can "fail"
                if (enabled && state.previewMode)
                    state.rebuildGraph = true;
            }
        }

        void GotoBeginingSequenceGUI()
        {
            if (GUILayout.Button(DirectorStyles.gotoBeginingContent, EditorStyles.toolbarButton))
            {
                state.editSequence.time = 0;
                state.EnsurePlayHeadIsVisible();
            }
        }

        // in the editor the play button starts/stops simulation
        void PlayButtonGUIEditor()
        {
            EditorGUI.BeginChangeCheck();
            var isPlaying = GUILayout.Toggle(state.playing, DirectorStyles.playContent, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                state.SetPlaying(isPlaying);
            }
        }

        // in playmode the button reflects the playing state.
        //  needs to disabled if playing is not possible
        void PlayButtonGUIPlayMode()
        {
            bool buttonEnabled = state.masterSequence.director != null &&
                state.masterSequence.director.isActiveAndEnabled;
            using (new EditorGUI.DisabledScope(!buttonEnabled))
            {
                PlayButtonGUIEditor();
            }
        }

        void PlayButtonGUI()
        {
            if (!Application.isPlaying)
                PlayButtonGUIEditor();
            else
                PlayButtonGUIPlayMode();
        }

        void NextEventButtonGUI()
        {
            if (GUILayout.Button(DirectorStyles.nextFrameContent, EditorStyles.toolbarButton))
            {
                state.referenceSequence.frame += 1;
            }
        }

        void PreviousEventButtonGUI()
        {
            if (GUILayout.Button(DirectorStyles.previousFrameContent, EditorStyles.toolbarButton))
            {
                state.referenceSequence.frame -= 1;
            }
        }

        void GotoEndSequenceGUI()
        {
            if (GUILayout.Button(DirectorStyles.gotoEndContent, EditorStyles.toolbarButton))
            {
                state.editSequence.time = state.editSequence.asset.duration;
                state.EnsurePlayHeadIsVisible();
            }
        }

        void PlayRangeButtonGUI()
        {
            using (new EditorGUI.DisabledScope(state.ignorePreview || state.IsEditingASubTimeline()))
            {
                state.playRangeEnabled = GUILayout.Toggle(state.playRangeEnabled, DirectorStyles.Instance.playrangeContent, EditorStyles.toolbarButton);
            }
        }

        void AddButtonGUI()
        {
            if (currentMode.trackOptionsState.newButton == TimelineModeGUIState.Hidden)
                return;

            using (new EditorGUI.DisabledScope(currentMode.trackOptionsState.newButton == TimelineModeGUIState.Disabled))
            {
                if (EditorGUILayout.DropdownButton(DirectorStyles.newContent, FocusType.Passive, EditorStyles.toolbarPopup))
                {
                    // if there is 1 and only 1 track selected, AND it's a group, add to that group
                    var groupTracks = SelectionManager.SelectedTracks().ToList();
                    if (groupTracks.Any(x => x.GetType() != typeof(GroupTrack) || x.lockedInHierarchy))
                        groupTracks = null;

                    SequencerContextMenu.ShowNewTracksContextMenu(groupTracks, state, EditorGUILayout.s_LastRect);
                }
            }
        }

        void ShowMarkersButton()
        {
            var asset = state.editSequence.asset;
            if (asset == null)
                return;

            var content = state.showMarkerHeader ? DirectorStyles.showMarkersOn : DirectorStyles.showMarkersOff;
            SetShowMarkerHeader(GUILayout.Toggle(state.showMarkerHeader, content, DirectorStyles.Instance.showMarkersBtn));
        }

        internal void SetShowMarkerHeader(bool newValue)
        {
            TimelineAsset asset = state.editSequence.asset;
            if (state.showMarkerHeader == newValue || asset == null)
                return;

            string undoOperation = L10n.Tr("Toggle Show Markers");
            if (newValue)
            {
                //Create the marker track if it does not exist
                TimelineUndo.PushUndo(asset, undoOperation);
                asset.CreateMarkerTrack();
            }
            else
            {
                SelectionManager.Remove(asset.markerTrack);
            }

            asset.markerTrack.SetShowTrackMarkers(newValue);
        }

        static void EditModeToolbarGUI(TimelineMode mode)
        {
            using (new EditorGUI.DisabledScope(mode.EditModeButtonsState(instance.state) == TimelineModeGUIState.Disabled))
            {
                var editType = EditMode.editType;

                EditorGUI.BeginChangeCheck();
                var mixIcon = editType == EditMode.EditType.Mix ? DirectorStyles.mixOn : DirectorStyles.mixOff;
                GUILayout.Toggle(editType == EditMode.EditType.Mix, mixIcon, DirectorStyles.Instance.editModeBtn);
                if (EditorGUI.EndChangeCheck())
                    EditMode.editType = EditMode.EditType.Mix;

                EditorGUI.BeginChangeCheck();
                var rippleIcon = editType == EditMode.EditType.Ripple ? DirectorStyles.rippleOn : DirectorStyles.rippleOff;
                GUILayout.Toggle(editType == EditMode.EditType.Ripple, rippleIcon, DirectorStyles.Instance.editModeBtn);
                if (EditorGUI.EndChangeCheck())
                    EditMode.editType = EditMode.EditType.Ripple;

                EditorGUI.BeginChangeCheck();
                var replaceIcon = editType == EditMode.EditType.Replace ? DirectorStyles.replaceOn : DirectorStyles.replaceOff;
                GUILayout.Toggle(editType == EditMode.EditType.Replace, replaceIcon, DirectorStyles.Instance.editModeBtn);
                if (EditorGUI.EndChangeCheck())
                    EditMode.editType = EditMode.EditType.Replace;
            }
        }

        // Draws the box to enter the time field
        void TimeCodeGUI()
        {
            const string timeFieldHint = "TimelineWindow-TimeCodeGUI";

            EditorGUI.BeginChangeCheck();
            var currentTime = state.editSequence.asset != null ? TimeReferenceUtility.ToTimeString(state.editSequence.time, "0.####") : "0";
            var r = EditorGUILayout.GetControlRect(false, EditorGUI.kSingleLineHeight, EditorStyles.toolbarTextField, GUILayout.Width(WindowConstants.timeCodeWidth));
            var id = GUIUtility.GetControlID(timeFieldHint.GetHashCode(), FocusType.Keyboard, r);
            var newCurrentTime = EditorGUI.DelayedTextFieldInternal(r, id, GUIContent.none, currentTime, null, EditorStyles.toolbarTextField);

            if (EditorGUI.EndChangeCheck())
                state.editSequence.time = TimeReferenceUtility.FromTimeString(newCurrentTime);
        }

        void ReferenceTimeGUI()
        {
            if (!state.IsEditingASubTimeline())
                return;

            EditorGUI.BeginChangeCheck();
            state.timeReferenceMode = (TimeReferenceMode)EditorGUILayout.CycleButton((int)state.timeReferenceMode, k_TimeReferenceGUIContents, DirectorStyles.Instance.timeReferenceButton);
            if (EditorGUI.EndChangeCheck())
                OnTimeReferenceModeChanged();
        }

        void OnTimeReferenceModeChanged()
        {
            m_TimeAreaDirty = true;
            InitTimeAreaFrameRate();
            SyncTimeAreaShownRange();

            foreach (var inspector in InspectorWindow.GetAllInspectorWindows())
            {
                inspector.Repaint();
            }
        }

        void DrawHeaderEditButtons()
        {
            if (state.editSequence.asset == null)
                return;

            using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Width(sequenceHeaderRect.width)))
            {
                GUILayout.Space(DirectorStyles.kBaseIndent);
                AddButtonGUI();
                GUILayout.FlexibleSpace();
                EditModeToolbarGUI(currentMode);
                ShowMarkersButton();
                EditorGUILayout.Space();
            }
        }
    }
}
