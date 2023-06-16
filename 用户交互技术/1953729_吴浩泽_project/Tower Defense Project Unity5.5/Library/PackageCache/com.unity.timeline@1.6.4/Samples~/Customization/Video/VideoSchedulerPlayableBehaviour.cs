using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline.Samples
{
    // The runtime instance of the VideoTrack. It is responsible for letting the VideoPlayableBehaviours
    //  they need to start loading the video
    public sealed class VideoSchedulerPlayableBehaviour : PlayableBehaviour
    {
        // Called every frame that the timeline is evaluated. This is called prior to
        // PrepareFrame on any of its input playables.
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // Searches for clips that are in the 'preload' area and prepares them for playback
            var timelineTime = playable.GetGraph().GetRootPlayable(0).GetTime();
            for (int i = 0; i < playable.GetInputCount(); i++)
            {
                if (playable.GetInput(i).GetPlayableType() != typeof(VideoPlayableBehaviour))
                    continue;

                if (playable.GetInputWeight(i) <= 0.0f)
                {
                    ScriptPlayable<VideoPlayableBehaviour> scriptPlayable = (ScriptPlayable<VideoPlayableBehaviour>)playable.GetInput(i);
                    VideoPlayableBehaviour videoPlayableBehaviour = scriptPlayable.GetBehaviour();
                    double preloadTime = Math.Max(0.0, videoPlayableBehaviour.preloadTime);
                    double clipStart = videoPlayableBehaviour.startTime;

                    if (timelineTime > clipStart - preloadTime && timelineTime <= clipStart)
                        videoPlayableBehaviour.PrepareVideo();
                }
            }
        }
    }
}
