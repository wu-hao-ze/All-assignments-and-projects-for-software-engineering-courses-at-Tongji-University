using System;
using System.Reflection;

using UnityEditor;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DockEditorWindow
    {
        static DockEditorWindow()
        {
            InitializeInfo();
        }

        internal static bool IsAvailable()
        {
            return mParentField != null
                && mAddTabMethod != null;
        }

        internal static void To(EditorWindow dockWindow, EditorWindow window)
        {
            var dockArea = mParentField.GetValue(dockWindow);

            mAddTabMethod.Invoke(dockArea, new object[] { window });
        }

        static void InitializeInfo()
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

            mParentField = typeof(EditorWindow).GetField("m_Parent", flags);

            var dockAreaType = typeof(EditorWindow).Assembly.GetType("UnityEditor.DockArea");

            if (dockAreaType == null)
                return;

            mAddTabMethod = dockAreaType.GetMethod("AddTab", flags,
                null, new Type[] { typeof(EditorWindow) }, null);
        }

        static MethodInfo mAddTabMethod;
        static FieldInfo mParentField;
    }
}
