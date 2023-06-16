using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting.Analytics
{
    internal static class HotkeyUsageAnalytics
    {
        private const int MaxEventsPerHour = 120;
        private const int MaxNumberOfElements = 1000;
        private const string VendorKey = "unity.visualscripting";
        private const string EventName = "VScriptHotkeyUsage";
        private static bool _isRegistered = false;

        private const int HotkeyUseLimitBeforeSend = 10000;
        private static bool _interruptEventsRegistered = false;
        private static bool _onControlSchemeEventRegistered = false;
        private static Data _collectedData = null;
        // Limiting to stop spam from some continuous events like pan or zoom
        private const int HotkeyFrameLimit = 2;
        private static Dictionary<Hotkey, FrameLimiterUtility> _frameLimiters;

        internal static void HotkeyUsed(Hotkey key)
        {
            EnsureCollectedDataInitialized();
            EnsureInterruptEventsRegistered();
            EnsureOnControlSchemeChangedRegistered();

            _collectedData.canvasControlSchemeName = Enum.GetName(typeof(CanvasControlScheme), BoltCore.Configuration.controlScheme);

            if (!_frameLimiters.ContainsKey(key))
                _frameLimiters.Add(key, new FrameLimiterUtility(HotkeyFrameLimit));

            if (!_frameLimiters[key].IsWithinFPSLimit()) return;

            var enumStringVal = Enum.GetName(typeof(Hotkey), key);
            if (enumStringVal == null)
                return;

            if (!_collectedData.HotkeyUsageCountDict.ContainsKey(enumStringVal))
                _collectedData.HotkeyUsageCountDict.Add(enumStringVal, 0);

            _collectedData.HotkeyUsageCountDict[enumStringVal]++;

            if (_collectedData.HotkeyUsageCountDict[enumStringVal] >= HotkeyUseLimitBeforeSend)
                Send();
        }

        private static void Send()
        {
            if (!EditorAnalytics.enabled)
                return;

            if (!RegisterEvent())
                return;

            // Because dicts aren't serializable, convert it to a list right before we send
            foreach (var count in _collectedData.HotkeyUsageCountDict)
            {
                var c = new HotkeyStringUsageCount();
                c.hotkey = count.Key;
                c.count = count.Value;
                _collectedData.hotkeyUsageCount.Add(c);
            }

            EditorAnalytics.SendEventWithLimit(EventName, _collectedData);

            ResetCollectedData();
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

        private static void EnsureInterruptEventsRegistered()
        {
            if (_interruptEventsRegistered) return;

            EditorApplication.quitting += Send;
            AssemblyReloadEvents.beforeAssemblyReload += Send;
            _interruptEventsRegistered = true;
        }

        private static void EnsureOnControlSchemeChangedRegistered()
        {
            if (_onControlSchemeEventRegistered) return;

            BoltCore.Configuration.ControlSchemeChanged += Send;
            _onControlSchemeEventRegistered = true;
        }

        private static void EnsureCollectedDataInitialized()
        {
            if (_collectedData != null) return;
            ResetCollectedData();

            _frameLimiters = new Dictionary<Hotkey, FrameLimiterUtility>();
        }

        private static void ResetCollectedData()
        {
            var controlScheme = Enum.GetName(typeof(CanvasControlScheme), BoltCore.Configuration.controlScheme);
            _collectedData = new Data()
            {
                canvasControlSchemeName = controlScheme,
                hotkeyUsageCount = new List<HotkeyStringUsageCount>(),
                HotkeyUsageCountDict = new Dictionary<string, int>(),
            };
        }

        [Serializable]
        private class Data
        {
            // Note: using strings instead of enums to make it resilient to enum order changing
            [SerializeField]
            internal string canvasControlSchemeName;

            [SerializeField]
            internal List<HotkeyStringUsageCount> hotkeyUsageCount;

            // Not actually sent to analytics because analytics uses UnitySerializer under the hood
            // which cannot serialize dictionaries
            internal Dictionary<string, int> HotkeyUsageCountDict;
        }

        [Serializable]
        private struct HotkeyStringUsageCount
        {
            [SerializeField] internal string hotkey;
            [SerializeField] internal int count;
        }

        internal enum Hotkey
        {
            Zoom,
            Scroll,
            PanMmb,
            PanAltLmb,
            Home,
            RmbRemoveConnections,
            Cut,
            Copy,
            Paste,
            Duplicate,
            Delete,
            SelectAll,
            Tab,
        }
    }
}
