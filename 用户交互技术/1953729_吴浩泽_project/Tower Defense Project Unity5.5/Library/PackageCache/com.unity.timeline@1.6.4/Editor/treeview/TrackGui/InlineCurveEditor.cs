using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    interface IClipCurveEditorOwner
    {
        ClipCurveEditor clipCurveEditor { get; }
        bool inlineCurvesSelected { get; }
        bool showLoops { get; }
        TrackAsset owner { get; }
        void SelectCurves();
        void ValidateCurvesSelection();
    }

    class InlineCurveResizeHandle : IBounds
    {
        public Rect boundingRect { get; private set; }

        public TimelineTrackGUI trackGUI { get; }

        public InlineCurveResizeHandle(TimelineTrackGUI trackGUI)
        {
            this.trackGUI = trackGUI;
        }

        public void Draw(Rect headerRect, WindowState state)
        {
            const float resizeHandleHeight = WindowConstants.trackResizeHandleHeight;
            var rect = new Rect(headerRect.xMin, headerRect.yMax - resizeHandleHeight + 1f, headerRect.width, resizeHandleHeight);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);

            boundingRect = trackGUI.ToWindowSpace(rect);

            if (Event.current.type == EventType.Repaint)
            {
                state.headerSpacePartitioner.AddBounds(this);
                EditorGUI.DrawRect(rect, DirectorStyles.Instance.customSkin.colorAnimEditorBinding);
                var dragStyle = DirectorStyles.Instance.inlineCurveHandle;
                dragStyle.Draw(rect, GUIContent.none, false, false, false, false);
            }
        }
    }

    class InlineCurveEditor : IBounds
    {
        Rect m_TrackRect;
        Rect m_HeaderRect;
        readonly TimelineTrackGUI m_TrackGUI;
        readonly InlineCurveResizeHandle m_ResizeHandle;

        bool m_LastSelectionWasClip;
        TimelineClipGUI m_LastSelectedClipGUI;

        Rect IBounds.boundingRect { get { return m_TrackGUI.ToWindowSpace(m_TrackRect); } }

        [UsedImplicitly] // Used in tests
        public TimelineClipGUI currentClipGui
        {
            get { return m_LastSelectedClipGUI; }
        }

        public IClipCurveEditorOwner currentCurveEditor
        {
            get { return m_LastSelectionWasClip ? (IClipCurveEditorOwner)m_LastSelectedClipGUI : (IClipCurveEditorOwner)m_TrackGUI; }
        }

        public InlineCurveEditor(TimelineTrackGUI trackGUI)
        {
            m_TrackGUI = trackGUI;
            m_ResizeHandle = new InlineCurveResizeHandle(trackGUI);
        }

        static bool MouseOverTrackArea(Rect curveRect, Rect trackRect)
        {
            curveRect.y = trackRect.y;
            curveRect.height = trackRect.height;

            // clamp the curve editor to the track. this allows the menu to scroll properly
            curveRect.xMin = Mathf.Max(curveRect.xMin, trackRect.xMin);
            curveRect.xMax = trackRect.xMax;

            return curveRect.Contains(Event.current.mousePosition);
        }

        static bool MouseOverHeaderArea(Rect headerRect, Rect trackRect)
        {
            headerRect.y = trackRect.y;
            headerRect.height = trackRect.height;

            return headerRect.Contains(Event.current.mousePosition);
        }

        static void DrawCurveEditor(IClipCurveEditorOwner clipCurveEditorOwner, WindowState state, Rect headerRect, Rect trackRect, Vector2 activeRange, bool locked)
        {
            ClipCurveEditor clipCurveEditor = clipCurveEditorOwner.clipCurveEditor;
            CurveDataSource dataSource = clipCurveEditor.dataSource;
            Rect curveRect = dataSource.GetBackgroundRect(state);

            var newlySelected = false;
            var currentEvent = Event.current;

            if (currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.ContextClick)
                newlySelected = MouseOverTrackArea(curveRect, trackRect) || MouseOverHeaderArea(headerRect, trackRect);

            // make sure to not use any event before drawing the curve.
            bool prevEnabledState = GUI.enabled;
            GUI.enabled = true;
            clipCurveEditorOwner.clipCurveEditor.DrawHeader(headerRect);
            GUI.enabled = prevEnabledState;

            bool displayAsSelected = !locked && (clipCurveEditorOwner.inlineCurvesSelected || newlySelected);

            using (new EditorGUI.DisabledScope(locked))
                clipCurveEditor.DrawCurveEditor(trackRect, state, activeRange, clipCurveEditorOwner.showLoops, displayAsSelected);

            if (newlySelected && !locked)
                OnMouseClick(clipCurveEditorOwner, currentEvent);
        }

        static void OnMouseClick(IClipCurveEditorOwner clipCurveEditorOwner, Event currentEvent)
        {
            if (currentEvent.modifiers == ManipulatorsUtils.actionModifier)
            {
                if (clipCurveEditorOwner.inlineCurvesSelected)
                    SelectionManager.Clear();
                else
                    clipCurveEditorOwner.SelectCurves();
            }
            else
            {
                clipCurveEditorOwner.SelectCurves();
            }

            HandleCurrentEvent();
        }

        public void Draw(Rect headerRect, Rect trackRect, WindowState state)
        {
            const float inlineCurveBottomPadding = WindowConstants.inlineCurveContentPadding;
            m_TrackRect = trackRect;
            m_TrackRect.height -= inlineCurveBottomPadding;

            if (Event.current.type == EventType.Repaint)
                state.spacePartitioner.AddBounds(this);

            // Remove the indentation of this track to render it properly, otherwise every GUI elements will be offsetted.
            headerRect.x -= DirectorStyles.kBaseIndent;
            headerRect.width += DirectorStyles.kBaseIndent;

            // Remove the width of the color swatch.
            headerRect.x += 4.0f;
            headerRect.width -= 4.0f;
            m_HeaderRect = headerRect;
            EditorGUI.DrawRect(m_HeaderRect, DirectorStyles.Instance.customSkin.colorAnimEditorBinding);

            if (ShouldShowClipCurves(state))
            {
                DrawCurveEditorsForClipsOnTrack(m_HeaderRect, m_TrackRect, state);
            }
            else if (ShouldShowTrackCurves())
            {
                DrawCurveEditorForTrack(m_HeaderRect, m_TrackRect, state);
            }
            else
            {
                DrawCurvesEditorForNothingSelected(m_HeaderRect, m_TrackRect, state);
            }

            m_ResizeHandle.Draw(headerRect, state);

            var bottomPadding = new Rect(trackRect.xMin, trackRect.yMax - inlineCurveBottomPadding, trackRect.width, inlineCurveBottomPadding);
            EditorGUI.DrawRect(bottomPadding, DirectorStyles.Instance.customSkin.colorTrackBackground);

            // If MouseDown or ContextClick are not consumed by the curves, use the event to prevent it from going deeper into the treeview.
            if (Event.current.type == EventType.ContextClick)
            {
                var r = Rect.MinMaxRect(m_HeaderRect.xMin, m_HeaderRect.yMin, m_TrackRect.xMax, m_TrackRect.yMax);
                if (r.Contains(Event.current.mousePosition))
                    Event.current.Use();
            }

            UpdateViewModel();
        }

        void DrawCurveEditorForTrack(Rect headerRect, Rect trackRect, WindowState state)
        {
            if (m_TrackGUI.clipCurveEditor == null)
                return;

            var activeRange = new Vector2(state.TimeToPixel(0.0d), state.TimeToPixel(state.editSequence.duration));
            DrawCurveEditor(m_TrackGUI, state, headerRect, trackRect, activeRange, m_TrackGUI.locked);
            m_LastSelectionWasClip = false;
        }

        void DrawCurveEditorsForClipsOnTrack(Rect headerRect, Rect trackRect, WindowState state)
        {
            if (m_TrackGUI.clips.Count == 0)
                return;

            if (Event.current.type == EventType.Layout)
            {
                var selectedClip = SelectionManager.SelectedClipGUI().FirstOrDefault(x => x.parent == m_TrackGUI);
                if (selectedClip != null)
                {
                    m_LastSelectedClipGUI = selectedClip;
                    SelectFromCurveOwner(m_LastSelectedClipGUI);
                }
                else if (state.recording && state.IsArmedForRecord(m_TrackGUI.track))
                {
                    if (m_LastSelectedClipGUI == null || !m_TrackGUI.track.IsRecordingToClip(m_LastSelectedClipGUI.clip))
                    {
                        var clip = m_TrackGUI.clips.FirstOrDefault(x => m_TrackGUI.track.IsRecordingToClip(x.clip));
                        if (clip != null)
                            m_LastSelectedClipGUI = clip;
                    }
                }

                if (m_LastSelectedClipGUI == null)
                    m_LastSelectedClipGUI = m_TrackGUI.clips[0];
            }

            if (m_LastSelectedClipGUI == null || m_LastSelectedClipGUI.clipCurveEditor == null || m_LastSelectedClipGUI.isInvalid)
                return;

            var activeRange = new Vector2(state.TimeToPixel(m_LastSelectedClipGUI.clip.start), state.TimeToPixel(m_LastSelectedClipGUI.clip.end));
            DrawCurveEditor(m_LastSelectedClipGUI, state, headerRect, trackRect, activeRange, m_TrackGUI.locked);
            m_LastSelectionWasClip = true;
        }

        void DrawCurvesEditorForNothingSelected(Rect headerRect, Rect trackRect, WindowState state)
        {
            if (m_LastSelectionWasClip || !TrackHasCurvesToShow() && m_TrackGUI.clips.Count > 0)
            {
                DrawCurveEditorsForClipsOnTrack(headerRect, trackRect, state);
            }
            else
            {
                DrawCurveEditorForTrack(headerRect, trackRect, state);
            }
        }

        bool ShouldShowClipCurves(WindowState state)
        {
            if (m_TrackGUI.clips.Count == 0)
                return false;

            // Is a clip selected or being recorded to?
            return SelectionManager.SelectedClipGUI().FirstOrDefault(x => x.parent == m_TrackGUI) != null ||
                state.recording && state.IsArmedForRecord(m_TrackGUI.track) && m_TrackGUI.clips.FirstOrDefault(x => m_TrackGUI.track.IsRecordingToClip(x.clip)) != null;
        }

        bool ShouldShowTrackCurves()
        {
            if (m_TrackGUI == null)
                return false;

            var isTrackSelected = SelectionManager.SelectedTrackGUI().FirstOrDefault(x => x == m_TrackGUI) != null;

            if (!isTrackSelected)
                return false;

            return TrackHasCurvesToShow();
        }

        bool TrackHasCurvesToShow()
        {
            var animTrack = m_TrackGUI.track as AnimationTrack;
            if (animTrack != null && !animTrack.inClipMode)
                return true;

            return m_TrackGUI.track.HasAnyAnimatableParameters();
        }

        void UpdateViewModel()
        {
            var curveEditor = currentCurveEditor.clipCurveEditor;
            if (curveEditor == null || curveEditor.bindingHierarchy.treeViewController == null)
                return;

            var vm = TimelineWindowViewPrefs.GetTrackViewModelData(m_TrackGUI.track);
            vm.inlineCurvesState = curveEditor.bindingHierarchy.treeViewController.state;
            vm.inlineCurvesShownAreaInsideMargins = curveEditor.shownAreaInsideMargins;
            vm.lastInlineCurveDataID = curveEditor.dataSource.id;
        }

        static void HandleCurrentEvent()
        {
#if UNITY_EDITOR_OSX
            Event.current.type = EventType.Ignore;
#else
            Event.current.Use();
#endif
        }

        static void SelectFromCurveOwner(IClipCurveEditorOwner curveOwner)
        {
            if (curveOwner.clipCurveEditor == null)
            {
                SelectionManager.SelectInlineCurveEditor(null);
            }
            else if (!curveOwner.inlineCurvesSelected && SelectionManager.Count() == 1)
            {
                SelectionManager.SelectInlineCurveEditor(curveOwner);
            }
        }
    }
}
