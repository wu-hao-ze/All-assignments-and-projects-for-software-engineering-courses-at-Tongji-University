using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Timeline.Samples
{
    // Editor used by the TimelineEditor to customize the view of a VideoPlayableAsset
    [CustomTimelineEditor(typeof(VideoPlayableAsset))]
    public class VideoAssetClipEditor : ClipEditor
    {
        // Called by the Timeline Editor to draw the background of the timeline clip
        // when the clip has a VideoPlayableAsset attached
        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            VideoPlayableAsset videoAsset = clip.asset as VideoPlayableAsset;
            if (videoAsset != null && videoAsset.videoClip != null)
            {
                // Load the preview or the thumbnail for the video
                Texture texturePreview = AssetPreview.GetAssetPreview(videoAsset.videoClip);
                if (texturePreview == null)
                    texturePreview = AssetPreview.GetMiniThumbnail(videoAsset.videoClip);

                if (texturePreview != null)
                {
                    Rect rect = region.position;
                    rect.width = texturePreview.width * rect.height / texturePreview.height;
                    GUI.DrawTexture(rect, texturePreview, ScaleMode.StretchToFill);
                }
            }
        }
    }
}
