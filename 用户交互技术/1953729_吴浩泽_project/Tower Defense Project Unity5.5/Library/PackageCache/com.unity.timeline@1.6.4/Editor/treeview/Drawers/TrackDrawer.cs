using System;
using UnityEngine;
using UnityEngine.Timeline;
namespace UnityEditor.Timeline
{
    class TrackDrawer : GUIDrawer
    {
        internal WindowState sequencerState { get; set; }


        public static TrackDrawer CreateInstance(TrackAsset trackAsset)
        {
            if (trackAsset == null)
                return Activator.CreateInstance<TrackDrawer>();

            TrackDrawer drawer;

            try
            {
                drawer = (TrackDrawer)Activator.CreateInstance(TimelineHelpers.GetCustomDrawer(trackAsset.GetType()));
            }
            catch (Exception)
            {
                drawer = Activator.CreateInstance<TrackDrawer>();
            }

            drawer.track = trackAsset;
            return drawer;
        }

        protected TrackAsset track { get; private set; }

        public virtual bool HasCustomTrackHeaderButton()
        {
            return false;
        }

        public virtual void DrawTrackHeaderButton(Rect rect, WindowState state) { }

        public virtual bool DrawTrack(Rect trackRect, TrackAsset trackAsset, Vector2 visibleTime, WindowState state)
        {
            return false;
        }

        public virtual void DrawRecordingBackground(Rect trackRect, TrackAsset trackAsset, Vector2 visibleTime, WindowState state)
        {
            EditorGUI.DrawRect(trackRect, DirectorStyles.Instance.customSkin.colorTrackBackgroundRecording);
        }
    }
}
