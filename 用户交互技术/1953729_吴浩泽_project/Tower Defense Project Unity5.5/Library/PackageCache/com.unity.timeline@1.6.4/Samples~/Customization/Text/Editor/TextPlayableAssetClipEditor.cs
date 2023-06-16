#if TEXT_TRACK_REQUIRES_TEXTMESH_PRO

using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Timeline.Samples
{
    // Editor used by the TimelineEditor to customize the view of a TextPlayableAsset
    [CustomTimelineEditor(typeof(TextPlayableAsset))]
    public class TextPlayableAssetClipEditor : ClipEditor
    {
        // Called when a clip value, it's attached PlayableAsset, or an animation curve on a template is changed from the TimelineEditor.
        // This is used to keep the displayName of the clip matching the text of the PlayableAsset.
        public override void OnClipChanged(TimelineClip clip)
        {
            var textPlayableasset = clip.asset as TextPlayableAsset;
            if (textPlayableasset != null && !string.IsNullOrEmpty(textPlayableasset.template.text))
                clip.displayName = textPlayableasset.template.text;
        }
    }
}

#endif
