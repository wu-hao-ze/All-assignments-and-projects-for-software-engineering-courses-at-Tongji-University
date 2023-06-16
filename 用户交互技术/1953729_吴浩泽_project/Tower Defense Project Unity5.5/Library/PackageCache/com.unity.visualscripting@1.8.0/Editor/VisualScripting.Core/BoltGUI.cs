using UnityEditor;
using UnityEngine;
using UnityEvent = UnityEngine.Event;

namespace Unity.VisualScripting
{
    public static class BoltGUI
    {
        private static UnityEvent e => UnityEvent.current;

        public static float GetVariableFieldHeight(GUIContent label, string value, ActionDirection direction)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public static string VariableField(Rect position, GUIContent label, string value, ActionDirection direction)
        {
            position = EditorGUI.PrefixLabel(position, label);
            var style = direction != ActionDirection.Any ? BoltStyles.variableFieldWithDirectionIndicator : BoltStyles.variableFieldWithoutDirectionIndicator;
            value = EditorGUI.TextField(position, GUIContent.none, value, style);

            var iconPosition = new Rect
                (
                position.x + 3,
                position.y + 2,
                12,
                12
                );

            GUI.DrawTexture(iconPosition, BoltCore.Icons.variable?[IconSize.Small]);

            if (direction != ActionDirection.Any)
            {
                var arrowPosition = new Rect
                    (
                    iconPosition.xMax + (direction == ActionDirection.Get ? 12 : -2),
                    position.y + 2,
                    12 * (direction == ActionDirection.Get ? -1 : 1),
                    12
                    );

                if (e.type == EventType.Repaint)
                {
                    BoltStyles.variableFieldDirectionIndicator.Draw(arrowPosition, false, false, false, false);
                }
            }

            return value;
        }
    }
}
