using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    [MenuEntry("Copy", MenuPriority.TimelineActionSection.copy)]
    [Shortcut("Main Menu/Edit/Copy", EventCommandNames.Copy)]
    class CopyAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context)
        {
            if (SelectionManager.Count() == 0)
                return ActionValidity.NotApplicable;
            if (context.tracks.ContainsTimelineMarkerTrack(context.timeline))
                return ActionValidity.NotApplicable;

            return ActionValidity.Valid;
        }

        public override bool Execute(ActionContext context)
        {
            TimelineEditor.clipboard.Clear();

            var clips = context.clips;
            if (clips.Any())
            {
                clips.Invoke<CopyClipsToClipboard>();
            }
            var markers = context.markers;
            if (markers.Any())
            {
                markers.Invoke<CopyMarkersToClipboard>();
            }
            var tracks = context.tracks;
            if (tracks.Any())
            {
                CopyTracksToClipboard.Do(tracks.ToArray());
            }

            return true;
        }
    }

    [MenuEntry("Paste", MenuPriority.TimelineActionSection.paste)]
    [Shortcut("Main Menu/Edit/Paste", EventCommandNames.Paste)]
    class PasteAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context)
        {
            return CanPaste(context.invocationTime) ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(ActionContext context)
        {
            if (!CanPaste(context.invocationTime))
                return false;

            PasteItems(context.invocationTime);
            PasteTracks();

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            return true;
        }

        static bool CanPaste(double? invocationTime)
        {
            var copiedItems = TimelineEditor.clipboard.GetCopiedItems().ToList();

            if (!copiedItems.Any())
                return TimelineEditor.clipboard.GetTracks().Any();

            return CanPasteItems(copiedItems, invocationTime);
        }

        static bool CanPasteItems(ICollection<ItemsPerTrack> itemsGroups, double? invocationTime)
        {
            var hasItemsCopiedFromMultipleTracks = itemsGroups.Count > 1;
            var allItemsCopiedFromCurrentAsset = itemsGroups.All(x => x.targetTrack.timelineAsset == TimelineEditor.inspectedAsset);
            var hasUsedShortcut = invocationTime == null;
            var anySourceLocked = itemsGroups.Any(x => x.targetTrack != null && x.targetTrack.lockedInHierarchy);

            var targetTrack = GetPickedTrack();
            if (targetTrack == null)
                targetTrack = SelectionManager.SelectedTracks().FirstOrDefault();

            //do not paste if the user copied items from another timeline
            //if the copied items comes from > 1 track (since we do not know where to paste the copied items)
            //or if a keyboard shortcut was used (since the user will not see the paste result)
            if (!allItemsCopiedFromCurrentAsset)
            {
                var isSelectedTrackInCurrentAsset = targetTrack != null && targetTrack.timelineAsset == TimelineEditor.inspectedAsset;
                if (hasItemsCopiedFromMultipleTracks || (hasUsedShortcut && !isSelectedTrackInCurrentAsset))
                    return false;
            }

            // pasting to items to their source track, if items from multiple tracks are selected
            // and no track is in the selection items will each be pasted to their respective track.
            if (targetTrack == null || itemsGroups.All(x => x.targetTrack == targetTrack))
                return !anySourceLocked;

            if (hasItemsCopiedFromMultipleTracks)
            {
                //do not paste if the track which received the paste action does not contain a copied clip
                return !anySourceLocked && itemsGroups.Select(x => x.targetTrack).Contains(targetTrack);
            }

            var copiedItems = itemsGroups.SelectMany(i => i.items);
            return IsTrackValidForItems(targetTrack, copiedItems);
        }

        static void PasteItems(double? invocationTime)
        {
            var copiedItems = TimelineEditor.clipboard.GetCopiedItems().ToList();
            var numberOfUniqueParentsInClipboard = copiedItems.Count();

            if (numberOfUniqueParentsInClipboard == 0) return;
            List<ITimelineItem> newItems;

            //if the copied items were on a single parent, then use the mouse position to get the parent OR the original parent
            if (numberOfUniqueParentsInClipboard == 1)
            {
                var itemsGroup = copiedItems.First();
                TrackAsset target = null;
                if (invocationTime.HasValue)
                    target = GetPickedTrack();
                if (target == null)
                    target = FindSuitableParentForSingleTrackPasteWithoutMouse(itemsGroup);

                var candidateTime = invocationTime ?? TimelineHelpers.GetCandidateTime(null, target);
                newItems = TimelineHelpers.DuplicateItemsUsingCurrentEditMode(TimelineEditor.clipboard.exposedPropertyTable, TimelineEditor.inspectedDirector, itemsGroup, target, candidateTime, "Paste Items").ToList();
            }
            //if copied items were on multiple parents, then the destination parents are the same as the original parents
            else
            {
                var time = invocationTime ?? TimelineHelpers.GetCandidateTime(null, copiedItems.Select(c => c.targetTrack).ToArray());
                newItems = TimelineHelpers.DuplicateItemsUsingCurrentEditMode(TimelineEditor.clipboard.exposedPropertyTable, TimelineEditor.inspectedDirector, copiedItems, time, "Paste Items").ToList();
            }

            TimelineHelpers.FrameItems(newItems);
            SelectionManager.RemoveTimelineSelection();
            foreach (var item in newItems)
            {
                SelectionManager.Add(item);
            }
        }

        static TrackAsset FindSuitableParentForSingleTrackPasteWithoutMouse(ItemsPerTrack itemsGroup)
        {
            var groupParent = itemsGroup.targetTrack; //set a main parent in the clipboard
            var selectedTracks = SelectionManager.SelectedTracks();

            if (selectedTracks.Contains(groupParent))
            {
                return groupParent;
            }

            //find a selected track suitable for all items
            var itemsToPaste = itemsGroup.items;
            var compatibleTrack = selectedTracks.FirstOrDefault(t => IsTrackValidForItems(t, itemsToPaste));
            return compatibleTrack != null ? compatibleTrack : groupParent;
        }

        static bool IsTrackValidForItems(TrackAsset track, IEnumerable<ITimelineItem> items)
        {
            if (track == null || track.lockedInHierarchy) return false;
            return items.All(i => i.IsCompatibleWithTrack(track));
        }

        static TrackAsset GetPickedTrack()
        {
            if (PickerUtils.pickedElements == null)
                return null;

            var rowGUI = PickerUtils.pickedElements.OfType<IRowGUI>().FirstOrDefault();
            if (rowGUI != null)
                return rowGUI.asset;

            return null;
        }

        static void PasteTracks()
        {
            var trackData = TimelineEditor.clipboard.GetTracks().ToList();
            if (trackData.Any())
            {
                SelectionManager.RemoveTimelineSelection();
            }

            foreach (var track in trackData)
            {
                var newTrack = track.item.Duplicate(TimelineEditor.clipboard.exposedPropertyTable, TimelineEditor.inspectedDirector, TimelineEditor.inspectedAsset);
                var newTracks = newTrack.GetFlattenedChildTracks().Append(newTrack);

                var bindingIt = track.bindings.GetEnumerator();
                var newTrackIt = newTracks.GetEnumerator();

                while (bindingIt.MoveNext() && newTrackIt.MoveNext())
                {
                    if (bindingIt.Current != null)
                        BindingUtility.Bind(TimelineEditor.inspectedDirector, newTrackIt.Current, bindingIt.Current);
                }

                SelectionManager.Add(newTrack);
                foreach (var childTrack in newTrack.GetFlattenedChildTracks())
                {
                    SelectionManager.Add(childTrack);
                }

                if (track.parent != null && track.parent.timelineAsset == TimelineEditor.inspectedAsset)
                {
                    TrackExtensions.ReparentTracks(new List<TrackAsset> { newTrack }, track.parent, track.item);
                }
            }
        }
    }

    [MenuEntry("Duplicate", MenuPriority.TimelineActionSection.duplicate)]
    [Shortcut("Main Menu/Edit/Duplicate", EventCommandNames.Duplicate)]
    class DuplicateAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context)
        {
            IEnumerable<TrackAsset> tracks = context.tracks.RemoveTimelineMarkerTrackFromList(context.timeline);
            return context.clips.Any() || tracks.Any() || context.markers.Any() ? ActionValidity.Valid : ActionValidity.NotApplicable;
        }

        public bool Execute(Func<ITimelineItem, ITimelineItem, double> gapBetweenItems)
        {
            return Execute(TimelineEditor.CurrentContext(), gapBetweenItems);
        }

        public override bool Execute(ActionContext context)
        {
            return Execute(context, (item1, item2) => ItemsUtils.TimeGapBetweenItems(item1, item2));
        }

        internal bool Execute(ActionContext context, Func<ITimelineItem, ITimelineItem, double> gapBetweenItems)
        {
            List<ITimelineItem> items = new List<ITimelineItem>();
            items.AddRange(context.clips.Select(p => p.ToItem()));
            items.AddRange(context.markers.Select(p => p.ToItem()));
            List<ItemsPerTrack> selectedItems = items.ToItemsPerTrack().ToList();
            if (selectedItems.Any())
            {
                var requestedTime = CalculateDuplicateTime(selectedItems, gapBetweenItems);
                var duplicatedItems = TimelineHelpers.DuplicateItemsUsingCurrentEditMode(TimelineEditor.inspectedDirector, TimelineEditor.inspectedDirector, selectedItems, requestedTime, "Duplicate Items");

                TimelineHelpers.FrameItems(duplicatedItems);
                SelectionManager.RemoveTimelineSelection();
                foreach (var item in duplicatedItems)
                    SelectionManager.Add(item);
            }

            var tracks = context.tracks.ToArray();
            if (tracks.Length > 0)
                tracks.Invoke<DuplicateTracks>();

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            return true;
        }

        static double CalculateDuplicateTime(IEnumerable<ItemsPerTrack> duplicatedItems, Func<ITimelineItem, ITimelineItem, double> gapBetweenItems)
        {
            //Find the end time of the rightmost item
            var itemsOnTracks = duplicatedItems.SelectMany(i => i.targetTrack.GetItems()).ToList();
            var time = itemsOnTracks.Max(i => i.end);

            //From all the duplicated items, select the leftmost items
            var firstDuplicatedItems = duplicatedItems.Select(i => i.leftMostItem);
            var leftMostDuplicatedItems = firstDuplicatedItems.OrderBy(i => i.start).GroupBy(i => i.start).FirstOrDefault();
            if (leftMostDuplicatedItems == null) return 0.0;

            foreach (var leftMostItem in leftMostDuplicatedItems)
            {
                var siblings = leftMostItem.parentTrack.GetItems();
                var rightMostSiblings = siblings.OrderByDescending(i => i.end).GroupBy(i => i.end).FirstOrDefault();
                if (rightMostSiblings == null) continue;

                foreach (var sibling in rightMostSiblings)
                    time = Math.Max(time, sibling.end + gapBetweenItems(leftMostItem, sibling));
            }

            return time;
        }
    }

    [MenuEntry("Delete", MenuPriority.TimelineActionSection.delete)]
    [Shortcut("Main Menu/Edit/Delete", EventCommandNames.Delete)]
    [ShortcutPlatformOverride(RuntimePlatform.OSXEditor, KeyCode.Backspace, ShortcutModifiers.Action)]
    [ActiveInMode(TimelineModes.Default)]
    class DeleteAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context)
        {
            return CanDelete(context) ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        static bool CanDelete(ActionContext context)
        {
            if (TimelineWindow.instance.state.editSequence.isReadOnly)
                return false;

            if (context.tracks.ContainsTimelineMarkerTrack(context.timeline))
                return false;

            // All() returns true when empty
            return context.tracks.All(x => !x.lockedInHierarchy) &&
                context.clips.All(x => x.GetParentTrack() == null || !x.GetParentTrack().lockedInHierarchy) &&
                context.markers.All(x => x.parent == null || !x.parent.lockedInHierarchy);
        }

        public override bool Execute(ActionContext context)
        {
            if (!CanDelete(context))
                return false;

            var selectedItems = context.clips.Select(p => p.ToItem()).ToList();
            selectedItems.AddRange(context.markers.Select(p => p.ToItem()));
            DeleteItems(selectedItems);

            if (context.tracks.Any() && SelectionManager.GetCurrentInlineEditorCurve() == null)
                context.tracks.Invoke<DeleteTracks>();

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            return selectedItems.Any() || context.tracks.Any();
        }

        internal static void DeleteItems(IEnumerable<ITimelineItem> items)
        {
            var tracks = items.GroupBy(c => c.parentTrack);

            foreach (var track in tracks)
                TimelineUndo.PushUndo(track.Key, L10n.Tr("Delete Items"));

            TimelineAnimationUtilities.UnlinkAnimationWindowFromClips(items.OfType<ClipItem>().Select(i => i.clip));

            EditMode.PrepareItemsDelete(ItemsUtils.ToItemsPerTrack(items));
            EditModeUtils.Delete(items);

            SelectionManager.RemoveAllClips();
        }
    }

    [MenuEntry("Match Content", MenuPriority.TimelineActionSection.matchContent)]
    [Shortcut(Shortcuts.Timeline.matchContent)]
    class MatchContent : TimelineAction
    {
        public override ActionValidity Validate(ActionContext actionContext)
        {
            var clips = actionContext.clips;

            if (!clips.Any())
                return ActionValidity.NotApplicable;

            return clips.Any(TimelineHelpers.HasUsableAssetDuration)
                ? ActionValidity.Valid
                : ActionValidity.Invalid;
        }

        public override bool Execute(ActionContext actionContext)
        {
            var clips = actionContext.clips;
            return clips.Any() && ClipModifier.MatchContent(clips);
        }
    }

    [Shortcut(Shortcuts.Timeline.play)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class PlayTimelineAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            var currentState = TimelineEditor.state.playing;
            TimelineEditor.state.SetPlaying(!currentState);
            return true;
        }
    }

    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class SelectAllAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            // otherwise select all tracks.
            SelectionManager.Clear();
            TimelineWindow.instance.allTracks.ForEach(x => SelectionManager.Add(x.track));
            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.previousFrame)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class PreviousFrameAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            if (TimelineEditor.inspectedAsset == null)
                return false;
            var inspectedFrame = TimeUtility.ToFrames(TimelineEditor.inspectedSequenceTime, TimelineEditor.inspectedAsset.editorSettings.frameRate);
            inspectedFrame = Mathf.Max(0, inspectedFrame - 1);
            TimelineEditor.inspectedSequenceTime = TimeUtility.FromFrames(inspectedFrame, TimelineEditor.inspectedAsset.editorSettings.frameRate);
            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.nextFrame)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class NextFrameAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            if (TimelineEditor.inspectedAsset == null)
                return false;
            var inspectedFrame = TimeUtility.ToFrames(TimelineEditor.inspectedSequenceTime, TimelineEditor.inspectedAsset.editorSettings.frameRate);
            inspectedFrame++;
            TimelineEditor.inspectedSequenceTime = TimeUtility.FromFrames(inspectedFrame, TimelineEditor.inspectedAsset.editorSettings.frameRate);
            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.frameAll)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class FrameAllAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context)
        {
            if (context.timeline != null && !context.timeline.flattenedTracks.Any())
                return ActionValidity.NotApplicable;

            return ActionValidity.Valid;
        }

        public override bool Execute(ActionContext actionContext)
        {
            var inlineCurveEditor = SelectionManager.GetCurrentInlineEditorCurve();
            if (FrameSelectedAction.ShouldHandleInlineCurve(inlineCurveEditor))
            {
                FrameSelectedAction.FrameInlineCurves(inlineCurveEditor, false);
                return true;
            }

            if (TimelineWindow.instance.state.IsCurrentEditingASequencerTextField())
                return false;

            var visibleTracks = TimelineWindow.instance.treeView.visibleTracks.ToList();

            if (TimelineEditor.inspectedAsset != null && TimelineEditor.inspectedAsset.markerTrack != null)
                visibleTracks.Add(TimelineEditor.inspectedAsset.markerTrack);

            if (visibleTracks.Count == 0)
                return false;

            var startTime = float.MaxValue;
            var endTime = float.MinValue;

            foreach (var t in visibleTracks)
            {
                if (t == null)
                    continue;

                // time range based on track's curves and clips.
                double trackStart, trackEnd, trackDuration;
                t.GetSequenceTime(out trackStart, out trackDuration);
                trackEnd = trackStart + trackDuration;

                // take track's markers into account
                double itemsStart, itemsEnd;
                ItemsUtils.GetItemRange(t, out itemsStart, out itemsEnd);

                startTime = Mathf.Min(startTime, (float)trackStart, (float)itemsStart);
                endTime = Mathf.Max(endTime, (float)(trackEnd), (float)itemsEnd);
            }

            FrameSelectedAction.FrameRange(startTime, endTime);
            return true;
        }
    }

    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class FrameSelectedAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public static void FrameRange(float startTime, float endTime)
        {
            if (startTime > endTime)
            {
                return;
            }

            var halfDuration = endTime - Math.Max(0.0f, startTime);

            if (halfDuration > 0.0f)
            {
                TimelineEditor.visibleTimeRange = new Vector2(Mathf.Max(0.0f, startTime - (halfDuration * 0.1f)), endTime + (halfDuration * 0.1f));
            }
            else
            {
                // start == end
                // keep the zoom level constant, only pan the time area to center the item
                var currentRange = TimelineEditor.visibleTimeRange.y - TimelineEditor.visibleTimeRange.x;
                TimelineEditor.visibleTimeRange = new Vector2(startTime - currentRange / 2, startTime + currentRange / 2);
            }

            TimelineZoomManipulator.InvalidateWheelZoom();
            TimelineEditor.Refresh(RefreshReason.SceneNeedsUpdate);
        }

        public override bool Execute(ActionContext actionContext)
        {
            var inlineCurveEditor = SelectionManager.GetCurrentInlineEditorCurve();
            if (ShouldHandleInlineCurve(inlineCurveEditor))
            {
                FrameInlineCurves(inlineCurveEditor, true);
                return true;
            }

            if (TimelineWindow.instance.state.IsCurrentEditingASequencerTextField())
                return false;

            if (SelectionManager.Count() == 0)
            {
                actionContext.Invoke<FrameAllAction>();
                return true;
            }

            var startTime = float.MaxValue;
            var endTime = float.MinValue;

            var clips = actionContext.clips.Select(ItemToItemGui.GetGuiForClip);
            var markers = actionContext.markers;
            if (!clips.Any() && !markers.Any())
                return false;

            foreach (var c in clips)
            {
                startTime = Mathf.Min(startTime, (float)c.clip.start);
                endTime = Mathf.Max(endTime, (float)c.clip.end);
                if (c.clipCurveEditor != null)
                {
                    c.clipCurveEditor.FrameClip();
                }
            }

            foreach (var marker in markers)
            {
                startTime = Mathf.Min(startTime, (float)marker.time);
                endTime = Mathf.Max(endTime, (float)marker.time);
            }

            FrameRange(startTime, endTime);

            return true;
        }

        public static bool ShouldHandleInlineCurve(IClipCurveEditorOwner curveEditorOwner)
        {
            return curveEditorOwner?.clipCurveEditor != null &&
                curveEditorOwner.inlineCurvesSelected &&
                curveEditorOwner.owner != null &&
                curveEditorOwner.owner.GetShowInlineCurves();
        }

        public static void FrameInlineCurves(IClipCurveEditorOwner curveEditorOwner, bool selectionOnly)
        {
            var curveEditor = curveEditorOwner.clipCurveEditor.curveEditor;
            var frameBounds = selectionOnly ? curveEditor.GetSelectionBounds() : curveEditor.GetClipBounds();

            var clipGUI = curveEditorOwner as TimelineClipGUI;
            var areaOffset = 0.0f;

            if (clipGUI != null)
            {
                areaOffset = (float)Math.Max(0.0, clipGUI.clip.FromLocalTimeUnbound(0.0));

                var timeScale = (float)clipGUI.clip.timeScale;  // Note: The getter for clip.timeScale is guaranteed to never be zero.

                // Apply scaling
                var newMin = frameBounds.min.x / timeScale;
                var newMax = (frameBounds.max.x - frameBounds.min.x) / timeScale + newMin;

                frameBounds.SetMinMax(
                    new Vector3(newMin, frameBounds.min.y, frameBounds.min.z),
                    new Vector3(newMax, frameBounds.max.y, frameBounds.max.z));
            }

            curveEditor.Frame(frameBounds, true, true);

            var area = curveEditor.shownAreaInsideMargins;
            area.x += areaOffset;

            var curveStart = curveEditorOwner.clipCurveEditor.dataSource.start;
            FrameRange(curveStart + frameBounds.min.x, curveStart + frameBounds.max.x);
        }
    }

    [Shortcut(Shortcuts.Timeline.previousKey)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class PrevKeyAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            if (TimelineEditor.inspectedAsset == null)
                return false;
            var keyTraverser = new Utilities.KeyTraverser(TimelineEditor.inspectedAsset, 0.01f / (float)TimelineEditor.inspectedAsset.editorSettings.frameRate);
            var time = keyTraverser.GetPrevKey((float)TimelineEditor.inspectedSequenceTime, TimelineWindow.instance.state.dirtyStamp);
            if (time != TimelineEditor.inspectedSequenceTime)
            {
                TimelineEditor.inspectedSequenceTime = time;
            }

            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.nextKey)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class NextKeyAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            if (TimelineEditor.inspectedAsset == null)
                return false;
            var keyTraverser = new Utilities.KeyTraverser(TimelineEditor.inspectedAsset, 0.01f / (float)TimelineEditor.inspectedAsset.editorSettings.frameRate);
            var time = keyTraverser.GetNextKey((float)TimelineEditor.inspectedSequenceTime, TimelineWindow.instance.state.dirtyStamp);
            if (time != TimelineEditor.inspectedSequenceTime)
            {
                TimelineEditor.inspectedSequenceTime = time;
            }

            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.goToStart)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class GotoStartAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            TimelineEditor.inspectedSequenceTime = 0.0f;
            TimelineWindow.instance.state.EnsurePlayHeadIsVisible();

            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.goToEnd)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class GotoEndAction : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            TimelineEditor.inspectedSequenceTime = TimelineWindow.instance.state.editSequence.duration;
            TimelineWindow.instance.state.EnsurePlayHeadIsVisible();

            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.zoomIn)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class ZoomIn : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            TimelineZoomManipulator.Instance.DoZoom(1.15f);
            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.zoomOut)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class ZoomOut : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            TimelineZoomManipulator.Instance.DoZoom(0.85f);
            return true;
        }
    }

    [Shortcut(Shortcuts.Timeline.navigateLeft)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class NavigateLeft : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.NavigateLeft(actionContext.tracks);
        }
    }


    [Shortcut(Shortcuts.Timeline.navigateRight)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class NavigateRight : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.NavigateRight(actionContext.tracks);
        }
    }

    [Shortcut(Shortcuts.Timeline.toggleCollapseTrack)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class ToggleCollapseGroup : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.ToggleCollapseGroup(actionContext.tracks);
        }
    }

    [Shortcut(Shortcuts.Timeline.selectLeftItem)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class SelectLeftClip : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            // Switches to track header if no left track exists
            return KeyboardNavigation.SelectLeftItem();
        }
    }

    [Shortcut(Shortcuts.Timeline.selectRightItem)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class SelectRightClip : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.SelectRightItem();
        }
    }

    [Shortcut(Shortcuts.Timeline.selectUpItem)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class SelectUpClip : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.SelectUpItem();
        }
    }

    [Shortcut(Shortcuts.Timeline.selectUpTrack)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class SelectUpTrack : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.SelectUpTrack();
        }
    }

    [Shortcut(Shortcuts.Timeline.selectDownItem)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class SelectDownClip : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.SelectDownItem();
        }
    }

    [Shortcut(Shortcuts.Timeline.selectDownTrack)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class SelectDownTrack : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            if (!KeyboardNavigation.ClipAreaActive() && !KeyboardNavigation.TrackHeadActive())
                return KeyboardNavigation.FocusFirstVisibleItem();
            else
                return KeyboardNavigation.SelectDownTrack();
        }
    }

    [Shortcut(Shortcuts.Timeline.multiSelectLeft)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class MultiselectLeftClip : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.SelectLeftItem(true);
        }
    }

    [Shortcut(Shortcuts.Timeline.multiSelectRight)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class MultiselectRightClip : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.SelectRightItem(true);
        }
    }

    [Shortcut(Shortcuts.Timeline.multiSelectUp)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class MultiselectUpTrack : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.SelectUpTrack(true);
        }
    }

    [Shortcut(Shortcuts.Timeline.multiSelectDown)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class MultiselectDownTrack : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;

        public override bool Execute(ActionContext actionContext)
        {
            return KeyboardNavigation.SelectDownTrack(true);
        }
    }

    [Shortcut(Shortcuts.Timeline.toggleClipTrackArea)]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class ToggleClipTrackArea : TimelineAction
    {
        public override ActionValidity Validate(ActionContext context) => ActionValidity.Valid;


        public override bool Execute(ActionContext actionContext)
        {
            if (KeyboardNavigation.TrackHeadActive())
                return KeyboardNavigation.FocusFirstVisibleItem(actionContext.tracks);

            if (!KeyboardNavigation.ClipAreaActive())
                return KeyboardNavigation.FocusFirstVisibleItem();

            var item = KeyboardNavigation.GetVisibleSelectedItems().LastOrDefault();
            if (item != null)
                SelectionManager.SelectOnly(item.parentTrack);
            return true;
        }
    }

    [MenuEntry("Key All Animated", MenuPriority.TimelineActionSection.keyAllAnimated)]
    [Shortcut(Shortcuts.Timeline.keyAllAnimated)]
    class KeyAllAnimated : TimelineAction
    {
        public override ActionValidity Validate(ActionContext actionContext)
        {
            return CanExecute(TimelineEditor.state, actionContext)
                ? ActionValidity.Valid
                : ActionValidity.NotApplicable;
        }

        public override bool Execute(ActionContext actionContext)
        {
            WindowState state = TimelineEditor.state;
            PlayableDirector director = TimelineEditor.inspectedDirector;

            if (!CanExecute(state, actionContext) || director == null)
                return false;

            IEnumerable<TrackAsset> keyableTracks = GetKeyableTracks(state, actionContext);

            var curveSelected = SelectionManager.GetCurrentInlineEditorCurve();
            if (curveSelected != null)
            {
                var sel = curveSelected.clipCurveEditor.GetSelectedProperties().ToList();
                var go = (director.GetGenericBinding(curveSelected.owner) as Component).gameObject;
                if (sel.Count > 0)
                {
                    TimelineRecording.KeyProperties(go, state, sel);
                }
                else
                {
                    var binding = director.GetGenericBinding(curveSelected.owner) as Component;
                    TimelineRecording.KeyAllProperties(binding, state);
                }
            }
            else
            {
                foreach (var track in keyableTracks)
                {
                    var binding = director.GetGenericBinding(track) as Component;
                    TimelineRecording.KeyAllProperties(binding, state);
                }
            }
            return true;
        }

        static IEnumerable<TrackAsset> GetKeyableTracks(WindowState state, ActionContext context)
        {
            if (!context.clips.Any() && !context.tracks.Any()) //no selection -> animate all recorded tracks
                return context.timeline.flattenedTracks.Where(state.IsArmedForRecord);

            List<TrackAsset> parentTracks = context.tracks.ToList();
            parentTracks.AddRange(context.clips.Select(clip => clip.GetParentTrack()).Distinct());

            if (!parentTracks.All(state.IsArmedForRecord))
                return Enumerable.Empty<TrackAsset>();

            return parentTracks;
        }

        static bool CanExecute(WindowState state, ActionContext context)
        {
            if (context.timeline == null)
                return false;

            if (context.markers.Any())
                return false;

            if (context.tracks.ContainsTimelineMarkerTrack(context.timeline))
                return false;

            IClipCurveEditorOwner curveSelected = SelectionManager.GetCurrentInlineEditorCurve();
            // Can't have an inline curve selected and have multiple tracks also.
            if (curveSelected != null)
            {
                return state.IsArmedForRecord(curveSelected.owner);
            }

            return GetKeyableTracks(state, context).Any();
        }
    }
}
