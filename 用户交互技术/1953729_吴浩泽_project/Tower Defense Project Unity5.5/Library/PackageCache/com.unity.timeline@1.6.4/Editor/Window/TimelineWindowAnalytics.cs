//#define ANALITICS_DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    class TimelineWindowAnalytics
    {
        const string vendorKey = "unity.timeline";
        const string eventName = "timeline_editor_info";
        const int version = 2;
        const int maxEventsPerHour = 1000;
        const int maxNumberOfElements = 1000;

        [Serializable]
        internal struct timeline_asset_stats
        {
            public string asset_guid;
            public double duration;
            public double frame_rate;
            public List<track_asset_stats> track_stats;
            public double mix_samples_count, ripple_samples_count, replace_samples_count;
            public string display_format;
        }

        [Serializable]
        internal struct track_asset_stats
        {
            public string track_type;
            public int clip_count;
            public int marker_count;
        }

        class WindowAnalyticsStats
        {
            internal int[] editModeSamples = new int[3]; // EditModes
        }

        static WindowAnalyticsStats analyticsStats = new WindowAnalyticsStats();

        public void SendPlayEvent(bool start)
        {
            if (!start || !EditorAnalytics.enabled)
            {
                return;
            }

            EditorAnalytics.RegisterEventWithLimit(eventName, maxEventsPerHour, maxNumberOfElements, vendorKey, version);

            var ret = GenerateTimelineAssetStats(out var data);
            if (!ret)
            {
                return;
            }
#if ANALITICS_DEBUG
            Debug.Log(JsonUtility.ToJson(data, true));
#endif
            EditorAnalytics.SendEventWithLimit(eventName, data, version);
            SendAfterSequenceChangeEvent();
        }

        public void SendAfterSequenceChangeEvent()
        {
            analyticsStats = new WindowAnalyticsStats(); // Wipe Window Stats
        }

        public void SendManipulationEndedEvent()
        {
            analyticsStats.editModeSamples[(int)EditMode.editType]++;
        }

        internal static bool GenerateTimelineAssetStats(out timeline_asset_stats data)
        {
            var timeline = TimelineEditor.inspectedAsset;
            if (timeline == null ||
                !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(timeline, out var guid, out long _))
            {
                data = new timeline_asset_stats();
                return false;
            }

            data = new timeline_asset_stats
            {
                asset_guid = guid,
                duration = timeline.duration,
                frame_rate = timeline.editorSettings.frameRate,
                track_stats = GetTrackAssetStats(timeline),
                display_format = TimelinePreferences.instance.timeFormat.ConvertToString(),
                mix_samples_count = analyticsStats.editModeSamples[(int)EditMode.EditType.Mix],
                ripple_samples_count = analyticsStats.editModeSamples[(int)EditMode.EditType.Ripple],
                replace_samples_count = analyticsStats.editModeSamples[(int)EditMode.EditType.Replace],
            };

            return true;
        }

        static List<track_asset_stats> GetTrackAssetStats(TimelineAsset timeline)
        {
            var ret = new List<track_asset_stats>();
            foreach (var track in timeline.flattenedTracks)
            {
                ret.Add(new track_asset_stats
                {
                    track_type = track.GetType().FullName,
                    clip_count = track.GetClips().Count(),
                    marker_count = track.GetMarkers().Count()
                }
                );
            }
            return ret;
        }
    }

    static class ConversionUtilities
    {
        internal static string ConvertToString<T>(this T e) where T : Enum
        {
            return Enum.GetName(typeof(T), e).ToSnakeCase();
        }

        static string ToSnakeCase(this string str)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length - 1; ++i)
            {
                var ch = str[i];
                var nCh = str[i + 1];
                if (char.IsUpper(ch) && char.IsLower(nCh))
                {
                    sb.Append("_");
                }

                sb.Append(ch.ToString().ToLower());
            }

            sb.Append(str[str.Length - 1].ToString().ToLower());

            return sb.ToString().TrimStart('_');
        }
    }
}
