using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class BoltStyles
    {
        static BoltStyles()
        {
            // Variables

            variableFieldDirectionIndicator = "AC LeftArrow";
            variableFieldWithoutDirectionIndicator = new GUIStyle(EditorStyles.textField);
            variableFieldWithoutDirectionIndicator.padding.left = IconSize.Small;
            variableFieldWithDirectionIndicator = new GUIStyle(variableFieldWithoutDirectionIndicator);
            variableFieldWithDirectionIndicator.padding.left = 24;
        }

        // Variables
        public static readonly GUIStyle variableFieldDirectionIndicator;
        public static readonly GUIStyle variableFieldWithDirectionIndicator;
        public static readonly GUIStyle variableFieldWithoutDirectionIndicator;
    }
}
