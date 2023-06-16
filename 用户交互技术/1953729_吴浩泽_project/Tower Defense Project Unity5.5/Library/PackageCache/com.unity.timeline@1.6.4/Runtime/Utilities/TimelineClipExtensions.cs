using System;

namespace UnityEngine.Timeline
{
    /// <summary>
    /// Extension methods for TimelineClip
    /// </summary>
    public static class TimelineClipExtensions
    {
        static readonly string k_UndoSetParentTrackText = "Move Clip";

        /// <summary>
        /// Tries to move a TimelineClip to a different track. Validates that the destination track can accept the clip before performing the move.
        /// Fails if the clip's PlayableAsset is null, the current and destination tracks are the same or the destination track cannot accept the clip.
        /// </summary>
        /// <param name="clip">Clip that is being moved</param>
        /// <param name="destinationTrack">Desired destination track</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="clip"/> or <paramref name="destinationTrack"/> are null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the PlayableAsset in the TimelineClip is null</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the current parent track and destination track are the same</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the destination track cannot hold tracks of the given type</exception>
        public static void MoveToTrack(this TimelineClip clip, TrackAsset destinationTrack)
        {
            if (clip == null)
            {
                throw new ArgumentNullException($"'this' argument for {nameof(MoveToTrack)} cannot be null.");
            }

            if (destinationTrack == null)
            {
                throw new ArgumentNullException("Cannot move TimelineClip to a null track.");
            }

            TrackAsset parentTrack = clip.GetParentTrack();
            Object asset = clip.asset;

            // If the asset is null we cannot validate its type against the destination track
            if (asset == null)
            {
                throw new InvalidOperationException("Cannot move a TimelineClip to a different track if the TimelineClip's PlayableAsset is null.");
            }

            if (parentTrack == destinationTrack)
            {
                throw new InvalidOperationException($"TimelineClip is already on {destinationTrack.name}.");
            }

            if (!destinationTrack.ValidateClipType(asset.GetType()))
            {
                throw new InvalidOperationException($"Track {destinationTrack.name} cannot contain clips of type {clip.GetType().Name}.");
            }

            MoveToTrack_Impl(clip, destinationTrack, asset, parentTrack);
        }

        /// <summary>
        /// Tries to move a TimelineClip to a different track. Validates that the destination track can accept the clip before performing the move.
        /// Fails if the clip's PlayableAsset is null, the current and destination tracks are the same or the destination track cannot accept the clip.
        /// </summary>
        /// <param name="clip">Clip that is being moved</param>
        /// <param name="destinationTrack">Desired destination track</param>
        /// <returns>Returns true if the clip was successfully moved to the destination track, false otherwise. Also returns false if either argument is null</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="clip"/> or <paramref name="destinationTrack"/> are null</exception>
        public static bool TryMoveToTrack(this TimelineClip clip, TrackAsset destinationTrack)
        {
            if (clip == null)
            {
                throw new ArgumentNullException($"'this' argument for {nameof(TryMoveToTrack)} cannot be null.");
            }

            if (destinationTrack == null)
            {
                throw new ArgumentNullException("Cannot move TimelineClip to a null parent.");
            }

            TrackAsset parentTrack = clip.GetParentTrack();
            Object asset = clip.asset;

            // If the asset is null we cannot validate its type against the destination track
            if (asset == null)
            {
                return false;
            }

            if (parentTrack != destinationTrack)
            {
                if (!destinationTrack.ValidateClipType(asset.GetType()))
                {
                    return false;
                }

                MoveToTrack_Impl(clip, destinationTrack, asset, parentTrack);

                return true;
            }

            return false;
        }

        static void MoveToTrack_Impl(TimelineClip clip, TrackAsset destinationTrack, Object asset, TrackAsset parentTrack)
        {
            TimelineUndo.PushUndo(asset, k_UndoSetParentTrackText);
            if (parentTrack != null)
            {
                TimelineUndo.PushUndo(parentTrack, k_UndoSetParentTrackText);
            }

            TimelineUndo.PushUndo(destinationTrack, k_UndoSetParentTrackText);

            clip.SetParentTrack_Internal(destinationTrack);

            if (parentTrack == null)
            {
                TimelineCreateUtilities.SaveAssetIntoObject(asset, destinationTrack);
            }
            else if (parentTrack.timelineAsset != destinationTrack.timelineAsset)
            {
                TimelineCreateUtilities.RemoveAssetFromObject(asset, parentTrack);
                TimelineCreateUtilities.SaveAssetIntoObject(asset, destinationTrack);
            }
        }
    }
}
