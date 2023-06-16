using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(AnimationCurve))]
    public class AnimationCurveInspector : Inspector
    {
        public AnimationCurveInspector(Metadata metadata) : base(metadata) { }

        public override void Initialize()
        {
            metadata.instantiate = true;
            metadata.instantiator = () => AnimationCurve.Linear(0, 0, 1, 1);

            base.Initialize();
        }

        protected override float GetHeight(float width, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var newValue = EditorGUI.CurveField(position, (AnimationCurve)metadata.value);

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }
    }
}
