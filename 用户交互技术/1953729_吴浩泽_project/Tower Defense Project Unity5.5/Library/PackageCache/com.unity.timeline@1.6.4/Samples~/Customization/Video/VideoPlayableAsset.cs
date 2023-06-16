using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace Timeline.Samples
{
    // Editor representation of a Clip to play video in Timeline.
    [Serializable]
    public class VideoPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public enum RenderMode
        {
            CameraFarPlane,
            CameraNearPlane
        };

        [Tooltip("The video clip to play.")]
        public VideoClip videoClip;

        [Tooltip("Mutes the audio from the video")]
        public bool mute;

        [Tooltip("Loops the video.")]
        public bool loop = true;

        [Tooltip("The amount of time before the video begins to start preloading the video stream.")]
        public double preloadTime = 0.3;

        [Tooltip("The aspect ratio of the video to playback.")]
        public VideoAspectRatio aspectRatio = VideoAspectRatio.FitHorizontally;

        [Tooltip("Where the video content will be drawn.")]
        public RenderMode renderMode = RenderMode.CameraFarPlane;

        [Tooltip("Specifies which camera to render to. If unassigned, the main camera will be used.")]
        public ExposedReference<Camera> targetCamera;

        [Tooltip("Specifies an optional audio source to output to.")]
        public ExposedReference<AudioSource> audioSource;

        // These are set by the track prior to CreatePlayable being called and are used by the VideoSchedulePlayableBehaviour
        // to schedule preloading of the video clip
        public double clipInTime { get; set; }
        public double startTime { get; set; }

        // Creates the playable that represents the instance that plays this clip.
        // Here a hidden VideoPlayer is being created for the PlayableBehaviour to use
        // to control playback. The PlayableBehaviour is responsible for deleting the player.
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            Camera camera = targetCamera.Resolve(graph.GetResolver());
            if (camera == null)
                camera = Camera.main;

            // If we are unable to create a player, return a playable with no behaviour attached.
            VideoPlayer player = CreateVideoPlayer(camera, audioSource.Resolve(graph.GetResolver()));
            if (player == null)
                return Playable.Create(graph);

            ScriptPlayable<VideoPlayableBehaviour> playable =
                ScriptPlayable<VideoPlayableBehaviour>.Create(graph);

            VideoPlayableBehaviour playableBehaviour = playable.GetBehaviour();
            playableBehaviour.videoPlayer = player;
            playableBehaviour.preloadTime = preloadTime;
            playableBehaviour.clipInTime = clipInTime;
            playableBehaviour.startTime = startTime;

            return playable;
        }

        // The playable assets duration is used to specify the initial or default duration of the clip in Timeline.
        public override double duration
        {
            get
            {
                if (videoClip == null)
                    return base.duration;
                return videoClip.length;
            }
        }

        // Implementation of ITimelineClipAsset. This specifies the capabilities of this timeline clip inside the editor.
        // For video clips, we are using built-in support for clip-in, speed, blending and looping.
        public ClipCaps clipCaps
        {
            get
            {
                var caps = ClipCaps.Blending | ClipCaps.ClipIn | ClipCaps.SpeedMultiplier;
                if (loop)
                    caps |= ClipCaps.Looping;
                return caps;
            }
        }


        VideoPlayer CreateVideoPlayer(Camera camera, AudioSource targetAudioSource)
        {
            if (videoClip == null)
                return null;

            GameObject gameObject = new GameObject(videoClip.name) { hideFlags = HideFlags.HideAndDontSave };
            VideoPlayer videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
            videoPlayer.waitForFirstFrame = false;
            videoPlayer.skipOnDrop = true;
            videoPlayer.targetCamera = camera;
            videoPlayer.renderMode = renderMode == RenderMode.CameraFarPlane ? VideoRenderMode.CameraFarPlane : VideoRenderMode.CameraNearPlane;
            videoPlayer.aspectRatio = aspectRatio;
            videoPlayer.isLooping = loop;

            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            if (mute)
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            }
            else if (targetAudioSource != null)
            {
                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                for (ushort i = 0; i < videoPlayer.clip.audioTrackCount; ++i)
                    videoPlayer.SetTargetAudioSource(i, targetAudioSource);
            }

            return videoPlayer;
        }
    }
}
