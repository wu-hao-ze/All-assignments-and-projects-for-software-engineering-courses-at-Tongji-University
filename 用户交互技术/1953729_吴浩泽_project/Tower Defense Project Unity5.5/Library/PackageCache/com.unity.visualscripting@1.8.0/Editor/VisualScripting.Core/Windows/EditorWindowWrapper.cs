using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class EditorWindowWrapper
    {
        protected EditorWindowWrapper() { }

        public WrappedEditorWindow window { get; private set; }

        public bool isOpen => window != null;

        private void CreateWindow()
        {
            if (isOpen)
            {
                Close();
            }

            window = ScriptableObject.CreateInstance<WrappedEditorWindow>();
            window.wrapper = this;
            ConfigureWindow();
            OnShow(); // HACK: Can't use WrappedEditorWindow.Awake/OnEnable, because it's called before we set the wrapper.
        }

        protected virtual void ConfigureWindow() { }

        public void Close()
        {
            try
            {
                window?.Close();
            }
            catch (NullReferenceException)
            {
                // EditorWindow.Close throws an NRE when it's
                // closed before the first frame it was opened.
            }

            window = null;
        }

        public virtual void OnShow() { }
        public virtual void Update() { }
        public virtual void OnGUI() { }
        public virtual void OnClose() { }
        public virtual void OnFocus() { }
        public virtual void OnLostFocus() { }

        protected void Show()
        {
            CreateWindow();
            window?.Show();
        }

        protected void Show(bool immediateDisplay)
        {
            CreateWindow();
            window?.Show(immediateDisplay);
        }

        protected void ShowAsDropDown(Rect buttonRect, Vector2 windowSize)
        {
            CreateWindow();
            window?.ShowAsDropDown(buttonRect, windowSize);
        }

        protected void ShowAuxWindow()
        {
            CreateWindow();
            window?.ShowAuxWindow();
        }

        protected void ShowPopup()
        {
            CreateWindow();
            window?.ShowPopup();
        }

        protected void ShowUtility()
        {
            CreateWindow();
            window?.ShowUtility();
        }
    }
}
