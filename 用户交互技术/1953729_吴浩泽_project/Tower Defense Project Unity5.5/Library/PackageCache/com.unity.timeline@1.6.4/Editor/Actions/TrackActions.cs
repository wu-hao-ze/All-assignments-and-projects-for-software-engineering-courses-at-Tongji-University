using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    [MenuEntry("Edit in Animation Window", MenuPriority.TrackActionSection.editInAnimationWindow)]
    class EditTrackInAnimationWindow : TrackAction
    {
        public static bool Do(TrackAsset track)
        {
            AnimationClip clipToEdit = null;

            AnimationTrack animationTrack = track as AnimationTrack;
            if (animationTrack != null)
            {
                if (!animationTrack.CanConvertToClipMode())
                    return false;

                clipToEdit = animationTrack.infiniteClip;
            }
            else if (track.hasCurves)
            {
                clipToEdit = track.curves;
            }

            if (clipToEdit == null)
                return false;

            GameObject gameObject = null;
            if (TimelineEditor.inspectedDirector != null)
                gameObject = TimelineUtility.GetSceneGameObject(TimelineEditor.inspectedDirector, track);

            var timeController = TimelineAnimationUtilities.CreateTimeController(CreateTimeControlClipData(track));
            TimelineAnimationUtilities.EditAnimationClipWithTimeController(clipToEdit, timeController, gameObject);

            return true;
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            if (!tracks.Any())
                return ActionValidity.Invalid;

            var firstTrack = tracks.First();
            if (firstTrack is AnimationTrack)
            {
                var animTrack = firstTrack as AnimationTrack;
                if (animTrack.CanConvertToClipMode())
                    return ActionValidity.Valid;
            }
            else if (firstTrack.hasCurves)
            {
                return ActionValidity.Valid;
            }

            return ActionValidity.NotApplicable;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            return Do(tracks.First());
        }

        static TimelineWindowTimeControl.ClipData CreateTimeControlClipData(TrackAsset track)
        {
            var data = new TimelineWindowTimeControl.ClipData();
            data.track = track;
            data.start = track.start;
            data.duration = track.duration;
            return data;
        }
    }

    [MenuEntry("Lock selected track only", MenuPriority.TrackActionSection.lockSelected)]
    class LockSelectedTrack : TrackAction, IMenuName
    {
        public static readonly string LockSelectedTrackOnlyText = L10n.Tr("Lock selected track only");
        public static readonly string UnlockSelectedTrackOnlyText = L10n.Tr("Unlock selected track only");

        public string menuName { get; private set; }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            UpdateMenuName(tracks);
            if (tracks.Any(track => TimelineUtility.IsLockedFromGroup(track) || track is GroupTrack || !track.subTracksObjects.Any()))
                return ActionValidity.NotApplicable;
            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            if (!tracks.Any()) return false;

            var hasUnlockedTracks = tracks.Any(x => !x.locked);
            Lock(tracks.Where(p => !(p is GroupTrack)).ToArray(), hasUnlockedTracks);
            return true;
        }

        void UpdateMenuName(IEnumerable<TrackAsset> tracks)
        {
            menuName = tracks.All(t => t.locked) ? UnlockSelectedTrackOnlyText : LockSelectedTrackOnlyText;
        }

        public static void Lock(TrackAsset[] tracks, bool shouldlock)
        {
            if (tracks.Length == 0)
                return;

            foreach (var track in tracks.Where(t => !TimelineUtility.IsLockedFromGroup(t)))
            {
                TimelineUndo.PushUndo(track, L10n.Tr("Lock Tracks"));
                track.locked = shouldlock;
            }
            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
        }
    }

    [MenuEntry("Lock", MenuPriority.TrackActionSection.lockTrack)]
    [Shortcut(Shortcuts.Timeline.toggleLock)]
    class LockTrack : TrackAction, IMenuName
    {
        static readonly string k_LockText = L10n.Tr("Lock");
        static readonly string k_UnlockText = L10n.Tr("Unlock");

        public string menuName { get; private set; }

        void UpdateMenuName(IEnumerable<TrackAsset> tracks)
        {
            menuName = tracks.Any(x => !x.locked) ? k_LockText : k_UnlockText;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            if (!tracks.Any()) return false;

            var hasUnlockedTracks = tracks.Any(x => !x.locked);
            SetLockState(tracks, hasUnlockedTracks);
            return true;
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            UpdateMenuName(tracks);
            tracks = tracks.RemoveTimelineMarkerTrackFromList(TimelineEditor.inspectedAsset);

            if (!tracks.Any())
                return ActionValidity.NotApplicable;
            if (tracks.Any(TimelineUtility.IsLockedFromGroup))
                return ActionValidity.Invalid;
            return ActionValidity.Valid;
        }

        public static void SetLockState(IEnumerable<TrackAsset> tracks, bool shouldLock)
        {
            if (!tracks.Any())
                return;

            foreach (var track in tracks)
            {
                if (TimelineUtility.IsLockedFromGroup(track))
                    continue;

                if (track as GroupTrack == null)
                    SetLockState(track.GetChildTracks().ToArray(), shouldLock);

                TimelineUndo.PushUndo(track, L10n.Tr("Lock Tracks"));
                track.locked = shouldLock;
            }

            // find the tracks we've locked. unselect anything locked and remove recording.
            foreach (var track in tracks)
            {
                if (TimelineUtility.IsLockedFromGroup(track) || !track.locked)
                    continue;

                var flattenedChildTracks = track.GetFlattenedChildTracks();
                foreach (var i in track.clips)
                    SelectionManager.Remove(i);
                track.UnarmForRecord();
                foreach (var child in flattenedChildTracks)
                {
                    SelectionManager.Remove(child);
                    child.UnarmForRecord();
                    foreach (var clip in child.GetClips())
                        SelectionManager.Remove(clip);
                }
            }

            // no need to rebuild, just repaint (including inspectors)
            InspectorWindow.RepaintAllInspectors();
            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
        }
    }

    [UsedImplicitly]
    [MenuEntry("Show Markers", MenuPriority.TrackActionSection.showHideMarkers)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class ShowHideMarkers : TrackAction, IMenuChecked
    {
        public bool isChecked { get; private set; }

        void UpdateCheckedStatus(IEnumerable<TrackAsset> tracks)
        {
            isChecked = tracks.All(x => x.GetShowMarkers());
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            UpdateCheckedStatus(tracks);
            if (tracks.Any(x => x is GroupTrack) || tracks.Any(t => t.GetMarkerCount() == 0))
                return ActionValidity.NotApplicable;

            if (tracks.Any(t => t.lockedInHierarchy))
            {
                return ActionValidity.Invalid;
            }

            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            if (!tracks.Any()) return false;

            var hasUnlockedTracks = tracks.Any(x => !x.GetShowMarkers());
            ShowHide(tracks, hasUnlockedTracks);
            return true;
        }

        static void ShowHide(IEnumerable<TrackAsset> tracks, bool shouldLock)
        {
            if (!tracks.Any())
                return;

            foreach (var track in tracks)
                track.SetShowTrackMarkers(shouldLock);

            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
        }
    }

    [MenuEntry("Mute selected track only", MenuPriority.TrackActionSection.muteSelected), UsedImplicitly]
    class MuteSelectedTrack : TrackAction, IMenuName
    {
        public static readonly string MuteSelectedText = L10n.Tr("Mute selected track only");
        public static readonly string UnmuteSelectedText = L10n.Tr("Unmute selected track only");

        public string menuName { get; private set; }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            UpdateMenuName(tracks);
            if (tracks.Any(track => TimelineUtility.IsParentMuted(track) || track is GroupTrack || !track.subTracksObjects.Any()))
                return ActionValidity.NotApplicable;
            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            if (!tracks.Any())
                return false;

            var hasUnmutedTracks = tracks.Any(x => !x.muted);
            Mute(tracks.Where(p => !(p is GroupTrack)).ToArray(), hasUnmutedTracks);
            return true;
        }

        void UpdateMenuName(IEnumerable<TrackAsset> tracks)
        {
            menuName = tracks.All(t => t.muted) ? UnmuteSelectedText : MuteSelectedText;
        }

        public static void Mute(TrackAsset[] tracks, bool shouldMute)
        {
            if (tracks.Length == 0)
                return;

            foreach (var track in tracks.Where(t => !TimelineUtility.IsParentMuted(t)))
            {
                TimelineUndo.PushUndo(track, L10n.Tr("Mute Tracks"));
                track.muted = shouldMute;
            }

            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }
    }

    [MenuEntry("Mute", MenuPriority.TrackActionSection.mute)]
    [Shortcut(Shortcuts.Timeline.toggleMute)]
    class MuteTrack : TrackAction, IMenuName
    {
        static readonly string k_MuteText = L10n.Tr("Mute");
        static readonly string k_UnMuteText = L10n.Tr("Unmute");

        public string menuName { get; private set; }

        void UpdateMenuName(IEnumerable<TrackAsset> tracks)
        {
            menuName = tracks.Any(x => !x.muted) ? k_MuteText : k_UnMuteText;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            if (!tracks.Any() || tracks.Any(TimelineUtility.IsParentMuted))
                return false;

            var hasUnmutedTracks = tracks.Any(x => !x.muted);
            Mute(tracks, hasUnmutedTracks);
            return true;
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            UpdateMenuName(tracks);
            if (tracks.Any(TimelineUtility.IsLockedFromGroup))
                return ActionValidity.Invalid;
            return ActionValidity.Valid;
        }

        public static void Mute(IEnumerable<TrackAsset> tracks, bool shouldMute)
        {
            if (!tracks.Any())
                return;

            foreach (var track in tracks)
            {
                if (track as GroupTrack == null)
                    Mute(track.GetChildTracks().ToArray(), shouldMute);
                TimelineUndo.PushUndo(track, L10n.Tr("Mute Tracks"));
                track.muted = shouldMute;
            }

            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }
    }

    class DeleteTracks : TrackAction
    {
        public static void Do(TimelineAsset timeline, TrackAsset track)
        {
            SelectionManager.Remove(track);
            TrackModifier.DeleteTrack(timeline, track);
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks) => ActionValidity.Valid;

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            tracks = tracks.RemoveTimelineMarkerTrackFromList(TimelineEditor.inspectedAsset);

            // disable preview mode so deleted tracks revert to default state
            // Case 956129: Disable preview mode _before_ deleting the tracks, since clip data is still needed
            TimelineEditor.state.previewMode = false;

            TimelineAnimationUtilities.UnlinkAnimationWindowFromTracks(tracks);

            foreach (var track in tracks)
                Do(TimelineEditor.inspectedAsset, track);

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            return true;
        }
    }

    class CopyTracksToClipboard : TrackAction
    {
        public static bool Do(TrackAsset[] tracks)
        {
            var action = new CopyTracksToClipboard();
            return action.Execute(tracks);
        }

        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks) => ActionValidity.Valid;

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            tracks = tracks.RemoveTimelineMarkerTrackFromList(TimelineEditor.inspectedAsset);
            TimelineEditor.clipboard.CopyTracks(tracks);
            return true;
        }
    }

    class DuplicateTracks : TrackAction
    {
        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks) => ActionValidity.Valid;

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            tracks = tracks.RemoveTimelineMarkerTrackFromList(TimelineEditor.inspectedAsset);
            if (tracks.Any())
            {
                SelectionManager.RemoveTimelineSelection();
            }

            foreach (var track in TrackExtensions.FilterTracks(tracks))
            {
                var newTrack = track.Duplicate(TimelineEditor.inspectedDirector, TimelineEditor.inspectedDirector);
                //Add all duplicated tracks to selection
                SelectionManager.Add(newTrack);
                foreach (var childTrack in newTrack.GetFlattenedChildTracks())
                {
                    SelectionManager.Add(childTrack);
                }

                //Duplicate bindings for tracks and subtracks
                if (TimelineEditor.inspectedDirector != null)
                {
                    DuplicateBindings(track, newTrack, TimelineEditor.inspectedDirector);
                }
            }

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            return true;
        }

        internal static void DuplicateBindings(TrackAsset track, TrackAsset newTrack, PlayableDirector director)
        {
            var originalTracks = track.GetFlattenedChildTracks().Append(track);
            var newTracks = newTrack.GetFlattenedChildTracks().Append(newTrack);
            var toBind = new List<Tuple<TrackAsset, Object>>();

            // Collect all track bindings to duplicate
            var originalIt = originalTracks.GetEnumerator();
            var newIt = newTracks.GetEnumerator();
            while (originalIt.MoveNext() && newIt.MoveNext())
            {
                var binding = director.GetGenericBinding(originalIt.Current);
                if (binding != null)
                    toBind.Add(new Tuple<TrackAsset, Object>(newIt.Current, binding));
            }

            //Only create Director undo if there are bindings to duplicate
            if (toBind.Count > 0)
                TimelineUndo.PushUndo(TimelineEditor.inspectedDirector, L10n.Tr("Duplicate"));

            //Assign bindings for all tracks after undo.
            foreach (var binding in toBind)
            {
                TimelineEditor.inspectedDirector.SetGenericBinding(binding.Item1, binding.Item2);
            }
        }
    }

    [MenuEntry("Remove Invalid Markers", MenuPriority.TrackActionSection.removeInvalidMarkers), UsedImplicitly]
    class RemoveInvalidMarkersAction : TrackAction
    {
        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            if (tracks.Any(target => target != null && target.GetMarkerCount() != target.GetMarkersRaw().Count()))
                return ActionValidity.Valid;

            return ActionValidity.NotApplicable;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            bool anyRemoved = false;
            foreach (var target in tracks)
            {
                var invalids = target.GetMarkersRaw().Where(x => !(x is IMarker)).ToList();
                foreach (var m in invalids)
                {
                    anyRemoved = true;
                    target.DeleteMarkerRaw(m);
                }
            }

            if (anyRemoved)
                TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            return anyRemoved;
        }
    }

    [Shortcut(Shortcuts.Timeline.collapseTrack)]
    [UsedImplicitly]
    class CollapseTrackAction : TrackAction
    {
        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            var collapsibleTracks = tracks.Where(track => track.subTracksObjects.Any());

            if (!collapsibleTracks.Any())
                return ActionValidity.NotApplicable;

            if (collapsibleTracks.All(track => track.IsCollapsed()))
                return ActionValidity.NotApplicable;

            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            return KeyboardNavigation.TryCollapse(tracks.Where(track => track.subTracksObjects.Any() && !track.IsCollapsed()));
        }
    }

    [Shortcut(Shortcuts.Timeline.expandTrack)]
    [UsedImplicitly]
    class ExpandTrackAction : TrackAction
    {
        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            var collapsibleTracks = tracks.Where(track => track.subTracksObjects.Any());

            if (!collapsibleTracks.Any())
                return ActionValidity.NotApplicable;

            if (collapsibleTracks.All(track => !track.IsCollapsed()))
                return ActionValidity.NotApplicable;

            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            return KeyboardNavigation.TryExpand(tracks.Where(track => track.subTracksObjects.Any() && track.IsCollapsed()));
        }
    }
}
