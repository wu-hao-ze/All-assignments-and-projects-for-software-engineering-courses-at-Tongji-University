using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace UnityEditor.Timeline
{
    [ApplyDefaultUndo("Match Offsets")]
    [MenuEntry("Match Offsets To Previous Clip", MenuPriority.CustomClipActionSection.matchPrevious), UsedImplicitly]
    class MatchOffsetsPreviousAction : ClipAction
    {
        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            if (clips == null || !clips.Any())
                return false;
            AnimationOffsetMenu.MatchClipsToPrevious(clips.Where(x => IsValidClip(x, TimelineEditor.inspectedDirector)).ToArray());
            return true;
        }

        static bool IsValidClip(TimelineClip clip, PlayableDirector director)
        {
            return clip != null &&
                clip.GetParentTrack() != null &&
                (clip.asset as AnimationPlayableAsset) != null &&
                clip.GetParentTrack().clips.Any(x => x.start < clip.start) &&
                TimelineUtility.GetSceneGameObject(director, clip.GetParentTrack()) != null;
        }

        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            if (!clips.All(TimelineAnimationUtilities.IsAnimationClip))
                return ActionValidity.NotApplicable;

            var director = TimelineEditor.inspectedDirector;
            if (TimelineEditor.inspectedDirector == null)
                return ActionValidity.NotApplicable;

            if (clips.Any(c => IsValidClip(c, director)))
                return ActionValidity.Valid;

            return ActionValidity.NotApplicable;
        }
    }

    [ApplyDefaultUndo("Match Offsets")]
    [MenuEntry("Match Offsets To Next Clip", MenuPriority.CustomClipActionSection.matchNext), UsedImplicitly]
    class MatchOffsetsNextAction : ClipAction
    {
        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            AnimationOffsetMenu.MatchClipsToNext(clips.Where(x => IsValidClip(x, TimelineEditor.inspectedDirector)).ToArray());
            return true;
        }

        static bool IsValidClip(TimelineClip clip, PlayableDirector director)
        {
            return clip != null &&
                clip.GetParentTrack() != null &&
                (clip.asset as AnimationPlayableAsset) != null &&
                clip.GetParentTrack().clips.Any(x => x.start > clip.start) &&
                TimelineUtility.GetSceneGameObject(director, clip.GetParentTrack()) != null;
        }

        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            if (!clips.All(TimelineAnimationUtilities.IsAnimationClip))
                return ActionValidity.NotApplicable;

            var director = TimelineEditor.inspectedDirector;
            if (TimelineEditor.inspectedDirector == null)
                return ActionValidity.NotApplicable;

            if (clips.Any(c => IsValidClip(c, director)))
                return ActionValidity.Valid;

            return ActionValidity.NotApplicable;
        }
    }

    [ApplyDefaultUndo]
    [MenuEntry("Reset Offsets", MenuPriority.CustomClipActionSection.resetOffset), UsedImplicitly]
    class ResetOffsets : ClipAction
    {
        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            AnimationOffsetMenu.ResetClipOffsets(clips.Where(TimelineAnimationUtilities.IsAnimationClip).ToArray());
            return true;
        }

        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            if (!clips.All(TimelineAnimationUtilities.IsAnimationClip))
                return ActionValidity.NotApplicable;

            return ActionValidity.Valid;
        }
    }
}
