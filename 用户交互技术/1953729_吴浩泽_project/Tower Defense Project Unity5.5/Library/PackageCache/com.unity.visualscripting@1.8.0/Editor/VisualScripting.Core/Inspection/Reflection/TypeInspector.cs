using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(Type))]
    public sealed class TypeInspector : Inspector
    {
        public TypeInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            base.Initialize();

            typeFilter = metadata.GetAttribute<TypeFilter>() ?? TypeFilter.Any;
        }

        private TypeFilter typeFilter;

        private IFuzzyOptionTree GetOptions()
        {
            return new TypeOptionTree(Codebase.GetTypeSetFromAttribute(metadata), typeFilter);
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
        }

        public override float GetAdaptiveWidth()
        {
            return LudiqGUI.GetTypeFieldAdaptiveWidth((Type)metadata.value);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var fieldPosition = new Rect
                (
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
                );

            var newType = LudiqGUI.TypeField(fieldPosition, GUIContent.none, (Type)metadata.value, GetOptions);

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newType;
            }
        }
    }
}
