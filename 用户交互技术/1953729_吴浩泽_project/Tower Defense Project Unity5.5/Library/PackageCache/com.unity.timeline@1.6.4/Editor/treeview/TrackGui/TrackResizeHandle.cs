using UnityEngine;

namespace UnityEditor.Timeline
{
    class TrackResizeHandle : IBounds
    {
        public Rect boundingRect { get; private set; }

        public TimelineTrackGUI trackGUI { get; }

        public TrackResizeHandle(TimelineTrackGUI trackGUI)
        {
            this.trackGUI = trackGUI;
        }

        public void Draw(Rect headerRect, WindowState state)
        {
            const float resizeHandleHeight = WindowConstants.trackResizeHandleHeight;
            var rect = new Rect(headerRect.xMin, headerRect.yMax - (0.5f * resizeHandleHeight), headerRect.width, resizeHandleHeight);
            boundingRect = trackGUI.ToWindowSpace(rect);

            Rect cursorRect = rect;
            cursorRect.height--;

            if (GUIUtility.hotControl == 0)
            {
                EditorGUIUtility.AddCursorRect(cursorRect, MouseCursor.SplitResizeUpDown);
                state.headerSpacePartitioner.AddBounds(this);
            }
        }
    }
}
