using System;
using System.Globalization;
using System.Text.RegularExpressions;
#if TIMELINE_FRAMEACCURATE
using UnityEngine.Playables;
#endif

namespace UnityEngine.Timeline
{
    /// <summary>
    /// The standard frame rates supported when locking Timeline playback to frames.
    /// The frame rate is expressed in frames per second (fps).
    /// </summary>
    public enum StandardFrameRates
    {
        /// <summary>
        /// Represents a frame rate of 24 fps. This is the common frame rate for film.
        /// </summary>
        Fps24,
        /// <summary>
        /// Represents a drop frame rate of 23.97 fps. This is the common frame rate for NTSC film broadcast.
        /// </summary>
        Fps23_97,
        /// <summary>
        /// Represents a frame rate of 25 fps. This is commonly used for non-interlaced PAL television broadcast.
        /// </summary>
        Fps25,
        /// <summary>
        /// Represents a frame rate of 30 fps. This is commonly used for HD footage.
        /// </summary>
        Fps30,
        /// <summary>
        /// Represents a drop frame rate of 29.97 fps. This is commonly used for NTSC television broadcast.
        /// </summary>
        Fps29_97,
        /// <summary>
        /// Represents a frame rate of 50 fps. This is commonly used for interlaced PAL television broadcast.
        /// </summary>
        Fps50,
        /// <summary>
        /// Represents a frame rate of 60 fps. This is commonly used for games.
        /// </summary>
        Fps60,
        /// <summary>
        /// Represents a drop frame rate of 59.94 fps. This is commonly used for interlaced NTSC television broadcast.
        /// </summary>
        Fps59_94
    }

    // Sequence specific utilities for time manipulation
    static class TimeUtility
    {
        // chosen because it will cause no rounding errors between time/frames for frames values up to at least 10 million
        public static readonly double kTimeEpsilon = 1e-14;
        public static readonly double kFrameRateEpsilon = 1e-6;
        public static readonly double k_MaxTimelineDurationInSeconds = 9e6; //104 days of running time
        public static readonly double kFrameRateRounding = 1e-2;


        static void ValidateFrameRate(double frameRate)
        {
            if (frameRate <= kTimeEpsilon)
                throw new ArgumentException("frame rate cannot be 0 or negative");
        }

        public static int ToFrames(double time, double frameRate)
        {
            ValidateFrameRate(frameRate);
            time = Math.Min(Math.Max(time, -k_MaxTimelineDurationInSeconds), k_MaxTimelineDurationInSeconds);
            // this matches OnFrameBoundary
            double tolerance = GetEpsilon(time, frameRate);
            if (time < 0)
            {
                return (int)Math.Ceiling(time * frameRate - tolerance);
            }
            return (int)Math.Floor(time * frameRate + tolerance);
        }

        public static double ToExactFrames(double time, double frameRate)
        {
            ValidateFrameRate(frameRate);
            return time * frameRate;
        }

        public static double FromFrames(int frames, double frameRate)
        {
            ValidateFrameRate(frameRate);
            return (frames / frameRate);
        }

        public static double FromFrames(double frames, double frameRate)
        {
            ValidateFrameRate(frameRate);
            return frames / frameRate;
        }

        public static bool OnFrameBoundary(double time, double frameRate)
        {
            return OnFrameBoundary(time, frameRate, GetEpsilon(time, frameRate));
        }

        public static double GetEpsilon(double time, double frameRate)
        {
            return Math.Max(Math.Abs(time), 1) * frameRate * kTimeEpsilon;
        }

        public static bool OnFrameBoundary(double time, double frameRate, double epsilon)
        {
            ValidateFrameRate(frameRate);

            double exact = ToExactFrames(time, frameRate);
            double rounded = Math.Round(exact);

            return Math.Abs(exact - rounded) < epsilon;
        }

        public static double RoundToFrame(double time, double frameRate)
        {
            ValidateFrameRate(frameRate);

            var frameBefore = (int)Math.Floor(time * frameRate) / frameRate;
            var frameAfter = (int)Math.Ceiling(time * frameRate) / frameRate;

            return Math.Abs(time - frameBefore) < Math.Abs(time - frameAfter) ? frameBefore : frameAfter;
        }

        public static string TimeAsFrames(double timeValue, double frameRate, string format = "F2")
        {
            if (OnFrameBoundary(timeValue, frameRate)) // make integral values when on time borders
                return ToFrames(timeValue, frameRate).ToString();
            return ToExactFrames(timeValue, frameRate).ToString(format);
        }

        public static string TimeAsTimeCode(double timeValue, double frameRate, string format = "F2")
        {
            ValidateFrameRate(frameRate);

            int intTime = (int)Math.Abs(timeValue);

            int hours = intTime / 3600;
            int minutes = (intTime % 3600) / 60;
            int seconds = intTime % 60;

            string result;
            string sign = timeValue < 0 ? "-" : string.Empty;
            if (hours > 0)
                result = hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
            else if (minutes > 0)
                result = minutes + ":" + seconds.ToString("D2");
            else
                result = seconds.ToString();

            int frameDigits = (int)Math.Floor(Math.Log10(frameRate) + 1);

            // Add partial digits on the frame if needed.
            // we are testing the original value (not the truncated), because the truncation can cause rounding errors leading
            // to invalid strings for items on frame boundaries
            string frames = (ToFrames(timeValue, frameRate) - ToFrames(intTime, frameRate)).ToString().PadLeft(frameDigits, '0');
            if (!OnFrameBoundary(timeValue, frameRate))
            {
                string decimals = ToExactFrames(timeValue, frameRate).ToString(format);
                int decPlace = decimals.IndexOf('.');
                if (decPlace >= 0)
                    frames += " [" + decimals.Substring(decPlace) + "]";
            }

            return sign + result + ":" + frames;
        }

        // Given a time code string, return the time in seconds
        // 1.5 -> 1.5 seconds
        // 1:1.5 -> 1 minute, 1.5 seconds
        // 1:1[.5] -> 1 second, 1.5 frames
        // 2:3:4 -> 2 minutes, 3 seconds, 4 frames
        // 1[.6] -> 1.6 frames
        public static double ParseTimeCode(string timeCode, double frameRate, double defaultValue)
        {
            timeCode = RemoveChar(timeCode, c => char.IsWhiteSpace(c));
            string[] sections = timeCode.Split(':');
            if (sections.Length == 0 || sections.Length > 4)
                return defaultValue;

            int hours = 0;
            int minutes = 0;
            double seconds = 0;
            double frames = 0;

            try
            {
                // depending on the format of the last numbers
                // seconds format
                string lastSection = sections[sections.Length - 1];
                if (Regex.Match(lastSection, @"^\d+\.\d+$").Success)
                {
                    seconds = double.Parse(lastSection);
                    if (sections.Length > 3) return defaultValue;
                    if (sections.Length > 1) minutes = int.Parse(sections[sections.Length - 2]);
                    if (sections.Length > 2) hours = int.Parse(sections[sections.Length - 3]);
                }
                // frame formats
                else
                {
                    if (Regex.Match(lastSection, @"^\d+\[\.\d+\]$").Success)
                    {
                        string stripped = RemoveChar(lastSection, c => c == '[' || c == ']');
                        frames = double.Parse(stripped);
                    }
                    else if (Regex.Match(lastSection, @"^\d*$").Success)
                    {
                        frames = int.Parse(lastSection);
                    }
                    else
                    {
                        return defaultValue;
                    }

                    if (sections.Length > 1) seconds = int.Parse(sections[sections.Length - 2]);
                    if (sections.Length > 2) minutes = int.Parse(sections[sections.Length - 3]);
                    if (sections.Length > 3) hours = int.Parse(sections[sections.Length - 4]);
                }
            }
            catch (FormatException)
            {
                return defaultValue;
            }

            return frames / frameRate + seconds + minutes * 60 + hours * 3600;
        }

        public static double ParseTimeSeconds(string timeCode, double frameRate, double defaultValue)
        {
            timeCode = RemoveChar(timeCode, c => char.IsWhiteSpace(c));
            string[] sections = timeCode.Split(':');
            if (sections.Length == 0 || sections.Length > 4)
                return defaultValue;

            int hours = 0;
            int minutes = 0;
            double seconds = 0;

            try
            {
                // depending on the format of the last numbers
                // seconds format
                string lastSection = sections[sections.Length - 1];
                {
                    if (!double.TryParse(lastSection, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds))
                        if (Regex.Match(lastSection, @"^\d+\.\d+$").Success)
                            seconds = double.Parse(lastSection);
                        else
                            return defaultValue;

                    if (!double.TryParse(lastSection, NumberStyles.Float, CultureInfo.InvariantCulture, out seconds))
                        return defaultValue;

                    if (sections.Length > 3) return defaultValue;
                    if (sections.Length > 1) minutes = int.Parse(sections[sections.Length - 2]);
                    if (sections.Length > 2) hours = int.Parse(sections[sections.Length - 3]);
                }
            }
            catch (FormatException)
            {
                return defaultValue;
            }

            return seconds + minutes * 60 + hours * 3600;
        }

        // fixes rounding errors from using single precision for length
        public static double GetAnimationClipLength(AnimationClip clip)
        {
            if (clip == null || clip.empty)
                return 0;

            double length = clip.length;
            if (clip.frameRate > 0)
            {
                double frames = Mathf.Round(clip.length * clip.frameRate);
                length = frames / clip.frameRate;
            }
            return length;
        }

        static string RemoveChar(string str, Func<char, bool> charToRemoveFunc)
        {
            var len = str.Length;
            var src = str.ToCharArray();
            var dstIdx = 0;
            for (var i = 0; i < len; i++)
            {
                if (!charToRemoveFunc(src[i]))
                    src[dstIdx++] = src[i];
            }
            return new string(src, 0, dstIdx);
        }

        public static FrameRate GetClosestFrameRate(double frameRate)
        {
            ValidateFrameRate(frameRate);
            var actualFrameRate = FrameRate.DoubleToFrameRate(frameRate);
            return Math.Abs(frameRate - actualFrameRate.rate) < kFrameRateRounding ? actualFrameRate : new FrameRate();
        }

        public static FrameRate ToFrameRate(StandardFrameRates enumValue)
        {
            switch (enumValue)
            {
                case StandardFrameRates.Fps23_97:
                    return FrameRate.k_23_976Fps;
                case StandardFrameRates.Fps24:
                    return FrameRate.k_24Fps;
                case StandardFrameRates.Fps25:
                    return FrameRate.k_25Fps;
                case StandardFrameRates.Fps29_97:
                    return FrameRate.k_29_97Fps;
                case StandardFrameRates.Fps30:
                    return FrameRate.k_30Fps;
                case StandardFrameRates.Fps50:
                    return FrameRate.k_50Fps;
                case StandardFrameRates.Fps59_94:
                    return FrameRate.k_59_94Fps;
                case StandardFrameRates.Fps60:
                    return FrameRate.k_60Fps;
                default:
                    return new FrameRate();
            }
            ;
        }

        internal static bool ToStandardFrameRate(FrameRate rate, out StandardFrameRates standard)
        {
            if (rate == FrameRate.k_23_976Fps)
                standard = StandardFrameRates.Fps23_97;
            else if (rate == FrameRate.k_24Fps)
                standard = StandardFrameRates.Fps24;
            else if (rate == FrameRate.k_25Fps)
                standard = StandardFrameRates.Fps25;
            else if (rate == FrameRate.k_30Fps)
                standard = StandardFrameRates.Fps30;
            else if (rate == FrameRate.k_29_97Fps)
                standard = StandardFrameRates.Fps29_97;
            else if (rate == FrameRate.k_50Fps)
                standard = StandardFrameRates.Fps50;
            else if (rate == FrameRate.k_59_94Fps)
                standard = StandardFrameRates.Fps59_94;
            else if (rate == FrameRate.k_60Fps)
                standard = StandardFrameRates.Fps60;
            else
            {
                standard = (StandardFrameRates)Enum.GetValues(typeof(StandardFrameRates)).Length;
                return false;
            }
            return true;
        }
    }
}
