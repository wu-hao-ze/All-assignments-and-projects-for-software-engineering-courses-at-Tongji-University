using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DocCodeExamples
{
    class TimelineAttributesExamples_HideAPI
    {
        #region declare-sampleTrackBindingAttr

        [TrackBindingType(typeof(Light), TrackBindingFlags.AllowCreateComponent)]
        public class LightTrack : TrackAsset { }

        #endregion

        #region declare-menuEntryAttribute

        [MenuEntry("Simple Menu Action")]
        class SimpleMenuAction : TimelineAction
        {
            public override ActionValidity Validate(ActionContext actionContext)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(ActionContext actionContext)
            {
                return true;
            }
        }

        [MenuEntry("Menu Action with priority", 9999)]
        class MenuActionWithPriority : TimelineAction
        {
            public override ActionValidity Validate(ActionContext actionContext)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(ActionContext actionContext)
            {
                return true;
            }
        }

        [MenuEntry("My Menu/Menu Action inside submenu")]
        class MenuActionInsideSubMenu : TimelineAction
        {
            public override ActionValidity Validate(ActionContext actionContext)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(ActionContext actionContext)
            {
                return true;
            }
        }

        #endregion

        #region declare-timelineShortcutAttr

        public class ShortcutAction : TimelineAction
        {
            public override ActionValidity Validate(ActionContext _)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(ActionContext _)
            {
                Debug.Log("Action executed.");
                return true;
            }

            [TimelineShortcut("Test Action", KeyCode.K, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
            public static void HandleShortCut(ShortcutArguments args)
            {
                Invoker.InvokeWithSelected<ShortcutAction>();
            }
        }

        #endregion

        #region declare-applyDefaultUndoAttr

        [ApplyDefaultUndo]
        public class SetNameToTypeAction : TrackAction
        {
            public override ActionValidity Validate(IEnumerable<TrackAsset> items)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(IEnumerable<TrackAsset> items)
            {
                foreach (TrackAsset track in items)
                    track.name = track.GetType().Name;
                return true;
            }
        }

        #endregion

        #region declare-customStyleMarkerAttr

        [CustomStyle("MyStyle")]
        public class MyMarker : UnityEngine.Timeline.Marker { }

        #endregion

        #region declare-customTimelineEditorAttr

        [CustomTimelineEditor(typeof(MyCustomClip))]
        class MyCustomClipEditor : ClipEditor { }

        #endregion

        class MyCustomClip : PlayableAsset
        {
            public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
            {
                return Playable.Null;
            }
        }
    }
}
