using System.IO;

using UnityEditor;
using UnityEngine.UIElements;

using PlasticGui;
using Unity.PlasticSCM.Editor.AssetUtils;

namespace Unity.PlasticSCM.Editor.UI.UIElements
{
    internal static class UIElementsExtensions
    {
        internal static void LoadLayout(
            this VisualElement element,
            string className)
        {
            string uxmlRelativePath = Path.Combine(
                AssetsPath.GetLayoutsFolderRelativePath(),
                string.Format("{0}.uxml", className));

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                uxmlRelativePath);

            if (visualTree == null)
            {
                UnityEngine.Debug.LogErrorFormat(
                    "Layout {0} not found at path {1}",
                    className,
                    uxmlRelativePath);
                return;
            }

            visualTree.CloneTree(element);
        }

        internal static void LoadStyle(
            this VisualElement element,
            string className)
        {
            string ussRelativePath = Path.Combine(
                AssetsPath.GetStylesFolderRelativePath(),
                string.Format("{0}.uss", className));

            StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                ussRelativePath);

            if (sheet != null)
            {
                element.styleSheets.Add(sheet);
            }

            string ussSkinRelativePath = Path.Combine(
                AssetsPath.GetStylesFolderRelativePath(),
                string.Format("{0}.{1}.uss",
                    className, EditorGUIUtility.isProSkin ? "dark" : "light"));

            StyleSheet skinSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                ussSkinRelativePath);

            if (skinSheet == null)
                return;

            element.styleSheets.Add(skinSheet);
        }

        internal static void FocusOnceLoaded(
            this TextField elem)
        {
            // Really weird behavior from UIElements, apperently in order
            // to focus a text field you have to wait for it to attach to
            // the panel and then focus it's TextInputBaseField child
            // control rather than the TextField itself. For more see:
            // https://forum.unity.com/threads/focus-doesnt-seem-to-work.901130/
            elem.RegisterCallback<AttachToPanelEvent>(
                _ => elem.Q(TextInputBaseField<string>.textInputUssName).Focus());
        }

        internal static void FocusWorkaround(this TextField textField)
        {
            // https://issuetracker.unity3d.com/issues/uielements-textfield-is-not-focused-and-you-are-not-able-to-type-in-characters-when-using-focus-method
            textField.Q("unity-text-input").Focus();
        }

        internal static void SetControlImage(
            this VisualElement element,
            string name,
            Images.Name imageName)
        {
            Image imageElem = element.Query<Image>(name).First();
            imageElem.image = Images.GetImage(imageName);
        }

        internal static void SetControlText<T>(
            this VisualElement element,
            string name, PlasticLocalization.Name fieldName,
            params string[] format) where T : VisualElement
        {
            dynamic control = element.Query<T>(name).First();
            string str = PlasticLocalization.GetString(fieldName);
            control.text = format.Length > 0 ? string.Format(str, format) : str;
        }

        internal static void SetControlLabel<T>(
            this VisualElement element,
            string name, PlasticLocalization.Name fieldName)  where T : VisualElement
        {
            dynamic control = element.Query<T>(name).First();
            control.label = PlasticLocalization.GetString(fieldName);
        }
    }
}