using UnityEngine;
using UnityEngine.Timeline;

namespace DocCodeExamples
{
    class TrackAssetExamples_HideAPI
    {
        #region declare-trackAssetExample

        [TrackColor(1, 0, 0)]
        [TrackBindingType(typeof(Animator))]
        [TrackClipType(typeof(AnimationClip))]
        public class CustomAnimationTrack : TrackAsset { }

        #endregion
    }
}
