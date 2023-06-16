using System;
using UnityEngine;

namespace UnityEditor.Timeline
{
    static class AnimationClipExtensions
    {
        public static UInt64 ClipVersion(this AnimationClip clip)
        {
            if (clip == null)
                return 0;

            var info = AnimationClipCurveCache.Instance.GetCurveInfo(clip);
            var version = (UInt32)info.version;
            var count = (UInt32)info.curves.Length;
            var result = (UInt64)version;
            result |= ((UInt64)count) << 32;
            return result;
        }

        public static CurveChangeType GetChangeType(this AnimationClip clip, ref UInt64 curveVersion)
        {
            var version = clip.ClipVersion();
            var changeType = CurveChangeType.None;
            if ((curveVersion >> 32) != (version >> 32))
                changeType = CurveChangeType.CurveAddedOrRemoved;
            else if (curveVersion != version)
                changeType = CurveChangeType.CurveModified;

            curveVersion = version;
            return changeType;
        }
    }
}
