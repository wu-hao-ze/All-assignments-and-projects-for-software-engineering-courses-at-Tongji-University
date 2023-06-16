using System;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

namespace DocCodeExamples
{
    class ActionExamples_HideAPI
    {
        #region declare-sampleClipAction

        [MenuEntry("Custom Actions/Sample clip Action")]
        public class SampleClipAction : ClipAction
        {
            public override ActionValidity Validate(IEnumerable<TimelineClip> clip)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(IEnumerable<TimelineClip> items)
            {
                Debug.Log("Test Action");
                return true;
            }

            [TimelineShortcut("SampleClipAction", KeyCode.K)]
            public static void HandleShortCut(ShortcutArguments args)
            {
                Invoker.InvokeWithSelectedClips<SampleClipAction>();
            }
        }

        #endregion

        #region declare-sampleMarkerAction

        [MenuEntry("Custom Actions/Sample marker Action")]
        public class SampleMarkerAction : MarkerAction
        {
            public override ActionValidity Validate(IEnumerable<IMarker> markers)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(IEnumerable<IMarker> items)
            {
                Debug.Log("Test Action");
                return true;
            }

            [TimelineShortcut("SampleMarkerAction", KeyCode.L)]
            public static void HandleShortCut(ShortcutArguments args)
            {
                Invoker.InvokeWithSelectedMarkers<SampleMarkerAction>();
            }
        }

        #endregion

        #region declare-sampleTrackAction

        [MenuEntry("Custom Actions/Sample track Action")]
        public class SampleTrackAction : TrackAction
        {
            public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(IEnumerable<TrackAsset> tracks)
            {
                Debug.Log("Test Action");
                return true;
            }

            [TimelineShortcut("SampleTrackAction", KeyCode.H)]
            public static void HandleShortCut(ShortcutArguments args)
            {
                Invoker.InvokeWithSelectedTracks<SampleTrackAction>();
            }
        }

        #endregion

        #region declare-sampleTimelineAction

        [MenuEntry("Custom Actions/Sample Timeline Action")]
        public class SampleTimelineAction : TimelineAction
        {
            public override ActionValidity Validate(ActionContext context)
            {
                return ActionValidity.Valid;
            }

            public override bool Execute(ActionContext context)
            {
                Debug.Log("Test Action");
                return true;
            }

            [TimelineShortcut("SampleTimelineAction", KeyCode.Q)]
            public static void HandleShortCut(ShortcutArguments args)
            {
                Invoker.InvokeWithSelected<SampleTimelineAction>();
            }
        }

        #endregion
    }
}
