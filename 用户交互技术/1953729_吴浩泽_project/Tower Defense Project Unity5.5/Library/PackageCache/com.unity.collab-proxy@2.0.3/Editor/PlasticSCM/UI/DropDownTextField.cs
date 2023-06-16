using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI
{
    internal static class DropDownTextField
    {
        internal static string DoDropDownTextField(
            string text,
            string controlName,
            List<string> dropDownOptions,
            GenericMenu.MenuFunction2 optionSelected,
            params GUILayoutOption[] options)
        {
            GUIContent textContent = new GUIContent(text);

            Rect textFieldRect = GUILayoutUtility.GetRect(
                textContent,
                EditorStyles.textField,
                options);

            return DoDropDownTextField(
                text,
                controlName,
                dropDownOptions,
                optionSelected,
                textFieldRect);
        }

        internal static string DoDropDownTextField(
            string text,
            string controlName,
            List<string> dropDownOptions,
            GenericMenu.MenuFunction2 optionSelected,
            Rect textFieldRect)
        {
            Texture popupIcon = Images.GetDropDownIcon();

            Rect popupButtonRect = new Rect(
                textFieldRect.x + textFieldRect.width - BUTTON_WIDTH,
                textFieldRect.y,
                BUTTON_WIDTH,
                textFieldRect.height);

            if (GUI.Button(popupButtonRect, string.Empty, EditorStyles.label))
            {
                GenericMenu menu = new GenericMenu();
                foreach (string option in dropDownOptions)
                {
                    menu.AddItem(
                        new GUIContent(UnityMenuItem.EscapedText(option)),
                        false,
                        optionSelected,
                        option);
                }

                menu.DropDown(textFieldRect);
            }

            Rect popupIconRect = new Rect(
                popupButtonRect.x,
                popupButtonRect.y + UnityConstants.DROPDOWN_ICON_Y_OFFSET,
                popupButtonRect.width,
                popupButtonRect.height);

            GUI.SetNextControlName(controlName);
            string result = GUI.TextField(textFieldRect, text);

            GUI.Label(popupIconRect, popupIcon);

            return result;
        }

        const int BUTTON_WIDTH = 16;
    }
}
