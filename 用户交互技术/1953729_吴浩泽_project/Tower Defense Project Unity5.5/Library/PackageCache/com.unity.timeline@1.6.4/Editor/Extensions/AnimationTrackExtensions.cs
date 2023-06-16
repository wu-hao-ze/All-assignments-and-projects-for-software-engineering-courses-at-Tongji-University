using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Extension Methods for AnimationTracks that require the Unity Editor, and may require the Timeline containing the Animation Track to be currently loaded in the Timeline Editor Window.
    /// </summary>
    public static class AnimationTrackExtensions
    {
        /// <summary>
        /// Determines whether the Timeline window can enable recording mode on an AnimationTrack.
        /// For a track to support recording, it needs to have a valid scene binding,
        /// its offset mode should not be Auto and needs to be currently visible in the Timeline Window.
        /// </summary>
        /// <param name="track">The track to query.</param>
        /// <returns>True if recording can start, False otherwise.</returns>
        public static bool CanStartRecording(this AnimationTrack track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }
            if (TimelineEditor.state == null)
            {
                return false;
            }

            var director = TimelineEditor.inspectedDirector;
            var animTrack = TimelineUtility.GetSceneReferenceTrack(track) as AnimationTrack;
            return animTrack != null && animTrack.trackOffset != TrackOffset.Auto &&
                TimelineEditor.inspectedAsset == animTrack.timelineAsset &&
                director != null && TimelineUtility.GetSceneGameObject(director, animTrack) != null;
        }

        /// <summary>
        /// Method that allows querying if a track is current enabled for animation recording.
        /// </summary>
        /// <param name="track">The track to query.</param>
        /// <returns>True if currently recording and False otherwise.</returns>
        public static bool IsRecording(this AnimationTrack track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }
            return TimelineEditor.state != null && TimelineEditor.state.IsArmedForRecord(track);
        }

        /// <summary>
        /// Method that enables animation recording for an AnimationTrack.
        /// </summary>
        /// <param name="track">The AnimationTrack which will be put in recording mode.</param>
        /// <returns>True if track was put successfully in recording mode, False otherwise. </returns>
        public static bool StartRecording(this AnimationTrack track)
        {
            if (!CanStartRecording(track))
            {
                return false;
            }
            TimelineEditor.state.ArmForRecord(track);
            return true;
        }

        /// <summary>
        /// Disables recording mode of an AnimationTrack.
        /// </summary>
        /// <param name="track">The AnimationTrack which will be taken out of recording mode.</param>
        public static void StopRecording(this AnimationTrack track)
        {
            if (!IsRecording(track) || TimelineEditor.state == null)
            {
                return;
            }

            TimelineEditor.state.UnarmForRecord(track);
        }

        internal static void ConvertToClipMode(this AnimationTrack track)
        {
            if (!track.CanConvertToClipMode())
                return;

            UndoExtensions.RegisterTrack(track, L10n.Tr("Convert To Clip"));

            if (!track.infiniteClip.empty)
            {
                var animClip = track.infiniteClip;
                TimelineUndo.PushUndo(animClip, L10n.Tr("Convert To Clip"));
                UndoExtensions.RegisterTrack(track, L10n.Tr("Convert To Clip"));
                var start = AnimationClipCurveCache.Instance.GetCurveInfo(animClip).keyTimes.FirstOrDefault();
                animClip.ShiftBySeconds(-start);

                track.infiniteClip = null;
                var clip = track.CreateClip(animClip);

                clip.start = start;
                clip.preExtrapolationMode = track.infiniteClipPreExtrapolation;
                clip.postExtrapolationMode = track.infiniteClipPostExtrapolation;
                clip.recordable = true;
                if (Mathf.Abs(animClip.length) < TimelineClip.kMinDuration)
                {
                    clip.duration = 1;
                }

                var animationAsset = clip.asset as AnimationPlayableAsset;
                if (animationAsset)
                {
                    animationAsset.position = track.infiniteClipOffsetPosition;
                    animationAsset.eulerAngles = track.infiniteClipOffsetEulerAngles;

                    // going to / from infinite mode should reset this. infinite mode
                    animationAsset.removeStartOffset = track.infiniteClipRemoveOffset;
                    animationAsset.applyFootIK = track.infiniteClipApplyFootIK;
                    animationAsset.loop = track.infiniteClipLoop;

                    track.infiniteClipOffsetPosition = Vector3.zero;
                    track.infiniteClipOffsetEulerAngles = Vector3.zero;
                }

                track.CalculateExtrapolationTimes();
            }

            track.infiniteClip = null;

            EditorUtility.SetDirty(track);
        }

        internal static void ConvertFromClipMode(this AnimationTrack track, TimelineAsset timeline)
        {
            if (!track.CanConvertFromClipMode())
                return;

            UndoExtensions.RegisterTrack(track, L10n.Tr("Convert From Clip"));

            var clip = track.clips[0];
            var delta = (float)clip.start;
            track.infiniteClipTimeOffset = 0.0f;
            track.infiniteClipPreExtrapolation = clip.preExtrapolationMode;
            track.infiniteClipPostExtrapolation = clip.postExtrapolationMode;

            var animAsset = clip.asset as AnimationPlayableAsset;
            if (animAsset)
            {
                track.infiniteClipOffsetPosition = animAsset.position;
                track.infiniteClipOffsetEulerAngles = animAsset.eulerAngles;
                track.infiniteClipRemoveOffset = animAsset.removeStartOffset;
                track.infiniteClipApplyFootIK = animAsset.applyFootIK;
                track.infiniteClipLoop = animAsset.loop;
            }

            // clone it, it may not be in the same asset
            var animClip = clip.animationClip;

            float scale = (float)clip.timeScale;
            if (!Mathf.Approximately(scale, 1.0f))
            {
                if (!Mathf.Approximately(scale, 0.0f))
                    scale = 1.0f / scale;
                animClip.ScaleTime(scale);
            }

            TimelineUndo.PushUndo(animClip, L10n.Tr("Convert From Clip"));
            animClip.ShiftBySeconds(delta);

            // manually delete the clip
            var asset = clip.asset;
            clip.asset = null;

            // Remove the clip, remove old assets
            ClipModifier.Delete(timeline, clip);
            TimelineUndo.PushDestroyUndo(null, track, asset);

            track.infiniteClip = animClip;

            EditorUtility.SetDirty(track);
        }

        internal static bool CanConvertToClipMode(this AnimationTrack track)
        {
            if (track == null || track.inClipMode)
                return false;
            return (track.infiniteClip != null && !track.infiniteClip.empty);
        }

        // Requirements to go from clip mode
        //  - one clip, recordable, and animation clip belongs to the same asset as the track
        internal static bool CanConvertFromClipMode(this AnimationTrack track)
        {
            if ((track == null) ||
                (!track.inClipMode) ||
                (track.clips.Length != 1) ||
                (track.clips[0].start < 0) ||
                (!track.clips[0].recordable))
                return false;

            var asset = track.clips[0].asset as AnimationPlayableAsset;
            if (asset == null)
                return false;

            return TimelineHelpers.HaveSameContainerAsset(track, asset.clip);
        }
    }
}
