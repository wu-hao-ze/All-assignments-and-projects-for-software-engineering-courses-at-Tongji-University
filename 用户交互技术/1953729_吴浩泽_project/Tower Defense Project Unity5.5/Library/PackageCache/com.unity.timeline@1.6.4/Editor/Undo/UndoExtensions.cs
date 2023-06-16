using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Use this class to record the state of a timeline or its components prior to modification.
    /// </summary>
    /// <remarks>
    /// These methods do not need to be used when adding or deleting tracks, clips or markers.
    /// Methods in the UnityEngine.Timeline namespace, such as <see cref="UnityEngine.Timeline.TimelineAsset.CreateTrack"/>
    /// or <see cref="UnityEngine.Timeline.TrackAsset.CreateDefaultClip"/> will apply the appropriate
    /// Undo calls when called in Editor.
    /// </remarks>
    public static class UndoExtensions
    {
        /// <summary>
        /// Records all items contained in an action context. Use this method to record all objects
        /// inside the context.
        /// </summary>
        /// <param name="context">The action context to record into the Undo system.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        public static void RegisterContext(ActionContext context, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
            {
                undo.Add(context.tracks);
                undo.Add(context.clips, true);
                undo.Add(context.markers);
            }
        }

        /// <summary>
        /// Records any changes done on the timeline after being called. This only applies
        /// to the timeline asset properties itself, and not any of the tracks or clips on the timeline
        /// </summary>
        /// <param name="asset">The timeline asset being modified.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        public static void RegisterTimeline(TimelineAsset asset, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.AddObject(asset);
        }

        /// <summary>
        /// Records any changes done on the timeline after being called, including any changes
        ///  to any clips, tracks and markers that occur on the timeline.
        /// </summary>
        /// <param name="asset">The timeline asset being modified.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        public static void RegisterCompleteTimeline(TimelineAsset asset, string undoTitle)
        {
            if (asset == null)
                return;

            using (var undo = new UndoScope(undoTitle))
            {
                undo.AddObject(asset);
                undo.Add(asset.flattenedTracks);
                foreach (var t in asset.flattenedTracks)
                {
                    undo.Add(t.GetClips(), true);
                    undo.Add(t.GetMarkers());
                }
            }
        }

        /// <summary>
        /// Records any changes done on the track after being called, including any changes
        ///  to clips on the track, but not on markers or PlayableAssets attached to the clips.
        /// </summary>
        /// <param name="asset">The timeline track being modified.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        public static void RegisterTrack(TrackAsset asset, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.AddObject(asset);
        }

        /// <summary>
        /// Records any changes done on the tracks after being called, including any changes
        ///  to clips on the tracks, but not on markers or PlayableAssets attached to the clips.
        /// </summary>
        /// <param name="tracks">The timeline track being modified.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        public static void RegisterTracks(IEnumerable<TrackAsset> tracks, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.Add(tracks);
        }

        /// <summary>
        /// Records any changes done on the clip after being called.
        /// </summary>
        /// <param name="clip">The timeline clip being modified.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        /// <param name="includePlayableAsset">Set this value to true to also record changes on the attached playable asset.</param>
        public static void RegisterClip(TimelineClip clip, string undoTitle, bool includePlayableAsset = true)
        {
            using (var undo = new UndoScope(undoTitle))
            {
                undo.AddClip(clip, includePlayableAsset);
            }
        }

        /// <summary>
        /// Records any changes done on the PlayableAsset after being called.
        /// </summary>
        /// <param name="asset">The timeline track being modified.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        public static void RegisterPlayableAsset(PlayableAsset asset, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.AddObject(asset);
        }

        /// <summary>
        /// Records any changes done on the clips after being called.
        /// </summary>
        /// <param name="clips">The timeline clips being modified.</param>
        /// <param name="name">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        /// <param name="includePlayableAssets">Set this value to true to also record changes on the attached playable assets.</param>
        public static void RegisterClips(IEnumerable<TimelineClip> clips, string name, bool includePlayableAssets = true)
        {
            using (var undo = new UndoScope(name))
                undo.Add(clips, includePlayableAssets);
        }

        /// <summary>
        /// Records any changes done on the Timeline Marker after being called.
        /// </summary>
        /// <param name="marker">The timeline clip being modified.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        public static void RegisterMarker(IMarker marker, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
            {
                if (marker is Object o)
                    undo.AddObject(o);
                else if (marker != null)
                    undo.AddObject(marker.parent);
            }
        }

        /// <summary>
        /// Records any changes done on the Timeline Markers after being called.
        /// </summary>
        /// <param name="markers">The timeline clip being modified.</param>
        /// <param name="undoTitle">The title of the action to appear in the undo history (i.e. visible in the undo menu).</param>
        public static void RegisterMarkers(IEnumerable<IMarker> markers, string undoTitle)
        {
            using (var undo = new UndoScope(undoTitle))
                undo.Add(markers);
        }
    }
}
