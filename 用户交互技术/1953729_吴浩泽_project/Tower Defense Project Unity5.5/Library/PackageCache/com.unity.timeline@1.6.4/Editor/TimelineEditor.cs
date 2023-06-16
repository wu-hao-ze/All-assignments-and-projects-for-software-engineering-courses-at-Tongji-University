using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Information currently being edited in the Timeline Editor Window.
    /// </summary>
    public static class TimelineEditor
    {
        /// <summary>
        /// Returns a reference to the Timeline Window.
        /// </summary>
        /// <returns>A reference to the TimelineWindow and null if the window is not opened.</returns>
        public static TimelineEditorWindow GetWindow()
        {
            return window;
        }

        /// <summary>
        /// Returns a reference to the Timeline Window. If the window is not opened, it will be opened.
        /// </summary>
        /// <returns>A reference to the TimelineWindow.</returns>
        public static TimelineEditorWindow GetOrCreateWindow()
        {
            if (window != null)
                return window;

            return EditorWindow.GetWindow<TimelineWindow>(false, null, false);
        }

        /// <summary>
        /// The PlayableDirector associated with the timeline currently being shown in the Timeline window.
        /// </summary>
        public static PlayableDirector inspectedDirector => state?.editSequence.director;

        /// <summary>
        /// The PlayableDirector responsible for the playback of the timeline currently being shown in the Timeline window.
        /// </summary>
        public static PlayableDirector masterDirector => state?.masterSequence.director;

        /// <summary>
        /// The TimelineAsset currently being shown in the Timeline window.
        /// </summary>
        public static TimelineAsset inspectedAsset => state?.editSequence.asset;

        /// <summary>
        /// The TimelineAsset at the root of the hierarchy currently being shown in the Timeline window.
        /// </summary>
        public static TimelineAsset masterAsset => state?.masterSequence.asset;

        /// <summary>
        /// The PlayableDirector currently being shown in the Timeline Editor Window.
        /// </summary>
        [Obsolete("playableDirector is ambiguous. Please select either inspectedDirector or masterDirector instead.", false)]
        public static PlayableDirector playableDirector
        {
            get { return inspectedDirector; }
        }

        /// <summary>
        /// The TimelineAsset currently being shown in the Timeline Editor Window.
        /// </summary>
        [Obsolete("timelineAsset is ambiguous. Please select either inspectedAsset or masterAsset instead.", false)]
        public static TimelineAsset timelineAsset
        {
            get { return inspectedAsset; }
        }


        /// <summary>
        /// <para>
        /// Refreshes the different components affected by the currently inspected
        /// <see cref="UnityEngine.Timeline.TimelineAsset"/>, based on the <see cref="RefreshReason"/> provided.
        /// </para>
        /// <para>
        /// For better performance, it is recommended that you invoke this method once, after you modify the
        /// <see cref="UnityEngine.Timeline.TimelineAsset"/>. You should also combine reasons using the <c>|</c> operator.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Note: This operation is not synchronous. It is performed during the next GUI loop.
        /// </remarks>
        /// <param name="reason">The reason why a refresh should be performed.</param>
        public static void Refresh(RefreshReason reason)
        {
            if (state == null)
                return;

            if ((reason & RefreshReason.ContentsAddedOrRemoved) != 0)
            {
                state.Refresh();
            }
            else if ((reason & RefreshReason.ContentsModified) != 0)
            {
                state.rebuildGraph = true;
            }
            else if ((reason & RefreshReason.SceneNeedsUpdate) != 0)
            {
                state.Evaluate();
            }

            window.Repaint();
        }

        internal static TimelineWindow window => TimelineWindow.instance;
        internal static WindowState state => window == null ? null : window.state;

        internal static readonly Clipboard clipboard = new Clipboard();

        /// <summary>
        /// The list of clips selected in the TimelineEditor.
        /// </summary>
        public static TimelineClip[] selectedClips
        {
            get { return Selection.GetFiltered<EditorClip>(SelectionMode.Unfiltered).Select(e => e.clip).Where(x => x != null).ToArray(); }
            set
            {
                if (value == null || value.Length == 0)
                {
                    Selection.objects = null;
                }
                else
                {
                    var objects = new List<UnityEngine.Object>();
                    foreach (var clip in value)
                    {
                        if (clip == null)
                            continue;

                        var editorClip = EditorClipFactory.GetEditorClip(clip);
                        if (editorClip != null)
                            objects.Add(editorClip);
                    }

                    Selection.objects = objects.ToArray();
                }
            }
        }

        /// <summary>
        /// The clip selected in the TimelineEditor.
        /// </summary>
        /// <remarks>
        /// If there are multiple clips selected, this property returns the first clip.
        /// </remarks>
        public static TimelineClip selectedClip
        {
            get
            {
                var editorClip = Selection.activeObject as EditorClip;
                if (editorClip != null)
                    return editorClip.clip;
                return null;
            }
            set
            {
                var editorClip = (value != null) ? EditorClipFactory.GetEditorClip(value) : null;
                Selection.activeObject = editorClip;
            }
        }

        /// <summary>
        /// Local time (in seconds) of the inspected sequence.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if timeline window is not available.</exception>
        internal static double inspectedSequenceTime
        {
            get => state?.editSequence.time ?? 0;
            set
            {
                if (state == null)
                    throw new InvalidOperationException("Cannot set time. Timeline Window may not be available.");
                state.editSequence.time = value;
            }
        }

        /// <summary>
        /// Global time (in seconds) of the master timeline.
        /// Same as local time if not inspected a subtimeline.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if timeline window is not available.</exception>
        internal static double masterSequenceTime
        {
            get => state?.editSequence.ToGlobalTime(state.editSequence.time) ?? 0;
            set
            {
                if (state == null)
                    throw new InvalidOperationException("Cannot set time. Timeline Window may not be available.");
                state.masterSequence.time = value;
            }
        }

        /// <summary>
        /// Visible time range (in seconds) in Editor.
        /// x : min time
        /// y : max time
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if timeline window is not available.</exception>
        internal static Vector2 visibleTimeRange
        {
            get => state?.timeAreaShownRange ?? TimelineAssetViewModel.TimeAreaDefaultRange;
            set
            {
                if (state == null)
                    throw new InvalidOperationException("Cannot set visible time range. Timeline Window may not be available.");
                state.timeAreaShownRange = value;
            }
        }

        internal static ActionContext CurrentContext(Vector2? mousePos = null)
        {
            return new ActionContext
            {
                invocationTime = mousePos != null ? TimelineHelpers.GetCandidateTime(mousePos) : (double?)null,
                clips = SelectionManager.SelectedClips(),
                tracks = SelectionManager.SelectedTracks(),
                markers = SelectionManager.SelectedMarkers(),
                timeline = inspectedAsset,
                director = inspectedDirector
            };
        }

        /// <summary>
        /// Converts time from the master timeline to the current inspected timeline.
        /// </summary>
        /// <param name="masterTime">Time in the referential of the main timeline</param>
        /// <returns>Time in the referential of the sub-timeline that is currently show.
        /// Returns <paramref name="masterTime"/> if there is no sub-timeline or if no timeline is shown.</returns>
        public static double GetInspectedTimeFromMasterTime(double masterTime)
        {
            ISequenceState editSequence = state?.editSequence;
            if (editSequence == null)
                return masterTime;
            return state.editSequence.ToLocalTime(masterTime);
        }

        /// <summary>
        /// Converts time from the current inspected timeline to the master timeline.
        /// </summary>
        /// <param name="inspectedTime">Time in the referential of the sub-timeline</param>
        /// <returns>Time in the referential of the main timeline.
        /// Returns <paramref name="inspectedTime"/> if there if no timeline is shown.</returns>
        public static double GetMasterTimeFromInspectedTime(double inspectedTime)
        {
            ISequenceState editSequence = state?.editSequence;
            if (editSequence == null)
                return inspectedTime;
            return editSequence.ToGlobalTime(inspectedTime);
        }

        internal static void RefreshPreviewPlay()
        {
            if (state == null || !state.playing)
                return;
            state.Pause();
            state.Play();
        }
    }

    /// <summary>
    /// <see cref="TimelineEditor.Refresh"/> uses these flags to determine what needs to be refreshed or updated.
    /// </summary>
    /// <remarks>
    /// Use the <c>|</c> operator to combine flags.
    /// <example>
    /// <code source="../DocCodeExamples/TimelineEditorExamples.cs" region="declare-refreshReason" title="refreshReason"/>
    /// </example>
    /// </remarks>
    [Flags]
    public enum RefreshReason
    {
        /// <summary>
        /// Use this flag when a change to the Timeline requires that the Timeline window be redrawn.
        /// </summary>
        WindowNeedsRedraw = 1 << 0,

        /// <summary>
        /// Use this flag when a change to the Timeline requires that the Scene be updated.
        /// </summary>
        SceneNeedsUpdate = 1 << 1,

        /// <summary>
        /// Use this flag when a Timeline element was modified.
        /// </summary>
        ContentsModified = 1 << 2,

        /// <summary>
        /// Use this flag when an element was added to or removed from the Timeline.
        /// </summary>
        ContentsAddedOrRemoved = 1 << 3
    }
}
