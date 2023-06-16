using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class WebWindow : EditorWindow
    {
        private void Initialize()
        {
            webView = new WebView(this, new Rect(Vector2.zero, position.size));

            if (uri != null)
            {
                webView.Load(uri);
                webView.Show();
            }
        }

        private WebView webView;

        [SerializeField]
        private string _uri;

        private bool syncingFocus;

        public string uri
        {
            get
            {
                return _uri;
            }
            set
            {
                _uri = value;

                if (uri != null)
                {
                    webView?.Load(uri);
                    webView?.Show();
                }
            }
        }

        private void OnEnable()
        {
            instance = this;
        }

        private void OnGUI()
        {
            // Initialization cannot occur in OnEnable because the
            // parent host view isn't yet assigned
            if (webView == null || webView.isDestroyed)
            {
                Initialize();
            }

            if (Event.current.type == EventType.Repaint)
            {
                webView.host = this;
                webView.position = new Rect(Vector2.zero, position.size);
                webView.Show();
            }
        }

        private void OnBecameInvisible()
        {
            // Necessary to force a refresh when (un)docking the window
            if (webView != null)
            {
                webView.host = null;
            }
        }

        private void OnFocus()
        {
            SetFocus(true);
        }

        private void OnLostFocus()
        {
            SetFocus(false);
        }

        private void OnDestroy()
        {
            webView?.Destroy();
        }

        public void Reload()
        {
            webView?.Reload();
        }

        private void SetFocus(bool value)
        {
            // Necessary to prevent an infinite recursion crash
            if (syncingFocus)
            {
                return;
            }

            syncingFocus = true;

            if (value)
            {
                webView?.Show();
            }

            if (webView != null)
            {
                webView.hasFocus = value;
            }

            syncingFocus = false;
        }

        public static WebWindow instance { get; private set; }

        public static void Show(GUIContent titleContent, string uri)
        {
#if false
            if (instance == null)
            {
                CreateInstance<WebWindow>().Show();
            }
            else
            {
                FocusWindowIfItsOpen<WebWindow>();
            }

            instance.titleContent = titleContent;
            instance.uri = uri;
#else
            // The window is way too bugged right now.
            Process.Start(uri);
#endif
        }

        public static void Show(string title, string uri)
        {
            Show(new GUIContent(title), uri);
        }
    }
}
