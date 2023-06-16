using System;
using System.Globalization;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// The available display modes for time in the Timeline Editor.
    /// </summary>
    public enum TimeFormat
    {
        /// <summary>Displays time values as frames.</summary>
        Frames,

        /// <summary>Displays time values as timecode (SS:FF) format.</summary>
        Timecode,

        /// <summary>Displays time values as seconds.</summary>
        Seconds
    };

    static class TimeDisplayUnitExtensions
    {
        public static TimeArea.TimeFormat ToTimeAreaFormat(this TimeFormat timeDisplayUnit)
        {
            switch (timeDisplayUnit)
            {
                case TimeFormat.Frames: return TimeArea.TimeFormat.Frame;
                case TimeFormat.Timecode: return TimeArea.TimeFormat.TimeFrame;
                case TimeFormat.Seconds: return TimeArea.TimeFormat.None;
            }

            return TimeArea.TimeFormat.Frame;
        }

        public static string ToTimeString(this TimeFormat timeFormat, double time, double frameRate, string format = "f2")
        {
            switch (timeFormat)
            {
                case TimeFormat.Frames: return TimeUtility.TimeAsFrames(time, frameRate, format);
                case TimeFormat.Timecode: return TimeUtility.TimeAsTimeCode(time, frameRate, format);
                case TimeFormat.Seconds: return time.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat);
            }

            return time.ToString(format);
        }

        public static string ToTimeStringWithDelta(this TimeFormat timeFormat, double time, double frameRate, double delta, string format = "f2")
        {
            const double epsilon = 1e-7;
            var result = ToTimeString(timeFormat, time, frameRate, format);
            if (delta > epsilon || delta < -epsilon)
            {
                var sign = ((delta >= 0) ? "+" : "-");
                var deltaStr = ToTimeString(timeFormat, Math.Abs(delta), frameRate, format);
                return $"{result} ({sign}{deltaStr})";
            }
            return result;
        }

        public static double FromTimeString(this TimeFormat timeFormat, string timeString, double frameRate, double defaultValue)
        {
            double time = defaultValue;
            switch (timeFormat)
            {
                case TimeFormat.Frames:
                    if (!double.TryParse(timeString, NumberStyles.Any, CultureInfo.InvariantCulture, out time))
                        return defaultValue;
                    time = TimeUtility.FromFrames(time, frameRate);
                    break;
                case TimeFormat.Seconds:
                    time = TimeUtility.ParseTimeSeconds(timeString, frameRate, defaultValue);
                    break;
                case TimeFormat.Timecode:
                    time = TimeUtility.ParseTimeCode(timeString, frameRate, defaultValue);
                    break;
                default:
                    time = defaultValue;
                    break;
            }

            return time;
        }
    }
}
