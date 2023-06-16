using System;

using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DrawActionHelpBox
    {
        internal static void For(
            Texture image,
            string labelText,
            string buttonText,
            Action buttonAction)
        {
            EditorGUILayout.BeginHorizontal(
                EditorStyles.helpBox, GUILayout.MinHeight(40));

            DoNotificationLabel(image, labelText);

            GUILayout.Space(10);

            DoActionButton(buttonText, buttonAction);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        static void DoNotificationLabel(
            Texture image, string labelText)
        {
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.Label(
                new GUIContent(labelText, image),
                UnityStyles.HelpBoxLabel);

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }

        static void DoActionButton(
            string buttonText, Action buttonAction)
        {
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUIContent buttonContent = new GUIContent(buttonText);

            float width = GetButtonWidth(
                buttonContent, EditorStyles.miniButton);

            if (GUILayout.Button(
                    buttonContent, EditorStyles.miniButton,
                    GUILayout.MinWidth(Math.Max(50, width))))
            {
                buttonAction();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }

        static float GetButtonWidth(
            GUIContent buttonContent, GUIStyle buttonStyle)
        {
            return buttonStyle.CalcSize(buttonContent).x + 10;
        }
    }
}
