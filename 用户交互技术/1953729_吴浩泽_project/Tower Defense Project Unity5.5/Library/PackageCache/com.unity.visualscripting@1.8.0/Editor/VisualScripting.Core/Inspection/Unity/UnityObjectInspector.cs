using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public sealed class UnityObjectInspector : Inspector
    {
        public UnityObjectInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            base.Initialize();

            nullMeansSelf = ComponentHolderProtocol.IsComponentHolderType(metadata.definedType) && metadata.AncestorHasAttribute<NullMeansSelfAttribute>();
        }

        private bool nullMeansSelf;

        protected override float GetHeight(float width, GUIContent label)
        {
            return HeightWithLabel(metadata, width, EditorGUIUtility.singleLineHeight, label);
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

            var allowSceneObjects = LudiqEditorUtility.editedObject.value.AsUnityNull()?.IsSceneBound() ?? false;

            var newValue = EditorGUI.ObjectField(fieldPosition, (UnityObject)metadata.value, metadata.definedType, allowSceneObjects);

            if (nullMeansSelf && metadata.value == null && e.type == EventType.Repaint)
            {
                var selfPatchPosition = new Rect
                    (
                    fieldPosition.x + 2,
                    fieldPosition.y + 2,
                    fieldPosition.width - 16 - 2 - 2 - 1,
                    fieldPosition.height - 2 - 2
                    );

                Color tmp = GUI.color;
                GUI.color = Color.white; // Force white to prevent Self label beeing transparent and ObjectField None (Object) appearing under

                GUI.Label(selfPatchPosition, "This", tmp.a > 0 && tmp.a < 1 ? Styles.selfPatchTransparent : Styles.selfPatch);
                GUI.color = tmp;
            }

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }

        public override float GetAdaptiveWidth()
        {
            string label;
            bool icon = false;

            if (metadata.value.IsUnityNull())
            {
                if (nullMeansSelf)
                {
                    label = "This";
                }
                else
                {
                    label = "None";
                    icon = true;
                }
            }
            else
            {
                label = ((UnityObject)metadata.value).name;
                icon = true;
            }

            var width = EditorStyles.objectField.CalcSize(new GUIContent(label)).x;

            if (icon)
            {
                width += 15;
            }

            return width;
        }

        public static class Styles
        {
            static Styles()
            {
                selfPatch = new GUIStyle(EditorStyles.label);
                selfPatch.normal.background = ColorPalette.unityBackgroundLight.GetPixel();
                selfPatch.padding = new RectOffset(1, 0, -1, 0);
                selfPatch.margin = new RectOffset(0, 0, 0, 0);

                selfPatchTransparent = new GUIStyle(selfPatch);
                selfPatchTransparent.normal.textColor = new Color(selfPatch.normal.textColor.r, selfPatch.normal.textColor.g,
                    selfPatch.normal.textColor.b, 0.3f);
            }

            public static readonly GUIStyle selfPatch;
            public static readonly GUIStyle selfPatchTransparent;
        }
    }
}
