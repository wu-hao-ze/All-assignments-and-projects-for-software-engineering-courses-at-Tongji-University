using UnityEngine;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Scrolling mode during playback for the timeline window.
    /// </summary>
    public enum PlaybackScrollMode
    {
        /// <summary>
        /// Timeline window doesn't change while the playhead is leaving the window.
        /// </summary>
        None,
        /// <summary>
        /// Timeline window pans its content when the playhead arrive at the right of the window (like a paging scrolling).
        /// </summary>
        Pan,
        /// <summary>
        /// Timeline window move the content as the playhead moves.
        /// When the playhead reach the middle of the window, it stays there and the content scroll behind it.
        /// </summary>
        Smooth
    }

    static class PlaybackScroller
    {
        public static void AutoScroll(WindowState state)
        {
            if (Event.current.type != EventType.Layout)
                return;

            switch (state.autoScrollMode)
            {
                case PlaybackScrollMode.Pan:
                    DoPanScroll(state);
                    break;
                case PlaybackScrollMode.Smooth:
                    DoSmoothScroll(state);
                    break;
            }
        }

        static void DoSmoothScroll(WindowState state)
        {
            if (state.playing)
                state.SetPlayHeadToMiddle();

            state.UpdateLastFrameTime();
        }

        static void DoPanScroll(WindowState state)
        {
            if (!state.playing)
                return;

            var paddingDeltaTime = state.PixelDeltaToDeltaTime(WindowConstants.autoPanPaddingInPixels);
            var showRange = state.timeAreaShownRange;
            var rightBoundForPan = showRange.y - paddingDeltaTime;
            if (state.editSequence.time > rightBoundForPan)
            {
                var leftBoundForPan = showRange.x + paddingDeltaTime;
                var delta = rightBoundForPan - leftBoundForPan;
                state.SetTimeAreaShownRange(showRange.x + delta, showRange.y + delta);
            }
        }
    }
}
