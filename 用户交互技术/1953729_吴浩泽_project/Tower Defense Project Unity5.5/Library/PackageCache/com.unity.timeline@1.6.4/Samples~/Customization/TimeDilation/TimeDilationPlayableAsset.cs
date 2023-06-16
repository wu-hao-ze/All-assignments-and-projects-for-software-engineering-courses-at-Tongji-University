using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Samples
{
    // A clip for the timeline dilation track.
    [Serializable]
    public class TimeDilationPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        // Using a template for the playable behaviour will allow any serializable fields on the behaviour
        // to be animated.
        [NoFoldOut]
        public TimeDilationBehaviour template = new TimeDilationBehaviour();

        // Implementation of ITimelineClipAsset, that tells the timeline editor which
        // features this clip supports.
        public ClipCaps clipCaps
        {
            get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
        }

        // Called to creates a runtime instance of the clip.
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // Note that template is passed as a parameter - this
            // creates a clone of the template PlayableBehaviour.
            return ScriptPlayable<TimeDilationBehaviour>.Create(graph, template);
        }
    }
}
