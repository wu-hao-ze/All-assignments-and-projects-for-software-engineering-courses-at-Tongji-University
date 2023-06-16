using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Inspector(typeof(LayerMask))]
    public class LayerMaskInspector : Inspector
    {
        public LayerMaskInspector(Metadata metadata) : base(metadata) { }

        protected override float GetHeight(float width, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override float GetAdaptiveWidth()
        {
            var layerMask = (LayerMask)metadata.value;
            string layerName;
            switch (layerMask.value)
            {
                case 0:
                    layerName = "Nothing";
                    break;
                case int _ when Mathf.IsPowerOfTwo(layerMask.value): // exactly one layer in the LayerMask
                    uint mask;
                    unchecked
                    {
                        // weirdly the LayerMask is an int. cast to uint to use log2
                        mask = (uint)((LayerMask)metadata.value).value;
                    }
                    // find power of two exponent. could be optimized
                    var log2 = (int)Mathf.Log(mask, 2);
                    layerName = LayerMask.LayerToName(log2);

                    break;
                default: // non-power of two means mixed layers
                    layerName = "Mixed...";
                    break;
            }

            return Mathf.Max(18, EditorStyles.popup.CalcSize(new GUIContent(layerName)).x + 1);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var newValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(EditorGUI.MaskField
                (
                    position,
                    InternalEditorUtility.LayerMaskToConcatenatedLayersMask((LayerMask)metadata.value),
                    InternalEditorUtility.layers
                )
            );

            if (EndBlock(metadata))
            {
                metadata.RecordUndo();
                metadata.value = newValue;
            }
        }
    }
}
