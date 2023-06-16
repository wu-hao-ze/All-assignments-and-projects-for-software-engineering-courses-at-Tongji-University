using System.Collections.Generic;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Interface to navigate through Timelines and SubTimelines for the Timeline window.
    /// </summary>
    /// <remarks>
    /// TimelineNavigator gives you access to the Timeline window breadcrumbs functionality. Use it to programmatically
    /// dig into SubTimelines, navigate to parent Timelines or navigate Timeline Window breadcrumbs.
    /// </remarks>
    public sealed class TimelineNavigator
    {
        TimelineWindow.TimelineNavigatorImpl m_Impl;
        internal TimelineNavigator(IWindowStateProvider windowState)
        {
            m_Impl = new TimelineWindow.TimelineNavigatorImpl(windowState);
        }

        /// <summary>
        /// Gets the SequenceContext associated with the Timeline currently shown in the Timeline window.
        /// </summary>
        /// <returns>The SequenceContext associated with the Timeline currently shown in the Timeline window.</returns>
        /// <remarks>Equivalent to <c>TimelineNavigator.GetBreadCrumbs().Last()</c></remarks>
        /// <exception cref="System.InvalidOperationException"> The Window associated to this instance has been destroyed.</exception>
        public SequenceContext GetCurrentContext()
        {
            return m_Impl.GetCurrentContext();
        }

        /// <summary>
        /// Gets the parent SequenceContext for the Timeline currently shown in the Timeline window.
        /// </summary>
        /// <returns>The parent SequenceContext for the Timeline currently shown in the Timeline window if there is one; an invalid SequenceContext otherwise. <seealso cref="SequenceContext.Invalid"/></returns>
        /// <exception cref="System.InvalidOperationException"> The Window associated to this instance has been destroyed.</exception>
        public SequenceContext GetParentContext()
        {
            return m_Impl.GetParentContext();
        }

        /// <summary>
        /// Gets the first SequenceContext in the breadcrumbs.
        /// </summary>
        /// <returns>The first SequenceContext in the breadcrumbs.</returns>
        /// <remarks>Equivalent to <c>TimelineNavigator.GetBreadCrumbs().First()</c></remarks>
        /// <exception cref="System.InvalidOperationException"> The Window associated to this instance has been destroyed.</exception>
        public SequenceContext GetRootContext()
        {
            return m_Impl.GetRootContext();
        }

        /// <summary>
        /// Gets the collection of child contexts that can be navigated to from the current context.
        /// </summary>
        /// <returns>The collection of child contexts that can be navigated to from the current context.</returns>
        /// <exception cref="System.InvalidOperationException"> The Window associated to this instance has been destroyed.</exception>
        public IEnumerable<SequenceContext> GetChildContexts()
        {
            return m_Impl.GetChildContexts();
        }

        /// <summary>
        /// Gets the collection of SequenceContexts associated with the breadcrumbs shown in the TimelineEditorWindow.
        /// </summary>
        /// <remarks>This operation can be expensive. Consider caching the results instead of calling the method multiple times.</remarks>
        /// <returns>The collection of SequenceContexts associated with the breadcrumbs shown in the TimelineEditorWindow, from the root context to the current context.</returns>
        /// <exception cref="System.InvalidOperationException"> The Window associated to this instance has been destroyed.</exception>
        public IEnumerable<SequenceContext> GetBreadcrumbs()
        {
            return m_Impl.GetBreadcrumbs();
        }

        /// <summary>
        /// Navigates to a new SequenceContext.
        /// </summary>
        /// <param name="context">The context to navigate to.</param>
        /// <remarks>
        /// The SequenceContext provided must be a valid navigation destination.
        ///
        /// Valid navigation destinations:
        /// * The parent context returned by <see cref="GetParentContext"/>.
        /// * The root context returned by <see cref="GetRootContext"/>.
        /// * Any SequenceContext returned by <see cref="GetChildContexts"/>.
        /// * Any SequenceContext returned by <see cref="GetBreadcrumbs"/>.
        ///
        /// Note: This method cannot be used to change the root SequenceContext. To change the root SequenceContext, use <see cref="TimelineEditorWindow.SetTimeline"/>.
        ///
        /// </remarks>
        /// <exception cref="System.InvalidOperationException"> The Window associated to this instance has been destroyed.</exception>
        /// <exception cref="System.ArgumentException"> The context is not valid.</exception>
        /// <exception cref="System.InvalidOperationException"> The context is not a valid navigation destination.</exception>
        public void NavigateTo(SequenceContext context)
        {
            m_Impl.NavigateTo(context);
        }
    }
}
