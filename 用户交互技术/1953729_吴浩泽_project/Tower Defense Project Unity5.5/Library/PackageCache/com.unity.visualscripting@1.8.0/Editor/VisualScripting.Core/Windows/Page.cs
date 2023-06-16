using System;
using UnityEngine;
using GUIEvent = UnityEngine.Event;

namespace Unity.VisualScripting
{
    public abstract class Page
    {
        protected Page()
        {
            completeLabel = "Done";
        }

        private Vector2 contentScroll;

        public string title { get; set; }
        public string shortTitle { get; set; }
        public string subtitle { get; set; }
        public EditorTexture icon { get; set; }
        public Action onComplete { private get; set; }
        public string completeLabel { get; set; }
        private bool shouldComplete;

        protected virtual void OnShow() { }

        public virtual void Update() { }

        public bool CompleteSwitch()
        {
            if (shouldComplete)
            {
                shouldComplete = false;
                onComplete.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void OnHeaderGUI()
        {
            LudiqGUI.WindowHeader(title, icon);
        }

        protected abstract void OnContentGUI();

        protected virtual void OnClose() { }

        public void Show()
        {
            contentScroll = Vector2.zero;
            OnShow();
        }

        public void Close()
        {
            OnClose();
        }

        public void DrawHeader()
        {
            OnHeaderGUI();
        }

        public void DrawContent()
        {
            contentScroll = GUILayout.BeginScrollView(contentScroll, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            OnContentGUI();
            GUILayout.EndScrollView();
        }

        protected virtual void Complete()
        {
            shouldComplete = true;
        }

        public virtual void OnFocus() { }

        public virtual void OnLostFocus() { }

        protected static GUIEvent e => GUIEvent.current;
    }
}
