using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// A context for the Timeline window (RO)
    /// </summary>
    /// <remarks>
    /// The SequenceContext represents a state of the Timeline window, and is used to interact with <see cref="TimelineNavigator"/>.
    /// </remarks>
    public readonly struct SequenceContext : IEquatable<SequenceContext>
    {
        /// <summary>
        /// The director associated with the Timeline window in the context. (RO)
        /// </summary>
        public PlayableDirector director { get; }

        /// <summary>
        /// The <see cref="TimelineClip"/> associated with the Timeline window in the context. (RO)
        /// </summary>
        /// <remarks>In a SubTimeline context, the clip is the <see cref="TimelineClip"/> that hosts the SubTimeline in the parent Timeline.
        /// In the root context, the clip is <see langword="null"/>.</remarks>
        public TimelineClip clip { get; }

        /// <summary>
        /// Initializes and returns an instance of SequenceContext.
        /// </summary>
        /// <param name="director">The PlayableDirector associated with the context. Must be a valid PlayableDirector reference. </param>
        /// <param name="clip">The TimelineClip reference that controls the sequence. Specify <see langword="null"/> to specify that the sequence is the root. If non-null, the clip must be part of a valid <see cref="TimelineAsset"/>.</param>
        /// <exception cref="System.ArgumentNullException"> <paramref name="director"/> is null.</exception>
        /// <exception cref="System.ArgumentException"> The <paramref name="clip"/> is not part of a <see cref="TrackAsset"/>.</exception>
        /// <exception cref="System.ArgumentException"> The <paramref name="clip"/> is part of a track but not part of a <see cref="TimelineAsset"/>.</exception>
        public SequenceContext(PlayableDirector director, TimelineClip clip)
        {
            if (director == null)
                throw new ArgumentNullException(nameof(director));

            var parentTrack = clip?.GetParentTrack();
            if (clip != null && parentTrack == null)
                throw new ArgumentException("The provided clip must be part of a track", nameof(clip));

            if (clip != null && parentTrack.timelineAsset == null)
                throw new ArgumentException("The provided clip must be part of a Timeline.", nameof(clip));

            this.director = director;
            this.clip = clip;
            m_Valid = true;
        }

        /// <summary>
        /// Assesses the validity of a SequenceContext.
        /// </summary>
        /// <remarks>To be valid, a SequenceContext must contain a valid PlayableDirector reference.</remarks>
        /// <returns><see langword="true" />  if the SequenceContext is valid,<see langword="false" /> otherwise</returns>
        public bool IsValid() => m_Valid;

        /// <summary>
        /// Equality operator overload.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns><see langword="true" /> if operands are equal, <see langword="false" /> otherwise.</returns>
        public static bool operator ==(SequenceContext left, SequenceContext right) => left.Equals(right);

        /// <summary>
        /// Inequality operator overload.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns><see langword="true" /> if operands are not equal, <see langword="false" /> otherwise.</returns>
        public static bool operator !=(SequenceContext left, SequenceContext right) => !left.Equals(right);

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(SequenceContext other)
        {
            return Equals(director, other.director) && Equals(clip, other.clip);
        }

        /// <summary>Indicates whether the current object is equal to another object of indeterminate type.</summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="obj" /> parameter; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            return obj is SequenceContext other && Equals(other);
        }

        /// <summary>Hash function for SequenceContext.</summary>
        /// <returns>
        /// Hash code for the SequenceContext.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((director != null ? director.GetHashCode() : 0) * 397) ^ (clip != null ? clip.GetHashCode() : 0);
            }
        }

        internal static SequenceContext Invalid = new SequenceContext();
        readonly bool m_Valid;
    }
}
