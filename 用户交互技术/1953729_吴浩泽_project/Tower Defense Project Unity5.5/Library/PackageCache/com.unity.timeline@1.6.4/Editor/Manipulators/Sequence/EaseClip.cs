using System;
using System.Text;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    class EaseClip : Manipulator
    {
        bool m_IsCaptured;
        bool m_UndoSaved;
        TimelineClipHandle m_EaseClipHandler;
        ManipulateEdges m_Edges;
        TimelineClip m_Clip;
        StringBuilder m_OverlayText = new StringBuilder("");
        double m_OriginalValue;

        public static readonly string EaseInClipText = L10n.Tr("Ease In Clip");
        public static readonly string EaseOutClipText = L10n.Tr("Ease Out Clip");
        public static readonly string EaseInText = L10n.Tr("Ease In");
        public static readonly string EaseOutText = L10n.Tr("Ease Out");
        public static readonly string DurationText = L10n.Tr("Duration: ");

        protected override bool MouseDown(Event evt, WindowState state)
        {
            if (evt.modifiers != ManipulatorsUtils.actionModifier)
                return false;
            return MouseDownInternal(evt, state, PickerUtils.TopmostPickedItem() as TimelineClipHandle);
        }

        protected bool MouseDownInternal(Event evt, WindowState state, TimelineClipHandle handle)
        {
            if (handle == null)
                return false;

            if (handle.clipGUI.clip != null && !handle.clipGUI.clip.clipCaps.HasAny(ClipCaps.Blending))
                return false;

            m_Edges = ManipulateEdges.Right;
            if (handle.trimDirection == TrimEdge.Start)
                m_Edges = ManipulateEdges.Left;

            if (m_Edges == ManipulateEdges.Left && handle.clipGUI.clip.hasBlendIn || m_Edges == ManipulateEdges.Right && handle.clipGUI.clip.hasBlendOut)
                return false;

            m_IsCaptured = true;
            m_UndoSaved = false;

            m_EaseClipHandler = handle;
            m_Clip = handle.clipGUI.clip;
            m_OriginalValue = m_Edges == ManipulateEdges.Left ? m_Clip.easeInDuration : m_Clip.easeOutDuration;


            // Change cursor only when OnGUI Process (not in test)
            if (GUIUtility.guiDepth > 0)
                TimelineCursors.SetCursor(m_Edges == ManipulateEdges.Left ? TimelineCursors.CursorType.MixRight : TimelineCursors.CursorType.MixLeft);

            state.AddCaptured(this);
            return true;
        }

        protected override bool MouseUp(Event evt, WindowState state)
        {
            if (!m_IsCaptured)
                return false;
            m_IsCaptured = false;
            m_UndoSaved = false;
            state.captured.Clear();

            // Clear cursor only when OnGUI Process (not in test)
            if (GUIUtility.guiDepth > 0)
                TimelineCursors.ClearCursor();

            return true;
        }

        protected override bool MouseDrag(Event evt, WindowState state)
        {
            if (!m_IsCaptured)
                return false;
            if (!m_UndoSaved)
            {
                var uiClip = m_EaseClipHandler.clipGUI;
                string undoName = m_Edges == ManipulateEdges.Left ? EaseInClipText : EaseOutClipText;
                UndoExtensions.RegisterClip(uiClip.clip, undoName);
                m_UndoSaved = true;
            }

            double d = state.PixelDeltaToDeltaTime(evt.delta.x);

            var duration = m_Clip.duration;
            var easeInDurationLimit = duration - m_Clip.easeOutDuration;
            var easeOutDurationLimit = duration - m_Clip.easeInDuration;

            if (m_Edges == ManipulateEdges.Left)
            {
                m_Clip.easeInDuration = Math.Min(easeInDurationLimit, Math.Max(0, m_Clip.easeInDuration + d));
            }
            else if (m_Edges == ManipulateEdges.Right)
            {
                m_Clip.easeOutDuration = Math.Min(easeOutDurationLimit, Math.Max(0, m_Clip.easeOutDuration - d));
            }
            RefreshOverlayStrings(m_EaseClipHandler, state);
            return true;
        }

        public override void Overlay(Event evt, WindowState state)
        {
            if (!m_IsCaptured)
                return;
            if (m_OverlayText.Length > 0)
            {
                int stringLength = m_OverlayText.Length;
                var r = new Rect(evt.mousePosition.x - (stringLength / 2.0f),
                    m_EaseClipHandler.clipGUI.rect.yMax,
                    stringLength, 20);
                GUI.Label(r, m_OverlayText.ToString(), TimelineWindow.styles.tinyFont);
            }
        }

        void RefreshOverlayStrings(TimelineClipHandle handle, WindowState state)
        {
            m_OverlayText.Length = 0;
            m_OverlayText.Append(m_Edges == ManipulateEdges.Left ? EaseInText : EaseOutText);

            var easeDuration = m_Edges == ManipulateEdges.Left ? m_Clip.easeInDuration : m_Clip.easeOutDuration;
            var deltaDuration = easeDuration - m_OriginalValue;

            // round to frame so we don't show partial time codes due to no frame snapping
            if (state.timeFormat == TimeFormat.Timecode)
            {
                easeDuration = TimeUtility.RoundToFrame(easeDuration, state.referenceSequence.frameRate);
                deltaDuration = TimeUtility.RoundToFrame(deltaDuration, state.referenceSequence.frameRate);
            }

            m_OverlayText.Append(DurationText);
            m_OverlayText.Append(state.timeFormat.ToTimeStringWithDelta(easeDuration, state.referenceSequence.frameRate, deltaDuration));
        }
    }
}
