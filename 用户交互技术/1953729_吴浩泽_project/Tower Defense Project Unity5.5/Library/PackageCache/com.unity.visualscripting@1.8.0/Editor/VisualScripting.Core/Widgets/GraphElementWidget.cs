using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class GraphElementWidget<TCanvas, TElement> : Widget<TCanvas, TElement>, IGraphElementWidget
        where TCanvas : class, ICanvas
        where TElement : class, IGraphElement
    {
        protected GraphElementWidget(TCanvas canvas, TElement element) : base(canvas, element) { }
        protected Rect headerPosition { get; set; }

        public override string ToString()
        {
            return base.ToString() + "\nGUID: " + element.guid;
        }

        public override void Dispose()
        {
            if (isSelected)
            {
                selection.Remove(element);
            }

            base.Dispose();
        }

        #region Model

        public TElement element => item;

        IGraphElement IGraphElementWidget.element => element;

        public override Metadata FetchMetadata()
        {
            return context.ElementMetadata(element);
        }

        protected T GetData<T>() where T : IGraphElementData
        {
            return reference.GetElementData<T>((IGraphElementWithData)element);
        }

        protected T GetDebugData<T>() where T : IGraphElementDebugData
        {
            return reference.GetElementDebugData<T>((IGraphElementWithDebugData)element);
        }

        #endregion


        #region Lifecycle

        public override void BeforeFrame()
        {
            base.BeforeFrame();

            if (canResize)
            {
                CalculateResizeAreas();
            }

            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                debug += $"\nGUID: {element.guid.ToString().PartBefore('-').ToUpper()}...";
            }
        }

        public override void HandleCapture()
        {
            e.HandleCapture(isMouseOver || canStartResize, false);
        }

        public override void HandleInput()
        {
            HandleResizing();
            HandleSelecting();
            RelayDragEvents();
            HandleDoubleClick();
            HandleContext();
        }

        #endregion


        #region Layouting

        public virtual bool canAlignAndDistribute => canDrag;

        #endregion


        #region Z-Ordering

        public override float zIndex { get; set; }

        #endregion


        #region Context

        protected override IEnumerable<DropdownOption> contextOptions
        {
            get
            {
                var suffix = selection.Count > 1 ? " Selection" : "";

                if (GraphClipboard.canCopySelection)
                {
                    yield return new DropdownOption((Action)GraphClipboard.CopySelection, "Copy" + suffix);
                    yield return new DropdownOption((Action)GraphClipboard.CutSelection, "Cut" + suffix);
                }

                if (GraphClipboard.canDuplicateSelection)
                {
                    yield return new DropdownOption((Action)GraphClipboard.DuplicateSelection, "Duplicate" + suffix);
                }

                if (selection.Count > 0)
                {
                    yield return new DropdownOption((Action)canvas.DeleteSelection, "Delete" + suffix);
                }

                if (GraphClipboard.CanPasteInside(element))
                {
                    yield return new DropdownOption((Action)(() => GraphClipboard.PasteInside(element)), "Paste Inside");
                }

                if (GraphClipboard.canPasteOutside)
                {
                    yield return new DropdownOption((Action)(() => GraphClipboard.PasteOutside(true)), "Paste Outside");
                }
            }
        }

        #endregion


        #region Double-Clicking

        protected virtual void HandleDoubleClick()
        {
            if (e.clickCount == 2 && isMouseOver && e.mouseButton == MouseButton.Left)
            {
                OnDoubleClick();
            }
        }

        protected virtual void OnDoubleClick()
        {
            if (element.graph.zoom != 1)
            {
                canvas.ViewElements(((IGraphElement)element).Yield());
                e.Use();
            }
        }

        #endregion


        #region Selecting

        public virtual bool canSelect => false;

        public bool isSelected => selection.Contains(element);

        private void Select()
        {
            if (e.shift || e.ctrlOrCmd)
            {
                selection.Add(element);
            }
            else
            {
                if (!selection.Contains(element))
                {
                    selection.Clear();
                }

                selection.Add(element);
            }
        }

        protected override void OnContext()
        {
            // Because using Ctrl+LMB on OSX will send the ContextClick
            // event before the MouseDown event, we need to make sure the widget
            // is selected before showing the context menu.
            // https://issuetracker.unity3d.com/issues/different-event-order-for-context-click
            // https://support.ludiq.io/forums/5-bolt/topics/660-/#comment-2946
            Select();

            base.OnContext();
        }

        private void HandleSelecting()
        {
            if (canSelect && (e.IsMouseDown(MouseButton.Left) || e.IsMouseDown(MouseButton.Right)))
            {
                Select();

                // Because using the MouseDown event on OSX will prevent the ContextClick
                // event from being sent, we need to avoid using it on RMB here.
                // https://issuetracker.unity3d.com/issues/different-event-order-for-context-click
                // https://support.ludiq.io/forums/5-bolt/topics/660-/#comment-2946
                if (e.mouseButton == MouseButton.Left)
                {
                    e.Use();
                }

                BringToFront();
            }
        }

        #endregion


        #region Resizing

        private bool isResizingXMin;

        private bool isResizingXMax;

        private bool isResizingYMin;

        private bool isResizingYMax;

        private float xMinResizeOffset;

        private float xMaxResizeOffset;

        private float yMinResizeOffset;

        private float yMaxResizeOffset;

        protected readonly RectOffset resizeInnerOffset = new RectOffset(8, 8, 8, 8);

        protected readonly RectOffset resizeOuterOffset = new RectOffset(8, 8, 8, 8);

        protected Vector2 minResizeSize = new Vector2(0, 0);

        protected Vector2 maxResizeSize = new Vector2(9999, 9999);

        public virtual bool canResizeHorizontal => false;

        public virtual bool canResizeVertical => false;

        public bool canResize => canResizeHorizontal || canResizeVertical;

        public bool isResizing =>
            isResizingXMin ||
            isResizingXMax ||
            isResizingYMin ||
            isResizingYMax;

        protected virtual Rect resizeArea => position;

        private Rect resizeTopArea;

        private Rect resizeBottomArea;

        private Rect resizeLeftArea;

        private Rect resizeRightArea;

        private Rect resizeTopLeftArea;

        private Rect resizeTopRightArea;

        private Rect resizeBottomLeftArea;

        private Rect resizeBottomRightArea;

        private void CalculateResizeAreas()
        {
            var resizeOuterArea = resizeOuterOffset.Add(resizeArea);

            Rect outerTopLeft,
                 outerTopCenter,
                 outerTopRight,
                 outerMiddleLeft,
                 outerMiddleCenter,
                 outerMiddleRight,
                 outerBottomLeft,
                 outerBottomCenter,
                 outerBottomRight;

            Rect innerTopLeft,
                 innerTopCenter,
                 innerTopRight,
                 innerMiddleLeft,
                 innerMiddleCenter,
                 innerMiddleRight,
                 innerBottomLeft,
                 innerBottomCenter,
                 innerBottomRight;

            resizeOuterArea.NineSlice(resizeOuterOffset,
                out outerTopLeft,
                out outerTopCenter,
                out outerTopRight,
                out outerMiddleLeft,
                out outerMiddleCenter,
                out outerMiddleRight,
                out outerBottomLeft,
                out outerBottomCenter,
                out outerBottomRight);

            resizeArea.NineSlice(resizeInnerOffset,
                out innerTopLeft,
                out innerTopCenter,
                out innerTopRight,
                out innerMiddleLeft,
                out innerMiddleCenter,
                out innerMiddleRight,
                out innerBottomLeft,
                out innerBottomCenter,
                out innerBottomRight);

            if (canResizeHorizontal && canResizeVertical)
            {
                resizeTopArea = outerTopCenter.Encompass(innerTopCenter);
                resizeBottomArea = outerBottomCenter.Encompass(innerBottomCenter);

                resizeLeftArea = outerMiddleLeft.Encompass(innerMiddleLeft);
                resizeRightArea = outerMiddleRight.Encompass(innerMiddleRight);

                resizeTopLeftArea = outerTopLeft.Encompass(innerTopLeft);
                resizeTopRightArea = outerTopRight.Encompass(innerTopRight);

                resizeBottomLeftArea = outerBottomLeft.Encompass(innerBottomLeft);
                resizeBottomRightArea = outerBottomRight.Encompass(innerBottomRight);
            }
            else if (canResizeHorizontal)
            {
                resizeLeftArea = outerTopLeft.Encompass(outerMiddleLeft).Encompass(outerBottomLeft).Encompass(innerTopLeft).Encompass(innerMiddleLeft).Encompass(innerBottomLeft);
                resizeRightArea = outerTopRight.Encompass(outerMiddleRight).Encompass(outerBottomRight).Encompass(innerTopRight).Encompass(innerMiddleRight).Encompass(innerBottomRight);
            }
            else if (canResizeVertical)
            {
                resizeTopArea = outerTopLeft.Encompass(outerTopCenter).Encompass(outerTopRight).Encompass(innerTopLeft).Encompass(innerTopCenter).Encompass(innerTopRight);
                resizeBottomArea = outerBottomLeft.Encompass(outerBottomCenter).Encompass(outerBottomRight).Encompass(innerBottomLeft).Encompass(innerBottomCenter).Encompass(innerBottomRight);
            }
        }

        protected bool isMouseOverHeaderArea
        {
            get
            {
                if (canDrag && !isMouseOverResizeArea)
                {
                    return headerPosition == Rect.zero || headerPosition.Contains(mousePosition);
                }

                return false;
            }
        }

        private bool isMouseOverResizeArea
        {
            get
            {
                if (canResizeHorizontal && canResizeVertical)
                {
                    return
                        resizeLeftArea.Contains(mousePosition) ||
                        resizeRightArea.Contains(mousePosition) ||
                        resizeTopArea.Contains(mousePosition) ||
                        resizeBottomArea.Contains(mousePosition) ||
                        resizeTopLeftArea.Contains(mousePosition) ||
                        resizeTopRightArea.Contains(mousePosition) ||
                        resizeBottomLeftArea.Contains(mousePosition) ||
                        resizeBottomRightArea.Contains(mousePosition);
                }
                else if (canResizeHorizontal)
                {
                    return
                        resizeLeftArea.Contains(mousePosition) ||
                        resizeRightArea.Contains(mousePosition);
                }
                else if (canResizeVertical)
                {
                    return
                        resizeTopArea.Contains(mousePosition) ||
                        resizeBottomArea.Contains(mousePosition);
                }
                else
                {
                    return false;
                }
            }
        }

        private bool canStartResize => canResize &&
        canvas.zoom >= GraphGUI.MinZoomForControls &&
        !canvas.isDragging &&
        isMouseOverResizeArea;

        private void HandleResizing()
        {
            if (e.IsMouseDrag(MouseButton.Left) && !isResizing && canStartResize)
            {
                if (resizeLeftArea.Contains(mousePosition))
                {
                    isResizingXMin = true;
                    xMinResizeOffset = mousePosition.x - position.xMin;
                }

                if (resizeRightArea.Contains(mousePosition))
                {
                    isResizingXMax = true;
                    xMaxResizeOffset = mousePosition.x - position.xMax;
                }

                if (resizeTopArea.Contains(mousePosition))
                {
                    isResizingYMin = true;
                    yMinResizeOffset = mousePosition.y - position.yMin;
                }

                if (resizeBottomArea.Contains(mousePosition))
                {
                    isResizingYMax = true;
                    yMaxResizeOffset = mousePosition.y - position.yMax;
                }

                if (resizeTopLeftArea.Contains(mousePosition))
                {
                    isResizingXMin = true;
                    isResizingYMin = true;
                    xMinResizeOffset = mousePosition.x - position.xMin;
                    yMinResizeOffset = mousePosition.y - position.yMin;
                }

                if (resizeTopRightArea.Contains(mousePosition))
                {
                    isResizingXMax = true;
                    isResizingYMin = true;
                    xMaxResizeOffset = mousePosition.x - position.xMax;
                    yMinResizeOffset = mousePosition.y - position.yMin;
                }

                if (resizeBottomLeftArea.Contains(mousePosition))
                {
                    isResizingXMin = true;
                    isResizingYMax = true;
                    xMinResizeOffset = mousePosition.x - position.xMin;
                    yMaxResizeOffset = mousePosition.y - position.yMax;
                }

                if (resizeBottomRightArea.Contains(mousePosition))
                {
                    isResizingXMax = true;
                    isResizingYMax = true;
                    xMaxResizeOffset = mousePosition.x - position.xMax;
                    yMaxResizeOffset = mousePosition.y - position.yMax;
                }

                e.Use();
            }
            else if (e.IsMouseDrag(MouseButton.Left) && isResizing)
            {
                var resizedPosition = position;

                if (isResizingXMin)
                {
                    resizedPosition.xMin = Mathf.Min(position.xMax - minResizeSize.x, mousePosition.x - xMinResizeOffset);

                    if (snapToGrid)
                    {
                        resizedPosition.xMin = GraphGUI.SnapToGrid(resizedPosition.xMin);
                    }
                }

                if (isResizingXMax)
                {
                    resizedPosition.xMax = Mathf.Max(position.xMin + minResizeSize.x, mousePosition.x - xMaxResizeOffset);

                    if (snapToGrid)
                    {
                        resizedPosition.xMax = GraphGUI.SnapToGrid(resizedPosition.xMax);
                    }
                }

                if (isResizingYMin)
                {
                    resizedPosition.yMin = Mathf.Min(position.yMax - minResizeSize.y, mousePosition.y - yMinResizeOffset);

                    if (snapToGrid)
                    {
                        resizedPosition.yMin = GraphGUI.SnapToGrid(resizedPosition.yMin);
                    }
                }

                if (isResizingYMax)
                {
                    resizedPosition.yMax = Mathf.Max(position.yMin + minResizeSize.y, mousePosition.y - yMaxResizeOffset);

                    if (snapToGrid)
                    {
                        resizedPosition.yMax = GraphGUI.SnapToGrid(resizedPosition.yMax);
                    }
                }

                resizedPosition.width = Mathf.Clamp(resizedPosition.width, minResizeSize.x, maxResizeSize.x);
                resizedPosition.height = Mathf.Clamp(resizedPosition.height, minResizeSize.y, maxResizeSize.y);

                UndoUtility.RecordEditedObject("Resize Graph Element");

                position = resizedPosition;

                Reposition();

                e.Use();
            }
            else if (e.IsMouseUp(MouseButton.Left) && isResizing)
            {
                isResizingXMin = false;
                isResizingXMax = false;
                isResizingYMin = false;
                isResizingYMax = false;
                e.Use();
            }
        }

        public void AddCursorRect(Rect rect, MouseCursor cursor)
        {
            window.AddCursorRect(rect, cursor);
        }

        private void AddResizeCursorRects()
        {
            if (canResizeHorizontal)
            {
                AddCursorRect(resizeLeftArea, MouseCursor.ResizeHorizontal);
                AddCursorRect(resizeRightArea, MouseCursor.ResizeHorizontal);
            }

            if (canResizeVertical)
            {
                AddCursorRect(resizeTopArea, MouseCursor.ResizeVertical);
                AddCursorRect(resizeBottomArea, MouseCursor.ResizeVertical);
            }

            if (canResizeHorizontal && canResizeVertical)
            {
                AddCursorRect(resizeTopLeftArea, MouseCursor.ResizeUpLeft);
                AddCursorRect(resizeTopRightArea, MouseCursor.ResizeUpRight);
                AddCursorRect(resizeBottomLeftArea, MouseCursor.ResizeUpRight);
                AddCursorRect(resizeBottomRightArea, MouseCursor.ResizeUpLeft);
            }
        }

        #endregion


        #region Dragging

        public virtual bool canDrag => false;

        public bool isDragging { get; private set; }

        private Rect dragPosition;

        private Rect dragLockOrigin;

        private void RelayDragEvents()
        {
            if ((!canDrag || !isMouseOverHeaderArea) && !canvas.isDragging)
            {
                return;
            }

            if (e.IsMouseDrag(MouseButton.Left))
            {
                if (!canvas.isDragging)
                {
                    canvas.BeginDrag(e);
                }
                else
                {
                    canvas.Drag(e);
                }
            }
            else if (e.IsMouseUp(MouseButton.Left))
            {
                canvas.EndDrag(e);
            }
        }

        public void BeginDrag()
        {
            dragPosition = position;
            isDragging = true;
        }

        public void Drag(Vector2 delta, Vector2 constraint)
        {
            dragPosition.position += delta;

            var lockedPosition = dragPosition;

            if (constraint.x == 0)
            {
                lockedPosition.x = dragLockOrigin.x;
            }

            if (constraint.y == 0)
            {
                lockedPosition.y = dragLockOrigin.y;
            }

            if (snapToGrid)
            {
                position = GraphGUI.SnapToGrid(lockedPosition, canResizeHorizontal && canResizeVertical);
            }
            else
            {
                position = lockedPosition.PixelPerfect();
            }

            Reposition();
        }

        public void EndDrag()
        {
            position = position.PixelPerfect();
            isDragging = false;
            Reposition();
            GUI.changed = true;
        }

        public void LockDragOrigin()
        {
            dragLockOrigin = position;
        }

        public virtual void ExpandDragGroup(HashSet<IGraphElement> dragGroup) { }

        #endregion


        #region Deleting

        public virtual bool canDelete => false;

        public void Delete()
        {
            if (!canDelete)
            {
                return;
            }

            var deleted = false;

            var deleteGroup = new HashSet<IGraphElement>(new IGraphElement[] { element });

            canvas.Widget(element).ExpandDeleteGroup(deleteGroup);

            foreach (var elementToDelete in deleteGroup)
            {
                if (canvas.Widget(elementToDelete).canDelete)
                {
                    UndoUtility.RecordEditedObject("Delete Graph Element");
                    element.graph.elements.Remove(elementToDelete);
                    selection.Remove(elementToDelete);
                    deleted = true;
                }
            }

            if (deleted)
            {
                GUI.changed = true;
                e.Use();
            }
        }

        public virtual void ExpandDeleteGroup(HashSet<IGraphElement> deleteGroup) { }

        #endregion


        #region Drawing

        public override void DrawOverlay()
        {
            base.DrawOverlay();

            if (canStartResize)
            {
                AddResizeCursorRects();
            }
        }

        #endregion


        #region Clipboard

        public virtual bool canCopy => true;

        public virtual void ExpandCopyGroup(HashSet<IGraphElement> copyGroup) { }

        #endregion
    }
}
