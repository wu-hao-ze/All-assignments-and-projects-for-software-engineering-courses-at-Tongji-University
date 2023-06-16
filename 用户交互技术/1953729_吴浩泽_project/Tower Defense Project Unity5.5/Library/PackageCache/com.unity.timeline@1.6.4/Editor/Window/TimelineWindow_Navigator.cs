using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    partial class TimelineWindow
    {
        /// <summary>
        /// The public Breadcrumb navigation controller, accessible through TimelineEditorWindow
        /// </summary>
        public override TimelineNavigator navigator => new TimelineNavigator(this);

        /// <summary>
        /// Implementation of TimelineNavigator
        /// </summary>
        /// <remarks>
        /// Always use TimelineNavigator, not this class.
        ///
        /// The class acts as a handle on the TimelineWindow, and lets users navigate the breadcrumbs and dive into subtimelines
        /// </remarks>
        internal class TimelineNavigatorImpl
        {
            /// <summary>
            ///
            /// </summary>
            /// <param name="window"></param>
            public TimelineNavigatorImpl(IWindowStateProvider window)
            {
                if (window == null)
                    throw new ArgumentNullException(nameof(window),
                        "TimelineNavigator cannot be used with a null window");
                m_Window = window;
            }

            /// <summary>
            /// Creates a SequenceContext from the top of the breadcrumb stack
            /// </summary>
            /// <returns></returns>
            public SequenceContext GetCurrentContext()
            {
                return GetBreadcrumbs().LastOrDefault();
            }

            /// <summary>
            /// Creates a SequenceContext from the second to last breadcrumb in the list
            /// </summary>
            /// <returns>Valid context if there is a parent, Invalid context otherwise</returns>
            public SequenceContext GetParentContext()
            {
                //If the edit sequence is the master sequence, there is no parent context
                if (windowState.editSequence == windowState.masterSequence)
                    return SequenceContext.Invalid;
                var contexts = GetBreadcrumbs();
                var length = contexts.Count();
                return contexts.ElementAtOrDefault(length - 2);
            }

            /// <summary>
            /// Creates a SequenceContext from the top of the breadcrumb stack
            /// </summary>
            /// <returns>Always returns a valid SequenceContext</returns>
            public SequenceContext GetRootContext()
            {
                return GetBreadcrumbs().FirstOrDefault();
            }

            /// <summary>
            /// Creates SequenceContexts for all the child Timelines
            /// </summary>
            /// <returns>Collection of SequenceContexts. Can be empty if there are no valid child contexts</returns>
            public IEnumerable<SequenceContext> GetChildContexts()
            {
                return windowState.GetSubSequences();
            }

            /// <summary>
            /// Creates SequenceContexts from the breadcrumb stack, from top to bottom
            /// </summary>
            /// <returns>Collection of SequenceContexts. Should never be empty</returns>
            public IEnumerable<SequenceContext> GetBreadcrumbs()
            {
                return CollectBreadcrumbContexts();
            }

            /// <summary>
            /// Changes the current Timeline shown in the TimelineWindow to a new SequenceContext (if different and valid)
            /// </summary>
            /// <remarks>
            /// Should only ever accept SequenceContexts that are in the breadcrumbs. SetTimeline is the proper
            /// method to use to switch root Timelines.
            /// </remarks>
            /// <param name="context">A valid SequenceContext. <paramref name="context"/> should always be found in the breadcrumbs</param>
            /// <exception cref="System.ArgumentException"> The context is not valid</exception>
            /// <exception cref="System.InvalidOperationException"> The context is not a valid navigation destination.</exception>
            public void NavigateTo(SequenceContext context)
            {
                if (!context.IsValid())
                    throw new ArgumentException(
                        $"Argument {nameof(context)} is not valid. Check validity with SequenceContext.IsValid.");

                //If the provided context is the current context
                if (windowState.editSequence.hostClip == context.clip &&
                    windowState.editSequence.director == context.director &&
                    windowState.editSequence.asset == context.director.playableAsset)
                {
                    return; // Nothing to do
                }

                if (context.clip == null)
                {
                    if (context.director != windowState.masterSequence.director)
                        throw new InvalidOperationException($"{nameof(context)} is not a valid destination in this context. " +
                            $"To change the root context, use TimelineEditorWindow.SetTimeline instead.");
                }

                var children = GetChildContexts().ToArray();
                var breadcrumbs = CollectBreadcrumbContexts().ToArray();

                if (!children.Contains(context) && !breadcrumbs.Contains(context))
                {
                    throw new InvalidOperationException(
                        "The provided SequenceContext is not a valid destination. " +
                        "Use GetChildContexts or GetBreadcrumbs to acquire valid destination contexts.");
                }

                if (children.Contains(context))
                {
                    windowState.SetCurrentSequence(context.director.playableAsset as TimelineAsset, context.director, context.clip);
                    return;
                }

                var idx = Array.IndexOf(breadcrumbs, context);
                if (idx != -1)
                {
                    windowState.PopSequencesUntilCount(idx + 1);
                }
            }

            private IWindowState windowState
            {
                get
                {
                    if (m_Window == null || m_Window.windowState == null)
                        throw new InvalidOperationException("The Window associated to this instance has been destroyed");
                    return m_Window.windowState;
                }
            }

            private IEnumerable<SequenceContext> CollectBreadcrumbContexts()
            {
                return windowState.allSequences?.Select(s => new SequenceContext(s.director, s.hostClip));
            }

            private readonly IWindowStateProvider m_Window;
        }
    }
}
