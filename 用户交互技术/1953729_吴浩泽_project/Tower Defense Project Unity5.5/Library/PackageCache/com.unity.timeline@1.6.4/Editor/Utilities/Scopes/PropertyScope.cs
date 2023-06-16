using System;
using UnityEngine;

namespace UnityEditor.Timeline
{
    readonly struct PropertyScope : IDisposable
    {
        public readonly GUIContent content;

        public PropertyScope(Rect totalPosition, GUIContent label, SerializedProperty property)
        {
            content = EditorGUI.BeginProperty(totalPosition, label, property);
        }

        public void Dispose()
        {
            EditorGUI.EndProperty();
        }
    }
}
