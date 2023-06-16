using UnityEngine.UIElements;

namespace Unity.PlasticSCM.Editor
{
    internal static class VisualElementExtensions
    {
        /// <summary>
        /// Shows the element regardless if it is has been hidden or collapsed.
        /// </summary>
        /// <param name="element">The element to show</param>
        /// <typeparam name="T">The element type</typeparam>
        internal static void Show<T>(this T element)
            where T: VisualElement
        {
            element.RemoveFromClassList("collapse");
            element.RemoveFromClassList("hide");
        }

        /// <summary>
        /// Removes the element from the layout, freeing its space and position.
        /// </summary>
        /// <param name="element">The element to collapse</param>
        /// <typeparam name="T">The element type</typeparam>
        internal static void Collapse<T>(this T element)
            where T: VisualElement
        {
            element.AddToClassList("collapse");
        }

        /// <summary>
        /// Hides the element while preserving its space and position in the layout.
        /// </summary>
        /// <param name="element">The element to hide</param>
        /// <typeparam name="T">The element type</typeparam>
        internal static void Hide<T>(this T element)
            where T: VisualElement
        {
            element.AddToClassList("hide");
        }
    }
}