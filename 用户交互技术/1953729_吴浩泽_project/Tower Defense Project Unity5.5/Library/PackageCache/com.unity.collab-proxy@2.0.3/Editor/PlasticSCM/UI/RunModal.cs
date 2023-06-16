using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class RunModal
    {
        static RunModal()
        {
            InitializeInfo();
        }

        internal static bool IsAvailable()
        {
            return mShowWithModeMethod != null
                && mCreateSavedGUIState != null
                && mApplyAndForgetMethod != null
                && mParentField != null
                && mParentWindowProp != null
                && mMakeModalMethod != null;
        }

        internal static void Dialog(EditorWindow window)
        {
            ShowAsUtility(window);

            object savedGUIState = CreateSavedGUIState();
            PushDispatcherContext(window);

            MakeModal(window);

            PopDispatcherContext(window);
            ApplySavedGUIState(savedGUIState);
        }

        static void MakeModal(EditorWindow window)
        {
            // MakeModal(m_Parent.window);
            var hostView = mParentField.GetValue(window);
            var parentWindow = mParentWindowProp.GetValue(hostView, null);

            mMakeModalMethod.Invoke(
                mMakeModalMethod.IsStatic ? null : window,
                new object[] { parentWindow });
        }

        static void ShowAsUtility(EditorWindow window)
        {
            // ShowWithMode(ShowMode.Utility);
            mShowWithModeMethod.Invoke(window, new object[] { 2 });
        }

        static object CreateSavedGUIState()
        {
            // SavedGUIState guiState = SavedGUIState.Create();
            return mCreateSavedGUIState.Invoke(null, null);
        }

        static void ApplySavedGUIState(object savedGUIState)
        {
            // guiState.ApplyAndForget();
            mApplyAndForgetMethod.Invoke(savedGUIState, null);
        }

        static void PopDispatcherContext(EditorWindow window)
        {
#if UNITY_2020_1_OR_NEWER
            //UnityEngine.UIElements.EventDispatcher.editorDispatcher.PopDispatcherContext();

            object editorDispatcher = mEditorDispatcherProp2020.GetValue(null);
            mPopContextMethod2020.Invoke(editorDispatcher, null);
#else
            // m_Parent.visualTree.panel.dispatcher?.PopDispatcherContext();

            object dispatcher = GetDispatcher(window);

            if (dispatcher != null)
                mPopContextMethod.Invoke(dispatcher, null);
#endif
        }

        static void PushDispatcherContext(EditorWindow window)
        {
#if UNITY_2020_1_OR_NEWER
            //UnityEngine.UIElements.EventDispatcher.editorDispatcher.PushDispatcherContext();

            object editorDispatcher = mEditorDispatcherProp2020.GetValue(null);
            mPushContextMethod2020.Invoke(editorDispatcher, null);
#else
            // m_Parent.visualTree.panel.dispatcher?.PushDispatcherContext();

            object dispatcher = GetDispatcher(window);

            if (dispatcher != null)
                mPushContextMethod.Invoke(dispatcher, null);
#endif
        }

        static object GetDispatcher(EditorWindow window)
        {
            object dispatcher = null;
            if (MayHaveDispatcher())
            {
                var parent = mParentField.GetValue(window);
                if (parent != null)
                {
                    var visualTree = mVisualTreeProp.GetValue(parent, null);
                    if (visualTree != null)
                    {
                        var panel = mPanelProp.GetValue(visualTree, null);
                        if (panel != null)
                        {
                            dispatcher = mDispatcherProp.GetValue(panel, null);
                        }
                    }
                }
            }

            return dispatcher;
        }

        static bool MayHaveDispatcher()
        {
            return mDispatcherType != null
                && mPushContextMethod != null
                && mPopContextMethod != null;
        }

        static void InitializeInfo()
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
            mMakeModalMethod = BuildMakeModalMethodInfo(flags);
            mShowWithModeMethod = typeof(EditorWindow).GetMethod("ShowWithMode", flags);
            mParentField = typeof(EditorWindow).GetField("m_Parent", flags);
            var hostViewType = mParentField.FieldType;
            mParentWindowProp = hostViewType.GetProperty("window", flags);

            var savedGUIStateType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SavedGUIState");
            mCreateSavedGUIState = savedGUIStateType.GetMethod("Create", flags);
            mApplyAndForgetMethod = savedGUIStateType.GetMethod("ApplyAndForget", flags);

#if UNITY_2020_1_OR_NEWER
            mEditorDispatcherProp2020 = typeof(UnityEngine.UIElements.EventDispatcher).GetProperty("editorDispatcher", flags);
            mPushContextMethod2020 = mEditorDispatcherProp2020.PropertyType.GetMethod("PushDispatcherContext", flags);
            mPopContextMethod2020 = mEditorDispatcherProp2020.PropertyType.GetMethod("PopDispatcherContext", flags);
#endif
            flags = BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.Public;

            mParentField = typeof(EditorWindow).GetField("m_Parent", flags);
            if (mParentField != null)
                hostViewType = mParentField.FieldType;
            if (hostViewType != null)
                mVisualTreeProp = hostViewType.GetProperty("visualTree");
            if (mVisualTreeProp != null)
            {
                var visualTreeType = mVisualTreeProp.PropertyType;
                if (visualTreeType != null)
                {
                    mPanelProp = visualTreeType.GetProperty("panel");
                    if (mPanelProp != null)
                    {
                        var panelType = mPanelProp.PropertyType;
                        if (panelType != null)
                        {
                            mDispatcherProp = panelType.GetProperty("dispatcher");
                            if (mDispatcherProp != null)
                            {
                                mDispatcherType = mDispatcherProp.PropertyType;
                                if (mDispatcherType != null)
                                {
                                    mPushContextMethod = mDispatcherType.GetMethod("PushDispatcherContext", flags);
                                    mPopContextMethod = mDispatcherType.GetMethod("PopDispatcherContext", flags);
                                }
                            }
                        }
                    }
                }
            }
        }

        static MethodInfo BuildMakeModalMethodInfo(BindingFlags flags)
        {
            if (EditorVersion.IsCurrentEditorOlderThan("2019.3.10f1"))
                return typeof(EditorWindow).GetMethod("MakeModal", flags);

            return typeof(EditorWindow).GetMethod("Internal_MakeModal", flags);
        }

        static FieldInfo mParentField;
        static PropertyInfo mParentWindowProp;
        static MethodInfo mMakeModalMethod;
        static MethodInfo mShowWithModeMethod;
        static MethodInfo mCreateSavedGUIState;
        static MethodInfo mApplyAndForgetMethod;
        static PropertyInfo mVisualTreeProp;
        static Type mDispatcherType;
        static MethodInfo mPushContextMethod;
        static MethodInfo mPopContextMethod;
        static PropertyInfo mPanelProp;
        static PropertyInfo mDispatcherProp;

#if UNITY_2020_1_OR_NEWER
        static PropertyInfo mEditorDispatcherProp2020;
        static MethodInfo mPushContextMethod2020;
        static MethodInfo mPopContextMethod2020;
#endif

        // // How ContainerWindows are visualized. Used with ContainerWindow.Show
        // internal enum ShowMode
        // {
        // 	// Show as a normal window with max, min & close buttons.
        // 	NormalWindow = 0,
        // 	// Used for a popup menu. On mac this means light shadow and no titlebar.
        // 	PopupMenu = 1,
        // 	// Utility window - floats above the app. Disappears when app loses focus.
        // 	Utility = 2,
        // 	// Window has no shadow or decorations. Used internally for dragging stuff around.
        // 	NoShadow = 3,
        // 	// The Unity main window. On mac, this is the same as NormalWindow, except window doesn't have a close button.
        // 	MainWindow = 4,
        // 	// Aux windows. The ones that close the moment you move the mouse out of them.
        // 	AuxWindow = 5,
        // 	// Like PopupMenu, but without keyboard focus
        // 	Tooltip = 6
        // }
    }
}
