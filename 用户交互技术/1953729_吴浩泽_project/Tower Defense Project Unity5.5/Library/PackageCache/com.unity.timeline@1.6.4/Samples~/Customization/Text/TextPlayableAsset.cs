#if TEXT_TRACK_REQUIRES_TEXTMESH_PRO

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Samples
{
    // Represents the serialized data for a clip on the TextTrack
    [Serializable]
    public class TextPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [NoFoldOut]
        [NotKeyable] // NotKeyable used to prevent Timeline from making fields available for animation.
        public TextPlayableBehaviour template = new TextPlayableBehaviour();

        // Implementation of ITimelineClipAsset. This specifies the capabilities of this timeline clip inside the editor.
        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        // Creates the playable that represents the instance of this clip.
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // Using a template will clone the serialized values
            return ScriptPlayable<TextPlayableBehaviour>.Create(graph, template);
        }
    }
}

#endif
