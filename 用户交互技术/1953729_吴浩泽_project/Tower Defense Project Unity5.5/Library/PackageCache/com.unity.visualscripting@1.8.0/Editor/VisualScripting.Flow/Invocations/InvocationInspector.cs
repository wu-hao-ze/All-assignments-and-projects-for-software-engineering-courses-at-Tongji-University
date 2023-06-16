/*
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class InvocationInspector : Inspector
    {
        protected InvocationInspector(Metadata metadata) : base(metadata) { }

        protected Metadata argumentsMetadata => metadata["_" + nameof(MemberInvocation.arguments)];

        protected IList<Metadata> parameters => argumentsMetadata.children;

        public virtual Type expectedType { get; set; }

        protected virtual IEnumerable<GUIContent> compactLabels => parameters.Select(p => p.label);

        protected void PrepareParameterLabel(Metadata parameter, string label, string tooltip)
        {
            if (StringUtility.IsNullOrWhiteSpace(label))
            {
                label = "(?)";
            }

            parameter.label.text = label;
            parameter.label.tooltip = tooltip;
        }

        protected void PrepareParameterInspector(Metadata parameter, Type expectedType)
        {
            parameter.Inspector<IExpressionInspector>().expectedType = expectedType;
        }

        protected float GetParametersHeight(float width)
        {
            var height = 0f;

            using (LudiqGUIUtility.labelWidth.Override(GetCompactLabelsWidth(width)))
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    var parameter = parameters[i];

                    height += GetParameterHeight(parameter, width);

                    if (i < parameters.Count - 1)
                    {
                        height += Styles.spaceBetweenParameters;
                    }
                }
            }

            return height;
        }

        protected float GetCompactLabelsWidth(float width)
        {
            var compactLabelsWidth = 0f;

            foreach (var compactLabel in compactLabels)
            {
                compactLabelsWidth = Mathf.Max(compactLabelsWidth, Styles.compactLabel.CalcSize(compactLabel).x);
            }

            compactLabelsWidth = Mathf.Min(compactLabelsWidth, width / 3);

            return compactLabelsWidth;
        }

        protected virtual float GetParameterHeight(Metadata parameter, float width)
        {
            return InspectorGUI.GetHeight(parameter, width, parameter.label, this);
        }

        protected virtual void OnParameterGUI(Rect parameterPosition, Metadata parameter)
        {
            InspectorGUI.Field(parameter, parameterPosition, parameter.label);
        }

        protected void OnParametersGUI(Rect position)
        {
            if (parameters.Any())
            {
                using (LudiqGUIUtility.labelWidth.Override(GetCompactLabelsWidth(position.width)))
                {
                    for (var i = 0; i < parameters.Count; i++)
                    {
                        var parameter = parameters[i];

                        var parameterPosition = position.VerticalSection(ref y, GetParameterHeight(parameter, position.width));

                        OnParameterGUI(parameterPosition, parameter);

                        if (i < parameters.Count - 1)
                        {
                            y += Styles.spaceBetweenParameters;
                        }
                    }
                }
            }
        }

        public static class Styles
        {
            static Styles()
            {
                compactLabel = new GUIStyle(EditorStyles.label);
                var padding = compactLabel.padding;
                padding.right += labelPadding;
                compactLabel.padding = padding;
            }

            public static readonly float spaceBetweenParameters = EditorGUIUtility.standardVerticalSpacing;
            public static readonly float spaceAfterLabel = 0;
            public static readonly int labelPadding = 3;
            public static readonly GUIStyle compactLabel;
        }
    }
}
*/
