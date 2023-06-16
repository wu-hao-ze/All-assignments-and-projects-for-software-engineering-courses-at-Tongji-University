using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Class containing methods to invoke actions.
    /// </summary>
    public static class Invoker
    {
        /// <summary>
        /// Execute a given action with a context parameter.
        /// </summary>
        /// <typeparam name="T">Action type to execute.</typeparam>
        /// <param name="context">Context for the action.</param>
        /// <returns>True if the action has been executed, false otherwise.</returns>
        public static bool Invoke<T>(this ActionContext context) where T : TimelineAction
        {
            var action = ActionManager.TimelineActions.GetCachedAction<T, TimelineAction>();
            return ActionManager.ExecuteTimelineAction(action, context);
        }

        /// <summary>
        /// Execute a given action with tracks
        /// </summary>
        /// <typeparam name="T">Action type to execute.</typeparam>
        ///  <param name="tracks">Tracks that the action will act on.</param>
        /// <returns>True if the action has been executed, false otherwise.</returns>
        public static bool Invoke<T>(this IEnumerable<TrackAsset> tracks) where T : TrackAction
        {
            var action = ActionManager.TrackActions.GetCachedAction<T, TrackAction>();
            return ActionManager.ExecuteTrackAction(action, tracks);
        }

        /// <summary>
        /// Execute a given action with clips
        /// </summary>
        /// <typeparam name="T">Action type to execute.</typeparam>
        ///  <param name="clips">Clips that the action will act on.</param>
        /// <returns>True if the action has been executed, false otherwise.</returns>
        public static bool Invoke<T>(this IEnumerable<TimelineClip> clips) where T : ClipAction
        {
            var action = ActionManager.ClipActions.GetCachedAction<T, ClipAction>();
            return ActionManager.ExecuteClipAction(action, clips);
        }

        /// <summary>
        /// Execute a given action with markers
        /// </summary>
        /// <typeparam name="T">Action type to execute.</typeparam>
        /// <param name="markers">Markers that the action will act on.</param>
        /// <returns>True if the action has been executed, false otherwise.</returns>
        public static bool Invoke<T>(this IEnumerable<IMarker> markers) where T : MarkerAction
        {
            var action = ActionManager.MarkerActions.GetCachedAction<T, MarkerAction>();
            return ActionManager.ExecuteMarkerAction(action, markers);
        }

        /// <summary>
        /// Execute a given timeline action with the selected clips, tracks and markers.
        /// </summary>
        /// <typeparam name="T">Action type to execute.</typeparam>
        /// <returns>True if the action has been executed, false otherwise.</returns>
        public static bool InvokeWithSelected<T>() where T : TimelineAction
        {
            return Invoke<T>(TimelineEditor.CurrentContext());
        }

        /// <summary>
        /// Execute a given clip action with the selected clips.
        /// </summary>
        /// <typeparam name="T">Action type to execute.</typeparam>
        /// <returns>True if the action has been executed, false otherwise.</returns>
        public static bool InvokeWithSelectedClips<T>() where T : ClipAction
        {
            return Invoke<T>(SelectionManager.SelectedClips());
        }

        /// <summary>
        /// Execute a given track action with the selected tracks.
        /// </summary>
        /// <typeparam name="T">Action type to execute.</typeparam>
        /// <returns>True if the action has been executed, false otherwise.</returns>
        public static bool InvokeWithSelectedTracks<T>() where T : TrackAction
        {
            return Invoke<T>(SelectionManager.SelectedTracks());
        }

        /// <summary>
        /// Execute a given marker action with the selected markers.
        /// </summary>
        /// <typeparam name="T">Action type to execute.</typeparam>
        /// <returns>True if the action has been executed, false otherwise.</returns>
        public static bool InvokeWithSelectedMarkers<T>() where T : MarkerAction
        {
            return Invoke<T>(SelectionManager.SelectedMarkers());
        }
    }
}
