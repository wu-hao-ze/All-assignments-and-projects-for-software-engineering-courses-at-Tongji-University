using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(LooseAssemblyName))]
    public sealed class LooseAssemblyNameInspector : Inspector
    {
        public LooseAssemblyNameInspector(Metadata metadata) : base(metadata) { }

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

            var popupLabel = StringUtility.FallbackEmpty(((LooseAssemblyName)metadata.value).name, "(No Assembly)");

            var newAssemblyName = (LooseAssemblyName)LudiqGUI.FuzzyPopup
                (
                    fieldPosition,
                    () => new LooseAssemblyNameOptionTree(),
                    ((LooseAssemblyName)metadata.value),
                    new GUIContent(popupLabel)
                );

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newAssemblyName;
            }
        }
    }
}
