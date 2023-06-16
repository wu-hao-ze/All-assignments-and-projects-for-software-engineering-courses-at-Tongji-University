using System;
using UnityEngine;

namespace UnityEditor.Timeline
{
    readonly struct HorizontalScope : IDisposable
    {
        public readonly Rect rect;

        public HorizontalScope(GUIContent content, GUIStyle style)
        {
            rect = EditorGUILayout.BeginHorizontal(content, style);
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}
