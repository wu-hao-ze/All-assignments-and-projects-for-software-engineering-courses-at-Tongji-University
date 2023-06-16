using System.Collections.Generic;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Base class for a clip action.
    /// Inherit from this class to make an action that would react on selected clips after a menu click and/or a key shortcut.
    /// </summary>
    /// <example>
    /// Simple Clip Action example (with context menu and shortcut support).
    /// <code source="../../DocCodeExamples/ActionExamples.cs" region="declare-sampleClipAction" title="SampleClipAction"/>
    /// </example>
    /// <remarks>
    /// To add an action as a menu item in the Timeline context menu, add <see cref="MenuEntryAttribute"/> on the action class.
    /// To make an action to react to a shortcut, use the Shortcut Manager API with <see cref="TimelineShortcutAttribute"/>.
    /// <seealso cref="UnityEditor.ShortcutManagement.ShortcutAttribute"/>
    /// </remarks>
    [ActiveInMode(TimelineModes.Default)]
    public abstract class ClipAction : IAction
    {
        /// <summary>
        /// Execute the action based on clips.
        /// </summary>
        /// <param name="clips">clips that the action will act on.</param>
        /// <returns>Returns true if the action has been correctly executed, false otherwise.</returns>
        public abstract bool Execute(IEnumerable<TimelineClip> clips);

        /// <summary>
        ///  Defines the validity of an Action for a given set of clips.
        /// </summary>
        ///  <param name="clips">The clips that the action will act on.</param>
        /// <returns>The validity of the set of clips.</returns>
        public abstract ActionValidity Validate(IEnumerable<TimelineClip> clips);
    }
}
