using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    static class KeyboardNavigation
    {
        public static void FrameTrackHeader(TreeViewItem treeItem = null)
        {
            if (TrackHeadActive())
                treeItem = treeItem ?? SelectionManager.SelectedTrackGUI().Last();
            else
            {
                var item = GetVisibleSelectedItems().LastOrDefault();
                treeItem = TimelineWindow.instance.allTracks.FirstOrDefault(
                    x => item != null && x.track == item.parentTrack);
            }

            if (treeItem != null)
                TimelineWindow.instance.treeView.FrameItem(treeItem);
        }

        public static bool TrackHeadActive()
        {
            return SelectionManager.SelectedTracks().Any(x => x.IsVisibleInHierarchy()) && !ClipAreaActive();
        }

        public static bool ClipAreaActive()
        {
            return GetVisibleSelectedItems().Any();
        }

        public static IEnumerable<ITimelineItem> GetVisibleSelectedItems()
        {
            return SelectionManager.SelectedItems().Where(x => x.parentTrack.IsVisibleInHierarchy());
        }

        public static IEnumerable<TimelineTrackBaseGUI> GetVisibleTracks()
        {
            return TimelineWindow.instance.allTracks.Where(x => x.track.IsVisibleInHierarchy());
        }

        static TrackAsset PreviousTrack(this TrackAsset track)
        {
            var uiOrderTracks = GetVisibleTracks().Select(t => t.track).ToList();
            var selIdx = uiOrderTracks.IndexOf(track);
            return selIdx > 0 ? uiOrderTracks[selIdx - 1] : null;
        }

        static TrackAsset NextTrack(this TrackAsset track)
        {
            var uiOrderTracks = GetVisibleTracks().Select(t => t.track).ToList();
            var selIdx = uiOrderTracks.IndexOf(track);
            return selIdx < uiOrderTracks.Count - 1 && selIdx != -1 ? uiOrderTracks[selIdx + 1] : null;
        }

        static ITimelineItem PreviousItem(this ITimelineItem item, bool clipOnly)
        {
            var items = item.parentTrack.GetItems().ToArray();
            if (clipOnly)
            {
                items = items.Where(x => x is ClipItem).ToArray();
            }
            else
            {
                items = items.Where(x => x is MarkerItem).ToArray();
            }

            var idx = Array.IndexOf(items, item);
            return idx > 0 ? items[idx - 1] : null;
        }

        static ITimelineItem NextItem(this ITimelineItem item, bool clipOnly)
        {
            var items = item.parentTrack.GetItems().ToArray();
            if (clipOnly)
            {
                items = items.Where(x => x is ClipItem).ToArray();
            }
            else
            {
                items = items.Where(x => x is MarkerItem).ToArray();
            }

            var idx = Array.IndexOf(items, item);
            return idx < items.Length - 1 ? items[idx + 1] : null;
        }

        static bool FilterItems(ref List<ITimelineItem> items)
        {
            var clipOnly = false;
            if (items.Any(x => x is ClipItem))
            {
                items = items.Where(x => x is ClipItem).ToList();
                clipOnly = true;
            }

            return clipOnly;
        }

        static ITimelineItem GetClosestItem(TrackAsset track, ITimelineItem refItem)
        {
            var start = refItem.start;
            var end = refItem.end;
            var items = track.GetItems().ToList();

            if (refItem is ClipItem)
            {
                items = items.Where(x => x is ClipItem).ToList();
            }
            else
            {
                items = items.Where(x => x is MarkerItem).ToList();
            }

            if (!items.Any())
                return null;
            ITimelineItem ret = null;
            var scoreToBeat = double.NegativeInfinity;

            foreach (var item in items)
            {
                // test for overlap
                var low = Math.Max(item.start, start);
                var high = Math.Min(item.end, end);
                if (low <= high)
                {
                    var score = high - low;
                    if (score >= scoreToBeat)
                    {
                        scoreToBeat = score;
                        ret = item;
                    }
                }
            }

            return ret;
        }

        public static bool FocusFirstVisibleItem(IEnumerable<TrackAsset> focusTracks = null)
        {
            var timeRange = TimelineEditor.visibleTimeRange;

            var tracks = focusTracks ?? TimelineWindow.instance.treeView.visibleTracks.Where(x => x.IsVisibleInHierarchy() && x.GetItems().Any());
            var items = tracks.SelectMany(t => t.GetItems().OfType<ClipItem>().Where(x => x.end >= timeRange.x && x.end <= timeRange.y ||
                x.start >= timeRange.x && x.start <= timeRange.y)).ToList();
            var itemFullyInView = items.Where(x => x.end >= timeRange.x && x.end <= timeRange.y &&
                x.start >= timeRange.x && x.start <= timeRange.y);
            var itemToSelect = itemFullyInView.FirstOrDefault() ?? items.FirstOrDefault();
            if (itemToSelect != null && !itemToSelect.parentTrack.lockedInHierarchy)
            {
                SelectionManager.SelectOnly(itemToSelect);
                return true;
            }
            return false;
        }

        public static bool NavigateLeft(IEnumerable<TrackAsset> tracks)
        {
            if (!TrackHeadActive())
                return false;

            if (TryCollapse(tracks))
                return true;

            return SelectAndShowParentTrack(tracks.LastOrDefault());
        }

        /// <summary>
        /// Tries to collapse any track from a list of tracks
        /// </summary>
        /// <param name="tracks"></param>
        /// <returns>true if any were collapsed, false otherwise</returns>
        public static bool TryCollapse(IEnumerable<TrackAsset> tracks)
        {
            var didCollapse = false;

            foreach (TrackAsset track in tracks)
            {
                if (!track.GetChildTracks().Any())
                    continue;

                if (!track.IsCollapsed())
                {
                    didCollapse = true;
                    track.SetCollapsed(true);
                }
            }

            if (didCollapse)
            {
                TimelineEditor.window.treeView.Reload();
                return true;
            }

            return false;
        }

        public static bool ToggleCollapseGroup(IEnumerable<TrackAsset> tracks)
        {
            if (!TrackHeadActive())
                return false;

            var didChange = false;

            foreach (TrackAsset track in tracks)
            {
                if (!track.GetChildTracks().Any())
                    continue;

                track.SetCollapsed(!track.IsCollapsed());
                didChange = true;
            }

            if (didChange)
                TimelineEditor.window.treeView.Reload();

            return didChange;
        }

        static bool SelectAndShowParentTrack(TrackAsset track)
        {
            TrackAsset parent = track != null ? track.parent as TrackAsset : null;
            if (parent)
            {
                SelectionManager.SelectOnly(parent);
                FrameTrackHeader(GetVisibleTracks().FirstOrDefault(x => x.track == parent));
                return true;
            }

            return false;
        }

        public static bool SelectLeftItem(bool shift = false)
        {
            if (ClipAreaActive())
            {
                var items = SelectionManager.SelectedItems().ToList();
                var clipOnly = FilterItems(ref items);

                var item = items.Last();
                var prev = item.PreviousItem(clipOnly);
                if (prev != null && !prev.parentTrack.lockedInHierarchy)
                {
                    if (shift)
                    {
                        if (SelectionManager.Contains(prev))
                            SelectionManager.Remove(item);
                        SelectionManager.Add(prev);
                    }
                    else
                        SelectionManager.SelectOnly(prev);
                    TimelineHelpers.FrameItems(new[] { prev });
                }
                else if (item != null && !shift && item.parentTrack != TimelineEditor.inspectedAsset.markerTrack)
                    SelectionManager.SelectOnly(item.parentTrack);
                return true;
            }
            return false;
        }

        public static bool SelectRightItem(bool shift = false)
        {
            if (ClipAreaActive())
            {
                var items = SelectionManager.SelectedItems().ToList();
                var clipOnly = FilterItems(ref items);

                var item = items.Last();
                var next = item.NextItem(clipOnly);
                if (next != null && !next.parentTrack.lockedInHierarchy)
                {
                    if (shift)
                    {
                        if (SelectionManager.Contains(next))
                            SelectionManager.Remove(item);
                        SelectionManager.Add(next);
                    }
                    else
                        SelectionManager.SelectOnly(next);
                    TimelineHelpers.FrameItems(new[] { next });
                    return true;
                }
            }
            return false;
        }

        public static bool NavigateRight(IEnumerable<TrackAsset> tracks)
        {
            if (!TrackHeadActive())
                return false;

            if (TryExpand(tracks))
                return true;

            return TrySelectFirstChild(tracks);
        }

        /// <summary>
        /// Tries to expand from a list of tracks
        /// </summary>
        /// <param name="tracks"></param>
        /// <returns>true if any expanded, false otherwise</returns>
        public static bool TryExpand(IEnumerable<TrackAsset> tracks)
        {
            var didExpand = false;
            foreach (TrackAsset track in tracks)
            {
                if (!track.GetChildTracks().Any())
                    continue;

                if (track.IsCollapsed())
                {
                    didExpand = true;
                    track.SetCollapsed(false);
                }
            }

            if (didExpand)
            {
                TimelineEditor.window.treeView.Reload();
            }

            return didExpand;
        }

        /// <summary>
        /// Tries to select the first clip from a list of tracks
        /// </summary>
        /// <param name="tracks"></param>
        /// <returns>true if any expanded, false otherwise</returns>
        public static bool TrySelectFirstChild(IEnumerable<TrackAsset> tracks)
        {
            foreach (var track in tracks)
            {
                //Try to navigate in group tracks first
                if (track is GroupTrack)
                {
                    if (track.GetChildTracks().Any())
                    {
                        SelectionManager.SelectOnly(track.GetChildTracks().First());
                        return true;
                    }
                    //Group tracks should not halt navigation
                    continue;
                }
                //if track is locked or has no clips, do nothing
                if (track.lockedInHierarchy || !track.clips.Any())
                    continue;

                var firstClip = track.clips.First();
                SelectionManager.SelectOnly(firstClip);
                TimelineHelpers.FrameItems(new ITimelineItem[] { firstClip.ToItem() });

                return true;
            }

            return false;
        }

        public static bool SelectUpTrack(bool shift = false)
        {
            if (TrackHeadActive())
            {
                var prevTrack = PreviousTrack(SelectionManager.SelectedTracks().Last());
                if (prevTrack != null)
                {
                    if (shift)
                    {
                        if (SelectionManager.Contains(prevTrack))
                            SelectionManager.Remove(SelectionManager.SelectedTracks().Last());
                        SelectionManager.Add(prevTrack);
                    }
                    else
                        SelectionManager.SelectOnly(prevTrack);
                    FrameTrackHeader(GetVisibleTracks().First(x => x.track == prevTrack));
                }
                return true;
            }
            return false;
        }

        public static bool SelectUpItem()
        {
            if (ClipAreaActive())
            {
                var refItem = SelectionManager.SelectedItems().Last();
                var prevTrack = refItem.parentTrack.PreviousTrack();
                while (prevTrack != null)
                {
                    var selectionItem = GetClosestItem(prevTrack, refItem);
                    if (selectionItem == null || prevTrack.lockedInHierarchy)
                    {
                        prevTrack = prevTrack.PreviousTrack();
                        continue;
                    }

                    SelectionManager.SelectOnly(selectionItem);
                    TimelineHelpers.FrameItems(new[] { selectionItem });
                    FrameTrackHeader(GetVisibleTracks().First(x => x.track == selectionItem.parentTrack));
                    break;
                }
                return true;
            }

            return false;
        }

        public static bool SelectDownTrack(bool shift = false)
        {
            if (TrackHeadActive())
            {
                var nextTrack = SelectionManager.SelectedTracks().Last().NextTrack();
                if (nextTrack != null)
                {
                    if (shift)
                    {
                        if (SelectionManager.Contains(nextTrack))
                            SelectionManager.Remove(SelectionManager.SelectedTracks().Last());
                        SelectionManager.Add(nextTrack);
                    }
                    else
                        SelectionManager.SelectOnly(nextTrack);

                    FrameTrackHeader(GetVisibleTracks().First(x => x.track == nextTrack));
                }
                return true;
            }

            return false;
        }

        public static bool SelectDownItem()
        {
            if (ClipAreaActive())
            {
                var refItem = SelectionManager.SelectedItems().Last();
                var nextTrack = refItem.parentTrack.NextTrack();
                while (nextTrack != null)
                {
                    var selectionItem = GetClosestItem(nextTrack, refItem);
                    if (selectionItem == null || nextTrack.lockedInHierarchy)
                    {
                        nextTrack = nextTrack.NextTrack();
                        continue;
                    }

                    SelectionManager.SelectOnly(selectionItem);
                    TimelineHelpers.FrameItems(new[] { selectionItem });
                    FrameTrackHeader(GetVisibleTracks().First(x => x.track == selectionItem.parentTrack));
                    break;
                }
                return true;
            }
            return false;
        }
    }
}
