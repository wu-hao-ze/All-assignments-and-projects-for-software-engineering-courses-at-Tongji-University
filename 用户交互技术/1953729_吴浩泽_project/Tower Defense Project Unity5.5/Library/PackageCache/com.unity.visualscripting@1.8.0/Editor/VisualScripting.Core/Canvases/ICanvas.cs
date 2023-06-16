using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface ICanvas : IDisposable, IDragAndDropHandler
    {
        void Cache();

        #region Model

        ICanvasWindow window { get; set; }

        IGraph graph { get; }

        WidgetProvider widgetProvider { get; }

        GraphSelection selection { get; }

        #endregion


        #region Widgets

        IEnumerable<IWidget> widgets { get; }

        void Recollect();

        void CacheWidgetCollections();

        #endregion


        #region Hovering

        IWidget hoveredWidget { get; }

        #endregion


        #region Deleting

        void DeleteSelection();

        #endregion


        #region Clipboard

        void ShrinkCopyGroup(HashSet<IGraphElement> copyGroup);

        #endregion


        #region Lifecycle

        void RegisterControls();

        void Open();

        void Close();

        void Update();

        void BeforeFrame();

        void OnGUI();

        #endregion


        #region Viewing

        float zoom { get; }

        Vector2 pan { get; }

        void UpdateViewport();

        Rect viewport { get; set; }

        Vector2 mousePosition { get; }

        bool isMouseOver { get; }

        bool isMouseOverBackground { get; }

        void ViewElements(IEnumerable<IGraphElement> elements);

        bool IsVisible(IWidget widget);

        #endregion


        #region Positioning

        void CacheWidgetPositions();

        #endregion


        #region Lassoing

        bool isLassoing { get; }

        Rect lassoArea { get; }

        #endregion


        #region Selecting

        bool isSelecting { get; }

        Rect selectionArea { get; }

        #endregion


        #region Grouping

        bool isGrouping { get; }

        Rect groupArea { get; }

        #endregion


        #region Dragging

        bool isDragging { get; }

        void BeginDrag(EventWrapper e);

        void Drag(EventWrapper e);

        void EndDrag(EventWrapper e);

        #endregion


        #region Layout

        void Align(AlignOperation operation);

        void Distribute(DistributeOperation operation);

        #endregion


        #region Timing

        float frameDeltaTime { get; }

        float eventDeltaTime { get; }

        float repaintDeltaTime { get; }

        #endregion


        #region Window

        void OnToolbarGUI();

        event Action delayCall;

        Queue<Action> delayedCalls { get; }

        #endregion
    }
}
