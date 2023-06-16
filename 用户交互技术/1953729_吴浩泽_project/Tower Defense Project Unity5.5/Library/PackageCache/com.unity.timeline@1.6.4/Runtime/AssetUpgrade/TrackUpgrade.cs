using System;
using UnityEngine.Serialization;

namespace UnityEngine.Timeline
{
    partial class TrackAsset : ISerializationCallbackReceiver
    {
        internal enum Versions
        {
            Initial = 0,
            RotationAsEuler = 1,
            RootMotionUpgrade = 2,
            AnimatedTrackProperties = 3
        }

        const int k_LatestVersion = (int)Versions.AnimatedTrackProperties;

        [SerializeField, HideInInspector] int m_Version;

        [Obsolete("Please use m_InfiniteClip (on AnimationTrack) instead.", false)]
        [SerializeField, HideInInspector, FormerlySerializedAs("m_animClip")]
        internal AnimationClip m_AnimClip;

        /// <summary>
        /// Called before a track is serialized.
        /// </summary>
        protected virtual void OnBeforeTrackSerialize() { }

        /// <summary>
        /// Called after a track has been deserialized.
        /// </summary>
        protected virtual void OnAfterTrackDeserialize() { }

        internal virtual void OnUpgradeFromVersion(int oldVersion) { }

        /// <summary>
        /// Called before Unity serializes this object.
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_Version = k_LatestVersion;

            //make sure children are correctly parented
            if (m_Children != null)
            {
                for (var i = m_Children.Count - 1; i >= 0; i--)
                {
                    var asset = m_Children[i] as TrackAsset;
                    if (asset != null && asset.parent != this)
                        asset.parent = this;
                }
            }

            OnBeforeTrackSerialize();
        }

        /// <summary>
        /// Called after Unity deserializes this object.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Clear the clip cache when a deserialize is performed, or
            // we can get out of sync when performing Undo
            m_ClipsCache = null;
            Invalidate();

            if (m_Version < k_LatestVersion)
            {
                UpgradeToLatestVersion(); //upgrade TrackAsset
                OnUpgradeFromVersion(m_Version); //upgrade derived classes
            }

            foreach (var marker in GetMarkers())
            {
                marker.Initialize(this);
            }

            OnAfterTrackDeserialize();
        }

        //fields that are used for upgrading should be put here, ideally as read-only
        void UpgradeToLatestVersion()
        { }

        //upgrade code should go into this class
        static class TrackAssetUpgrade
        { }
    }
}
