using UnityEngine.UIElements;

namespace Unity.PlasticSCM.Editor
{
    internal static class QueryVisualElementsExtensions
    {
        /// <summary>
        /// Shows the element regardless if it is has been hidden or collapsed.
        /// </summary>
        /// <param name="query">The element query</param>
        /// <typeparam name="T">The element type</typeparam>
        internal static void Show<T>(this UQueryBuilder<T> query)
            where T: VisualElement
        {
            ((T)query).Show();
        }

        /// <summary>
        /// Removes the element from the layout, freeing its space and position.
        /// </summary>
        /// <param name="query">The element query</param>
        /// <typeparam name="T">The element type</typeparam>
        internal static void Collapse<T>(this UQueryBuilder<T> query)
            where T: VisualElement
        {
            ((T)query).Collapse();
        }

        /// <summary>
        /// Hides the element while preserving its space and position in the layout.
        /// </summary>
        /// <param name="query">The element query</param>
        /// <typeparam name="T">The element type</typeparam>
        internal static void Hide<T>(this UQueryBuilder<T> query)
            where T: VisualElement
        {
            ((T)query).Collapse();
        }
    }
}