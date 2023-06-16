using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI
{
    internal abstract class PlasticDialog : EditorWindow, IPlasticDialogCloser
    {
        protected virtual Rect DefaultRect
        {
            get
            {
                int pixelWidth = Screen.currentResolution.width;
                float x = (pixelWidth - DEFAULT_WIDTH) / 2;
                return new Rect(x, 200, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            }
        }

        protected virtual bool IsResizable { get; set; }

        internal void OkButtonAction()
        {
            CompleteModal(ResponseType.Ok);
        }

        internal void CancelButtonAction()
        {
            CompleteModal(ResponseType.Cancel);
        }

        internal void CloseButtonAction()
        {
            CompleteModal(ResponseType.None);
        }

        internal void ApplyButtonAction()
        {
            CompleteModal(ResponseType.Apply);
        }

        internal ResponseType RunModal(EditorWindow parentWindow)
        {
            InitializeVars(parentWindow);

            if (!IsResizable)
                MakeNonResizable();

            if (UI.RunModal.IsAvailable())
            {
                UI.RunModal.Dialog(this);
                return mAnswer;
            }

            EditorUtility.DisplayDialog(
                PlasticLocalization.GetString(PlasticLocalization.Name.UnityVersionControl),
                PlasticLocalization.GetString(PlasticLocalization.Name.PluginModalInformation),
                PlasticLocalization.GetString(PlasticLocalization.Name.CloseButton));
            return ResponseType.None;
        }

        protected void OnGUI()
        {
            try
            {
                // If the Dialog has been saved into the Unity editor layout and persisted between restarts, the methods
                // to configure the dialogs will be skipped. Simple fix here is to close it when this state is detected.
                // Fixes a NPE loop when the state mentioned above is occurring.
                if (!mIsConfigured)
                {
                    mIsClosed = true;
                    Close();
                    return;
                }

                if (Event.current.type == EventType.Layout)
                {
                    EditorDispatcher.Update();
                }

                if (!mFocusedOnce)
                {
                    // Somehow the prevents the dialog from jumping when dragged
                    // NOTE(rafa): We cannot do every frame because the modal kidnaps focus for all processes (in mac at least)
                    Focus();
                    mFocusedOnce = true;
                }

                ProcessKeyActions();

                if (mIsClosed)
                    return;

                GUI.Box(new Rect(0, 0, position.width, position.height), GUIContent.none, EditorStyles.label);

                float margin = 25;
                float marginTop = 25;
                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(position.height)))
                {
                    GUILayout.Space(margin);
                    using (new EditorGUILayout.VerticalScope(GUILayout.Height(position.height)))
                    {
                        GUILayout.Space(marginTop);
                        OnModalGUI();
                        GUILayout.Space(margin);
                    }
                    GUILayout.Space(margin);
                }

                var lastRect = GUILayoutUtility.GetLastRect();
                float desiredHeight = lastRect.yMax;
                Rect newPos = position;
                newPos.height = desiredHeight;
                if (position.height < desiredHeight)
                    position = newPos;

                if (Event.current.type == EventType.Repaint)
                {
                    if (mIsCompleted)
                    {
                        mIsClosed = true;
                        Close();
                    }
                }
            }
            finally
            {
                if (mIsClosed)
                    EditorGUIUtility.ExitGUI();
            }
        }

        void OnDestroy()
        {
            if (!mIsConfigured)
                return;

            SaveSettings();

            if (mParentWindow == null)
                return;

            mParentWindow.Focus();
        }

        protected virtual void SaveSettings() { }
        protected abstract void OnModalGUI();
        protected abstract string GetTitle();

        protected void Paragraph(string text)
        {
            GUILayout.Label(text, UnityStyles.Paragraph);
            GUILayout.Space(DEFAULT_PARAGRAPH_SPACING);
        }

        protected void TextBlockWithEndLink(
            string url, string formattedExplanation,
            GUIStyle textblockStyle)
        {
            DrawTextBlockWithEndLink.For(url, formattedExplanation, textblockStyle);
        }

        protected static void Title(string text)
        {
            GUILayout.Label(text, UnityStyles.Dialog.Toggle);
        }

        protected static bool TitleToggle(string text, bool isOn)
        {
            return EditorGUILayout.ToggleLeft(text, isOn, UnityStyles.Dialog.Toggle);
        }

        protected static bool TitleToggle(string text, bool isOn, GUIStyle style)
        {
            return EditorGUILayout.ToggleLeft(text, isOn, style);
        }

        protected static string TextEntry(
            string label,
            string value,
            float width,
            float x)
        {
            return TextEntry(
                label,
                value,
                null,
                width,
                x);
        }

        protected static string TextEntry(
            string label, string value, string controlName, float width, float x)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EntryLabel(label);

                GUILayout.FlexibleSpace();

                var rt = GUILayoutUtility.GetRect(
                    new GUIContent(value), UnityStyles.Dialog.EntryLabel);
                rt.width = width;
                rt.x = x;

                if (!string.IsNullOrEmpty(controlName))
                    GUI.SetNextControlName(controlName);

                return GUI.TextField(rt, value);
            }
        }

        protected static string ComboBox(
            string label,
            string value,
            string controlName,
            List<string> dropDownOptions,
            GenericMenu.MenuFunction2 optionSelected,
            float width,
            float x)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EntryLabel(label);

                GUILayout.FlexibleSpace();

                var rt = GUILayoutUtility.GetRect(
                    new GUIContent(value), UnityStyles.Dialog.EntryLabel);
                rt.width = width;
                rt.x = x;

                return DropDownTextField.DoDropDownTextField(
                    value,
                    label,
                    dropDownOptions,
                    optionSelected,
                    rt);
            }
        }

        protected static string PasswordEntry(
            string label, string value, float width, float x)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EntryLabel(label);

                GUILayout.FlexibleSpace();

                var rt = GUILayoutUtility.GetRect(
                    new GUIContent(value), UnityStyles.Dialog.EntryLabel);
                rt.width = width;
                rt.x = x;

                return GUI.PasswordField(rt, value, '*');
            }
        }

        protected static bool ToggleEntry(
            string label, bool value, float width, float x)
        {
            var rt = GUILayoutUtility.GetRect(
                new GUIContent(label), UnityStyles.Dialog.EntryLabel);
            rt.width = width;
            rt.x = x;

            return GUI.Toggle(rt, value, label);
        }

        protected static bool NormalButton(string text)
        {
            return GUILayout.Button(
                text, UnityStyles.Dialog.NormalButton,
                GUILayout.MinWidth(80),
                GUILayout.Height(25));
        }

        void IPlasticDialogCloser.CloseDialog()
        {
            OkButtonAction();
        }

        void ProcessKeyActions()
        {
            Event e = Event.current;

            if (mEnterKeyAction != null &&
                Keyboard.IsReturnOrEnterKeyPressed(e))
            {
                mEnterKeyAction.Invoke();
                e.Use();
                return;
            }

            if (mEscapeKeyAction != null &&
                Keyboard.IsKeyPressed(e, KeyCode.Escape))
            {
                mEscapeKeyAction.Invoke();
                e.Use();
                return;
            }
        }

        protected static bool AcceptButton(string text, int extraWidth = 10)
        {
            GUI.color = new Color(0.098f, 0.502f, 0.965f, .8f);

            int textWidth = (int)((GUIStyle)UnityStyles.Dialog.AcceptButtonText)
                .CalcSize(new GUIContent(text)).x;

            bool pressed = GUILayout.Button(
                string.Empty, GetEditorSkin().button,
                GUILayout.MinWidth(Math.Max(80, textWidth + extraWidth)),
                GUILayout.Height(25));

            GUI.color = Color.white;

            Rect buttonRect = GUILayoutUtility.GetLastRect();
            GUI.Label(buttonRect, text, UnityStyles.Dialog.AcceptButtonText);

            return pressed;
        }

        void CompleteModal(ResponseType answer)
        {
            mIsCompleted = true;
            mAnswer = answer;
        }

        void InitializeVars(EditorWindow parentWindow)
        {
            mIsConfigured = true;
            mIsCompleted = false;
            mIsClosed = false;
            mAnswer = ResponseType.Cancel;

            titleContent = new GUIContent(GetTitle());

            mFocusedOnce = false;

            position = DefaultRect;
            mParentWindow = parentWindow;
        }

        void MakeNonResizable()
        {
            maxSize = DefaultRect.size;
            minSize = maxSize;
        }

        static void EntryLabel(string labelText)
        {
            GUIContent labelContent = new GUIContent(labelText);
            GUIStyle labelStyle = UnityStyles.Dialog.EntryLabel;

            Rect rt = GUILayoutUtility.GetRect(labelContent, labelStyle);

            GUI.Label(rt, labelText, EditorStyles.label);
        }

        static GUISkin GetEditorSkin()
        {
            return EditorGUIUtility.isProSkin ?
                EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene) :
                EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        }

        bool mIsConfigured;
        bool mIsCompleted;
        bool mIsClosed;
        ResponseType mAnswer;

        protected Action mEnterKeyAction = null;
        protected Action mEscapeKeyAction = null;

        bool mFocusedOnce;

        Dictionary<string, string[]> mWrappedTextLines =
            new Dictionary<string, string[]>();

        EditorWindow mParentWindow;

        protected const float DEFAULT_LINE_SPACING = -5f;
        const float DEFAULT_WIDTH = 500f;
        const float DEFAULT_HEIGHT = 180f;
        const float DEFAULT_PARAGRAPH_SPACING = 10f;

        static class BuildLine
        {
            internal static string ForIndex(string text, int index)
            {
                if (index < 0 || index > text.Length)
                    return string.Empty;

                return text.Substring(index).Trim();
            }

            internal static string ForIndexAndLenght(string text, int index, int lenght)
            {
                if (index < 0 || index > text.Length)
                    return string.Empty;

                return text.Substring(index, lenght);
            }
        }
    }
}
