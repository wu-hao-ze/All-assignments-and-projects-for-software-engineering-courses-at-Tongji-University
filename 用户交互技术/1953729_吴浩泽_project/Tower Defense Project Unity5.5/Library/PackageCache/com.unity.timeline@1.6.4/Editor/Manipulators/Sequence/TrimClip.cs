using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    class TrimClip : Manipulator
    {
        private readonly string kDurationText = L10n.Tr("Duration:");
        private readonly string kSpeedText = L10n.Tr("Speed:");

        class TrimClipAttractionHandler : IAttractionHandler
        {
            public void OnAttractedEdge(IAttractable attractable, ManipulateEdges manipulateEdges, AttractedEdge edge, double time)
            {
                var clipGUI = attractable as TimelineClipGUI;
                if (clipGUI == null)
                    return;

                var clipItem = ItemsUtils.ToItem(clipGUI.clip);
                if (manipulateEdges == ManipulateEdges.Right)
                {
                    bool affectTimeScale = IsAffectingTimeScale(clipGUI.clip);
                    EditMode.TrimEnd(clipItem, time, affectTimeScale);
                }
                else if (manipulateEdges == ManipulateEdges.Left)
                {
                    bool affectTimeScale = IsAffectingTimeScale(clipGUI.clip);
                    EditMode.TrimStart(clipItem, time, affectTimeScale);
                }
            }

            private bool IsAffectingTimeScale(TimelineClip clip)
            {
                bool autoScale = (clip.clipCaps & ClipCaps.AutoScale) == ClipCaps.AutoScale;

                // TODO Do not use Event.current from here.
                bool affectTimeScale = (autoScale && (Event.current.modifiers != EventModifiers.Shift))
                    || (!autoScale && (Event.current.modifiers == EventModifiers.Shift));
                return affectTimeScale;
            }
        }

        bool m_IsCaptured;
        TimelineClipHandle m_TrimClipHandler;

        double m_OriginalDuration;
        double m_OriginalTimeScale;
        double m_OriginalEaseInDuration;
        double m_OriginalEaseOutDuration;

        bool m_UndoSaved;
        SnapEngine m_SnapEngine;

        readonly List<string> m_OverlayStrings = new List<string>();

        static readonly double kEpsilon = 0.0000001;

        protected override bool MouseDown(Event evt, WindowState state)
        {
            var handle = PickerUtils.TopmostPickedItem() as TimelineClipHandle;
            if (handle == null)
                return false;

            if (handle.clipGUI.clip.GetParentTrack() != null && handle.clipGUI.clip.GetParentTrack().lockedInHierarchy)
                return false;

            m_TrimClipHandler = handle;

            m_IsCaptured = true;
            state.AddCaptured(this);

            m_UndoSaved = false;

            var clip = m_TrimClipHandler.clipGUI.clip;

            m_OriginalDuration = clip.duration;
            m_OriginalTimeScale = clip.timeScale;
            m_OriginalEaseInDuration = clip.easeInDuration;
            m_OriginalEaseOutDuration = clip.easeOutDuration;

            RefreshOverlayStrings(m_TrimClipHandler, state);

            // in ripple trim, the right edge moves and needs to snap
            var edges = ManipulateEdges.Right;
            if (EditMode.editType != EditMode.EditType.Ripple && m_TrimClipHandler.trimDirection == TrimEdge.Start)
                edges = ManipulateEdges.Left;
            m_SnapEngine = new SnapEngine(m_TrimClipHandler.clipGUI, new TrimClipAttractionHandler(), edges, state,
                evt.mousePosition);

            EditMode.BeginTrim(ItemsUtils.ToItem(clip), m_TrimClipHandler.trimDirection);

            return true;
        }

        protected override bool MouseUp(Event evt, WindowState state)
        {
            if (!m_IsCaptured)
                return false;

            m_IsCaptured = false;
            m_TrimClipHandler = null;
            m_UndoSaved = false;
            m_SnapEngine = null;
            EditMode.FinishTrim();

            state.captured.Clear();

            return true;
        }

        protected override bool MouseDrag(Event evt, WindowState state)
        {
            if (state.editSequence.isReadOnly)
                return false;

            if (!m_IsCaptured)
                return false;

            var uiClip = m_TrimClipHandler.clipGUI;
            if (!m_UndoSaved)
            {
                UndoExtensions.RegisterClip(uiClip.clip, L10n.Tr("Trim Clip"));
                if (TimelineUtility.IsRecordableAnimationClip(uiClip.clip))
                {
                    TimelineUndo.PushUndo(uiClip.clip.animationClip, L10n.Tr("Trim Clip"));
                }

                m_UndoSaved = true;
            }

            //Reset to original ease values. The trim operation will calculate the proper blend values.
            uiClip.clip.easeInDuration = m_OriginalEaseInDuration;
            uiClip.clip.easeOutDuration = m_OriginalEaseOutDuration;

            if (m_SnapEngine != null)
                m_SnapEngine.Snap(evt.mousePosition, evt.modifiers);

            RefreshOverlayStrings(m_TrimClipHandler, state);

            if (Selection.activeObject != null)
                EditorUtility.SetDirty(Selection.activeObject);

            // updates the duration of the graph without rebuilding
            state.UpdateRootPlayableDuration(state.editSequence.duration);

            return true;
        }

        public override void Overlay(Event evt, WindowState state)
        {
            if (!m_IsCaptured)
                return;

            EditMode.DrawTrimGUI(state, m_TrimClipHandler.clipGUI, m_TrimClipHandler.trimDirection);

            bool trimStart = m_TrimClipHandler.trimDirection == TrimEdge.Start;

            TimeIndicator.Draw(state, trimStart ? m_TrimClipHandler.clipGUI.start : m_TrimClipHandler.clipGUI.end);

            if (m_SnapEngine != null)
                m_SnapEngine.OnGUI(trimStart, !trimStart);

            if (m_OverlayStrings.Count > 0)
            {
                const float padding = 4.0f;
                var labelStyle = TimelineWindow.styles.tinyFont;
                var longestLine = labelStyle.CalcSize(
                    new GUIContent(m_OverlayStrings.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur)));
                var stringLength = longestLine.x + padding;
                var lineHeight = longestLine.y + padding;

                var r = new Rect(evt.mousePosition.x - (stringLength / 2.0f),
                    m_TrimClipHandler.clipGUI.rect.yMax,
                    stringLength, lineHeight);

                foreach (var s in m_OverlayStrings)
                {
                    GUI.Label(r, s, labelStyle);
                    r.y += lineHeight;
                }
            }
        }

        void RefreshOverlayStrings(TimelineClipHandle handle, WindowState state)
        {
            m_OverlayStrings.Clear();

            var differenceDuration = handle.clipGUI.clip.duration - m_OriginalDuration;
            m_OverlayStrings.Add($"{kDurationText} {state.timeFormat.ToTimeStringWithDelta(handle.clipGUI.clip.duration, state.referenceSequence.frameRate, differenceDuration)}");

            var differenceSpeed = m_OriginalTimeScale - handle.clipGUI.clip.timeScale;
            if (Math.Abs(differenceSpeed) > kEpsilon)
            {
                var sign = differenceSpeed > 0 ? "+" : "";
                var timeScale = handle.clipGUI.clip.timeScale.ToString("f2");
                var deltaSpeed = differenceSpeed.ToString("p2");
                m_OverlayStrings.Add($"{kSpeedText} {timeScale} ({sign}{deltaSpeed}) ");
            }
        }
    }
}
