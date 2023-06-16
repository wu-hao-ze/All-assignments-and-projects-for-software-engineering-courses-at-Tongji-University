using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// The flags that indicate the view status of a marker.
    /// </summary>
    [System.Flags]
    public enum MarkerUIStates
    {
        /// <summary>
        /// No extra state specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The marker is selected.
        /// </summary>
        Selected = 1 << 0,

        /// <summary>
        /// The marker is in a collapsed state.
        /// </summary>
        Collapsed = 1 << 1
    }

    /// <summary>
    /// The user-defined options for drawing a marker.
    /// </summary>
    public struct MarkerDrawOptions
    {
        /// <summary>
        /// The tooltip for the marker.
        /// </summary>
        public string tooltip { get; set; }

        /// <summary>
        /// Text that indicates if the marker should display an error.
        /// </summary>
        /// <remarks>
        /// If the error text is not empty or null, then the marker displays a warning. The error text is used as the tooltip.
        /// </remarks>
        public string errorText { get; set; }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>Returns <c>true</c> if <paramref name="obj"/> and this instance are the same type and represent the same value.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is MarkerDrawOptions))
                return false;

            return Equals((MarkerDrawOptions)obj);
        }

        /// <summary>
        /// Compares this object with another <c>MarkerDrawOptions</c>.
        /// </summary>
        /// <param name="other">The object to compare with.</param>
        /// <returns>Returns true if <c>this</c> and <paramref name="other"/> are equal.</returns>
        public bool Equals(MarkerDrawOptions other)
        {
            return errorText == other.errorText &&
                tooltip == other.tooltip;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashUtility.CombineHash(
                errorText != null ? errorText.GetHashCode() : 0,
                tooltip != null ? tooltip.GetHashCode() : 0
            );
        }

        /// <summary>
        /// Compares two <c>MarkerDrawOptions</c> objects.
        /// </summary>
        /// <param name="options1">The first object.</param>
        /// <param name="options2">The second object.</param>
        /// <returns>Returns true if they are equal.</returns>
        public static bool operator ==(MarkerDrawOptions options1, MarkerDrawOptions options2)
        {
            return options1.Equals(options2);
        }

        /// <summary>
        /// Compares two <c>MarkerDrawOptions</c> objects.
        /// </summary>
        /// <param name="options1">The first object.</param>
        /// <param name="options2">The second object.</param>
        /// <returns>Returns true if they are not equal.</returns>
        public static bool operator !=(MarkerDrawOptions options1, MarkerDrawOptions options2)
        {
            return !options1.Equals(options2);
        }
    }


    /// <summary>
    /// The description of the on-screen area where the marker is drawn.
    /// </summary>
    public struct MarkerOverlayRegion
    {
        /// <summary>
        /// The area where the marker is being drawn.
        /// </summary>
        public Rect markerRegion { get; private set; }

        /// <summary>
        /// The area where the overlay is being drawn.
        ///
        /// This region extends from the top of the time ruler to the bottom of the window, excluding any scrollbars.
        /// </summary>
        public Rect timelineRegion { get; private set; }

        /// <summary>
        /// The sub-area of the timelineRegion where the tracks are drawn.
        ///
        /// The region extends from the bottom of the time ruler, or the timeline marker region if not hidden.
        /// Use this region to clip overlays that should not be drawn over the timeline marker region or time ruler.
        /// </summary>
        /// <example>
        /// <code source="../../DocCodeExamples/MarkerEditorExamples.cs" region="declare-trackRegion" title="trackRegion"/>
        /// </example>
        public Rect trackRegion => Rect.MinMaxRect(timelineRegion.xMin, timelineRegion.yMin + m_TrackOffset, timelineRegion.xMax, timelineRegion.yMax);

        /// <summary>
        /// The start time of the visible region of the window.
        /// </summary>
        public double startTime { get; private set; }

        /// <summary>
        /// The end time of the visible region of the window.
        /// </summary>
        public double endTime { get; private set; }

        private float m_TrackOffset;

        /// <summary>Constructor</summary>
        /// <param name="_markerRegion">The area where the marker is being drawn.</param>
        /// <param name="_timelineRegion">The area where the overlay is being drawn.</param>
        /// <param name="_startTime">The start time of the visible region of the window.</param>
        /// <param name="_endTime">The end time of the visible region of the window.</param>
        public MarkerOverlayRegion(Rect _markerRegion, Rect _timelineRegion, double _startTime, double _endTime)
            : this(_markerRegion, _timelineRegion, _startTime, _endTime, 19.0f)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="_markerRegion">The area where the marker is being drawn.</param>
        /// <param name="_timelineRegion">The area where the overlay is being drawn.</param>
        /// <param name="_startTime">The start time of the visible region of the window.</param>
        /// <param name="_endTime">The end time of the visible region of the window.</param>
        /// <param name="_trackOffset">The offset from the timelineRegion to the trackRegion</param>
        public MarkerOverlayRegion(Rect _markerRegion, Rect _timelineRegion, double _startTime, double _endTime, float _trackOffset)
        {
            markerRegion = _markerRegion;
            timelineRegion = _timelineRegion;
            startTime = _startTime;
            endTime = _endTime;
            m_TrackOffset = _trackOffset;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>Returns <c>true</c> if <paramref name="obj"/> and this instance are the same type and represent the same value.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is MarkerOverlayRegion))
                return false;

            return Equals((MarkerOverlayRegion)obj);
        }

        /// <summary>
        /// Compares this object with another <c>MarkerOverlayRegion</c>.
        /// </summary>
        /// <param name="other">The object to compare with.</param>
        /// <returns>Returns true if <c>this</c> and <paramref name="other"/> are equal.</returns>
        public bool Equals(MarkerOverlayRegion other)
        {
            return markerRegion == other.markerRegion &&
                timelineRegion == other.timelineRegion &&
                startTime == other.startTime &&
                endTime == other.endTime &&
                m_TrackOffset == other.m_TrackOffset;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashUtility.CombineHash(
                markerRegion.GetHashCode(),
                timelineRegion.GetHashCode(),
                startTime.GetHashCode(),
                endTime.GetHashCode(),
                m_TrackOffset.GetHashCode()
            );
        }

        /// <summary>
        /// Compares two <c>MarkerOverlayRegion</c> objects.
        /// </summary>
        /// <param name="region1">The first object.</param>
        /// <param name="region2">The second object.</param>
        /// <returns>Returns true if they are equal.</returns>
        public static bool operator ==(MarkerOverlayRegion region1, MarkerOverlayRegion region2)
        {
            return region1.Equals(region2);
        }

        /// <summary>
        /// Compares two <c>MarkerOverlayRegion</c> objects.
        /// </summary>
        /// <param name="region1">The first object.</param>
        /// <param name="region2">The second object.</param>
        /// <returns>Returns true if they are not equal.</returns>
        public static bool operator !=(MarkerOverlayRegion region1, MarkerOverlayRegion region2)
        {
            return !region1.Equals(region2);
        }
    }

    /// <summary>
    /// Use this class to customize marker types in the TimelineEditor.
    /// </summary>
    public class MarkerEditor
    {
        internal readonly bool supportsDrawOverlay;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MarkerEditor()
        {
            supportsDrawOverlay = TypeUtility.HasOverrideMethod(GetType(), nameof(DrawOverlay));
        }

        /// <summary>
        /// Implement this method to override the default options for drawing a marker.
        /// </summary>
        /// <param name="marker">The marker to draw.</param>
        /// <returns></returns>
        public virtual MarkerDrawOptions GetMarkerOptions(IMarker marker)
        {
            return new MarkerDrawOptions()
            {
                tooltip = string.Empty,
                errorText = string.Empty,
            };
        }

        /// <summary>
        /// Called when a marker is created.
        /// </summary>
        /// <param name="marker">The marker that is created.</param>
        /// <param name="clonedFrom">TThe source that the marker was copied from. This can be set to null if the marker is not a copy.</param>
        /// <remarks>
        /// The callback occurs before the marker is assigned to the track.
        /// </remarks>
        public virtual void OnCreate(IMarker marker, IMarker clonedFrom)
        {
        }

        /// <summary>
        /// Draws additional overlays for a marker.
        /// </summary>
        /// <param name="marker">The marker to draw.</param>
        /// <param name="uiState">The visual state of the marker.</param>
        /// <param name="region">The on-screen area where the marker is being drawn.</param>
        /// <remarks>
        /// Notes:
        /// * It is only called during TimelineWindow's Repaint step.
        /// * If there are multiple markers on top of each other, only the topmost marker receives the DrawOverlay call.
        /// </remarks>
        public virtual void DrawOverlay(IMarker marker, MarkerUIStates uiState, MarkerOverlayRegion region)
        {
        }
    }
}
