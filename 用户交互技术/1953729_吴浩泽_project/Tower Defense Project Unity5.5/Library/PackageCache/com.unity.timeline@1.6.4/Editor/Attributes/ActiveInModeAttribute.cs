using System;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Define the activeness of an action depending on its timeline mode.
    /// </summary>
    /// <seealso cref="TimelineModes"/>
    [AttributeUsage(AttributeTargets.Class)]
    public class ActiveInModeAttribute : Attribute
    {
        /// <summary>
        /// Modes that will be used for activeness of an action.
        /// </summary>
        public TimelineModes modes { get; }

        /// <summary>
        /// Defines in which mode the action will be active.
        /// </summary>
        /// <param name="timelineModes">Modes that will define activeness of the action.</param>
        public ActiveInModeAttribute(TimelineModes timelineModes)
        {
            modes = timelineModes;
        }
    }
}
