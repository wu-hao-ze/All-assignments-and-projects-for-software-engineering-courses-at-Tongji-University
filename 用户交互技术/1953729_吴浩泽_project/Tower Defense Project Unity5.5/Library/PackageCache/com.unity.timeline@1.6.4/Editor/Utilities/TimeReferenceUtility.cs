using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    static class TimeReferenceUtility
    {
        static WindowState state { get { return TimelineWindow.instance.state; } }

        public static float PixelToTime(Vector2 mousePos)
        {
            return PixelToTime(mousePos.x);
        }

        public static float PixelToTime(float pixelX)
        {
            return state.PixelToTime(pixelX);
        }

        public static double GetSnappedTimeAtMousePosition(Vector2 mousePos)
        {
            return state.GetSnappedTimeAtMousePosition(mousePos);
        }

        public static double SnapToFrameIfRequired(double currentTime)
        {
            return TimelinePreferences.instance.snapToFrame ? SnapToFrame(currentTime) : currentTime;
        }

        public static double SnapToFrame(double time)
        {
            if (state.timeReferenceMode == TimeReferenceMode.Global)
            {
                time = state.editSequence.ToGlobalTime(time);
                time = TimeUtility.RoundToFrame(time, state.referenceSequence.frameRate);
                return state.editSequence.ToLocalTime(time);
            }

            return TimeUtility.RoundToFrame(time, state.referenceSequence.frameRate);
        }

        public static string ToTimeString(double time, string format = "F2")
        {
            if (state.timeReferenceMode == TimeReferenceMode.Global)
                time = state.editSequence.ToGlobalTime(time);

            return state.timeFormat.ToTimeString(time, state.referenceSequence.frameRate, format);
        }

        public static double FromTimeString(string timeString)
        {
            double newTime = state.timeFormat.FromTimeString(timeString, state.referenceSequence.frameRate, -1);
            if (newTime >= 0.0)
            {
                return state.timeReferenceMode == TimeReferenceMode.Global ?
                    state.editSequence.ToLocalTime(newTime) : newTime;
            }

            return state.editSequence.time;
        }
    }
}
