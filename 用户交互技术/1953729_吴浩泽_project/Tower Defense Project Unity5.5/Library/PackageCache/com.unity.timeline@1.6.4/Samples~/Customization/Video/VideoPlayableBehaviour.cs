using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace Timeline.Samples
{
    // The runtime instance of a video clip player in Timeline.
    public sealed class VideoPlayableBehaviour : PlayableBehaviour
    {
        public VideoPlayer videoPlayer;

        public double preloadTime;
        public double clipInTime;
        public double startTime;

        private bool preparing;

        // Called by the mixer (VideoSchedulerPlayableBehaviour) when this is nearly active to
        // give the video time to load.
        public void PrepareVideo()
        {
            if (videoPlayer == null || videoPlayer.isPrepared || preparing)
                return;

            videoPlayer.targetCameraAlpha = 0.0f;
            videoPlayer.time = clipInTime;
            videoPlayer.Prepare();
            preparing = true;
        }

        // Called each frame the clip is active.
        //
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (videoPlayer == null)
                return;

            // Pause or Play the video to match whether the graph is being scrubbed or playing
            //  If we need to hold the last frame, this will treat the last frame as a pause
            bool shouldBePlaying = info.evaluationType == FrameData.EvaluationType.Playback;
            if (!videoPlayer.isLooping && playable.GetTime() >= videoPlayer.clip.length)
                shouldBePlaying = false;

            if (shouldBePlaying)
            {
                // this will use the timeline time to prevent drift
                videoPlayer.timeReference = VideoTimeReference.ExternalTime;
                if (!videoPlayer.isPlaying)
                    videoPlayer.Play();
                videoPlayer.externalReferenceTime = playable.GetTime() / videoPlayer.playbackSpeed;
            }
            else
            {
                videoPlayer.timeReference = VideoTimeReference.Freerun;
                if (!videoPlayer.isPaused)
                    videoPlayer.Pause();
                SyncVideoToPlayable(playable);
            }

            // use the accumulated blend value to set the alpha and the audio volume
            videoPlayer.targetCameraAlpha = info.effectiveWeight;
            if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
            {
                for (ushort i = 0; i < videoPlayer.clip.audioTrackCount; ++i)
                    videoPlayer.SetDirectAudioVolume(i, info.effectiveWeight);
            }
        }

        // Called when the clip becomes active.
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (videoPlayer == null)
                return;

            SyncVideoToPlayable(playable);
            videoPlayer.playbackSpeed = Mathf.Clamp(info.effectiveSpeed, 1 / 10f, 10f);
            videoPlayer.Play();
            preparing = false;
        }

        // Called when the clip becomes inactive OR the timeline is 'paused'
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (videoPlayer == null)
                return;

            preparing = false;

            // The effective weight will be greater than 0 if the graph is paused and the playhead is still on this clip.
            if (info.effectiveWeight <= 0)
                videoPlayer.Stop();
            else
                videoPlayer.Pause();
        }

        // Called when the playable is destroyed.
        public override void OnPlayableDestroy(Playable playable)
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
                if (Application.isPlaying)
                    Object.Destroy(videoPlayer.gameObject);
                else
                    Object.DestroyImmediate(videoPlayer.gameObject);
            }
        }

        // Syncs the video player time to playable time
        private void SyncVideoToPlayable(Playable playable)
        {
            if (videoPlayer == null || videoPlayer.clip == null)
                return;

            if (videoPlayer.isLooping)
                videoPlayer.time = playable.GetTime() % videoPlayer.clip.length;
            else
                videoPlayer.time = System.Math.Min(playable.GetTime(), videoPlayer.clip.length);
        }
    }
}
