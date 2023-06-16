using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace UnityEditor.Timeline.Actions
{
    /// <summary>
    /// Use this attribute to make an action work with the shortcut system.
    /// </summary>
    /// <example>
    /// TimelineShortcutAttribute needs to be added to a static method.
    /// <code source="../../DocCodeExamples/TimelineAttributesExamples.cs" region="declare-timelineShortcutAttr" title="TimelineShortcutAttr"/>
    /// </example>
    public class TimelineShortcutAttribute : ShortcutManagement.ShortcutAttribute
    {
        /// <summary>
        /// TimelineShortcutAttribute Constructor
        /// </summary>
        /// <param name="id">Id to register the shortcut. It will automatically be prefix by 'Timeline/' in order to be in the 'Timeline' section of the shortcut manager.</param>
        /// <param name="defaultKeyCode">Optional key code for default binding.</param>
        /// <param name="defaultShortcutModifiers">Optional shortcut modifiers for default binding.</param>
        public TimelineShortcutAttribute(string id, KeyCode defaultKeyCode, ShortcutModifiers defaultShortcutModifiers = ShortcutModifiers.None)
            : base("Timeline/" + id, typeof(TimelineWindow), defaultKeyCode, defaultShortcutModifiers) { }
    }
}
