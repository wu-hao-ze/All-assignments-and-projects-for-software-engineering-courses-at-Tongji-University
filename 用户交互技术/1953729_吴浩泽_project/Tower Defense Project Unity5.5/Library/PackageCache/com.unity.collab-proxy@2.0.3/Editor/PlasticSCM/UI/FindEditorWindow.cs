using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class FindEditorWindow
    {
        internal static EditorWindow ProjectWindow()
        {
            Type projectBrowserType = typeof(EditorWindow).Assembly.GetType(
                    "UnityEditor.ProjectBrowser");

            UnityEngine.Object[] windows = Resources.FindObjectsOfTypeAll(
                 projectBrowserType);

            if (windows.Length == 0)
                return null;

            return windows[0] as EditorWindow;
        }

        internal static EditorWindow ToDock<T>()
        {
            List<EditorWindow> windows = GetAvailableWindows();

            IEnumerable<EditorWindow> candidateWindows = windows
                .Where(w => !(w is T))
                .Where(w => w.position.width > 400 && w.position.height > 300)
                .OrderByDescending(w => w.position.width * w.position.height);

            return candidateWindows.FirstOrDefault();
        }

        static List<EditorWindow> GetAvailableWindows()
        {
            List<EditorWindow> result = new List<EditorWindow>();

            var hostViewField = typeof(EditorWindow).GetField(
                "m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);

            if (hostViewField == null)
                return null;

            var hostViewType = hostViewField.FieldType;
            var actualViewField = hostViewType.GetField(
                "m_ActualView", BindingFlags.Instance | BindingFlags.NonPublic);

            if (actualViewField == null)
                return null;

            foreach (var window in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                var hostView = hostViewField.GetValue(window);

                if (hostView == null)
                    continue;

                EditorWindow actualDrawnWindow = actualViewField
                    .GetValue(hostView) as EditorWindow;

                if (actualDrawnWindow == null)
                    continue;

                if (result.Contains(actualDrawnWindow))
                    continue;

                result.Add(actualDrawnWindow);
            }

            return result;
        }
    }
}
