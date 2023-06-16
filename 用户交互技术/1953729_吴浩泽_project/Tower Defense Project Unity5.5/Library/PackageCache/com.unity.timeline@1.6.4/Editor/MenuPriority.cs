using System;
using UnityEngine;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Priorities for menu item ordering. See <see cref="MenuEntryAttribute"/>.
    /// </summary>
    public static class MenuPriority
    {
        /// <summary>
        ///  Default priority for a menu. It will add at the end of the context menu before the 'add' menus.
        /// </summary>
        public const int defaultPriority = 9000;
        /// <summary>
        ///  This value is the separator difference that will be needed to create a separator between menu item.
        /// </summary>
        public const int separatorAt = 1000;

        /// <summary>
        ///  Priorities for Timeline Action menu items.
        /// </summary>
        public static class TimelineActionSection
        {
            /// <summary>
            ///  First Timeline action menu item priority.
            /// </summary>
            public const int start = 1000;
            /// <summary>
            /// Copy menu item priority.
            /// </summary>
            public const int copy = start + 100;
            /// <summary>
            /// Paste menu item priority.
            /// </summary>
            public const int paste = start + 200;
            /// <summary>
            /// Duplicate menu item priority.
            /// </summary>
            public const int duplicate = start + 300;
            /// <summary>
            /// Delete menu item priority.
            /// </summary>
            public const int delete = start + 400;

            /// <summary>
            /// Keyframe All animated item priority.
            /// </summary>
            public const int keyAllAnimated = start + 450;

            /// <summary>
            /// Match Content menu item priority.
            /// </summary>
            public const int matchContent = start + 500;
        }

        /// <summary>
        ///  Priorities for Track action menu items.
        /// </summary>
        public static class TrackActionSection
        {
            /// <summary>
            ///  First Track action menu item priority.
            /// </summary>
            public const int start = TimelineActionSection.start + separatorAt;
            /// <summary>
            /// Lock track menu item priority.
            /// </summary>
            public const int lockTrack = start + 100;
            /// <summary>
            /// Lock selected track menu item priority.
            /// </summary>
            public const int lockSelected = start + 150;
            /// <summary>
            /// Mute track menu item priority.
            /// </summary>
            public const int mute = start + 200;
            /// <summary>
            /// Mute selected track menu item priority.
            /// </summary>
            public const int muteSelected = start + 250;
            /// <summary>
            /// Show hide marker menu item priority.
            /// </summary>
            public const int showHideMarkers = start + 300;
            /// <summary>
            /// Remove Invalid Markers menu item priority.
            /// </summary>
            public const int removeInvalidMarkers = start + 400;
            /// <summary>
            /// Edit Track In Animation Window menu item priority.
            /// </summary>
            public const int editInAnimationWindow = start + 800;
        }

        /// <summary>
        ///  Priorities for Add Tracks menu items.
        /// </summary>
        public static class AddTrackMenu
        {
            /// <summary>
            ///  First Add Track menu item priority.
            /// </summary>
            public const int start = TrackActionSection.start + separatorAt;
            /// <summary>
            ///  Add Layer Track menu item priority.
            /// </summary>
            public const int addLayerTrack = start;
        }

        /// <summary>
        ///  Priorities for Clip edition menu items.
        /// </summary>
        public static class ClipEditActionSection
        {
            /// <summary>
            /// First Edit Clip menu item priority.
            /// </summary>
            public const int start = AddTrackMenu.start + separatorAt;
            /// <summary>
            /// Edit Clip In Animation Window menu item priority.
            /// </summary>
            public const int editInAnimationWindow = start + 100;
            /// <summary>
            /// Edit Clip Sub Timeline menu item priority.
            /// </summary>
            public const int editSubTimeline = start + 200;
        }

        /// <summary>
        ///  Priorities for Clip action menu items.
        /// </summary>
        public static class ClipActionSection
        {
            /// <summary>
            ///  First Clip action menu item priority.
            /// </summary>
            public const int start = ClipEditActionSection.start + separatorAt;
            /// <summary>
            /// Trim start menu item priority.
            /// </summary>
            public const int trimStart = start + 100;
            /// <summary>
            /// Trim end menu item priority.
            /// </summary>
            public const int trimEnd = start + 110;
            /// <summary>
            /// Split menu item priority.
            /// </summary>
            public const int split = start + 120;
            /// <summary>
            /// Complete Last Loop menu item priority.
            /// </summary>
            public const int completeLastLoop = start + separatorAt;
            /// <summary>
            /// Trim Last Loop menu item priority.
            /// </summary>
            public const int trimLastLoop = start + separatorAt + 110;
            /// <summary>
            /// Match duration menu item priority.
            /// </summary>
            public const int matchDuration = start + separatorAt + 120;
            /// <summary>
            /// Double Speed menu item priority.
            /// </summary>
            public const int doubleSpeed = start + 2 * separatorAt;
            /// <summary>
            /// Half Speed menu item priority.
            /// </summary>
            public const int halfSpeed = start + 2 * separatorAt + 110;
            /// <summary>
            /// Reset Duration menu item priority.
            /// </summary>
            public const int resetDuration = start + 3 * separatorAt;
            /// <summary>
            /// Reset Speed menu item priority.
            /// </summary>
            public const int resetSpeed = start + 3 * separatorAt + 110;
            /// <summary>
            /// Reset All menu item priority.
            /// </summary>
            public const int resetAll = start + 3 * separatorAt + 120;
            /// <summary>
            /// Tile menu item priority.
            /// </summary>
            public const int tile = start + 300;
            /// <summary>
            /// Find source asset menu item priority.
            /// </summary>
            public const int findSourceAsset = start + 400;
        }

        /// <summary>
        ///  Priorities for Marker action menu items.
        /// </summary>
        public static class MarkerActionSection
        {
            /// <summary>
            ///  First Marker action menu item priority.
            /// </summary>
            public const int start = ClipActionSection.start + separatorAt;
        }

        /// <summary>
        ///  Priorities for custom Timeline action menu items.
        /// </summary>
        public static class CustomTimelineActionSection
        {
            /// <summary>
            ///  First custom Timeline action menu item priority.
            /// </summary>
            public const int start = MarkerActionSection.start + separatorAt;
        }

        /// <summary>
        ///  Priorities for Custom Track action menu items.
        /// </summary>
        public static class CustomTrackActionSection
        {
            /// <summary>
            ///  First custom track action menu item priority.
            /// </summary>
            public const int start = CustomTimelineActionSection.start + separatorAt;
            /// <summary>
            /// Convert Animation to clip menu item priority.
            /// </summary>
            public const int convertToClipMode = start + 100;
            /// <summary>
            /// Convert Clip to animation menu item priority.
            /// </summary>
            public const int convertFromClipMode = start + 200;
            /// <summary>
            /// Apply Track offset menu item priority.
            /// </summary>
            public const int applyTrackOffset = start + 300;
            /// <summary>
            /// Apply Scene offset menu item priority.
            /// </summary>
            public const int applySceneOffset = start + 310;
            /// <summary>
            /// Apply Auto offset menu item priority.
            /// </summary>
            public const int applyAutoOffset = start + 320;
            /// <summary>
            /// Add override track menu item priority.
            /// </summary>
            public const int addOverrideTrack = start + 500;
            /// <summary>
            ///  User custom track action menu item priority.
            /// </summary>
            public const int customTrackAction = start + 900;
        }

        /// <summary>
        /// Custom clip action menu item priority.
        /// </summary>
        public static class CustomClipActionSection
        {
            /// <summary>
            ///  First custom clip action menu item priority.
            /// </summary>
            public const int start = CustomTrackActionSection.start + separatorAt;
            /// <summary>
            /// Match previous menu item priority.
            /// </summary>
            public const int matchPrevious = start + 100;
            /// <summary>
            /// Match next menu item priority.
            /// </summary>
            public const int matchNext = start + 110;
            /// <summary>
            /// Reset offset menu item priority.
            /// </summary>
            public const int resetOffset = start + 120;
            /// <summary>
            ///  User custom clip action menu item priority.
            /// </summary>
            public const int customClipAction = start + 900;
        }

        /// <summary>
        /// Priorities for menu entries to create Timeline items.
        /// </summary>
        public static class AddItem
        {
            /// <summary>
            /// Add group menu item priority.
            /// </summary>
            public const int addGroup = defaultPriority + separatorAt;
            /// <summary>
            /// Add track menu item priority.
            /// </summary>
            public const int addTrack = addGroup + separatorAt;
            /// <summary>
            /// Add custom track menu item priority.
            /// </summary>
            public const int addCustomTrack = addTrack + separatorAt;
            /// <summary>
            /// Add clip menu item priority.
            /// </summary>
            public const int addClip = addCustomTrack + separatorAt;
            /// <summary>
            /// Add custom clip menu item priority.
            /// </summary>
            public const int addCustomClip = addClip + separatorAt;
            /// <summary>
            /// Add marker menu item priority.
            /// </summary>
            public const int addMarker = addCustomClip + separatorAt;
            /// <summary>
            /// Add custom marker menu item priority.
            /// </summary>
            public const int addCustomMarker = addMarker + separatorAt;
        }
    }
}
