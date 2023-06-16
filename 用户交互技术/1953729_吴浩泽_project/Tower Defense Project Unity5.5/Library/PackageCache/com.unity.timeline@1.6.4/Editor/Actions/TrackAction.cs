using System.Collections.Generic;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Base class for a track action.
    /// Inherit from this class to make an action that would react on selected tracks after a menu click and/or a key shortcut.
    /// </summary>
    /// <example>
    /// Simple track Action example (with context menu and shortcut support).
    /// <code source="../../DocCodeExamples/ActionExamples.cs" region="declare-sampleTrackAction" title="SampleTrackAction"/>
    /// </example>
    /// <remarks>
    /// To add an action as a menu item in the Timeline context menu, add <see cref="MenuEntryAttribute"/> on the action class.
    /// To make an action to react to a shortcut, use the Shortcut Manager API with <see cref="TimelineShortcutAttribute"/>.
    /// <seealso cref="UnityEditor.ShortcutManagement.ShortcutAttribute"/>
    /// </remarks>
    [ActiveInMode(TimelineModes.Default)]
    public abstract class TrackAction : IAction
    {
        /// <summary>
        ///  Execute the action.
        /// </summary>
        /// <param name="tracks">Tracks that will be used for the action. </param>
        /// <returns>true if the action has been executed. false otherwise</returns>
        public abstract bool Execute(IEnumerable<TrackAsset> tracks);

        /// <summary>
        ///  Defines the validity of an Action for a given set of tracks.
        /// </summary>
        ///  <param name="tracks">tracks that the action would act on.</param>
        /// <returns>The validity of the set of tracks.</returns>
        public abstract ActionValidity Validate(IEnumerable<TrackAsset> tracks);
    }
}
