using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Samples
{
    // Timeline track that supports changing the game time
    // The TimeDilation track demonstrates how to
    //  * Support blended and/or extrapolated clips.
    //  * Support changing a Unity static variable from timeline.
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(TimeDilationPlayableAsset))]
    public class TimeDilationTrack : TrackAsset
    {
        // Creates a runtime instance of the track, represented by a PlayableBehaviour.
        // The runtime instance performs mixing on the timeline clips.
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TimeDilationMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
