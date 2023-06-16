using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public sealed class WebView
    {
        public WebView(EditorWindow host, Rect position)
        {
            Ensure.That(nameof(host)).IsNotNull(host);

            var hostView = GetHostView(host);

            if (hostView == null)
            {
                throw new InvalidOperationException("Web view host view cannot be null during initialization.");
            }

            position = ToWindowRect(position);
            webView = ScriptableObject.CreateInstance(WebViewType);
            webView.hideFlags = HideFlags.HideAndDontSave;
            WebView_InitWebView.Invoke(webView, new[] { hostView, (int)position.x, (int)position.y, (int)position.width, (int)position.height, false });
        }

        private readonly ScriptableObject webView;

        public Rect position
        {
            set
            {
                value = ToWindowRect(value);
                WebView_SetSizeAndPosition.InvokeOptimized(webView, (int)value.x, (int)value.y, (int)value.width, (int)value.height);
            }
        }

        public EditorWindow host
        {
            set
            {
                WebView_SetHostView.InvokeOptimized(webView, GetHostView(value));
            }
        }

        public bool isDestroyed => webView == null || !webView || (bool)WebView_IntPtrIsNull.InvokeOptimized(webView);

        public bool hasFocus
        {
            set
            {
                WebView_SetFocus.InvokeOptimized(webView, value);
                WebView_SetApplicationFocus.InvokeOptimized(webView, value);
            }
        }

        public void Destroy()
        {
            UnityObject.DestroyImmediate(webView);
        }

        public void Load(string uri)
        {
            if (uri.StartsWith("file:"))
            {
                WebView_LoadFile.InvokeOptimized(webView, uri);
            }
            else
            {
                WebView_LoadUrl.InvokeOptimized(webView, uri);
            }
        }

        public void Show()
        {
            WebView_Show.InvokeOptimized(webView);
        }

        public void Hide()
        {
            WebView_Hide.InvokeOptimized(webView);
        }

        public void Forward()
        {
            WebView_Forward.InvokeOptimized(webView);
        }

        public void Back()
        {
            WebView_Back.InvokeOptimized(webView);
        }

        public void Reload()
        {
            WebView_Reload.InvokeOptimized(webView);
        }

        static WebView()
        {
            try
            {
                WebViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.WebView", true);

                WebView_InitWebView = WebViewType.GetMethod("InitWebView", BindingFlags.Instance | BindingFlags.Public);
                WebView_LoadUrl = WebViewType.GetMethod("LoadURL", BindingFlags.Instance | BindingFlags.Public);
                WebView_LoadFile = WebViewType.GetMethod("LoadFile", BindingFlags.Instance | BindingFlags.Public);
                WebView_SetHostView = WebViewType.GetMethod("SetHostView", BindingFlags.Instance | BindingFlags.Public);
                WebView_SetSizeAndPosition = WebViewType.GetMethod("SetSizeAndPosition", BindingFlags.Instance | BindingFlags.Public);
                WebView_SetFocus = WebViewType.GetMethod("SetFocus", BindingFlags.Instance | BindingFlags.Public);
                WebView_SetApplicationFocus = WebViewType.GetMethod("SetApplicationFocus", BindingFlags.Instance | BindingFlags.Public);
                WebView_Show = WebViewType.GetMethod("Show", BindingFlags.Instance | BindingFlags.Public);
                WebView_Hide = WebViewType.GetMethod("Hide", BindingFlags.Instance | BindingFlags.Public);
                WebView_Back = WebViewType.GetMethod("Back", BindingFlags.Instance | BindingFlags.Public);
                WebView_Forward = WebViewType.GetMethod("Forward", BindingFlags.Instance | BindingFlags.Public);
                WebView_Reload = WebViewType.GetMethod("Reload", BindingFlags.Instance | BindingFlags.Public);
                WebView_IntPtrIsNull = WebViewType.GetMethod("IntPtrIsNull", BindingFlags.Instance | BindingFlags.NonPublic);

                if (WebView_InitWebView == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "InitWebView");
                }
                if (WebView_LoadUrl == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "LoadURL");
                }
                if (WebView_LoadFile == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "LoadFile");
                }
                if (WebView_SetHostView == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "SetHostView");
                }
                if (WebView_SetSizeAndPosition == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "SetSizeAndPosition");
                }
                if (WebView_SetFocus == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "SetFocus");
                }
                if (WebView_SetApplicationFocus == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "SetApplicationFocus");
                }
                if (WebView_Show == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "Show");
                }
                if (WebView_Hide == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "Hide");
                }
                if (WebView_Back == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "Back");
                }
                if (WebView_Forward == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "Forward");
                }
                if (WebView_Reload == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "Reload");
                }
                if (WebView_IntPtrIsNull == null)
                {
                    throw new MissingMemberException(WebViewType.FullName, "IntPtrIsNull");
                }

                EditorWindow_m_Parent = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            catch (Exception ex)
            {
                throw new UnityEditorInternalException(ex);
            }
        }

        private static readonly Type WebViewType;
        private static readonly MethodInfo WebView_InitWebView; // public extern void InitWebView(GUIView host, int x, int y, int width, int height, bool showResizeHandle);
        private static readonly MethodInfo WebView_LoadUrl; // public extern void LoadURL(string url);
        private static readonly MethodInfo WebView_LoadFile; // public extern void LoadFile(string path);
        private static readonly MethodInfo WebView_SetHostView; // public extern void SetHostView(GUIView view);
        private static readonly MethodInfo WebView_SetSizeAndPosition; // public extern void SetSizeAndPosition(int x, int y, int width, int height);
        private static readonly MethodInfo WebView_SetFocus; // public extern void SetFocus(bool value);
        private static readonly MethodInfo WebView_SetApplicationFocus; // public extern void SetApplicationFocus(bool applicationFocus);
        private static readonly MethodInfo WebView_Show; // public extern void Show();
        private static readonly MethodInfo WebView_Hide; // public extern void Hide();
        private static readonly MethodInfo WebView_Back; // public extern void Back();
        private static readonly MethodInfo WebView_Forward; // public extern void Forward();
        private static readonly MethodInfo WebView_Reload; // public extern void Reload();
        private static readonly MethodInfo WebView_IntPtrIsNull; // private extern bool IntPtrIsNull();
        private static readonly FieldInfo EditorWindow_m_Parent; // internal HostView m_Parent;

        private static object GetHostView(EditorWindow window)
        {
            if (window == null)
            {
                return null;
            }

            return EditorWindow_m_Parent.GetValue(window);
        }

        private static Rect ToWindowRect(Rect rect)
        {
            return LudiqGUIUtility.Unclip(new Rect(0, 0, rect.width, rect.height));
        }
    }
}
