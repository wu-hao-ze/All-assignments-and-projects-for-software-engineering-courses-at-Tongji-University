using System;
using UnityEngine;

namespace UnityEditor.Timeline
{
    enum TimelineModeGUIState
    {
        Disabled,
        Hidden,
        Enabled
    }

    abstract class TimelineMode
    {
        public struct HeaderState
        {
            public TimelineModeGUIState breadCrumb;
            public TimelineModeGUIState sequenceSelector;
            public TimelineModeGUIState options;
        }

        public struct TrackOptionsState
        {
            public TimelineModeGUIState newButton;
            public TimelineModeGUIState editAsAssetButton;
        }

        public HeaderState headerState { get; protected set; }
        public TrackOptionsState trackOptionsState { get; protected set; }
        public TimelineModes mode { get; protected set; }

        public abstract bool ShouldShowPlayRange(WindowState state);
        public abstract bool ShouldShowTimeCursor(WindowState state);

        public virtual bool ShouldShowTrackBindings(WindowState state)
        {
            return ShouldShowTimeCursor(state);
        }

        public virtual bool ShouldShowTimeArea(WindowState state)
        {
            return !state.IsEditingAnEmptyTimeline();
        }

        public abstract TimelineModeGUIState TrackState(WindowState state);
        public abstract TimelineModeGUIState ToolbarState(WindowState state);

        public virtual TimelineModeGUIState PreviewState(WindowState state)
        {
            return state.ignorePreview ? TimelineModeGUIState.Disabled : TimelineModeGUIState.Enabled;
        }

        public virtual TimelineModeGUIState EditModeButtonsState(WindowState state)
        {
            return TimelineModeGUIState.Enabled;
        }
    }

    /// <summary>
    /// Different mode for Timeline
    /// </summary>
    [Flags]
    public enum TimelineModes
    {
        /// <summary>
        /// A playable director with a valid timeline is selected in editor.
        /// </summary>
        Active = 1,
        /// <summary>
        /// The timeline is not editable. (the TimelineAsset file is either readonly on disk or locked by source control).
        /// </summary>
        ReadOnly = 2,
        /// <summary>
        /// The timeline cannot be played or previewed.
        /// </summary>
        Inactive = 4,
        /// <summary>
        /// Disabled Timeline.
        /// </summary>
        Disabled = 8,
        /// <summary>
        /// Timeline in AssetEditing mode.
        /// This mode is enabled when a timeline asset is selected in the project window.
        /// </summary>
        AssetEdition = 16,
        /// <summary>
        /// The timeline can be edited (either through playable director or selected timeline asset in project window).
        /// </summary>
        Default = Active | AssetEdition
    }
}
