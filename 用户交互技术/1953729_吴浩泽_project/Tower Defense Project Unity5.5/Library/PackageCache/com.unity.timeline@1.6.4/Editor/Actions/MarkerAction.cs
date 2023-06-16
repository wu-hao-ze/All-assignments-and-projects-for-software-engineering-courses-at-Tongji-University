using System.Collections.Generic;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Base class for a marker action.
    /// Inherit from this class to make an action that would react on selected markers after a menu click and/or a key shortcut.
    /// </summary>
    /// <example>
    /// Simple track Action example (with context menu and shortcut support).
    /// <code source="../../DocCodeExamples/ActionExamples.cs" region="declare-sampleMarkerAction" title="SampleMarkerAction"/>
    /// </example>
    /// <remarks>
    /// To add an action as a menu item in the Timeline context menu, add <see cref="MenuEntryAttribute"/> on the action class.
    /// To make an action to react to a shortcut, use the Shortcut Manager API with <see cref="TimelineShortcutAttribute"/>.
    /// <seealso cref="UnityEditor.ShortcutManagement.ShortcutAttribute"/>
    /// </remarks>
    [ActiveInMode(TimelineModes.Default)]
    public abstract class MarkerAction : IAction
    {
        /// <summary>
        ///  Execute the action.
        /// </summary>
        /// <param name="markers">Markers that will be used for the action. </param>
        /// <returns>true if the action has been executed. false otherwise</returns>
        public abstract bool Execute(IEnumerable<IMarker> markers);
        /// <summary>
        ///  Defines the validity of an Action for a given set of markers.
        /// </summary>
        /// <param name="markers">Markers that will be used for the action. </param>
        /// <returns>The validity of the set of markers.</returns>
        public abstract ActionValidity Validate(IEnumerable<IMarker> markers);
    }
}
