#if UNITY_EDITOR
using UnityEditor;

namespace Unity.VisualScripting
{
    public class FrameDelayedCallback
    {
        private System.Action m_Callback;
        private int m_FrameDelay;
        public FrameDelayedCallback(System.Action function, int frameDelay)
        {
            m_Callback = function;
            m_FrameDelay = frameDelay;
            EditorApplication.update += Update;
        }

        public void Clear()
        {
            EditorApplication.update -= Update;
            m_FrameDelay = 0;
            m_Callback = null;
        }

        private void Update()
        {
            if (--m_FrameDelay == 0)
            {
                // Clear state before firing callback to ensure reset (callback could call ExitGUI)
                var callback = m_Callback;
                Clear();

                callback?.Invoke();
            }
        }
    }
}
#endif
