using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Base class of the TimelineWindow.
    /// </summary>
    public abstract class TimelineEditorWindow : EditorWindow
    {
        /// <summary>
        /// Interface used to navigate between Timelines and SubTimelines. (RO)
        /// </summary>
        public abstract TimelineNavigator navigator { get; }
        /// <summary>
        /// Allows retrieving and and setting the Timeline Window lock state. When the lock is off, the window focus follows the Unity selection.
        /// </summary>
        /// <remarks>When lock transitions from true to false, the focused timeline will be synchronized with the Unity selection.</remarks>>
        public abstract bool locked { get; set; }
        /// <summary>
        /// Allows setting which TimelineAsset is shown in the TimelineWindow.
        /// </summary>
        /// <param name="sequence">The asset to show.</param>
        /// <remarks>Calling this method will put the window in asset edit mode and certain features might be missing (eg: timeline cannot be evaluated, bindings will not be available, etc).
        /// Ignores window lock mode. Calling with null, will clear the displayed timeline.</remarks>
        public abstract void SetTimeline(TimelineAsset sequence);
        /// <summary>
        /// Allows setting which TimelineAsset is shown in the TimelineWindow and which PlayableDirector is used to evaluate it.
        /// </summary>
        /// <param name="director">The PlayableDirector who's timeline should be shown.</param>
        /// <remarks>Ignores window lock mode. Calling with null, will clear the displayed timeline.</remarks>
        public abstract void SetTimeline(PlayableDirector director);
        /// <summary>
        /// Allows clearing the TimelineAsset that is shown in the TimelineWindow.
        /// </summary>
        /// <remarks>Ignores window lock mode.</remarks>>
        public abstract void ClearTimeline();
    }
}
