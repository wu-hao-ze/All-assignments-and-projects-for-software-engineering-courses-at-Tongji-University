using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class WrappedEditorWindow : EditorWindow
    {
        // The wrapper reference is not serialized and will be lost
        // on assembly reload. Hence why we lock assembly reload while
        // this window is open.
        public EditorWindowWrapper wrapper { get; set; }

        private void Awake() { }

        private void OnDestroy() { }

        private void OnEnable()
        {
            EditorApplicationUtility.LockReloadAssemblies();
            wrapper?.OnShow();
        }

        private void OnDisable()
        {
            wrapper?.OnClose();
            EditorApplicationUtility.UnlockReloadAssemblies();
        }

        private void Update()
        {
            if (wrapper == null)
            {
                Close();
                return;
            }

            try
            {
                wrapper.Update();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnGUI()
        {
            try
            {
                wrapper?.OnGUI();
            }
            catch (ExitGUIException) { }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnFocus()
        {
            try
            {
                wrapper?.OnFocus();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnLostFocus()
        {
            try
            {
                wrapper?.OnLostFocus();
            }
            catch (WindowClose)
            {
                Close();
            }
        }
    }
}
