using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface ICanvasWindow
    {
        GraphReference reference { get; set; }

        bool maximized { get; set; }

        bool graphInspectorEnabled { get; set; }

        bool variablesInspectorEnabled { get; set; }


        void AddCursorRect(Rect rect, MouseCursor cursor);
    }

    public static class XCanvasWindow
    {
        public static bool IsFocused(this ICanvasWindow window)
        {
            return EditorWindow.focusedWindow == (EditorWindow)window;
        }

        public static void Focus(this ICanvasWindow window)
        {
            ((EditorWindow)window).Focus();
        }
    }
}
