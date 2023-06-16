using System;
using UnityEngine;

namespace UnityEngine.Timeline
{
    public partial class TimelineAsset
    {
        /// <summary>
        /// Enum to specify the type of a track. This enum is obsolete.
        /// </summary>
        [Obsolete("MediaType has been deprecated. It is no longer required, and will be removed in a future release.", false)]
        public enum MediaType
        {
            /// <summary>
            /// Specifies that a track is used for animation.
            /// <see cref="UnityEngine.Timeline.TimelineAsset.MediaType"/> is obsolete.
            /// </summary>
            Animation,

            /// <summary>
            /// Specifies that a track is used for audio.
            /// <see cref="UnityEngine.Timeline.TimelineAsset.MediaType"/> is obsolete.
            /// </summary>
            Audio,

            /// <summary>
            /// Specifies that a track is used for a texture.
            /// <see cref="UnityEngine.Timeline.TimelineAsset.MediaType"/> is obsolete.
            /// </summary>
            Texture = 2,

            /// <summary>
            /// Specifies that a track is used for video.
            /// <see cref="UnityEngine.Timeline.TimelineAsset.MediaType"/> is obsolete.
            /// </summary>
            [Obsolete("Use Texture MediaType instead. (UnityUpgradable) -> UnityEngine.Timeline.TimelineAsset/MediaType.Texture", false)]
            Video = 2,

            /// <summary>
            /// Specifies that a track is used for scripting.
            /// <see cref="UnityEngine.Timeline.TimelineAsset.MediaType"/> is obsolete.
            /// </summary>
            Script,

            /// <summary>
            /// Specifies that a track is used for multiple media types.
            /// <see cref="UnityEngine.Timeline.TimelineAsset.MediaType"/> is obsolete.
            /// </summary>
            Hybrid,

            /// <summary>
            /// Specifies that a track is used for a group.
            /// <see cref="UnityEngine.Timeline.TimelineAsset.MediaType"/> is obsolete.
            /// </summary>
            Group
        }
    }


    /// <summary>
    /// TrackMediaType defines the type of a track. This attribute is obsolete; it will have no effect.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Obsolete("TrackMediaType has been deprecated. It is no longer required, and will be removed in a future release.", false)]
    public class TrackMediaType : Attribute // Defines the type of a track
    {
        /// <summary>
        /// MediaType of a track.
        /// <see cref="UnityEngine.Timeline.TrackMediaType"/> is obsolete; it will have no effect.
        /// </summary>
        public readonly TimelineAsset.MediaType m_MediaType;

        /// <summary>
        /// Constructs a MediaType attribute.
        /// <see cref="UnityEngine.Timeline.TrackMediaType"/> is obsolete; it will have no effect.
        /// </summary>
        /// <param name="mt"><inheritdoc cref="m_MediaType"/></param>
        public TrackMediaType(TimelineAsset.MediaType mt)
        {
            m_MediaType = mt;
        }
    }
}
