using System;

namespace UnityEditor.Timeline
{
    readonly struct LabelWidthScope : IDisposable
    {
        readonly float m_PrevValue;

        public LabelWidthScope(float newValue)
        {
            m_PrevValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = newValue;
        }

        public void Dispose()
        {
            EditorGUIUtility.labelWidth = m_PrevValue;
        }
    }
}
