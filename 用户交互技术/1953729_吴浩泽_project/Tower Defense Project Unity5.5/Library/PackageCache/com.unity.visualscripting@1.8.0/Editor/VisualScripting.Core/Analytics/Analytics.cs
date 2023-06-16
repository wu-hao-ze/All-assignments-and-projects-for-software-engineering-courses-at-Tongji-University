using UnityEditor;

namespace Unity.VisualScripting
{
    public sealed class UsageAnalytics
    {
        const int k_MaxEventsPerHour = 1000;
        const int k_MaxNumberOfElements = 1000;
        const string k_VendorKey = "unity.bolt";
        const string k_EventName = "BoltUsage";
        static bool isRegistered = false;

        public static void CollectAndSend()
        {
            if (!EditorAnalytics.enabled)
                return;

            if (!RegisterEvent())
                return;

            var data = CollectData();

            EditorAnalytics.SendEventWithLimit(k_EventName, data);
        }

        private static bool RegisterEvent()
        {
            if (!isRegistered)
            {
                var result = EditorAnalytics.RegisterEventWithLimit(k_EventName, k_MaxEventsPerHour, k_MaxNumberOfElements, k_VendorKey);
                if (result == UnityEngine.Analytics.AnalyticsResult.Ok)
                {
                    isRegistered = true;
                }
            }

            return isRegistered;
        }

        private static UsageAnalyticsData CollectData()
        {
            var data = new UsageAnalyticsData
            {
                productVersion = BoltProduct.instance.version.ToString(),
            };

            return data;
        }

        private struct UsageAnalyticsData
        {
            public string productVersion;
        }
    }
}
