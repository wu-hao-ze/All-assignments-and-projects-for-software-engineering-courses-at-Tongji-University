using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    [MenuEntry("Add Override Track", MenuPriority.CustomTrackActionSection.addOverrideTrack), UsedImplicitly]
    class AddOverrideTrackAction : TrackAction
    {
        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            foreach (var animTrack in tracks.OfType<AnimationTrack>())
            {
                TimelineHelpers.CreateTrack(typeof(AnimationTrack), animTrack, "Override " + animTrack.GetChildTracks().Count());
            }

            return true;
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            if (tracks.Any(t => t.isSubTrack || !t.GetType().IsAssignableFrom(typeof(AnimationTrack))))
                return ActionValidity.NotApplicable;

            if (tracks.Any(t => t.lockedInHierarchy))
                return ActionValidity.Invalid;

            return ActionValidity.Valid;
        }
    }

    [MenuEntry("Convert To Clip Track", MenuPriority.CustomTrackActionSection.convertToClipMode), UsedImplicitly]
    class ConvertToClipModeAction : TrackAction
    {
        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            foreach (var animTrack in tracks.OfType<AnimationTrack>())
                animTrack.ConvertToClipMode();

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            return true;
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            if (tracks.Any(t => !t.GetType().IsAssignableFrom(typeof(AnimationTrack))))
                return ActionValidity.NotApplicable;

            if (tracks.Any(t => t.lockedInHierarchy))
                return ActionValidity.Invalid;

            if (tracks.OfType<AnimationTrack>().All(a => a.CanConvertToClipMode()))
                return ActionValidity.Valid;

            return ActionValidity.NotApplicable;
        }
    }

    [MenuEntry("Convert To Infinite Clip", MenuPriority.CustomTrackActionSection.convertFromClipMode), UsedImplicitly]
    class ConvertFromClipTrackAction : TrackAction
    {
        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            foreach (var animTrack in tracks.OfType<AnimationTrack>())
                animTrack.ConvertFromClipMode(TimelineEditor.inspectedAsset);

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            return true;
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            if (tracks.Any(t => !t.GetType().IsAssignableFrom(typeof(AnimationTrack))))
                return ActionValidity.NotApplicable;

            if (tracks.Any(t => t.lockedInHierarchy))
                return ActionValidity.Invalid;

            if (tracks.OfType<AnimationTrack>().All(a => a.CanConvertFromClipMode()))
                return ActionValidity.Valid;

            return ActionValidity.NotApplicable;
        }
    }

    abstract class TrackOffsetBaseAction : TrackAction
    {
        public abstract TrackOffset trackOffset { get; }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            if (tracks.Any(t => !t.GetType().IsAssignableFrom(typeof(AnimationTrack))))
                return ActionValidity.NotApplicable;

            if (tracks.Any(t => t.lockedInHierarchy))
            {
                return ActionValidity.Invalid;
            }

            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            foreach (var animTrack in tracks.OfType<AnimationTrack>())
            {
                animTrack.UnarmForRecord();
                animTrack.trackOffset = trackOffset;
            }

            TimelineEditor.Refresh(RefreshReason.ContentsModified);
            return true;
        }
    }


    [MenuEntry("Track Offsets/Apply Transform Offsets", MenuPriority.CustomTrackActionSection.applyTrackOffset), UsedImplicitly]
    [ApplyDefaultUndo]
    class ApplyTransformOffsetAction : TrackOffsetBaseAction
    {
        public override TrackOffset trackOffset
        {
            get { return TrackOffset.ApplyTransformOffsets; }
        }
    }

    [MenuEntry("Track Offsets/Apply Scene Offsets", MenuPriority.CustomTrackActionSection.applySceneOffset), UsedImplicitly]
    [ApplyDefaultUndo]
    class ApplySceneOffsetAction : TrackOffsetBaseAction
    {
        public override TrackOffset trackOffset
        {
            get { return TrackOffset.ApplySceneOffsets; }
        }
    }

    [MenuEntry("Track Offsets/Auto (Deprecated)", MenuPriority.CustomTrackActionSection.applyAutoOffset), UsedImplicitly]
    [ApplyDefaultUndo]
    class ApplyAutoAction : TrackOffsetBaseAction
    {
        public override TrackOffset trackOffset
        {
            get { return TrackOffset.Auto; }
        }
    }
}
