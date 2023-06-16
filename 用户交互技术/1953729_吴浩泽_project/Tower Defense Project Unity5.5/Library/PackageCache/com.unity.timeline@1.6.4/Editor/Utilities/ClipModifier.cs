using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace UnityEditor.Timeline
{
    static class ClipModifier
    {
        public static bool Delete(TimelineAsset timeline, TimelineClip clip)
        {
            return timeline.DeleteClip(clip);
        }

        public static bool Tile(IEnumerable<TimelineClip> clips)
        {
            if (clips.Count() < 2)
                return false;

            var clipsByTracks = clips.GroupBy(x => x.GetParentTrack())
                .Select(track => new { track.Key, Items = track.OrderBy(c => c.start) });

            foreach (var track in clipsByTracks)
            {
                UndoExtensions.RegisterTrack(track.Key, L10n.Tr("Tile"));
            }

            foreach (var track in clipsByTracks)
            {
                double newStart = track.Items.First().start;
                foreach (var c in track.Items)
                {
                    c.start = newStart;
                    newStart += c.duration;
                }
            }

            return true;
        }

        public static bool TrimStart(IEnumerable<TimelineClip> clips, double trimTime)
        {
            var result = false;

            foreach (var clip in clips)
                result |= TrimStart(clip, trimTime);

            return result;
        }

        public static bool TrimStart(TimelineClip clip, double trimTime)
        {
            if (clip.asset == null)
                return false;

            if (clip.start > trimTime)
                return false;

            if (clip.end < trimTime)
                return false;

            UndoExtensions.RegisterClip(clip, L10n.Tr("Trim Clip Start"));

            // Note: We are NOT using edit modes in this case because we want the same result
            // regardless of the selected EditMode: split at cursor and delete left part
            SetStart(clip, trimTime, false);
            clip.ConformEaseValues();

            return true;
        }

        public static bool TrimEnd(IEnumerable<TimelineClip> clips, double trimTime)
        {
            var result = false;

            foreach (var clip in clips)
                result |= TrimEnd(clip, trimTime);

            return result;
        }

        public static bool TrimEnd(TimelineClip clip, double trimTime)
        {
            if (clip.asset == null)
                return false;

            if (clip.start > trimTime)
                return false;

            if (clip.end < trimTime)
                return false;

            UndoExtensions.RegisterClip(clip, L10n.Tr("Trim Clip End"));
            TrimClipWithEditMode(clip, TrimEdge.End, trimTime);

            return true;
        }

        public static bool MatchDuration(IEnumerable<TimelineClip> clips)
        {
            double referenceDuration = clips.First().duration;
            UndoExtensions.RegisterClips(clips, L10n.Tr("Match Clip Duration"));
            foreach (var clip in clips)
            {
                var newEnd = clip.start + referenceDuration;
                TrimClipWithEditMode(clip, TrimEdge.End, newEnd);
            }

            return true;
        }

        public static bool Split(IEnumerable<TimelineClip> clips, double splitTime, PlayableDirector director)
        {
            var result = false;

            foreach (var clip in clips)
            {
                if (clip.start >= splitTime)
                    continue;

                if (clip.end <= splitTime)
                    continue;

                UndoExtensions.RegisterClip(clip, L10n.Tr("Split Clip"));

                TimelineClip newClip = TimelineHelpers.Clone(clip, director, director, clip.start);

                clip.easeInDuration = 0;
                newClip.easeOutDuration = 0;

                SetStart(clip, splitTime, false);
                SetEnd(newClip, splitTime, false);

                // Sort produced by cloning clips on top of each other is unpredictable (it varies between mono runtimes)
                clip.GetParentTrack().SortClips();

                result = true;
            }

            return result;
        }

        public static void SetStart(TimelineClip clip, double time, bool affectTimeScale)
        {
            var supportsClipIn = clip.SupportsClipIn();
            var supportsPadding = TimelineUtility.IsRecordableAnimationClip(clip);
            bool calculateTimeScale = (affectTimeScale && clip.SupportsSpeedMultiplier());

            // treat empty recordable clips as not supporting clip in (there are no keys to modify)
            if (supportsPadding && (clip.animationClip == null || clip.animationClip.empty))
            {
                supportsClipIn = false;
            }

            if (supportsClipIn && !supportsPadding && !calculateTimeScale)
            {
                var minStart = clip.FromLocalTimeUnbound(0.0);
                if (time < minStart)
                    time = minStart;
            }

            var maxStart = clip.end - TimelineClip.kMinDuration;
            if (time > maxStart)
                time = maxStart;

            var timeOffset = time - clip.start;
            var duration = clip.duration - timeOffset;

            if (calculateTimeScale)
            {
                var f = clip.duration / duration;
                clip.timeScale *= f;
            }


            if (supportsClipIn && !calculateTimeScale)
            {
                if (supportsPadding)
                {
                    double clipInGlobal = clip.clipIn / clip.timeScale;
                    double keyShift = -timeOffset;
                    if (timeOffset < 0) // left drag, eliminate clipIn before shifting
                    {
                        double clipInDelta = Math.Max(-clipInGlobal, timeOffset);
                        keyShift = -Math.Min(0, timeOffset - clipInDelta);
                        clip.clipIn += clipInDelta * clip.timeScale;
                    }
                    else if (timeOffset > 0) // right drag, elimate padding in animation clip before adding clip in
                    {
                        var clipInfo = AnimationClipCurveCache.Instance.GetCurveInfo(clip.animationClip);
                        double keyDelta = clip.FromLocalTimeUnbound(clipInfo.keyTimes.Min()) - clip.start;
                        keyShift = -Math.Max(0, Math.Min(timeOffset, keyDelta));
                        clip.clipIn += Math.Max(timeOffset + keyShift, 0) * clip.timeScale;
                    }
                    if (keyShift != 0)
                    {
                        AnimationTrackRecorder.ShiftAnimationClip(clip.animationClip, (float)(keyShift * clip.timeScale));
                    }
                }
                else
                {
                    clip.clipIn += timeOffset * clip.timeScale;
                }
            }

            clip.start = time;
            clip.duration = duration;
        }

        public static void SetEnd(TimelineClip clip, double time, bool affectTimeScale)
        {
            var duration = Math.Max(time - clip.start, TimelineClip.kMinDuration);

            if (affectTimeScale && clip.SupportsSpeedMultiplier())
            {
                var f = clip.duration / duration;
                clip.timeScale *= f;
            }

            clip.duration = duration;
        }

        public static bool ResetEditing(IEnumerable<TimelineClip> clips)
        {
            var result = false;

            foreach (var clip in clips)
                result = result || ResetEditing(clip);

            return result;
        }

        public static bool ResetEditing(TimelineClip clip)
        {
            if (clip.asset == null)
                return false;

            UndoExtensions.RegisterClip(clip, L10n.Tr("Reset Clip Editing"));

            clip.clipIn = 0.0;

            if (clip.clipAssetDuration < double.MaxValue)
            {
                var duration = clip.clipAssetDuration / clip.timeScale;
                TrimClipWithEditMode(clip, TrimEdge.End, clip.start + duration);
            }

            return true;
        }

        public static bool MatchContent(IEnumerable<TimelineClip> clips)
        {
            var result = false;

            foreach (var clip in clips)
                result |= MatchContent(clip);

            return result;
        }

        public static bool MatchContent(TimelineClip clip)
        {
            if (clip.asset == null)
                return false;

            UndoExtensions.RegisterClip(clip, L10n.Tr("Match Clip Content"));

            var newStartCandidate = clip.start - clip.clipIn / clip.timeScale;
            var newStart = newStartCandidate < 0.0 ? 0.0 : newStartCandidate;

            TrimClipWithEditMode(clip, TrimEdge.Start, newStart);

            // In case resetting the start was blocked by edit mode or timeline start, we do the best we can
            clip.clipIn = (clip.start - newStartCandidate) * clip.timeScale;
            if (clip.clipAssetDuration > 0 && TimelineHelpers.HasUsableAssetDuration(clip))
            {
                var duration = TimelineHelpers.GetLoopDuration(clip);
                var offset = (clip.clipIn / clip.timeScale) % duration;
                TrimClipWithEditMode(clip, TrimEdge.End, clip.start - offset + duration);
            }

            return true;
        }

        public static void TrimClipWithEditMode(TimelineClip clip, TrimEdge edge, double time)
        {
            var clipItem = ItemsUtils.ToItem(clip);
            EditMode.BeginTrim(clipItem, edge);
            if (edge == TrimEdge.Start)
                EditMode.TrimStart(clipItem, time, false);
            else
                EditMode.TrimEnd(clipItem, time, false);
            EditMode.FinishTrim();
        }

        public static bool CompleteLastLoop(IEnumerable<TimelineClip> clips)
        {
            foreach (var clip in clips)
            {
                CompleteLastLoop(clip);
            }

            return true;
        }

        public static void CompleteLastLoop(TimelineClip clip)
        {
            FixLoops(clip, true);
        }

        public static bool TrimLastLoop(IEnumerable<TimelineClip> clips)
        {
            foreach (var clip in clips)
            {
                TrimLastLoop(clip);
            }

            return true;
        }

        public static void TrimLastLoop(TimelineClip clip)
        {
            FixLoops(clip, false);
        }

        static void FixLoops(TimelineClip clip, bool completeLastLoop)
        {
            if (!TimelineHelpers.HasUsableAssetDuration(clip))
                return;

            var loopDuration = TimelineHelpers.GetLoopDuration(clip);
            var firstLoopDuration = loopDuration - clip.clipIn * (1.0 / clip.timeScale);

            // Making sure we don't trim to zero
            if (!completeLastLoop && firstLoopDuration > clip.duration)
                return;

            var numLoops = (clip.duration - firstLoopDuration) / loopDuration;
            var numCompletedLoops = Math.Floor(numLoops);

            if (!(numCompletedLoops < numLoops))
                return;

            if (completeLastLoop)
                numCompletedLoops += 1;

            var newEnd = clip.start + firstLoopDuration + loopDuration * numCompletedLoops;

            UndoExtensions.RegisterClip(clip, L10n.Tr("Trim Clip Last Loop"));

            TrimClipWithEditMode(clip, TrimEdge.End, newEnd);
        }

        public static bool DoubleSpeed(IEnumerable<TimelineClip> clips)
        {
            foreach (var clip in clips)
            {
                if (clip.SupportsSpeedMultiplier())
                {
                    UndoExtensions.RegisterClip(clip, L10n.Tr("Double Clip Speed"));
                    clip.timeScale = clip.timeScale * 2.0f;
                }
            }

            return true;
        }

        public static bool HalfSpeed(IEnumerable<TimelineClip> clips)
        {
            foreach (var clip in clips)
            {
                if (clip.SupportsSpeedMultiplier())
                {
                    UndoExtensions.RegisterClip(clip, L10n.Tr("Half Clip Speed"));
                    clip.timeScale = clip.timeScale * 0.5f;
                }
            }

            return true;
        }

        public static bool ResetSpeed(IEnumerable<TimelineClip> clips)
        {
            foreach (var clip in clips)
            {
                if (clip.timeScale != 1.0)
                {
                    UndoExtensions.RegisterClip(clip, L10n.Tr("Reset Clip Speed"));
                    clip.timeScale = 1.0;
                }
            }

            return true;
        }
    }
}
