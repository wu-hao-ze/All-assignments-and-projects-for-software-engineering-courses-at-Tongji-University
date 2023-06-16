using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class DocumentedOption<T> : FuzzyOption<T>
    {
        public XmlDocumentationTags documentation { get; protected set; }

        public bool zoom { get; protected set; }

        public bool showType { get; protected set; }

        public override bool hasFooter => documentation != null;

        private bool showIcon => zoom && icon != null;

        public override float GetFooterHeight(float width)
        {
            if (showIcon)
            {
                width -= Styles.zoomSize + Styles.zoomSpacing;
            }

            var height = 0f;

            width -= 2;

            if (documentation.summary != null)
            {
                height += GetSummaryHeight(width);
            }

            foreach (var parameterDocumentation in documentation.parameters)
            {
                Type parameterType = null;

                if (documentation.parameterTypes.ContainsKey(parameterDocumentation.Key))
                {
                    parameterType = documentation.parameterTypes[parameterDocumentation.Key];
                }

                height += GetParameterHeight(parameterDocumentation, parameterType, width);
            }

            if (documentation.returns != null)
            {
                height += GetReturnsHeight(width);
            }

            if (documentation.remarks != null)
            {
                height += GetRemarksHeight(width);
            }

            if (showIcon)
            {
                return Mathf.Max(Styles.zoomSize + 2 * Styles.zoomSpacing, height);
            }

            return height;
        }

        public override void OnFooterGUI(Rect position)
        {
            if (showIcon)
            {
                var zoomPosition = new Rect
                    (
                    position.x + Styles.zoomSpacing,
                    position.y + Styles.zoomSpacing,
                    Styles.zoomSize,
                    Styles.zoomSize
                    );

                position.x += Styles.zoomSize + Styles.zoomSpacing;
                position.width -= Styles.zoomSize + Styles.zoomSpacing;

                GUI.DrawTexture(zoomPosition, icon?[IconSize.Medium]);
            }

            var y = position.y;

            if (documentation.summary != null)
            {
                var summaryPosition = new Rect
                    (
                    position.x,
                    y,
                    position.width,
                    GetSummaryHeight(position.width)
                    );

                OnSummaryGUI(summaryPosition);

                y = summaryPosition.yMax;
            }

            if (documentation.parameters.Count > 0)
            {
                var parameterPosition = new Rect
                    (
                    position.x,
                    y,
                    position.width,
                    0
                    );

                foreach (var parameterDocumentation in documentation.parameters)
                {
                    Type parameterType = null;

                    if (documentation.parameterTypes.ContainsKey(parameterDocumentation.Key))
                    {
                        parameterType = documentation.parameterTypes[parameterDocumentation.Key];
                    }

                    parameterPosition.height = GetParameterHeight(parameterDocumentation, parameterType, position.width);

                    OnParameterGUI(parameterPosition, parameterDocumentation, parameterType);

                    parameterPosition.y += parameterPosition.height;

                    y = parameterPosition.y;
                }
            }

            if (documentation.returns != null)
            {
                var returnsPosition = new Rect
                    (
                    position.x,
                    y,
                    position.width,
                    GetReturnsHeight(position.width)
                    );

                OnReturnsGUI(returnsPosition);

                y = returnsPosition.yMax;
            }

            if (documentation.remarks != null)
            {
                var remarksPosition = new Rect
                    (
                    position.x,
                    y,
                    position.width,
                    GetRemarksHeight(position.width)
                    );

                OnRemarksGUI(remarksPosition);

                y = remarksPosition.yMax;
            }
        }

        private float GetSummaryHeight(float width)
        {
            return Styles.summary.CalcHeight(new GUIContent(documentation.summary), width);
        }

        private void OnSummaryGUI(Rect summaryPosition)
        {
            EditorGUI.LabelField(summaryPosition, documentation.summary, Styles.summary);
        }

        private float GetParameterReturnsHeight(GUIContent label, float width)
        {
            width -= IconSize.Small + Styles.iconSpacing;

            return Styles.parameterSummary.CalcHeight(label, width);
        }

        private void OnParameterReturnsGUI(Type type, GUIContent label, Rect position)
        {
            var x = position.x + Styles.parameterSummary.padding.left;
            var width = position.width;

            if (type != null)
            {
                var icon = type.Icon()?[IconSize.Small];

                if (icon != null)
                {
                    var iconPosition = new Rect
                        (
                        x,
                        position.y - 1,
                        IconSize.Small,
                        IconSize.Small
                        );

                    x += iconPosition.width + Styles.iconSpacing;
                    width -= iconPosition.width + Styles.iconSpacing;

                    GUI.DrawTexture(iconPosition, icon);
                }
            }

            var labelPosition = new Rect
                (
                x,
                position.y,
                width,
                position.height
                );

            GUI.Label(labelPosition, label, Styles.parameterSummary);
        }

        private float GetParameterHeight(KeyValuePair<string, string> documentation, Type type, float width)
        {
            return GetParameterReturnsHeight(GetParameterLabel(documentation, type), width);
        }

        private GUIContent GetParameterLabel(KeyValuePair<string, string> documentation, Type type)
        {
            var label = BoltCore.Configuration.humanNaming ? StringUtility.Prettify(documentation.Key) : documentation.Key;

            if (showType && type != null)
            {
                return new GUIContent($"<b>{label}: </b>{documentation.Value} ({type.CSharpName(false)})");
            }
            else
            {
                return new GUIContent($"<b>{label}: </b>{documentation.Value}");
            }
        }

        private GUIContent GetReturnsLabel()
        {
            if (showType && documentation.returnType != null)
            {
                return new GUIContent($"<b>Returns: </b>{documentation.returns} ({documentation.returnType})");
            }
            else
            {
                return new GUIContent($"<b>Returns: </b>{documentation.returns}");
            }
        }

        private void OnParameterGUI(Rect parameterPosition, KeyValuePair<string, string> documentation, Type type)
        {
            OnParameterReturnsGUI(type, GetParameterLabel(documentation, type), parameterPosition);
        }

        private float GetReturnsHeight(float width)
        {
            return GetParameterReturnsHeight(GetReturnsLabel(), width);
        }

        private void OnReturnsGUI(Rect returnsPosition)
        {
            OnParameterReturnsGUI(documentation.returnType, GetReturnsLabel(), returnsPosition);
        }

        private float GetRemarksHeight(float width)
        {
            return Styles.remarks.CalcHeight(new GUIContent(documentation.remarks), width);
        }

        private void OnRemarksGUI(Rect remarksPosition)
        {
            GUI.Label(remarksPosition, documentation.remarks, Styles.remarks);
        }

        public static class Styles
        {
            static Styles()
            {
                summary = new GUIStyle(EditorStyles.label);
                summary.padding = new RectOffset(7, 7, 7, 7);
                summary.wordWrap = true;
                summary.richText = true;

                parameterSummary = new GUIStyle(EditorStyles.label);
                parameterSummary.padding = new RectOffset(7, 7, 0, 7);
                parameterSummary.wordWrap = true;
                parameterSummary.richText = true;

                remarks = new GUIStyle(EditorStyles.label);
                remarks.padding = new RectOffset(7, 7, 7, 7);
                remarks.wordWrap = true;
                remarks.richText = true;
                remarks.fontSize = 10;
                remarks.normal.textColor = ColorPalette.unityForegroundDim;
                //remarks.fontStyle = FontStyle.Italic;
            }

            public static readonly GUIStyle summary;
            public static readonly GUIStyle parameterSummary;
            public static readonly GUIStyle remarks;
            public static readonly float iconSpacing = 0;
            public static readonly float zoomSize = 32;
            public static readonly float zoomSpacing = 8;
        }
    }
}
