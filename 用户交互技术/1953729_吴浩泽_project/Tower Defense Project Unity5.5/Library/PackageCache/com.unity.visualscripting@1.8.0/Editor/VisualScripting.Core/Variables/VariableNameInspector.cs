using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public sealed class VariableNameInspector : Inspector
    {
        public VariableNameInspector(Metadata metadata, Func<IEnumerable<string>> getSuggestions) : base(metadata)
        {
            Ensure.That(nameof(getSuggestions)).IsNotNull(getSuggestions);

            this.getSuggestions = getSuggestions;
        }

        public Func<IEnumerable<string>> getSuggestions { get; }


        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, GetFieldHeight(width, label), label);
        }

        private float GetFieldHeight(float width, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private bool mouseInRect;
        private bool shouldRecalculateSuggestions;
        private string[] suggestions;

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var fieldPosition = position.VerticalSection(ref y, GetFieldHeight(position.width, GUIContent.none));

            var textFieldPosition = new Rect
                (
                fieldPosition.x,
                fieldPosition.y,
                fieldPosition.width - Styles.popup.fixedWidth,
                fieldPosition.height
                );

            var popupPosition = new Rect
                (
                textFieldPosition.xMax,
                fieldPosition.y,
                Styles.popup.fixedWidth,
                fieldPosition.height
                );

            var newValue = EditorGUI.TextField(textFieldPosition, (string)metadata.value, Styles.textField);

            CalculateVariableNameSuggestions(position);

            EditorGUI.BeginDisabledGroup(suggestions.Length == 0);

            var currentSuggestionIndex = Array.IndexOf(suggestions, (string)metadata.value);

            EditorGUI.BeginChangeCheck();

            var newSuggestionIndex = EditorGUI.Popup(popupPosition, currentSuggestionIndex, suggestions, Styles.popup);

            if (EditorGUI.EndChangeCheck())
            {
                newValue = suggestions[newSuggestionIndex];
            }

            EditorGUI.EndDisabledGroup();

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        private void CalculateVariableNameSuggestions(Rect position)
        {
            // Calling getSuggestions() can be *very* expensive because it's a delegate, so we're trying to call it as little as possible.
            // See BOLT-2201 (https://jira.unity3d.com/browse/BOLT-2201) for details

            // After we've calculated it the first time, we still need to recalculate it when there are potentially new variables
            // There is no callback for us to check when an enum popup is opened, so instead we recalculate when the user hovers their mouse over the popup
            if (position.Contains(e.mousePosition))
            {
                if (!mouseInRect)
                {
                    shouldRecalculateSuggestions = true;
                }
                mouseInRect = true;
            }
            else
            {
                mouseInRect = false;
            }

            if (shouldRecalculateSuggestions)
            {
                suggestions = getSuggestions().ToArray();
                shouldRecalculateSuggestions = false;
            }

            // If we haven't calculated it yet, we need to do it even if we don't show the popup so we know whether or not to disable the popup in the first place
            suggestions ??= getSuggestions().ToArray();
        }

        public override float GetAdaptiveWidth()
        {
            return Mathf.Max(30, EditorStyles.textField.CalcSize(new GUIContent(metadata.value?.ToString())).x + 1 + Styles.popup.fixedWidth);
        }

        public static class Styles
        {
            static Styles()
            {
                textField = new GUIStyle(EditorStyles.textField);

                popup = new GUIStyle("TextFieldDropDown");
                popup.fixedWidth = 18;
                popup.clipping = TextClipping.Clip;
                popup.normal.textColor = ColorPalette.transparent;
                popup.active.textColor = ColorPalette.transparent;
                popup.hover.textColor = ColorPalette.transparent;
                popup.focused.textColor = ColorPalette.transparent;
                popup.onNormal.textColor = ColorPalette.transparent;
                popup.onActive.textColor = ColorPalette.transparent;
                popup.onHover.textColor = ColorPalette.transparent;
                popup.onFocused.textColor = ColorPalette.transparent;
            }

            public static readonly GUIStyle textField;
            public static readonly GUIStyle popup;
        }
    }
}
