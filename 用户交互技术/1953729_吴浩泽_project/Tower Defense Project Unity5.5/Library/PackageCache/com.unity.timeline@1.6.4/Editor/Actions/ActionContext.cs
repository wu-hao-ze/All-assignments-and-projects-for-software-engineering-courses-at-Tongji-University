using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Action context to be used by actions.
    /// </summary>
    /// <seealso cref="Invoker"/>
    /// <seealso cref="TimelineAction"/>
    public struct ActionContext
    {
        IEnumerable<TrackAsset> m_Tracks;
        IEnumerable<TimelineClip> m_Clips;
        IEnumerable<IMarker> m_Markers;

        /// <summary>
        ///  The Timeline asset that is currently opened in the Timeline window.
        /// </summary>
        public TimelineAsset timeline;

        /// <summary>
        ///  The PlayableDirector that is used to play the current Timeline asset.
        /// </summary>
        public PlayableDirector director;

        /// <summary>
        ///  Time based on the position of the cursor on the timeline (in seconds).
        ///  null if the time is not available (in case of a shortcut for example).
        /// </summary>
        public double? invocationTime;

        /// <summary>
        ///  Tracks that will be used by the actions.
        /// </summary>
        public IEnumerable<TrackAsset> tracks
        {
            get => m_Tracks ?? Enumerable.Empty<TrackAsset>();
            set => m_Tracks = value;
        }

        /// <summary>
        ///  Clips that will be used by the actions.
        /// </summary>
        public IEnumerable<TimelineClip> clips
        {
            get => m_Clips ?? Enumerable.Empty<TimelineClip>();
            set => m_Clips = value;
        }

        /// <summary>
        ///  Markers that will be used by the actions.
        /// </summary>
        public IEnumerable<IMarker> markers
        {
            get => m_Markers ?? Enumerable.Empty<IMarker>();
            set => m_Markers = value;
        }
    }
}
