using System;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    interface ISystemObjectInspector : IDisposable
    {
        void OnGUI(Rect position, GUIContent label, bool showLabels, ref float y);
        float GetHeight(float width, GUIContent label);
        float GetWidth();
    }

    class ValueInspector : ISystemObjectInspector
    {
        SystemObjectInspector parent { get; }
        private Metadata typeMetadata;

        public ValueInspector(SystemObjectInspector parent)
        {
            this.parent = parent;

            typeMetadata = parent.metadata.parent[nameof(VariableDeclaration.typeHandle)];
            typeMetadata.valueChanged += OnTypeChanged;
        }

        public void Dispose()
        {
            typeMetadata.valueChanged -= OnTypeChanged;
        }

        void OnTypeChanged(object obj)
        {
            var typeHandle = (SerializableType)typeMetadata.value;

            // Fix for migration from 1.6 to 1.7
            // TypeHandle did not exist before 1.7, so the TypeHandle identification is null,
            // Do not reset the value in this case
            if (!string.IsNullOrEmpty(typeHandle.Identification))
            {
                ResolveType();
                parent.SetValue();
                parent.SetHeightDirty();
            }
        }

        bool ResolveType()
        {
            var typeHandle = (SerializableType)typeMetadata.value;
            var value = parent.metadata.value;

            // Fix for migration from 1.6 to 1.7
            // TypeHandle did not exist before 1.7, so the TypeHandle is null,
            // Infer type from the value
            if (string.IsNullOrEmpty(typeHandle.Identification) && value != null)
            {
                parent.type = value.GetType();
                typeMetadata.value = parent.type.GenerateTypeHandle();
            }
            else
            {
                var newType = typeHandle.Resolve();
                parent.type = newType;
            }

            return parent.type != null && parent.type != typeof(Unknown);
        }

        public void OnGUI(Rect position, GUIContent label, bool showLabels, ref float y)
        {
            if (ResolveType())
            {
                parent.SetHeightDirty();

                var x = position.x;
                var remainingWidth = position.width;

                if (showLabels)
                {
                    var valueLabel = label == GUIContent.none ? new GUIContent("Value") : new GUIContent(label.text + " Value");

                    var valueLabelPosition = new Rect
                    (
                        x,
                        y,
                        SystemObjectInspector.Styles.labelWidth,
                        EditorGUIUtility.singleLineHeight
                    );

                    GUI.Label(valueLabelPosition, valueLabel, Inspector.ProcessLabelStyle(parent.metadata, null));

                    x += valueLabelPosition.width;
                    remainingWidth -= valueLabelPosition.width;
                }

                var valuePosition = new Rect
                (
                    x,
                    y,
                    remainingWidth,
                    EditorGUIUtility.singleLineHeight
                );

                LudiqGUI.Inspector(parent.metadata.Cast(parent.type), valuePosition, GUIContent.none);
            }
        }

        public float GetHeight(float width, GUIContent label)
        {
            if (!ResolveType())
            {
                return 0f;
            }

            var height = LudiqGUI.GetInspectorHeight(parent, parent.metadata.Cast(parent.type), width, GUIContent.none);
            return Inspector.HeightWithLabel(parent.metadata, width, height, label);
        }

        public float GetWidth()
        {
            var width = SystemObjectInspector.Styles.labelWidth;

            if (ResolveType())
            {
                width += LudiqGUI.GetInspectorAdaptiveWidth(parent.metadata.Cast(parent.type));
            }

            return width;
        }
    }

    class ValueTypeInspector : ISystemObjectInspector
    {
        SystemObjectInspector parent { get; }

        public ValueTypeInspector(SystemObjectInspector parent)
        {
            this.parent = parent;
            parent.metadata.valueChanged += InferType;
        }

        public void Dispose()
        {
            parent.metadata.valueChanged -= InferType;
        }

        public void OnGUI(Rect position, GUIContent label, bool showLabels, ref float y)
        {
            if (parent.chooseType)
            {
                var x = position.x;
                var remainingWidth = position.width;

                if (showLabels)
                {
                    var typeLabel = label == GUIContent.none ? new GUIContent("Type") : new GUIContent(label.text + " Type");

                    var typeLabelPosition = new Rect
                        (
                        x,
                        y,
                        SystemObjectInspector.Styles.labelWidth,
                        EditorGUIUtility.singleLineHeight
                        );

                    GUI.Label(typeLabelPosition, typeLabel, Inspector.ProcessLabelStyle(parent.metadata, null));

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

                var newType = LudiqGUI.TypeField(typePosition, GUIContent.none, parent.type, GetTypeOptions, new GUIContent("(Null)"));

                if (EditorGUI.EndChangeCheck())
                {
                    parent.metadata.RecordUndo();
                    parent.type = newType;
                    parent.SetValue();
                    parent.SetHeightDirty();
                }

                y += typePosition.height;
            }

            if (parent.chooseType && parent.showValue)
            {
                y += SystemObjectInspector.Styles.spaceBetweenTypeAndValue;
            }

            if (parent.showValue)
            {
                Rect valuePosition;

                if (parent.chooseType)
                {
                    var x = position.x;
                    var remainingWidth = position.width;

                    if (showLabels)
                    {
                        var valueLabel = label == GUIContent.none ? new GUIContent("Value") : new GUIContent(label.text + " Value");

                        var valueLabelPosition = new Rect
                            (
                            x,
                            y,
                            SystemObjectInspector.Styles.labelWidth,
                            EditorGUIUtility.singleLineHeight
                            );

                        GUI.Label(valueLabelPosition, valueLabel, Inspector.ProcessLabelStyle(parent.metadata, null));

                        x += valueLabelPosition.width;
                        remainingWidth -= valueLabelPosition.width;
                    }

                    valuePosition = new Rect
                        (
                        x,
                        y,
                        remainingWidth,
                        EditorGUIUtility.singleLineHeight
                        );

                    LudiqGUI.Inspector(parent.metadata.Cast(parent.type), valuePosition, GUIContent.none);
                }
                else
                {
                    valuePosition = new Rect
                        (
                        position.x,
                        y,
                        position.width,
                        LudiqGUI.GetInspectorHeight(parent, parent.metadata.Cast(parent.type), position.width, label)
                        );

                    LudiqGUI.Inspector(parent.metadata.Cast(parent.type), valuePosition, label);
                }

                y += valuePosition.height;
            }
            else
            {
                parent.metadata.value = null;
            }
        }

        public float GetHeight(float width, GUIContent label)
        {
            var height = 0f;

            if (parent.chooseType)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            if (parent.chooseType && parent.showValue)
            {
                height += SystemObjectInspector.Styles.spaceBetweenTypeAndValue;
            }

            if (parent.showValue)
            {
                height += LudiqGUI.GetInspectorHeight(parent, parent.metadata.Cast(parent.type), width, GUIContent.none);
            }

            return Inspector.HeightWithLabel(parent.metadata, width, height, label);
        }

        public float GetWidth()
        {
            var width = 0f;

            if (parent.chooseType)
            {
                width = Mathf.Max(width, LudiqGUI.GetTypeFieldAdaptiveWidth(parent.type, new GUIContent("(Null)")));
            }

            if (parent.showValue)
            {
                width = Mathf.Max(width, LudiqGUI.GetInspectorAdaptiveWidth(parent.metadata.Cast(parent.type)));
            }

            width += SystemObjectInspector.Styles.labelWidth;

            return width;
        }

        IFuzzyOptionTree GetTypeOptions()
        {
            return new TypeOptionTree(Codebase.GetTypeSetFromAttribute(parent.metadata), parent.typeFilter);
        }

        void InferType(object previousValue)
        {
            var value = parent.metadata.value;

            if (value == null)
            {
                // Fix a bug when 2 SystemObjectInspectors are open.
                // If one inspector change its type to any reference type (default == null), the other one needs
                // to set its type to null in order to not reset the first inspector to its previous type by
                // the OnGUI method which calls EnforceType.
                if (previousValue != null && previousValue.GetType() == parent.type)
                {
                    parent.type = null;
                }
                return;
            }

            parent.type = value.GetType();

            parent.SetValue();

            parent.SetHeightDirty();
        }
    }

    public class SystemObjectInspector : Inspector
    {
        internal Type type;
        private TypeFilter _typeFilter;
        private ISystemObjectInspector inspector;

        public bool chooseType => true;
        public bool showValue => type != null && InspectorProvider.instance.GetDecoratorType(type) != typeof(SystemObjectInspector);
        public TypeFilter typeFilter
        {
            get => _typeFilter;
            private set
            {
                value = value.Clone().Configured();
                value.Abstract = false;
                value.Interfaces = false;
                value.Object = false;
                _typeFilter = value;
            }
        }

        public SystemObjectInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            base.Initialize();

            bool isVariableDeclarationContext = metadata.parent.valueType == typeof(VariableDeclaration);
            if (isVariableDeclarationContext && metadata.GetAncestorAttribute<ValueAttribute>() != null)
            {
                inspector = new ValueInspector(this);
            }
            else
            {
                inspector = new ValueTypeInspector(this);
            }

            typeFilter = metadata.GetAttribute<TypeFilter>() ?? TypeFilter.Any;
        }

        public override void Dispose()
        {
            base.Dispose();
            inspector.Dispose();
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            // Super hacky hotfix:
            // If the value changes in between OnGUI calls,
            // the OnValueChange event will not be called, because
            // we don't even look at the value until showField is true.
            // For example, an object that was null and becomes non-null
            // will be reset to null by the inspector unless this line is here,
            // because type will be null and showField will thus be false.
            var haxHotfix = metadata.value;
            // TL;DR: storing a local private type field that does not
            // take the actual, current variable type into consideration is a
            // very bad idea and will inevitably cause inspector v. codebase fighting
            // or inspector v. inspector fighting.

            var showLabels = !adaptiveWidth && position.width >= 120;

            BeginLabeledBlock(metadata, position, GUIContent.none);

            inspector.OnGUI(position, label, showLabels, ref y);

            EndBlock(metadata);
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return inspector.GetHeight(width, label);
        }

        public override float GetAdaptiveWidth()
        {
            return inspector.GetWidth();
        }

        internal void SetValue()
        {
            if (metadata.value?.GetType() == type)
            {
                return;
            }

            metadata.UnlinkChildren();

            if (type == null)
            {
                metadata.value = null;
            }
            else if (ConversionUtility.CanConvert(metadata.value, type, true))
            {
                metadata.value = ConversionUtility.Convert(metadata.value, type);
            }
            else
            {
                metadata.value = type.TryInstantiate();
            }

            metadata.InferOwnerFromParent();
        }

        public static class Styles
        {
            public static readonly float spaceBetweenTypeAndValue = 2;
            public static readonly float labelWidth = 38;
        }
    }
}
