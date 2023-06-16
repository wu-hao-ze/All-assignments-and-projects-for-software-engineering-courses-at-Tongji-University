using System;
using UnityEngine;

namespace UnityEditor.Timeline
{
    class MarkerHeaderTrackManipulator : Manipulator
    {
        protected override bool ContextClick(Event evt, WindowState state)
        {
            if (!IsMouseOverMarkerHeader(evt.mousePosition, state))
                return false;

            SelectionManager.SelectOnly(state.editSequence.asset.markerTrack);
            SequencerContextMenu.ShowTrackContextMenu(evt.mousePosition);
            return true;
        }

        protected override bool MouseDown(Event evt, WindowState state)
        {
            if (evt.button != 0 || !IsMouseOverMarkerHeader(evt.mousePosition, state))
                return false;

            SelectionManager.SelectOnly(state.editSequence.asset.markerTrack);
            return true;
        }

        static bool IsMouseOverMarkerHeader(Vector2 mousePosition, WindowState state)
        {
            if (!state.showMarkerHeader)
                return false;

            return state.GetWindow().markerHeaderRect.Contains(mousePosition)
                || state.GetWindow().markerContentRect.Contains(mousePosition);
        }
    }
}
