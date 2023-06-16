namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Base class for a timeline action.
    /// Inherit from this class to make an action on a timeline after a menu click and/or a key shortcut.
    /// </summary>
    /// <remarks>
    /// To add an action as a menu item in the Timeline context menu, add <see cref="MenuEntryAttribute"/> on the action class.
    /// To make an action to react to a shortcut, use the Shortcut Manager API with <see cref="TimelineShortcutAttribute"/>.
    /// <seealso cref="UnityEditor.ShortcutManagement.ShortcutAttribute"/>
    /// <seealso cref="ActiveInModeAttribute"/>
    /// </remarks>
    /// <example>
    /// Simple Timeline Action example (with context menu and shortcut support).
    /// <code source="../../DocCodeExamples/ActionExamples.cs" region="declare-sampleTimelineAction" title="SampleTimelineAction"/>
    /// </example>
    [ActiveInMode(TimelineModes.Default)]
    public abstract class TimelineAction : IAction
    {
        /// <summary>
        ///  Execute the action.
        /// </summary>
        /// <param name="context">Context for the action.</param>
        /// <returns>true if the action has been executed. false otherwise</returns>
        public abstract bool Execute(ActionContext context);

        /// <summary>
        /// Defines the validity of an Action based on the context.
        /// </summary>
        /// <param name="context">Context for the action.</param>
        /// <returns>Visual state of the menu for the action.</returns>
        public abstract ActionValidity Validate(ActionContext context);
    }
}
