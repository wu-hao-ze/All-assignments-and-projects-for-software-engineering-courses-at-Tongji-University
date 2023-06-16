using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    static class AnimationOffsetMenu
    {
        public static string MatchFieldsPrefix = L10n.Tr("Match Offsets Fields/");

        static bool EnforcePreviewMode()
        {
            TimelineEditor.state.previewMode = true; // try and set the preview mode
            if (!TimelineEditor.state.previewMode)
            {
                Debug.LogError("Match clips cannot be completed because preview mode cannot be enabed");
                return false;
            }
            return true;
        }

        internal static void MatchClipsToPrevious(TimelineClip[] clips)
        {
            if (!EnforcePreviewMode())
                return;

            clips = clips.OrderBy(x => x.start).ToArray();
            foreach (var clip in clips)
            {
                var sceneObject = TimelineUtility.GetSceneGameObject(TimelineEditor.inspectedDirector, clip.GetParentTrack());
                if (sceneObject != null)
                {
                    TimelineAnimationUtilities.MatchPrevious(clip, sceneObject.transform, TimelineEditor.inspectedDirector);
                }
            }

            InspectorWindow.RepaintAllInspectors();
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        internal static void MatchClipsToNext(TimelineClip[] clips)
        {
            if (!EnforcePreviewMode())
                return;

            clips = clips.OrderByDescending(x => x.start).ToArray();
            foreach (var clip in clips)
            {
                var sceneObject = TimelineUtility.GetSceneGameObject(TimelineEditor.inspectedDirector, clip.GetParentTrack());
                if (sceneObject != null)
                {
                    TimelineAnimationUtilities.MatchNext(clip, sceneObject.transform, TimelineEditor.inspectedDirector);
                }
            }

            InspectorWindow.RepaintAllInspectors();
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        public static void ResetClipOffsets(TimelineClip[] clips)
        {
            foreach (var clip in clips)
            {
                var asset = clip.asset as AnimationPlayableAsset;
                if (asset != null)
                    asset.ResetOffsets();
            }

            InspectorWindow.RepaintAllInspectors();
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }
    }
}
