using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    static class FrameRateDisplayUtility
    {
        private static string[] s_FrameRateLabels;
        public static bool GetStandardFromFrameRate(double frameRate, out StandardFrameRates standard)
        {
            FrameRate frameRateObj = TimeUtility.GetClosestFrameRate(RoundFrameRate(frameRate));
            return TimeUtility.ToStandardFrameRate(frameRateObj, out standard);
        }

        public static double RoundFrameRate(double frameRate)
        {
            double trunc = Math.Truncate(frameRate * (1 / TimeUtility.kFrameRateRounding)) * TimeUtility.kFrameRateRounding;
            return Math.Min(Math.Max(TimelineAsset.EditorSettings.kMinFrameRate, trunc),
                TimelineAsset.EditorSettings.kMaxFrameRate);
        }

        public static string[] GetDefaultFrameRatesLabels(StandardFrameRates option)
        {
            string[] labels;
            if (s_FrameRateLabels == null || !s_FrameRateLabels.Any())
            {
                var frameRates = (StandardFrameRates[])Enum.GetValues(typeof(StandardFrameRates));
                labels = Array.ConvertAll(frameRates, GetLabelForStandardFrameRate);
                s_FrameRateLabels = labels;
            }
            else
            {
                labels = s_FrameRateLabels;
            }

            if (!Enum.IsDefined(typeof(StandardFrameRates), option))
            {
                Array.Resize(ref labels, (int)option + 1);
                labels[(int)option] = GetLabelForStandardFrameRate(option);
            }
            return labels;
        }

        static string GetLabelForStandardFrameRate(StandardFrameRates option)
        {
            switch (option)
            {
                case StandardFrameRates.Fps23_97:
                    return L10n.Tr("Film NTSC: 23.97 fps");
                case StandardFrameRates.Fps24:
                    return L10n.Tr("Film: 24 fps");
                case StandardFrameRates.Fps25:
                    return L10n.Tr("PAL: 25 fps");
                case StandardFrameRates.Fps29_97:
                    return L10n.Tr("NTSC: 29.97 fps");
                case StandardFrameRates.Fps30:
                    return L10n.Tr("HD: 30 fps");
                case StandardFrameRates.Fps50:
                    return L10n.Tr("Interlaced PAL: 50 fps");
                case StandardFrameRates.Fps59_94:
                    return L10n.Tr("Interlaced NTSC: 59.94 fps");
                case StandardFrameRates.Fps60:
                    return L10n.Tr("Game: 60 fps");
                default:
                    return L10n.Tr("Custom");
            }
        }
    }
}
