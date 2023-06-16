using System;

namespace UnityEditor.Timeline
{
    readonly struct IndentLevelScope : IDisposable
    {
        readonly int m_PrevValue;

        public IndentLevelScope(int newValue)
        {
            m_PrevValue = EditorGUI.indentLevel;
            EditorGUI.indentLevel = newValue;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = m_PrevValue;
        }
    }
}
