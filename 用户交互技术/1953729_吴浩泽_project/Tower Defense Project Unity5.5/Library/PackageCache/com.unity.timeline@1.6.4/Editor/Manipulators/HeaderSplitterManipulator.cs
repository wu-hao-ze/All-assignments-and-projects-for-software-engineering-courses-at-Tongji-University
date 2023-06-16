using System;
using UnityEngine;

namespace UnityEditor.Timeline
{
    class HeaderSplitterManipulator : Manipulator
    {
        bool m_Captured;

        protected override bool MouseDown(Event evt, WindowState state)
        {
            Rect headerSplitterRect = state.GetWindow().headerSplitterRect;
            if (headerSplitterRect.Contains(evt.mousePosition))
            {
                m_Captured = true;
                state.AddCaptured(this);
                return true;
            }

            return false;
        }

        protected override bool MouseDrag(Event evt, WindowState state)
        {
            if (!m_Captured)
                return false;

            state.sequencerHeaderWidth = evt.mousePosition.x;
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
            Rect rect = state.GetWindow().sequenceRect;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight);
        }
    }
}
