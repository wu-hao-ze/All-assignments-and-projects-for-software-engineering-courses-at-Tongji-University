using System;

using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class TabButton
    {
        internal bool DrawTabButton(
            string buttonText,
            bool wasActive,
            float width)
        {
            bool isCloseButtonClicked;

            return DrawClosableTabButton(
                buttonText,
                wasActive,
                false,
                width,
                null,
                out isCloseButtonClicked);
        }

        internal bool DrawClosableTabButton(
            string buttonText,
            bool wasActive,
            bool isClosable,
            float width,
            Action repaintAction,
            out bool isCloseButtonClicked)
        {
            isCloseButtonClicked = false;

            GUIContent buttonContent = new GUIContent(buttonText);

            GUIStyle buttonStyle = UnityStyles.PlasticWindow.TabButton;

            Rect toggleRect = GUILayoutUtility.GetRect(
                buttonContent, buttonStyle,
                GUILayout.Width(width));

            if (isClosable && Event.current.type == EventType.MouseMove)
            {
                if (mCloseButtonRect.Contains(Event.current.mousePosition))
                {
                    SetCloseButtonState(
                        CloseButtonState.Hovered,
                        repaintAction);
                }
                else
                {
                    SetCloseButtonState(
                        CloseButtonState.Normal,
                        repaintAction);
                }
            }

            if (isClosable && Event.current.type == EventType.MouseDown)
            {
                if (mCloseButtonRect.Contains(Event.current.mousePosition))
                {
                    SetCloseButtonState(
                        CloseButtonState.Clicked,
                        repaintAction);
                    Event.current.Use();
                }
            }

            if (isClosable && Event.current.type == EventType.MouseUp)
            {
                if (mCloseButtonRect.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    isCloseButtonClicked = true;
                }

                if (IsTabClickWithMiddleButton(toggleRect, Event.current))
                {
                    Event.current.Use();
                    isCloseButtonClicked = true;
                }

                SetCloseButtonState(
                    CloseButtonState.Normal,
                    repaintAction);
            }

            bool isActive = GUI.Toggle(
                toggleRect, wasActive, buttonText, buttonStyle);

            if (isClosable && toggleRect.height > 1)
            {
                mCloseButtonRect = DrawCloseButton(
                    toggleRect,
                    mCloseButtonState);
            }

            if (wasActive)
            {
                DrawUnderline(toggleRect);
            }

            return isActive;
        }

        static Rect DrawCloseButton(
            Rect toggleRect,
            CloseButtonState state)
        {
            int closeButtonSize = 15;

            GUIContent closeImage = new GUIContent(GetCloseImage(state));

            Rect closeTabRect = new Rect(
                toggleRect.xMax - closeButtonSize - 1,
                toggleRect.y + (toggleRect.height / 2 - closeButtonSize / 2),
                closeButtonSize,
                closeButtonSize);

            GUI.Button(closeTabRect, closeImage, EditorStyles.label);

            return new Rect(
                closeTabRect.x - 1,
                closeTabRect.y - 1,
                closeTabRect.width + 2,
                closeTabRect.height + 2);
        }

        static void DrawUnderline(Rect toggleRect)
        {
            GUIStyle activeTabStyle =
                UnityStyles.PlasticWindow.ActiveTabUnderline;

            Rect underlineRect = new Rect(
                toggleRect.x,
                toggleRect.yMax - (activeTabStyle.fixedHeight / 2),
                toggleRect.width,
                activeTabStyle.fixedHeight);

            GUI.Label(underlineRect, string.Empty, activeTabStyle);
        }

        static bool IsTabClickWithMiddleButton(Rect toggleRect, Event currentEvent)
        {
            if (currentEvent.button != 2)
                return false;

            return toggleRect.height > 1 &&
                   toggleRect.Contains(Event.current.mousePosition);
        }

        static Texture GetCloseImage(CloseButtonState state)
        {
            if (state == CloseButtonState.Hovered)
                return Images.GetHoveredCloseIcon();

            if (state == CloseButtonState.Clicked)
                return Images.GetClickedCloseIcon();

            return Images.GetCloseIcon();
        }

        void SetCloseButtonState(
            CloseButtonState newState,
            Action repaintAction)
        {
            if (mCloseButtonState == newState)
                return;

            mCloseButtonState = newState;

            if (repaintAction != null)
                repaintAction();
        }

        Rect mCloseButtonRect;
        CloseButtonState mCloseButtonState;

        enum CloseButtonState
        {
            Normal,
            Clicked,
            Hovered,
        }
    }
}
