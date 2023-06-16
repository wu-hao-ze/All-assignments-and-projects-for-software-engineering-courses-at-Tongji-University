using System;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Use this attribute on action classes (<see cref="TimelineAction"/>,
    /// <see cref="ClipAction"/>,
    /// <see cref="MarkerAction"/> and
    /// <see cref="TrackAction"/>)
    /// to have the default undo behaviour applied.
    ///
    /// By default, applying this attribute will record all objects passed to the Execute method with the Undo system,
    /// using the name of Action it is applied to.
    ///
    /// <example>
    /// Simple track Action example (with context menu and shortcut support).
    /// <code source="../../DocCodeExamples/TimelineAttributesExamples.cs" region="declare-applyDefaultUndoAttr" title="ApplyDefaultUndoAttr"/>
    /// </example>
    /// </summary>
    /// <seealso cref="TimelineAction"/>
    /// <seealso cref="TrackAction"/>
    /// <seealso cref="ClipAction"/>
    /// <seealso cref="MarkerAction"/>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ApplyDefaultUndoAttribute : Attribute
    {
        /// <summary>
        /// The title of the action to appear in the undo history. If not specified, the name is taken from the DisplayName attribute,
        /// or derived from the name of the class this attribute is applied to.
        /// </summary>
        public string UndoTitle;

        /// <summary>Use this attribute on action classes to have the default undo behaviour applied.
        /// </summary>
        /// <param name="undoTitle">The title of the action to appear in the undo history.</param>
        public ApplyDefaultUndoAttribute(string undoTitle = null)
        {
            UndoTitle = undoTitle;
        }
    }
}
