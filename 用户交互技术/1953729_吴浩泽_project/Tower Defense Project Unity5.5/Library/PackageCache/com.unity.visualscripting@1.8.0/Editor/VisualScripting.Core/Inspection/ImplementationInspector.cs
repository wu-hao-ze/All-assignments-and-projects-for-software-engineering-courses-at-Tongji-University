using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class ImplementationInspector<T> : Inspector where T : class
    {
        public ImplementationInspector(Metadata metadata) : base(metadata) { }

        private bool compactSelector = false;

        private bool hideSelector = false;

        private bool indentImplementation = true;

        public Metadata implementationMetadata => metadata.value != null ? metadata.Cast(metadata.valueType) : null;

        protected virtual GUIContent nullSelectorLabel { get; } = new GUIContent("(Nothing)");

        protected virtual GUIContent selectorLabel => new GUIContent(" " + metadata.value, metadata.valueType.Icon()?[IconSize.Small]);

        protected virtual EditorTexture typeSelectorIcon => implementationMetadata.definedType.Icon();

        protected sealed override float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            var widthWithoutLabel = WidthWithoutLabel(metadata, width, label);

            if (metadata.value == null)
            {
                height = GetNullSelectorHeight(widthWithoutLabel);
            }
            else if (compactSelector)
            {
                height = Mathf.Max(GetCompactSelectorHeight(), GetImplementationHeight(GetCompactedImplementationWidth(widthWithoutLabel)));
            }
            else
            {
                height += GetSelectorHeight(widthWithoutLabel);
                height += EditorGUIUtility.standardVerticalSpacing;
                height += GetImplementationHeight(widthWithoutLabel);
            }

            return HeightWithLabel(metadata, width, height, label);
        }

        private float GetNullSelectorHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private float GetSelectorHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private float GetCompactSelectorHeight()
        {
            return Styles.compactSelector.fixedHeight;
        }

        private float GetImplementationHeight(float implementationWidth)
        {
            return LudiqGUI.GetInspectorHeight(this, implementationMetadata, implementationWidth, GUIContent.none);
        }

        private float GetCompactedImplementationWidth(float width)
        {
            return width - Styles.spaceAroundCompactSelector - compactSelectorWidth;
        }

        protected sealed override void OnGUI(Rect position, GUIContent label)
        {
            if (metadata.value == null)
            {
                position = BeginLabeledBlock(metadata, position, label);

                var nullSelectorPosition = new Rect
                    (
                    position.x,
                    y,
                    position.width,
                    GetNullSelectorHeight(position.width)
                    );

                OnNullSelectorGUI(nullSelectorPosition);
            }
            else if (hideSelector)
            {
                position = BeginLabeledBlock(metadata, position, label);

                OnImplementationGUI(position);
            }
            else if (compactSelector)
            {
                position = BeginLabeledBlock(metadata, position, label);

                var implementationWidth = GetCompactedImplementationWidth(position.width);

                var compactSelectorPosition = new Rect
                    (
                    position.x,
                    y,
                    compactSelectorWidth,
                    GetCompactSelectorHeight()
                    );

                var implementationPosition = new Rect
                    (
                    compactSelectorPosition.xMax + Styles.spaceAroundCompactSelector,
                    y,
                    implementationWidth,
                    GetImplementationHeight(implementationWidth)
                    );

                OnCompactSelectorGUI(compactSelectorPosition);

                OnImplementationGUI(implementationPosition);
            }
            else
            {
                if (indentImplementation)
                {
                    position = BeginLabeledBlock(metadata, position, label);
                }
                else
                {
                    position = BeginLabeledBlock(metadata, position, GUIContent.none);
                }

                var selectorPosition = position.VerticalSection(ref y, GetSelectorHeight(position.width));

                if (!indentImplementation)
                {
                    selectorPosition = PrefixLabel(metadata, selectorPosition, label);
                }

                y += EditorGUIUtility.standardVerticalSpacing;

                var implementationPosition = position.VerticalSection(ref y, GetImplementationHeight(position.width));

                OnSelectorGUI(selectorPosition);

                OnImplementationGUI(implementationPosition);
            }

            EndBlock(metadata);
        }

        private void OnImplementationGUI(Rect implementationPosition)
        {
            LudiqGUI.Inspector(implementationMetadata, implementationPosition, GUIContent.none);
        }

        private void OnNullSelectorGUI(Rect nullSelectorPosition)
        {
            EditorGUI.BeginChangeCheck();

            var newImplementation = LudiqGUI.FuzzyPopup
                (
                    nullSelectorPosition,
                    GetImplementationOptions,
                    null,
                    nullSelectorLabel
                );

            if (EditorGUI.EndChangeCheck())
            {
                ChangeImplementation(newImplementation);
            }
        }

        private void OnSelectorGUI(Rect selectorPosition)
        {
            EditorGUI.BeginChangeCheck();

            var newImplementation = LudiqGUI.FuzzyPopup
                (
                    selectorPosition,
                    GetImplementationOptions,
                    null,
                    selectorLabel
                );

            if (EditorGUI.EndChangeCheck())
            {
                ChangeImplementation(newImplementation);
            }
        }

        private void OnCompactSelectorGUI(Rect selectorPosition)
        {
            EditorGUI.BeginChangeCheck();

            var newImplementation = LudiqGUI.FuzzyPopup
                (
                    selectorPosition,
                    GetImplementationOptions,
                    null,
                    Styles.showCompactSelectorIcon ? new GUIContent(typeSelectorIcon?[IconSize.Small]) : Styles.compactSelectorWithoutIconContent,
                        Styles.compactSelector
                );

            if (EditorGUI.EndChangeCheck())
            {
                ChangeImplementation(newImplementation);
            }
        }

        private void ChangeImplementation(object newImplementation)
        {
            metadata.RecordUndo();
            metadata.UnlinkChildren();
            metadata.value = CreateImplementation(newImplementation);
            metadata.InferOwnerFromParent();
        }

        protected virtual IFuzzyOptionTree GetImplementationOptions()
        {
            return new TypeOptionTree(implementors) { rootMode = TypeOptionTree.RootMode.Types };
        }

        protected virtual T CreateImplementation(object option)
        {
            return (T)(((Type)option).Instantiate());
        }

        static ImplementationInspector()
        {
            implementors = Codebase.editorTypes
                .Where(type => type.HasAttribute<InspectorAttribute>())
                .OrderBy(type => type.GetAttribute<InspectorImplementationOrderAttribute>()?.order ?? int.MaxValue)
                .SelectMany(type => type.GetAttributes<InspectorAttribute>())
                .Select(attribute => attribute.type)
                .Where(type => type != typeof(T) && typeof(T).IsAssignableFrom(type))
                .ToList();
        }

        private static readonly List<Type> implementors;

        private static float compactSelectorWidth => Styles.compactSelector.fixedWidth;

        public static float compactSelectorSubtractedWidth => compactSelectorWidth + Styles.spaceAroundCompactSelector;

        public static class Styles
        {
            static Styles()
            {
                compactSelectorWithoutIcon = new GUIStyle
                {
                    imagePosition = ImagePosition.ImageOnly,
                    contentOffset = new Vector2(1, -1),
                    clipping = TextClipping.Clip,
                    alignment = TextAnchor.MiddleCenter,
                    fixedWidth = 9,
                    fixedHeight = EditorGUIUtility.singleLineHeight
                };

                compactSelectorWithoutIconContent = new GUIContent(BoltCore.Resources.LoadTexture("TypeSelector.png", new TextureResolution[] { 9, 18 }, CreateTextureOptions.PixelPerfect)?[9]);

                compactSelectorWithIcon = new GUIStyle
                {
                    imagePosition = ImagePosition.ImageOnly,
                    contentOffset = new Vector2(0, 0),
                    alignment = TextAnchor.MiddleCenter,
                    fixedWidth = 16,
                    fixedHeight = EditorGUIUtility.singleLineHeight
                };
            }

            public static readonly GUIStyle compactSelectorWithoutIcon;
            public static readonly GUIStyle compactSelectorWithIcon;
            public static readonly GUIContent compactSelectorWithoutIconContent;
            public static readonly int spaceAroundCompactSelector = 3;
            public static readonly bool showCompactSelectorIcon = true;
            public static GUIStyle compactSelector => showCompactSelectorIcon ? compactSelectorWithIcon : compactSelectorWithoutIcon;
        }
    }
}
