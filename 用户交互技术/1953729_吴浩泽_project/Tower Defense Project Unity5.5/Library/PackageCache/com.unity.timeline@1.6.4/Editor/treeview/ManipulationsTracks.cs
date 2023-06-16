using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    class InlineCurveResize : Manipulator
    {
        bool m_Captured;

        float m_CapturedHeight;
        float m_CaptureMouseYPos;

        InlineCurveResizeHandle m_Target;

        protected override bool MouseDown(Event evt, WindowState state)
        {
            m_Target = PickerUtils.FirstPickedElementOfType<InlineCurveResizeHandle>();
            if (m_Target == null)
                return false;

            m_Captured = true;
            m_CapturedHeight = TimelineWindowViewPrefs.GetInlineCurveHeight(m_Target.trackGUI.track);
            m_CaptureMouseYPos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition).y;
            state.AddCaptured(this);

            return true;
        }

        protected override bool MouseDrag(Event evt, WindowState state)
        {
            if (!m_Captured || m_Target == null)
                return false;

            var trackGUI = m_Target.trackGUI;

            float inlineTrackHeight = m_CapturedHeight +
                (GUIUtility.GUIToScreenPoint(Event.current.mousePosition).y - m_CaptureMouseYPos);

            TimelineWindowViewPrefs.SetInlineCurveHeight(trackGUI.track, Mathf.Max(inlineTrackHeight, 60.0f));

            state.GetWindow().treeView.CalculateRowRects();

            return true;
        }

        protected override bool MouseUp(Event evt, WindowState state)
        {
            if (!m_Captured)
                return false;

            state.RemoveCaptured(this);
            m_Captured = false;

            return true;
        }

        public override void Overlay(Event evt, WindowState state)
        {
            var rect = state.GetWindow().sequenceRect;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);
        }
    }

    class TrackResize : Manipulator
    {
        bool m_Captured;
        int m_NumberOfContributingTracks;

        TrackResizeHandle m_Target;
        List<TimelineTrackGUI> m_TracksToResize;

        protected override bool MouseDown(Event evt, WindowState state)
        {
            m_Target = PickerUtils.FirstPickedElementOfType<TrackResizeHandle>();
            if (m_Target == null)
                return false;

            m_NumberOfContributingTracks = 1;
            var selectedTracks = SelectionManager.SelectedTrackGUI().ToList();

            if (selectedTracks.Any() && selectedTracks.Contains(m_Target.trackGUI)) //resize all selected tracks
            {
                var allTrackGui = state.GetWindow().treeView.allTrackGuis;
                m_TracksToResize = allTrackGui.OfType<TimelineTrackGUI>().Where(i => SelectionManager.Contains(i.track)).ToList();
                m_NumberOfContributingTracks += m_TracksToResize.IndexOf(m_Target.trackGUI);
            }
            else
                m_TracksToResize = new List<TimelineTrackGUI> { m_Target.trackGUI };

            m_Captured = true;
            state.AddCaptured(this);

            return true;
        }

        protected override bool MouseDrag(Event evt, WindowState state)
        {
            if (!m_Captured || m_Target == null)
                return false;

            var delta = evt.mousePosition.y - m_Target.boundingRect.center.y;
            var extension = Mathf.RoundToInt(delta / m_NumberOfContributingTracks / state.trackScale);
            foreach (var track in m_TracksToResize)
                track.heightExtension += extension;

            state.GetWindow().treeView.CalculateRowRects();
            return true;
        }

        protected override bool MouseUp(Event evt, WindowState state)
        {
            if (!m_Captured)
                return false;

            foreach (var track in m_TracksToResize)
                CommitExtension(track);

            state.GetWindow().treeView.CalculateRowRects();
            state.RemoveCaptured(this);
            m_Captured = false;

            return true;
        }

        public override void Overlay(Event evt, WindowState state)
        {
            var rect = state.GetWindow().sequenceRect;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);
        }

        static void CommitExtension(TimelineTrackGUI trackGUI)
        {
            if (trackGUI != null)
                TimelineWindowViewPrefs.SetTrackHeightExtension(trackGUI.track, trackGUI.heightExtension);
        }
    }

    class TrackDoubleClick : Manipulator
    {
        protected override bool DoubleClick(Event evt, WindowState state)
        {
            if (evt.button != 0)
                return false;

            var trackGUI = PickerUtils.FirstPickedElementOfType<TimelineTrackBaseGUI>();

            if (trackGUI == null)
                return false;

            // Double-click is only available for AnimationTracks: it conflicts with selection mechanics on other tracks
            if ((trackGUI.track as AnimationTrack) == null)
                return false;

            return EditTrackInAnimationWindow.Do(trackGUI.track);
        }
    }

    class TrackShortcutManipulator : Manipulator
    {
        protected override bool KeyDown(Event evt, WindowState state)
        {
            return InternalExecute(evt, state);
        }

        protected override bool ExecuteCommand(Event evt, WindowState state)
        {
            return InternalExecute(evt, state);
        }

        static bool InternalExecute(Event evt, WindowState state)
        {
            if (state.IsCurrentEditingASequencerTextField())
                return false;

            var tracks = SelectionManager.SelectedTracks().ToList();
            var items = SelectionManager.SelectedClipGUI();

            foreach (var item in items)
            {
                var trackGUI = item.parent as TimelineTrackBaseGUI;
                if (trackGUI == null)
                    continue;

                if (!tracks.Contains(trackGUI.track))
                    tracks.Add(trackGUI.track);
            }

            return ActionManager.HandleShortcut(evt,
                ActionManager.TrackActions,
                x => ActionManager.ExecuteTrackAction(x, tracks));
        }
    }
}
