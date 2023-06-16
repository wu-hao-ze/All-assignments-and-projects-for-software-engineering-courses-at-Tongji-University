#if UNITY_EDITOR && UNITY_2021_1_OR_NEWER
#define CAN_USE_CUSTOM_HELP_URL
#endif

using System;
using System.Diagnostics;
using UnityEngine;

namespace UnityEngine.Timeline
{
#if CAN_USE_CUSTOM_HELP_URL

    using UnityEditor.PackageManager;

    [Conditional("UNITY_EDITOR")]
    class TimelineHelpURLAttribute : HelpURLAttribute
    {
        const string k_BaseURL = "https://docs.unity3d.com/Packages/com.unity.timeline@";
        const string k_MidURL = "/api/";
        const string k_EndURL = ".html";
        const string k_FallbackVersion = "latest";

        static readonly string k_PackageVersion;

        static TimelineHelpURLAttribute()
        {
            PackageInfo packageInfo = PackageInfo.FindForAssembly(typeof(TimelineAsset).Assembly);
            k_PackageVersion = packageInfo == null ? k_FallbackVersion : packageInfo.version.Substring(0, 3);
        }

        public TimelineHelpURLAttribute(Type type)
            : base(HelpURL(type)) {}

        static string HelpURL(Type type)
        {
            return $"{k_BaseURL}{k_PackageVersion}{k_MidURL}{type.FullName}{k_EndURL}";
        }
    }
#else //HelpURL attribute is `sealed` in previous Unity versions
    [Conditional("UNITY_EDITOR")]
    class TimelineHelpURLAttribute : Attribute
    {
        public TimelineHelpURLAttribute(Type type) { }
    }
#endif
}
