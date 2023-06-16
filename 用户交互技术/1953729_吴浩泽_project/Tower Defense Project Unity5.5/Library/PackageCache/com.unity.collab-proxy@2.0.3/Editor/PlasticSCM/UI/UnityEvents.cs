using UnityEngine;

using Codice.Utils;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class Keyboard
    {
        internal static bool IsShiftPressed(Event e)
        {
            if (e == null)
                return false;

            return e.type == EventType.KeyDown
                && e.shift;
        }

        internal static bool IsReturnOrEnterKeyPressed(Event e)
        {
            if (e == null)
                return false;

            return IsKeyPressed(e, KeyCode.Return) ||
                   IsKeyPressed(e, KeyCode.KeypadEnter);
        }

        internal static bool IsKeyPressed(Event e, KeyCode keyCode)
        {
            if (e == null)
                return false;

            return e.type == EventType.KeyDown
                && e.keyCode == keyCode;
        }

        internal static bool IsControlOrCommandKeyPressed(Event e)
        {
            if (e == null)
                return false;

            if (PlatformIdentifier.IsMac())
                return e.type == EventType.KeyDown && e.command;

            return e.type == EventType.KeyDown && e.control;
        }
    }

    internal class Mouse
    {
        internal static bool IsLeftMouseButtonPressed(Event e)
        {
            if (e == null)
                return false;

            if (!e.isMouse)
                return false;

            return e.button == UnityConstants.LEFT_MOUSE_BUTTON
                && e.type == EventType.MouseDown;
        }

        internal static bool IsRightMouseButtonPressed(Event e)
        {
            if (e == null)
                return false;

            if (!e.isMouse)
                return false;

            return e.button == UnityConstants.RIGHT_MOUSE_BUTTON
                && e.type == EventType.MouseDown;
        }
    }
}
