using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    class TimelineMarkerHeaderGUI : IRowGUI, ILayerable
    {
        static readonly GUIContent k_Muted = L10n.TextContent("Muted");

        int m_TrackHash;
        TimelineAsset timeline { get; }
        WindowState state { get; }
        MarkersLayer m_Layer;
        LayerZOrder m_ZOrder = new LayerZOrder(Layer.MarkerHeaderTrack, 0);

        struct DrawData
        {
            public Rect headerRect;
            public Rect contentRect;
            public GUIStyle trackHeaderFont;
            public Color colorTrackFont;
            public bool isMuted;
            public bool isSelected;
        }

        public TimelineMarkerHeaderGUI(TimelineAsset asset, WindowState state)
        {
            m_TrackHash = -1;
            timeline = asset;
            this.state = state;
        }

        public TrackAsset asset => timeline.markerTrack;
        public Rect boundingRect { get; private set; }

        public bool showMarkers => state.showMarkerHeader;
        public bool muted => timeline.markerTrack != null && timeline.markerTrack.muted;
        public bool locked => timeline.markerTrack.locked;
        public LayerZOrder zOrder => m_ZOrder;

        Rect IRowGUI.ToWindowSpace(Rect rect)
        {
            //header gui is already in global coordinates
            return rect;
        }

        public void Draw(Rect markerHeaderRect, Rect markerContentRect, WindowState state)
        {
            boundingRect = markerContentRect;
            var data = new DrawData
            {
                headerRect = markerHeaderRect,
                contentRect = markerContentRect,
                trackHeaderFont = DirectorStyles.Instance.trackHeaderFont,
                colorTrackFont = DirectorStyles.Instance.customSkin.colorTrackFont,
                isMuted = muted,
                isSelected = IsSelected()
            };

            if (state.showMarkerHeader)
            {
                DrawMarkerDrawer(data);
                if (Event.current.type == EventType.Repaint)
                    state.spacePartitioner.AddBounds(this, boundingRect);
            }

            if (asset != null && Hash() != m_TrackHash)
                Rebuild();

            Rect rect = state.showMarkerHeader ? markerContentRect : state.timeAreaRect;
            using (new GUIViewportScope(rect))
            {
                if (m_Layer != null)
                    m_Layer.Draw(rect, state);

                HandleDragAndDrop();
            }

            if (state.showMarkerHeader && data.isMuted)
                DrawMuteOverlay(data);
        }

        public void Rebuild()
        {
            if (asset == null)
                return;

            m_Layer = new MarkersLayer(Layer.MarkersOnHeader, this);
            m_TrackHash = Hash();
        }

        void HandleDragAndDrop()
        {
            if (state.editSequence.isReadOnly || !state.showMarkerHeader)
                return;

            if (Event.current == null || Event.current.type != EventType.DragUpdated &&
                Event.current.type != EventType.DragPerform && Event.current.type != EventType.DragExited)
                return;

            var objectsBeingDropped = DragAndDrop.objectReferences.OfType<Object>();
            var candidateTime = TimelineHelpers.GetCandidateTime(Event.current.mousePosition);
            var perform = Event.current.type == EventType.DragPerform;
            var director = state.editSequence != null ? state.editSequence.director : null;
            DragAndDrop.visualMode = TimelineDragging.HandleClipPaneObjectDragAndDrop(objectsBeingDropped, timeline.markerTrack, perform,
                timeline, null, director, candidateTime, ResolveType);
            if (perform && DragAndDrop.visualMode == DragAndDropVisualMode.Copy)
            {
                DragAndDrop.AcceptDrag();
            }
        }

        static bool ResolveType(IEnumerable<Type> types, Action<Type> onComplete, string formatString)
        {
            void CreateMarkerTrackOnComplete(Type type)
            {
                WindowState state = TimelineWindow.instance.state;
                state.editSequence.asset.CreateMarkerTrack();
                state.showMarkerHeader = true;
                onComplete(type);
            }

            return TimelineDragging.ResolveType(types, CreateMarkerTrackOnComplete, formatString);
        }

        int Hash()
        {
            return timeline.markerTrack == null ? 0 : timeline.markerTrack.Hash();
        }

        static void DrawMarkerDrawer(DrawData data)
        {
            DrawMarkerDrawerHeaderBackground(data);
            DrawMarkerDrawerHeader(data);
            DrawMarkerDrawerContentBackground(data);
        }

        static void DrawMarkerDrawerHeaderBackground(DrawData data)
        {
            Color backgroundColor = data.isSelected
                ? DirectorStyles.Instance.customSkin.colorSelection
                : DirectorStyles.Instance.customSkin.markerHeaderDrawerBackgroundColor;
            EditorGUI.DrawRect(data.headerRect, backgroundColor);
        }

        static void DrawMarkerDrawerHeader(DrawData data)
        {
            var textStyle = data.trackHeaderFont;
            textStyle.normal.textColor = data.colorTrackFont;
            var labelRect = data.headerRect;
            labelRect.x += DirectorStyles.kBaseIndent;

            EditorGUI.LabelField(labelRect, DirectorStyles.timelineMarkerTrackHeader);

            const float buttonSize = WindowConstants.trackHeaderButtonSize;
            const float padding = WindowConstants.trackHeaderButtonPadding;
            var x = data.headerRect.xMax - buttonSize - padding - 2f;
            var y = data.headerRect.y + (data.headerRect.height - buttonSize) / 2.0f;
            var buttonRect = new Rect(x, y, buttonSize, buttonSize);

            DrawTrackDropDownMenu(buttonRect);
            buttonRect.x -= 21.0f;

            DrawMuteButton(buttonRect, data);
        }

        static void DrawMarkerDrawerContentBackground(DrawData data)
        {
            Color trackBackgroundColor = DirectorStyles.Instance.customSkin.markerDrawerBackgroundColor;
            if (data.isSelected)
                trackBackgroundColor = DirectorStyles.Instance.customSkin.colorTrackBackgroundSelected;
            EditorGUI.DrawRect(data.contentRect, trackBackgroundColor);
        }

        static void DrawMuteOverlay(DrawData data)
        {
            DirectorStyles styles = TimelineWindow.styles;

            var colorOverlay = OverlayDrawer.CreateColorOverlay(GUIClip.Unclip(data.contentRect), styles.customSkin.colorTrackDarken);
            colorOverlay.Draw();

            Rect textRect = Graphics.CalculateTextBoxSize(data.contentRect, styles.fontClip, k_Muted, WindowConstants.overlayTextPadding);
            var boxOverlay = OverlayDrawer.CreateTextBoxOverlay(
                GUIClip.Unclip(textRect),
                k_Muted.text,
                styles.fontClip,
                Color.white,
                styles.customSkin.colorLockTextBG,
                styles.displayBackground);
            boxOverlay.Draw();
        }

        static void DrawTrackDropDownMenu(Rect rect)
        {
            if (GUI.Button(rect, GUIContent.none, DirectorStyles.Instance.trackOptions))
            {
                SelectionManager.SelectOnly(TimelineEditor.inspectedAsset.markerTrack);
                SequencerContextMenu.ShowTrackContextMenu(null);
            }
        }

        static void DrawMuteButton(Rect rect, DrawData data)
        {
            bool muted = GUI.Toggle(rect, data.isMuted, string.Empty, TimelineWindow.styles.trackMuteButton);
            if (muted != data.isMuted)
                new[] { TimelineEditor.inspectedAsset.markerTrack }.Invoke<MuteTrack>();
        }

        bool IsSelected()
        {
            return SelectionManager.Contains(asset);
        }
    }
}
