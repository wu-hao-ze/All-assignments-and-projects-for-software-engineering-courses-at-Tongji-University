using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace UnityEditor.Timeline
{
    [MenuEntry("Edit in Animation Window", MenuPriority.ClipEditActionSection.editInAnimationWindow), UsedImplicitly]
    class EditClipInAnimationWindow : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            if (!GetEditableClip(clips, out _, out _))
                return ActionValidity.NotApplicable;
            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            TimelineClip clip;
            AnimationClip clipToEdit;
            if (!GetEditableClip(clips, out clip, out clipToEdit))
                return false;

            GameObject gameObject = null;
            if (TimelineEditor.inspectedDirector != null)
                gameObject = TimelineUtility.GetSceneGameObject(TimelineEditor.inspectedDirector, clip.GetParentTrack());

            var timeController = TimelineAnimationUtilities.CreateTimeController(clip);
            TimelineAnimationUtilities.EditAnimationClipWithTimeController(
                clipToEdit, timeController, clip.animationClip != null ? gameObject : null);

            return true;
        }

        private static bool GetEditableClip(IEnumerable<TimelineClip> clips, out TimelineClip clip, out AnimationClip animClip)
        {
            clip = null;
            animClip = null;

            if (clips.Count() != 1)
                return false;

            clip = clips.FirstOrDefault();
            if (clip == null)
                return false;

            if (clip.animationClip != null)
                animClip = clip.animationClip;
            else if (clip.curves != null && !clip.curves.empty)
                animClip = clip.curves;

            return animClip != null;
        }
    }

    [MenuEntry("Edit Sub-Timeline", MenuPriority.ClipEditActionSection.editSubTimeline), UsedImplicitly]
    class EditSubTimeline : ClipAction
    {
        private static readonly string MultiItemPrefix = "Edit Sub-Timelines/";
        private static readonly string SingleItemPrefix = "Edit ";

        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            if (clips == null || clips.Count() != 1 || TimelineEditor.inspectedDirector == null)
                return ActionValidity.NotApplicable;

            var clip = clips.First();
            var directors = TimelineUtility.GetSubTimelines(clip, TimelineEditor.inspectedDirector);
            return directors.Any(x => x != null) ? ActionValidity.Valid : ActionValidity.NotApplicable;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            if (Validate(clips) != ActionValidity.Valid) return false;

            var clip = clips.First();

            var directors = TimelineUtility.GetSubTimelines(clip, TimelineEditor.inspectedDirector);
            ExecuteInternal(directors, 0, clip);

            return true;
        }

        static void ExecuteInternal(IList<PlayableDirector> directors, int directorIndex, TimelineClip clip)
        {
            SelectionManager.Clear();
            TimelineWindow.instance.SetCurrentTimeline(directors[directorIndex], clip);
        }

        internal void AddMenuItem(List<MenuActionItem> menuItems)
        {
            var clips = TimelineEditor.selectedClips;
            if (clips == null || clips.Length != 1)
                return;

            var mode = TimelineWindow.instance.currentMode.mode;
            MenuEntryAttribute menuAttribute = GetType().GetCustomAttributes(typeof(MenuEntryAttribute), false).OfType<MenuEntryAttribute>().FirstOrDefault();
            var menuItem = new MenuActionItem()
            {
                category = menuAttribute.subMenuPath ?? string.Empty,
                entryName = menuAttribute.name,
                isActiveInMode = this.IsActionActiveInMode(mode),
                priority = menuAttribute.priority,
                state = Validate(clips),
                callback = null
            };

            var subDirectors = TimelineUtility.GetSubTimelines(clips[0], TimelineEditor.inspectedDirector);
            if (subDirectors.Count == 1)
            {
                menuItem.entryName = SingleItemPrefix + DisplayNameHelper.GetDisplayName(subDirectors[0]);
                menuItem.callback = () =>
                {
                    Execute(clips);
                };
                menuItems.Add(menuItem);
            }
            else
            {
                for (int i = 0; i < subDirectors.Count; i++)
                {
                    var index = i;
                    menuItem.category = MultiItemPrefix;
                    menuItem.entryName = DisplayNameHelper.GetDisplayName(subDirectors[i]);
                    menuItem.callback = () =>
                    {
                        ExecuteInternal(subDirectors, index, clips[0]);
                    };
                    menuItems.Add(menuItem);
                }
            }
        }
    }

    [MenuEntry("Editing/Trim Start", MenuPriority.ClipActionSection.trimStart)]
    [Shortcut(Shortcuts.Clip.trimStart), UsedImplicitly]
    class TrimStart : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            return clips.All(x => TimelineEditor.inspectedSequenceTime <= x.start || TimelineEditor.inspectedSequenceTime >= x.start + x.duration) ? ActionValidity.Invalid : ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.TrimStart(clips, TimelineEditor.inspectedSequenceTime);
        }
    }

    [MenuEntry("Editing/Trim End", MenuPriority.ClipActionSection.trimEnd), UsedImplicitly]
    [Shortcut(Shortcuts.Clip.trimEnd)]
    class TrimEnd : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            return clips.All(x => TimelineEditor.inspectedSequenceTime <= x.start || TimelineEditor.inspectedSequenceTime >= x.start + x.duration) ? ActionValidity.Invalid : ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.TrimEnd(clips, TimelineEditor.inspectedSequenceTime);
        }
    }

    [Shortcut(Shortcuts.Clip.split)]
    [MenuEntry("Editing/Split", MenuPriority.ClipActionSection.split), UsedImplicitly]
    class Split : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            return clips.All(x => TimelineEditor.inspectedSequenceTime <= x.start || TimelineEditor.inspectedSequenceTime >= x.start + x.duration) ? ActionValidity.Invalid : ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            bool success = ClipModifier.Split(clips, TimelineEditor.inspectedSequenceTime, TimelineEditor.inspectedDirector);
            if (success)
                TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            return success;
        }
    }

    [MenuEntry("Editing/Complete Last Loop", MenuPriority.ClipActionSection.completeLastLoop), UsedImplicitly]
    class CompleteLastLoop : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            bool canDisplay = clips.Any(TimelineHelpers.HasUsableAssetDuration);
            return canDisplay ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.CompleteLastLoop(clips);
        }
    }

    [MenuEntry("Editing/Trim Last Loop", MenuPriority.ClipActionSection.trimLastLoop), UsedImplicitly]
    class TrimLastLoop : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            bool canDisplay = clips.Any(TimelineHelpers.HasUsableAssetDuration);
            return canDisplay ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.TrimLastLoop(clips);
        }
    }

    [MenuEntry("Editing/Match Duration", MenuPriority.ClipActionSection.matchDuration), UsedImplicitly]
    class MatchDuration : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            return clips.Count() > 1 ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.MatchDuration(clips);
        }
    }

    [MenuEntry("Editing/Double Speed", MenuPriority.ClipActionSection.doubleSpeed), UsedImplicitly]
    class DoubleSpeed : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            bool canDisplay = clips.All(x => x.SupportsSpeedMultiplier());

            return canDisplay ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.DoubleSpeed(clips);
        }
    }

    [MenuEntry("Editing/Half Speed", MenuPriority.ClipActionSection.halfSpeed), UsedImplicitly]
    class HalfSpeed : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            bool canDisplay = clips.All(x => x.SupportsSpeedMultiplier());

            return canDisplay ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.HalfSpeed(clips);
        }
    }

    [MenuEntry("Editing/Reset Duration", MenuPriority.ClipActionSection.resetDuration), UsedImplicitly]
    class ResetDuration : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            bool canDisplay = clips.Any(TimelineHelpers.HasUsableAssetDuration);
            return canDisplay ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.ResetEditing(clips);
        }
    }

    [MenuEntry("Editing/Reset Speed", MenuPriority.ClipActionSection.resetSpeed), UsedImplicitly]
    class ResetSpeed : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            bool canDisplay = clips.All(x => x.SupportsSpeedMultiplier());

            return canDisplay ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.ResetSpeed(clips);
        }
    }

    [MenuEntry("Editing/Reset All", MenuPriority.ClipActionSection.resetAll), UsedImplicitly]
    class ResetAll : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            bool canDisplay = clips.Any(TimelineHelpers.HasUsableAssetDuration) || clips.All(x => x.SupportsSpeedMultiplier());

            return canDisplay ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            var speedResult = ClipModifier.ResetSpeed(clips);
            var editResult = ClipModifier.ResetEditing(clips);
            return speedResult || editResult;
        }
    }

    [MenuEntry("Tile", MenuPriority.ClipActionSection.tile), UsedImplicitly]
    class Tile : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            return clips.Count() > 1 ? ActionValidity.Valid : ActionValidity.Invalid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            return ClipModifier.Tile(clips);
        }
    }

    [MenuEntry("Find Source Asset", MenuPriority.ClipActionSection.findSourceAsset), UsedImplicitly]
    [ActiveInMode(TimelineModes.Default | TimelineModes.ReadOnly)]
    class FindSourceAsset : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips)
        {
            if (clips.Count() > 1)
                return ActionValidity.Invalid;

            if (GetUnderlyingAsset(clips.First()) == null)
                return ActionValidity.Invalid;

            return ActionValidity.Valid;
        }

        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            EditorGUIUtility.PingObject(GetUnderlyingAsset(clips.First()));
            return true;
        }

        private static UnityEngine.Object GetExternalPlayableAsset(TimelineClip clip)
        {
            if (clip.asset == null)
                return null;

            if ((clip.asset.hideFlags & HideFlags.HideInHierarchy) != 0)
                return null;

            return clip.asset;
        }

        private static UnityEngine.Object GetUnderlyingAsset(TimelineClip clip)
        {
            var asset = clip.asset as ScriptableObject;
            if (asset == null)
                return null;

            var fields = ObjectReferenceField.FindObjectReferences(asset.GetType());
            if (fields.Length == 0)
                return GetExternalPlayableAsset(clip);

            // Find the first non-null field
            foreach (var field in fields)
            {
                // skip scene refs in asset mode
                if (TimelineEditor.inspectedDirector == null && field.isSceneReference)
                    continue;
                var obj = field.Find(asset, TimelineEditor.inspectedDirector);
                if (obj != null)
                    return obj;
            }

            return GetExternalPlayableAsset(clip);
        }
    }

    class CopyClipsToClipboard : ClipAction
    {
        public override ActionValidity Validate(IEnumerable<TimelineClip> clips) => ActionValidity.Valid;
        public override bool Execute(IEnumerable<TimelineClip> clips)
        {
            TimelineEditor.clipboard.CopyItems(clips.ToItems());
            return true;
        }
    }
}
