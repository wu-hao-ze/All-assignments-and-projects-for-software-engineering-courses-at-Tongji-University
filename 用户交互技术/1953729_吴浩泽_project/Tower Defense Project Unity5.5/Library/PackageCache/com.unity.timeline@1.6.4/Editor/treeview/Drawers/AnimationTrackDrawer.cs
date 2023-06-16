using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Timeline;

namespace UnityEditor.Timeline
{
    [CustomTrackDrawer(typeof(AnimationTrack)), UsedImplicitly]
    class AnimationTrackDrawer : TrackDrawer
    {
        static class Styles
        {
            public static readonly GUIContent AvatarMaskActiveTooltip = L10n.TextContent(string.Empty, "Enable Avatar Mask");
            public static readonly GUIContent AvatarMaskInactiveTooltip = L10n.TextContent(string.Empty, "Disable Avatar Mask");
        }

        public override void DrawTrackHeaderButton(Rect rect, WindowState state)
        {
            var animTrack = track as AnimationTrack;
            if (animTrack == null) return;

            var style = DirectorStyles.Instance.trackAvatarMaskButton;
            var tooltip = animTrack.applyAvatarMask ? Styles.AvatarMaskInactiveTooltip : Styles.AvatarMaskActiveTooltip;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var toggle = GUI.Toggle(rect, animTrack.applyAvatarMask, tooltip, style);
                if (check.changed)
                {
                    animTrack.applyAvatarMask = toggle;
                    if (state != null)
                        state.rebuildGraph = true;
                }
            }
        }

        public override void DrawRecordingBackground(Rect trackRect, TrackAsset trackAsset, Vector2 visibleTime, WindowState state)
        {
            base.DrawRecordingBackground(trackRect, trackAsset, visibleTime, state);
            DrawBorderOfAddedRecordingClip(trackRect, trackAsset, visibleTime, (WindowState)state);
        }

        static void DrawBorderOfAddedRecordingClip(Rect trackRect, TrackAsset trackAsset, Vector2 visibleTime, WindowState state)
        {
            if (!state.IsArmedForRecord(trackAsset))
                return;

            AnimationTrack animTrack = trackAsset as AnimationTrack;
            if (animTrack == null || !animTrack.inClipMode)
                return;

            // make sure there is no clip but we can add one
            TimelineClip clip = null;
            if (trackAsset.FindRecordingClipAtTime(state.editSequence.time, out clip) || clip != null)
                return;

            float yMax = trackRect.yMax;
            float yMin = trackRect.yMin;

            double startGap = 0;
            double endGap = 0;

            trackAsset.GetGapAtTime(state.editSequence.time, out startGap, out endGap);
            if (double.IsInfinity(endGap))
                endGap = visibleTime.y;

            if (startGap > visibleTime.y || endGap < visibleTime.x)
                return;


            startGap = Math.Max(startGap, visibleTime.x);
            endGap = Math.Min(endGap, visibleTime.y);

            float xMin = state.TimeToPixel(startGap);
            float xMax = state.TimeToPixel(endGap);

            var r = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
            ClipDrawer.DrawClipSelectionBorder(r, ClipBorder.Recording(), ClipBlends.kNone);
        }

        public override bool HasCustomTrackHeaderButton()
        {
            var animTrack = track as AnimationTrack;
            if (animTrack == null) return false;

            return animTrack != null && animTrack.avatarMask != null;
        }
    }
}
