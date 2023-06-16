using System;
using UnityEngine;
using GUIEvent = UnityEngine.Event;

namespace Unity.VisualScripting
{
    public sealed class EventWrapper
    {
        private static GUIEvent e => GUIEvent.current;

        public int controlHint { get; private set; }

        public int control { get; private set; }

        public EventWrapper(int controlHint)
        {
            this.controlHint = controlHint;
        }

        public EventWrapper(object controlHint) : this(controlHint?.GetHashCode() ?? 0) { }

        public EventWrapper() : this("EventWrapper") { }

        public event Action mouseCaptured;

        public event Action mouseReleased;

        public event Action keyboardCaptured;

        public event Action keyboardReleased;

        public bool supportsKeyboard { get; private set; }

        public void RegisterControl(FocusType focusType)
        {
            control = GUIUtility.GetControlID(controlHint, focusType);
            supportsKeyboard = focusType == FocusType.Keyboard;
        }

        private bool shouldReleaseMouse;

        private bool canCaptureMouse;

        private bool canCaptureKeyboard;

        public void HandleCapture(bool canCaptureMouse, bool canCaptureKeyboard)
        {
            this.canCaptureMouse = canCaptureMouse;
            this.canCaptureKeyboard = canCaptureKeyboard;

            if (e.type == EventType.MouseDown)
            {
                //Debug.Log($"Clicked on {control}\nHot: {GUIUtility.hotControl}, Can Capture: {canCaptureMouse}");

                if (couldControlMouse && this.canCaptureMouse)
                {
                    CaptureMouse();
                }

                if (supportsKeyboard && this.canCaptureKeyboard)
                {
                    CaptureKeyboard();
                }
            }

            // Cache this here in case the code after HandleCapture uses the MouseUp event.
            shouldReleaseMouse = e.rawType == EventType.MouseUp;
        }

        public void HandleRelease()
        {
            if (shouldReleaseMouse)
            {
                ReleaseMouse();
            }
        }

        public bool controlsMouse => GUIUtility.hotControl == control;

        public bool controlsKeyboard => GUIUtility.keyboardControl == control;

        public static bool couldControlMouse => GUIUtility.hotControl == 0;

        public static bool couldControlKeyboard => GUIUtility.keyboardControl == 0;

        public EventType freeType => e.type;

        public EventType rawType => e.rawType;

        public EventType controlType => e.GetTypeForControl(control);

        public EventType mouseType => controlsMouse ? controlType : EventType.Ignore;

        public EventType keyboardType => controlsKeyboard ? controlType : EventType.Ignore;

        public void CaptureMouse()
        {
            if (controlsMouse)
            {
                return;
            }

            GUIUtility.hotControl = control;
            mouseCaptured?.Invoke();
        }

        public void ReleaseMouse()
        {
            if (!controlsMouse)
            {
                return;
            }

            GUIUtility.hotControl = 0;
            mouseReleased?.Invoke();
        }

        public void CaptureKeyboard()
        {
            if (!supportsKeyboard)
            {
                throw new NotSupportedException("Use FocusType.Keyboard to enable keyboard control.");
            }

            if (controlsKeyboard)
            {
                return;
            }

            GUIUtility.keyboardControl = control;
            keyboardCaptured?.Invoke();
        }

        public void ReleaseKeyboard()
        {
            if (!supportsKeyboard)
            {
                throw new NotSupportedException("Use FocusType.Keyboard to enable keyboard control.");
            }

            if (!controlsKeyboard)
            {
                return;
            }

            GUIUtility.keyboardControl = 0;
            keyboardReleased?.Invoke();
        }

        public bool IsUsed => controlType == EventType.Used;
        public bool IsRepaint => controlType == EventType.Repaint;
        public bool IsLayout => controlType == EventType.Layout;

        public bool IsAnyMouse => controlsMouse && e.isMouse;
        public bool IsAnyMouseDown => mouseType == EventType.MouseDown;
        public bool IsAnyMouseUp => mouseType == EventType.MouseUp;
        public bool IsAnyMouseDrag => mouseType == EventType.MouseDrag;
        public bool IsMouseMove => mouseType == EventType.MouseMove;
        public bool IsMouseDown(MouseButton button) => IsAnyMouseDown && mouseButton == button;
        public bool IsMouseDown(MouseButton button, EventModifiers modifiers) => IsMouseDown(button) && this.modifiers == modifiers;
        public bool IsMouseUp(MouseButton button) => IsAnyMouseUp && mouseButton == button;
        public bool IsMouseUp(MouseButton button, EventModifiers modifiers) => IsMouseUp(button) && this.modifiers == modifiers;
        public bool IsMouseDrag(MouseButton button) => IsAnyMouseDrag && mouseButton == button;
        public bool IsMouseDrag(MouseButton button, EventModifiers modifiers) => IsMouseDrag(button) && this.modifiers == modifiers;

        public bool IsAnyKeyboard => controlsMouse && e.isKey;
        public bool IsAnyKeyDown => keyboardType == EventType.KeyDown;
        public bool IsAnyKeyUp => keyboardType == EventType.KeyUp;
        public bool IsKeyDown(KeyCode key) => IsAnyKeyDown && keyCode == key;
        public bool IsKeyDown(KeyCode key, EventModifiers modifiers) => IsKeyDown(key) && this.modifiers == modifiers;
        public bool IsKeyUp(KeyCode key) => IsAnyKeyUp && keyCode == key;
        public bool IsKeyUp(KeyCode key, EventModifiers modifiers) => IsKeyUp(key) && this.modifiers == modifiers;

        public bool IsContextClick => canCaptureMouse && (controlType == EventType.ContextClick || (IsKeyDown(KeyCode.E) && ctrlOrCmd));

        public bool IsValidateCommand(string name) => keyboardType == EventType.ValidateCommand && commandName == name;
        public bool IsExecuteCommand(string name) => keyboardType == EventType.ExecuteCommand && commandName == name;

        public bool IsFree(EventType type) => freeType == type;
        public bool IsRaw(EventType type) => rawType == type;

        public void Use()
        {
            e.Use();
        }

        public void TryUse()
        {
            e?.TryUse();
        }

        public void ValidateCommand()
        {
            if (controlType != EventType.ValidateCommand)
            {
                throw new InvalidOperationException();
            }

            // In Unity, validating a command means using the ValidateCommand event.
            Use();
        }

        public Vector2 mousePosition => e.mousePosition;
        public Vector2 mouseDelta => e.delta;
        public int clickCount => e.clickCount;
        public KeyCode keyCode => e.keyCode;
        public string commandName => e.commandName;

        public EventModifiers modifiers => e.modifiers;
        public bool alt => e.alt;
        public bool shift => e.shift;
        public bool ctrl => e.control;
        public bool cmd => e.command;
        public bool ctrlOrCmd => Application.platform == RuntimePlatform.OSXEditor ? cmd : ctrl;

        public MouseButton mouseButton
        {
            get
            {
                if (Application.platform == RuntimePlatform.OSXEditor && e.control && e.button == (int)MouseButton.Left)
                {
                    return MouseButton.Right;
                }

                return (MouseButton)e.button;
            }
        }
    }
}
