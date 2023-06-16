using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class TypeHandleInspector : Inspector
    {
        const float k_LabelWidth = 38;

        Type m_Type;
        TypeFilter m_TypeFilter;

        TypeFilter typeFilter
        {
            get => m_TypeFilter;
            set
            {
                value = value.Clone().Configured();
                value.Abstract = false;
                value.Interfaces = false;
                value.Object = false;
                m_TypeFilter = value;
            }
        }

        public TypeHandleInspector(Metadata metadata)
            : base(metadata) { }

        public override void Initialize()
        {
            base.Initialize();

            typeFilter = metadata.GetAttribute<TypeFilter>() ?? TypeFilter.Any;
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            return HeightWithLabel(metadata, width, height, label);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            var showLabels = !adaptiveWidth && position.width >= 120;
            BeginLabeledBlock(metadata, position, GUIContent.none);

            var x = position.x;
            var remainingWidth = position.width;

            if (showLabels)
            {
                var typeLabel = label == GUIContent.none ? new GUIContent("Type") : new GUIContent(label.text + " Type");

                var typeLabelPosition = new Rect
                (
                    x,
                    y,
                    k_LabelWidth,
                    EditorGUIUtility.singleLineHeight
                );

                GUI.Label(typeLabelPosition, typeLabel, ProcessLabelStyle(metadata, null));

                x += typeLabelPosition.width;
                remainingWidth -= typeLabelPosition.width;
            }

            var typePosition = new Rect
            (
                x,
                y,
                remainingWidth,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.BeginChangeCheck();

            var type = ((SerializableType)metadata.value).Resolve();
            if (type == typeof(Unknown))
                type = null;
            var newType = LudiqGUI.TypeField(typePosition, GUIContent.none, type, GetTypeOptions, new GUIContent("(Null)"));

            if (EditorGUI.EndChangeCheck())
            {
                metadata.RecordUndo();
                m_Type = newType;
                metadata.value = m_Type.GenerateTypeHandle();
                SetHeightDirty();
            }

            y += typePosition.height;

            EndBlock(metadata);
        }

        public override float GetAdaptiveWidth()
        {
            var width = 0f;
            width = Mathf.Max(width, LudiqGUI.GetTypeFieldAdaptiveWidth(m_Type, new GUIContent("(Null)")));
            width += k_LabelWidth;
            return width;
        }

        IFuzzyOptionTree GetTypeOptions()
        {
            return new TypeOptionTree(Codebase.GetTypeSetFromAttribute(metadata), typeFilter);
        }
    }
}
