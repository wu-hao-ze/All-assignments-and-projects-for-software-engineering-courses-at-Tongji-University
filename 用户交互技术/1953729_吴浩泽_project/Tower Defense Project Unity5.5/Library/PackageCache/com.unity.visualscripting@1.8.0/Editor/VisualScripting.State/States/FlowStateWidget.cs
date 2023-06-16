using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(FlowState))]
    public sealed class FlowStateWidget : NesterStateWidget<FlowState>, IDragAndDropHandler
    {
        public FlowStateWidget(StateCanvas canvas, FlowState state) : base(canvas, state)
        {
            state.nest.beforeGraphChange += BeforeGraphChange;
            state.nest.afterGraphChange += AfterGraphChange;

            if (state.nest.graph != null)
            {
                state.nest.graph.elements.CollectionChanged += CacheEventLinesOnUnityThread;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            state.nest.beforeGraphChange -= BeforeGraphChange;
            state.nest.afterGraphChange -= AfterGraphChange;
        }

        private void BeforeGraphChange()
        {
            if (state.nest.graph != null)
            {
                state.nest.graph.elements.CollectionChanged -= CacheEventLinesOnUnityThread;
            }
        }

        private void AfterGraphChange()
        {
            CacheEventLinesOnUnityThread();

            if (state.nest.graph != null)
            {
                state.nest.graph.elements.CollectionChanged += CacheEventLinesOnUnityThread;
            }
        }

        #region Model

        private List<EventLine> eventLines { get; } = new List<EventLine>();

        private void CacheEventLinesOnUnityThread()
        {
            UnityAPI.Async(CacheEventLines);
        }

        private void CacheEventLines()
        {
            eventLines.Clear();

            if (state.nest.graph != null)
            {
                eventLines.AddRange(state.nest.graph.units
                    .OfType<IEventUnit>()
                    .Select(e => e.GetType())
                    .Distinct()
                    .Select(eventType => new EventLine(eventType))
                    .OrderBy(eventLine => eventLine.content.text));
            }

            Reposition();
        }

        protected override void CacheItemFirstTime()
        {
            base.CacheItemFirstTime();

            CacheEventLines();
        }

        #endregion


        #region Positioning

        public Dictionary<EventLine, Rect> eventLinesPositions { get; } = new Dictionary<EventLine, Rect>();

        public override void CachePosition()
        {
            base.CachePosition();

            eventLinesPositions.Clear();

            var y = contentInnerPosition.y;

            foreach (var eventLine in eventLines)
            {
                var eventLinePosition = new Rect
                    (
                    contentInnerPosition.x,
                    y,
                    contentInnerPosition.width,
                    eventLine.GetHeight(contentInnerPosition.width)
                    );

                eventLinesPositions.Add(eventLine, eventLinePosition);

                y += eventLinePosition.height;
            }
        }

        protected override float GetContentHeight(float width)
        {
            var eventLinesHeight = 0f;

            foreach (var eventLine in eventLines)
            {
                eventLinesHeight += eventLine.GetHeight(width);
            }

            return eventLinesHeight;
        }

        #endregion


        #region Drawing

        protected override bool showContent => eventLines.Count > 0;

        protected override void DrawContent()
        {
            foreach (var eventLine in eventLines)
            {
                eventLine.Draw(eventLinesPositions[eventLine]);
            }
        }

        #endregion


        #region Drag & Drop

        public DragAndDropVisualMode dragAndDropVisualMode => DragAndDropVisualMode.Generic;

        public bool AcceptsDragAndDrop()
        {
            return DragAndDropUtility.Is<ScriptGraphAsset>();
        }

        public void PerformDragAndDrop()
        {
            UndoUtility.RecordEditedObject("Drag & Drop Macro");
            state.nest.source = GraphSource.Macro;
            state.nest.macro = DragAndDropUtility.Get<ScriptGraphAsset>();
            state.nest.embed = null;
            GUI.changed = true;
        }

        public void UpdateDragAndDrop() { }

        public void DrawDragAndDropPreview()
        {
            GraphGUI.DrawDragAndDropPreviewLabel(new Vector2(edgePosition.x, outerPosition.yMax), "Replace with: " + DragAndDropUtility.Get<ScriptGraphAsset>().name, typeof(ScriptGraphAsset).Icon());
        }

        public void ExitDragAndDrop() { }

        #endregion


        public new static class Styles
        {
            static Styles()
            {
                eventLine = new GUIStyle(EditorStyles.label);
                eventLine.wordWrap = true;
                eventLine.imagePosition = ImagePosition.TextOnly; // The icon is drawn manually
                eventLine.padding = new RectOffset(0, 0, 3, 3);
            }

            public static readonly GUIStyle eventLine;

            public static readonly float spaceAroundLineIcon = 5;
        }

        public class EventLine
        {
            public EventLine(Type eventType)
            {
                content = new GUIContent(BoltFlowNameUtility.UnitTitle(eventType, false, true), eventType.Icon()?[IconSize.Small]);
            }

            public GUIContent content { get; }

            public float GetHeight(float width)
            {
                var labelWidth = width - Styles.spaceAroundLineIcon - IconSize.Small - Styles.spaceAroundLineIcon;

                return Styles.eventLine.CalcHeight(content, labelWidth);
            }

            public void Draw(Rect position)
            {
                var iconPosition = new Rect
                    (
                    position.x + Styles.spaceAroundLineIcon,
                    position.y + Styles.eventLine.padding.top - 1,
                    IconSize.Small,
                    IconSize.Small
                    );

                var labelPosition = new Rect
                    (
                    iconPosition.xMax + Styles.spaceAroundLineIcon,
                    position.y,
                    position.width - Styles.spaceAroundLineIcon - iconPosition.width - Styles.spaceAroundLineIcon,
                    position.height
                    );

                if (content.image != null)
                {
                    GUI.DrawTexture(iconPosition, content.image);
                }

                GUI.Label(labelPosition, content, Styles.eventLine);
            }
        }
    }
}
