#if PACKAGE_INPUT_SYSTEM_EXISTS
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting.InputSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.VisualScripting
{
    [Inspector(typeof(InputAction))]
    public class InputActionInspector : Inspector
    {
        private readonly GraphReference m_Reference;
        private readonly OnInputSystemEvent m_InputSystemUnit;

        public InputActionInspector(Metadata metadata, GraphReference reference, OnInputSystemEvent inputSystemUnit) : base(metadata)
        {
            m_Reference = reference;
            m_InputSystemUnit = inputSystemUnit;
        }

        // Called by reflection from TypeUtility.Instantiate
        [UsedImplicitly]
        public InputActionInspector(Metadata metadata) : base(metadata)
        {
        }

        protected override float GetHeight(float width, GUIContent label) => EditorGUIUtility.singleLineHeight;
        public override float GetAdaptiveWidth()
        {
            return Mathf.Max(100, metadata.value is InputAction action && action.name != null
                ? (EditorStyles.popup.CalcSize(new GUIContent(action.name)).x + 1)
                : 0);
        }

        protected override void OnGUI(Rect position, GUIContent label)
        {
            position = BeginLabeledBlock(metadata, position, label);

            var togglePosition = position.VerticalSection(ref y, EditorGUIUtility.singleLineHeight);

            if (m_InputSystemUnit == null)
            {
                EndBlock(metadata);
                return;
            }

            var inputActionAsset = Flow.Predict<PlayerInput>(m_InputSystemUnit.Target, m_Reference)?.actions;
            if (!inputActionAsset)
                EditorGUI.LabelField(togglePosition, "No Actions found");
            else
            {
                var value = metadata.value is InputAction ? (InputAction)metadata.value : default;

                int currentIndex = -1;
                if (value != null && value.id != default)
                {
                    int i = 0;
                    foreach (var playerInputAction in inputActionAsset)
                    {
                        if (playerInputAction.id == value.id)
                        {
                            currentIndex = i;
                            break;
                        }
                        i++;
                    }
                }

                var displayedOptions = Enumerable.Repeat(new GUIContent("<None>"), 1).Concat(inputActionAsset.Select(a => new GUIContent(a.name))).ToArray();
                var newIndex = EditorGUI.Popup(togglePosition, currentIndex + 1, displayedOptions);
                if (EndBlock(metadata) || ActionTypeHasChanged(currentIndex, value))
                {
                    metadata.RecordUndo();
                    if (newIndex == 0)
                        metadata.value = default;
                    else
                    {
                        var inputAction = inputActionAsset.ElementAt(newIndex - 1);

                        metadata.value =
                            InputAction_DirectConverter.MakeInputActionWithId(inputAction.id.ToString(),
                                inputAction.name, inputAction.expectedControlType, inputAction.type);
                        m_InputSystemUnit.Analyser(m_Reference).isDirty = true;
                    }
                }
                return;
            }

            EndBlock(metadata);

            bool ActionTypeHasChanged(int currentIndex, InputAction value)
            {
                try
                {
                    // getting the total action count would be expensive, as it would need to be computed everytime
                    return value?.type != inputActionAsset.ElementAt(currentIndex)?.type;
                }
                catch
                {
                    return true;
                }
            }
        }
    }
}
#endif
