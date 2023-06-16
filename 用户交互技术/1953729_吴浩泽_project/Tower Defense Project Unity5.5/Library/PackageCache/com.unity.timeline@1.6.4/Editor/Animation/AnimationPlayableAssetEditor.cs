using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    [CustomTimelineEditor(typeof(AnimationPlayableAsset)), UsedImplicitly]
    class AnimationPlayableAssetEditor : ClipEditor
    {
        public static readonly string k_NoClipAssignedError = L10n.Tr("No animation clip assigned");
        public static readonly string k_LegacyClipError = L10n.Tr("Legacy animation clips are not supported");
        static readonly string k_MotionCurveError = L10n.Tr("You are using motion curves without applyRootMotion enabled on the Animator. The root transform will not be animated");
        static readonly string k_RootCurveError = L10n.Tr("You are using root curves without applyRootMotion enabled on the Animator. The root transform will not be animated");

        /// <inheritdoc/>
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var clipOptions = base.GetClipOptions(clip);
            var asset = clip.asset as AnimationPlayableAsset;

            if (asset != null)
                clipOptions.errorText = GetErrorText(asset, clip.GetParentTrack() as AnimationTrack, clipOptions.errorText);

            if (clip.recordable)
                clipOptions.highlightColor = DirectorStyles.Instance.customSkin.colorAnimationRecorded;

            return clipOptions;
        }

        /// <inheritdoc />
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            var asset = clip.asset as AnimationPlayableAsset;
            if (asset != null && asset.clip != null && asset.clip.legacy)
            {
                asset.clip = null;
                Debug.LogError("Legacy Animation Clips are not supported");
            }
        }

        string GetErrorText(AnimationPlayableAsset animationAsset, AnimationTrack track, string defaultError)
        {
            if (animationAsset.clip == null)
                return k_NoClipAssignedError;
            if (animationAsset.clip.legacy)
                return k_LegacyClipError;
            if (animationAsset.clip.hasMotionCurves || animationAsset.clip.hasRootCurves)
            {
                if (track != null && track.trackOffset == TrackOffset.Auto)
                {
                    var animator = track.GetBinding(TimelineEditor.inspectedDirector);
                    if (animator != null && !animator.applyRootMotion && !animationAsset.clip.hasGenericRootTransform)
                    {
                        if (animationAsset.clip.hasMotionCurves)
                            return k_MotionCurveError;
                        return k_RootCurveError;
                    }
                }
            }

            return defaultError;
        }
    }
}
