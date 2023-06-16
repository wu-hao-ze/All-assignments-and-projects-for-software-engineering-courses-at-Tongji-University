using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    [CustomTimelineEditor(typeof(AudioPlayableAsset)), UsedImplicitly]
    class AudioPlayableAssetEditor : ClipEditor
    {
        readonly string k_NoClipAssignedError = L10n.Tr("No audio clip assigned");
        readonly Dictionary<TimelineClip, WaveformPreview> m_PersistentPreviews = new Dictionary<TimelineClip, WaveformPreview>();
        ColorSpace m_ColorSpace = ColorSpace.Uninitialized;

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var clipOptions = base.GetClipOptions(clip);
            var audioAsset = clip.asset as AudioPlayableAsset;
            if (audioAsset != null && audioAsset.clip == null)
                clipOptions.errorText = k_NoClipAssignedError;
            return clipOptions;
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            if (!TimelineWindow.instance.state.showAudioWaveform)
                return;

            var rect = region.position;
            if (rect.width <= 0)
                return;

            var audioClip = clip.asset as AudioClip;
            if (audioClip == null)
            {
                var audioPlayableAsset = clip.asset as AudioPlayableAsset;
                if (audioPlayableAsset != null)
                    audioClip = audioPlayableAsset.clip;
            }

            if (audioClip == null)
                return;

            var quantizedRect = new Rect(Mathf.Ceil(rect.x), Mathf.Ceil(rect.y), Mathf.Ceil(rect.width), Mathf.Ceil(rect.height));

            WaveformPreview preview = GetOrCreateWaveformPreview(clip, audioClip, quantizedRect, region.startTime, region.endTime);
            if (Event.current.type == EventType.Repaint)
                DrawWaveformPreview(preview, quantizedRect);
        }

        public WaveformPreview GetOrCreateWaveformPreview(TimelineClip clip, AudioClip audioClip, Rect rect, double startTime, double endTime)
        {
            if (QualitySettings.activeColorSpace != m_ColorSpace)
            {
                m_ColorSpace = QualitySettings.activeColorSpace;
                m_PersistentPreviews.Clear();
            }

            bool previewExists = m_PersistentPreviews.TryGetValue(clip, out WaveformPreview preview);
            bool audioClipHasChanged = preview != null && audioClip != preview.presentedObject;
            if (!previewExists || audioClipHasChanged)
            {
                if (AssetDatabase.Contains(audioClip))
                    preview = CreateWaveformPreview(audioClip, rect);
                m_PersistentPreviews[clip] = preview;
            }

            if (preview == null)
                return null;

            preview.looping = clip.SupportsLooping();
            preview.SetTimeInfo(startTime, endTime - startTime);
            preview.OptimizeForSize(rect.size);
            return preview;
        }

        public static void DrawWaveformPreview(WaveformPreview preview, Rect rect)
        {
            if (preview != null)
            {
                preview.ApplyModifications();
                preview.Render(rect);
            }
        }

        static WaveformPreview CreateWaveformPreview(AudioClip audioClip, Rect quantizedRect)
        {
            WaveformPreview preview = WaveformPreviewFactory.Create((int)quantizedRect.width, audioClip);
            Color waveColour = GammaCorrect(DirectorStyles.Instance.customSkin.colorAudioWaveform);
            Color transparent = waveColour;
            transparent.a = 0;
            preview.backgroundColor = transparent;
            preview.waveColor = waveColour;
            preview.SetChannelMode(WaveformPreview.ChannelMode.MonoSum);
            preview.updated += () => TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
            return preview;
        }

        static Color GammaCorrect(Color color)
        {
            return (QualitySettings.activeColorSpace == ColorSpace.Linear) ? color.gamma : color;
        }
    }
}
