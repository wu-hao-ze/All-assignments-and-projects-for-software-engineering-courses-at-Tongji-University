using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting.Analytics
{
    internal static class OnPreprocessBuildAnalytics
    {
        private const int MaxEventsPerHour = 120;
        private const int MaxNumberOfElements = 1000;
        private const string VendorKey = "unity.visualscripting";
        private const string EventName = "VScriptOnPreprocessBuild";
        private static bool _isRegistered = false;

        internal static void Send(Data data)
        {
            if (!EditorAnalytics.enabled)
                return;

            if (!RegisterEvent())
                return;

            EditorAnalytics.SendEventWithLimit(EventName, data);
        }

        private static bool RegisterEvent()
        {
            if (!_isRegistered)
            {
                var result = EditorAnalytics.RegisterEventWithLimit(EventName, MaxEventsPerHour, MaxNumberOfElements, VendorKey);
                if (result == UnityEngine.Analytics.AnalyticsResult.Ok)
                {
                    _isRegistered = true;
                }
            }

            return _isRegistered;
        }

        [Serializable]
        internal struct Data
        {
            [SerializeField]
            internal string guid;
            [SerializeField]
            internal BuildTarget buildTarget;
            [SerializeField]
            internal BuildTargetGroup buildTargetGroup;
        }
    }
}
