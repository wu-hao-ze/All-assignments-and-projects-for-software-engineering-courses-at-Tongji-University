using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Analytics;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class VisualScriptingCanvas<TGraph> : ICanvas
        where TGraph : class, IGraph
    {
        public WidgetProvider widgetProvider { get; }

        public GraphSelection selection { get; }

        public ICanvasWindow window { get; set; }

        protected EventWrapper e { get; }

        public TGraph graph { get; }

        IGraph ICanvas.graph => graph;

        protected VisualScriptingCanvas(TGraph graph)
        {
            Ensure.That(nameof(graph)).IsNotNull(graph);

            this.graph = graph;

            e = new EventWrapper(GetType());

            selection = new GraphSelection();

            widgetProvider = new WidgetProvider(this);

            graph.elements.CollectionChanged += Recollect;

            widgets = new WidgetList<IWidget>(this);
            elementWidgets = new WidgetList<IGraphElementWidget>(this);
            widgetsByAscendingZ = new WidgetList<IWidget>(this);
            visibleWidgetsByAscendingZ = new WidgetList<IWidget>(this);
            visibleWidgetsByDescendingZ = new WidgetList<IWidget>(this);
        }

        public virtual void Dispose()
        {
            graph.elements.CollectionChanged -= Recollect;

            widgetProvider.FreeAll();
        }

        #region Context Shortcuts

        protected IGraphContext context => LudiqGraphsEditorUtility.editedContext.value;

        protected GraphReference reference => context.reference;

        #endregion


        public void Cache()
        {
            CacheWidgetCollections();
            CacheWidgetItems();
            CacheWidgetPositions();
            CacheWidgetVisibility();
        }

        #region Model

        public void CacheWidgetItems()
        {
            foreach (var widget in widgets)
            {
                widget.CacheItem();
            }
        }

        #endregion


        #region Widgets

        IEnumerable<IWidget> ICanvas.widgets => widgets;

        private readonly WidgetList<IWidget> widgets;

        private readonly WidgetList<IGraphElementWidget> elementWidgets;

        private IEnumerable<IWidget> GetWidgets()
        {
            foreach (var element in graph.elements)
            {
                foreach (var widget in GetWidgetsRecursive(this.Widget(element)))
                {
                    yield return widget;
                }
            }
        }

        private IEnumerable<IWidget> GetWidgetsRecursive(IWidget widget)
        {
            yield return widget;

            foreach (var subWidget in widget.subWidgets)
            {
                foreach (var subWidgetRecursive in GetWidgetsRecursive(subWidget))
                {
                    yield return subWidgetRecursive;
                }
            }
        }

        private bool collectionsAreValid;

        public void Recollect()
        {
            collectionsAreValid = false;
        }

        public void CacheWidgetCollections()
        {
            // Dispose widgets that are no longer within the graph
            // so that they unregister any event handler
            widgetProvider.FreeInvalid();

            // Remove invalid widgets from the drag group
            dragGroup.RemoveWhere(element => !widgetProvider.IsValid(element));

            // Rebuild the widget collection
            widgets.Clear();
            widgets.AddRange(GetWidgets());
            elementWidgets.Clear();
            elementWidgets.AddRange(widgets.OfType<IGraphElementWidget>());

            // Normalize and cache the z-ordering
            var zIndex = 0;

            foreach (var widget in elementWidgets.OrderBy(widget => widget.zIndex))
            {
                widget.zIndex = zIndex++;
            }

            widgetsByAscendingZ.Clear();

            // Convoluted way to avoid allocation while sorting
            var _widgetsByAscendingZ = ListPool<IWidget>.New();

            foreach (var widget in widgets)
            {
                widget.Reposition();
                _widgetsByAscendingZ.Add(widget);
            }

            _widgetsByAscendingZ.Sort((a, b) => a.zIndex.CompareTo(b.zIndex));

            foreach (var widget in _widgetsByAscendingZ)
            {
                widgetsByAscendingZ.Add(widget);
            }

            _widgetsByAscendingZ.Free();

            // Mark as complete
            collectionsAreValid = true;
        }

        #endregion


        #region Lifecycle

        public virtual void Open() { }

        public virtual void Close()
        {
            isLassoing = false;
            isDragging = false;

            foreach (var dragged in dragGroup)
            {
                if (widgetProvider.IsValid(dragged))
                {
                    this.Widget(dragged).EndDrag();
                }
            }

            dragGroup.Clear();
        }

        public void RegisterControls()
        {
            e.RegisterControl(FocusType.Keyboard);

            foreach (var widget in widgets)
            {
                widget.RegisterControl();
            }
        }

        public void Update()
        {
            foreach (var widget in widgets)
            {
                widget.Update();
            }
        }

        public void BeforeFrame()
        {
            if (!collectionsAreValid)
            {
                // Fetch the list of all widgets
                CacheWidgetCollections();
            }

            // Update the widgets with the data from their model
            CacheWidgetItems();

            // Send a before frame event to widgets
            foreach (var widget in widgets)
            {
                widget.BeforeFrame();
            }

            // Calculate the positions for models that have been repositioned
            CacheWidgetPositions();

            lastFrameTime = DateTime.Now;
        }

        public void OnGUI()
        {
            // Cache the mouse position across different windows
            HandleMouseMovement();

            // Determine the hovered widget first to determine if the mouse is over the background
            DetermineHoveredWidget();

            if (e.IsRepaint)
            {
                // Cache the list of visible widgets
                CacheWidgetVisibility();
            }

            // Handle automated edge panning
            // Needed in repaint too because idling
            // near the edges should cause a pan too
            HandleEdgePanning();

            if (!e.IsRepaint)
            {
                // Handle mouse / keyboard capture
                HandleEventCapture();

                // Handle panning and zoom
                HandleViewportInput();

                // Handle any input that takes priority over widgets
                HandleHighPriorityInput();
            }

            // Draw the canvas background
            DrawBackground();

            // Draw the widgets
            DrawWidgetsBackground();
            DrawWidgetsForeground();
            DrawWidgetsOverlay();

            // Draw the canvas overlay
            DrawOverlay();

            // Draw the drag and drop preview
            if (e.IsRepaint)
            {
                dragAndDropHandler?.DrawDragAndDropPreview();
            }

            if (!e.IsRepaint)
            {
                // Handle the widgets' input
                HandleWidgetInput();

                // Handle any input that is less important than the widget's
                HandleLowPriorityInput();

                // Handle mouse / keyboard release
                HandleEventRelease();
            }

            // Update timing for deltas
            lastEventTime = DateTime.Now;

            if (e.IsRepaint)
            {
                lastRepaintTime = DateTime.Now;
            }
        }

        protected void HandleEventCapture()
        {
            foreach (var widget in visibleWidgetsByDescendingZ)
            {
                widget.HandleCapture();
            }

            e.HandleCapture(isMouseOverBackground, isMouseOver);
        }

        protected virtual void HandleHighPriorityInput() { }

        protected void HandleWidgetInput()
        {
            foreach (var widget in visibleWidgetsByDescendingZ)
            {
                widget.HandleInput();
            }
        }

        protected void HandleEventRelease()
        {
            foreach (var widget in widgets)
            {
                widget.HandleRelease();
            }

            e.HandleRelease();
        }

        protected virtual void HandleLowPriorityInput()
        {
            HandleClipboard();

            HandleLassoing();

            HandleSelecting();

            HandleDeleting();

            HandleContext();

            HandleDragAndDrop();

            HandleMaximization();
        }

        protected virtual void HandleMaximization()
        {
            if (e.IsMouseDown(MouseButton.Left) && e.clickCount == 2)
            {
                ToggleMaximized();
            }
        }

        protected void ToggleMaximized()
        {
            window.maximized = !window.maximized;
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
            GUIUtility.ExitGUI();
        }

        #endregion


        #region Viewing

        public float zoom { get; private set; }

        public Vector2 pan { get; private set; }

        private Rect _viewport;

        public Rect viewport
        {
            get => _viewport;
            set
            {
                if (value == viewport)
                {
                    return;
                }

                _viewport = value;
                OnViewportChange();
            }
        }

        private float viewTweenDuration;

        private Vector2 viewTweenStartPan;

        private Vector2 viewTweenEndPan;

        private float viewTweenStartZoom;

        private float viewTweenEndZoom;

        private DateTime viewTweenStart;

        private bool isTweeningView;

        public Vector2 mousePosition { get; private set; }

        private void HandleMouseMovement()
        {
            if (isMouseOver)
            {
                mousePosition = e.mousePosition;
            }
        }

        public bool isMouseOver => viewport.Contains(e.mousePosition);

        public bool isMouseOverBackground => isMouseOver && hoveredWidget == null;

        private readonly RectOffset dragPanOffset = new RectOffset(20, 20, 20, 20);

        private bool isResizing
        {
            get
            {
                foreach (var widget in widgets)
                {
                    if ((widget as IGraphElementWidget)?.isResizing ?? false)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        protected virtual bool shouldEdgePan => isDragging || isLassoing || isResizing;

        private readonly WidgetList<IWidget> widgetsByAscendingZ;

        private readonly WidgetList<IWidget> visibleWidgetsByAscendingZ;

        private readonly WidgetList<IWidget> visibleWidgetsByDescendingZ;

        public void UpdateViewport()
        {
            if (isTweeningView)
            {
                var t = (float)(DateTime.Now - viewTweenStart).TotalSeconds / viewTweenDuration;

                if (t >= 1)
                {
                    isTweeningView = false;
                }
                else
                {
                    pan = Vector2.Lerp(viewTweenStartPan, viewTweenEndPan, t);
                    zoom = Mathf.Lerp(viewTweenStartZoom, viewTweenEndZoom, t);
                }
            }
            else
            {
                pan = graph.pan;
                zoom = graph.zoom;
            }
        }

        public void TweenViewport(Vector2 pan, float zoom, float duration)
        {
            if (context == null)
            {
                return;
            }

            isTweeningView = true;
            viewTweenDuration = duration;
            viewTweenStartPan = this.pan;
            viewTweenStartZoom = this.zoom;
            viewTweenEndPan = pan;
            viewTweenEndZoom = zoom;
            viewTweenStart = DateTime.Now;

            graph.pan = pan;
            graph.zoom = zoom;
        }

        public void CacheWidgetVisibility()
        {
            visibleWidgetsByAscendingZ.Clear();
            visibleWidgetsByDescendingZ.Clear();

            for (int i = 0; i < widgetsByAscendingZ.Count; i++)
            {
                var ascending = widgetsByAscendingZ[i];
                var descending = widgetsByAscendingZ[widgetsByAscendingZ.Count - i - 1];

                ascending.isVisible = IsVisible(ascending);
                descending.isVisible = IsVisible(descending);

                if (ascending.isVisible)
                {
                    visibleWidgetsByAscendingZ.Add(ascending);
                }

                if (descending.isVisible)
                {
                    visibleWidgetsByDescendingZ.Add(descending);
                }
            }
        }

        private void OnViewportChange()
        {
            foreach (var widget in widgets)
            {
                widget.OnViewportChange();
            }
        }

        private void HandleViewportInput()
        {
            if (isTweeningView)
            {
                return;
            }

            var controlScheme = BoltCore.Configuration.controlScheme;

            if (isMouseOver && e.controlType == EventType.ScrollWheel)
            {
                bool zooming;
                bool scrolling;

                if (controlScheme == CanvasControlScheme.Default)
                {
                    zooming = e.ctrlOrCmd;
                    scrolling = !zooming;
                }
                else if (controlScheme == CanvasControlScheme.Alternate)
                {
                    zooming = true;
                    scrolling = false;
                }
                else
                {
                    throw new UnexpectedEnumValueException<CanvasControlScheme>(controlScheme);
                }

                if (zooming)
                {
                    var zoomDelta = MathfEx.NearestMultiple(-e.mouseDelta.y * BoltCore.Configuration.zoomSpeed, GraphGUI.ZoomSteps);
                    zoomDelta = Mathf.Clamp(graph.zoom + zoomDelta, GraphGUI.MinZoom, GraphGUI.MaxZoom) - graph.zoom;

                    if (zoomDelta != 0)
                    {
                        var oldZoom = graph.zoom;
                        var newZoom = graph.zoom + zoomDelta;
                        var matrix = MathfEx.ScaleAroundPivot(mousePosition, (oldZoom / newZoom) * Vector3.one);
                        graph.pan = matrix.MultiplyPoint(graph.pan);
                        graph.zoom = newZoom;
                    }

                    HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.Zoom);

                    e.Use();
                }

                if (scrolling)
                {
                    var panDelta = e.mouseDelta * BoltCore.Configuration.panSpeed;

                    if (panDelta.magnitude != 0)
                    {
                        graph.pan += panDelta;
                    }

                    HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.Scroll);

                    e.Use();
                }
            }

            if (e.controlType == EventType.MouseDown || e.controlType == EventType.MouseDrag)
            {
                bool panning;

                if (controlScheme == CanvasControlScheme.Default)
                {
                    panning = e.mouseButton == MouseButton.Middle;
                }
                else if (controlScheme == CanvasControlScheme.Alternate)
                {
                    panning = (e.mouseButton == MouseButton.Middle) || (e.alt && e.mouseButton == (int)MouseButton.Left);
                }
                else
                {
                    throw new UnexpectedEnumValueException<CanvasControlScheme>(controlScheme);
                }

                if (panning)
                {
                    var panDelta = -e.mouseDelta;

                    if (panDelta.magnitude != 0)
                    {
                        graph.pan += panDelta;
                    }

                    if (e.mouseButton == MouseButton.Middle)
                        HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.PanMmb);
                    if (e.alt && e.mouseButton == (int)MouseButton.Left)
                        HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.PanAltLmb);

                    e.Use();
                }
            }

            if (e.IsKeyDown(KeyCode.Home))
            {
                ViewSelection();
                HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.Home);
                e.Use();
            }
        }

        private void HandleEdgePanning()
        {
            if (isTweeningView || !window.IsFocused() || !shouldEdgePan)
            {
                return;
            }

            var mousePosition = e.mousePosition;

            var dragSpeedMultiplier = 120 * BoltCore.Configuration.dragPanSpeed;

            var dragLeftSpeed = 0f;
            var dragRightSpeed = 0f;
            var dragTopSpeed = 0f;
            var dragBottomSpeed = 0f;

            if (mousePosition.x < viewport.x)
            {
                dragLeftSpeed = 3;
            }
            else if (mousePosition.x < viewport.x + dragPanOffset.left)
            {
                dragLeftSpeed = 1;
            }

            if (mousePosition.x > viewport.xMax)
            {
                dragRightSpeed = 3;
            }
            else if (mousePosition.x > viewport.xMax - dragPanOffset.right)
            {
                dragRightSpeed = 1;
            }

            if (mousePosition.y < viewport.y)
            {
                dragTopSpeed = 3;
            }
            else if (mousePosition.y < viewport.y + dragPanOffset.top)
            {
                dragTopSpeed = 1;
            }

            if (mousePosition.y > viewport.yMax)
            {
                dragBottomSpeed = 3;
            }
            else if (mousePosition.y > viewport.yMax - dragPanOffset.bottom)
            {
                dragBottomSpeed = 1;
            }

            var delta = new Vector2(dragRightSpeed - dragLeftSpeed, dragBottomSpeed - dragTopSpeed) * eventDeltaTime * dragSpeedMultiplier;

            graph.pan += delta;

            if (isDragging)
            {
                foreach (var draggedElement in dragGroup)
                {
                    using (LudiqGUI.matrix.Override(Matrix4x4.identity))
                    {
                        this.Widget(draggedElement).CachePosition();
                    }

                    this.Widget(draggedElement).Drag(delta, dragConstraint);
                }
            }
        }

        private void Overview()
        {
            ViewElements(graph.elements);
        }

        private void ViewSelection()
        {
            ViewElements(selection.Count > 0 ? (IEnumerable<IGraphElement>)selection : graph.elements);
        }

        public void ViewElements(IEnumerable<IGraphElement> elements)
        {
            var view = GraphGUI.CalculateArea(elements.Select(e => this.Widget(e)));

            var pan = view.center;

            var padding = 50;

            var zoomX = (viewport.width * graph.zoom) / (view.width + padding);
            var zoomY = (viewport.height * graph.zoom) / (view.height + padding);
            var zoom = Mathf.Clamp(Mathf.Min(zoomX, zoomY), GraphGUI.MinZoom, GraphGUI.MaxZoom);

            TweenViewport(pan, zoom, BoltCore.Configuration.overviewSmoothing);
        }

        public bool IsVisible(IWidget widget)
        {
            return !widget.canClip || viewport.Overlaps(widget.clippingPosition);
        }

        #endregion


        #region Positioning

        private readonly List<IWidget> widgetsToReposition = new List<IWidget>();

        public void CacheWidgetPositions()
        {
            widgetsToReposition.Clear();

            widgetsToReposition.AddRange(widgets.Where(widget => !widget.isPositionValid).OrderByDependencies(widget => widget.positionDependencies));

            using (LudiqGUI.matrix.Override(Matrix4x4.identity))
            {
                foreach (var widget in widgetsToReposition)
                {
                    widget.CachePositionFirstPass();
                }

                foreach (var widget in widgetsToReposition)
                {
                    widget.CachePosition();
                    widget.isPositionValid = true;
                }
            }
        }

        #endregion


        #region Hovering

        public IWidget hoveredWidget { get; private set; }

        private void DetermineHoveredWidget()
        {
            hoveredWidget = null;

            foreach (var widget in visibleWidgetsByDescendingZ)
            {
                if (widget.isMouseThrough)
                {
                    hoveredWidget = widget;

                    break;
                }
            }
        }

        #endregion


        #region Lassoing

        private Vector2 lassoOrigin;

        public bool isLassoing { get; private set; }

        public Rect lassoArea
        {
            get
            {
                return new Rect()
                {
                    xMin = Mathf.Min(lassoOrigin.x, mousePosition.x),
                    xMax = Mathf.Max(lassoOrigin.x, mousePosition.x),
                    yMin = Mathf.Min(lassoOrigin.y, mousePosition.y),
                    yMax = Mathf.Max(lassoOrigin.y, mousePosition.y),
                };
            }
        }

        private void HandleLassoing()
        {
            if (e.IsMouseDrag(MouseButton.Left))
            {
                if (!isLassoing)
                {
                    lassoOrigin = mousePosition;
                    isLassoing = true;
                }
            }
            else if (e.IsMouseUp(MouseButton.Left))
            {
                if (isSelecting)
                {
                    isLassoing = false;
                }

                if (isGrouping)
                {
                    isLassoing = false;

                    var group = new GraphGroup() { position = groupArea };
                    UndoUtility.RecordEditedObject("Create Graph Group");
                    graph.elements.Add(group);
                    selection.Select(group);
                    this.Widget<GraphGroupWidget>(group).FocusLabel();
                    GUI.changed = true;
                }
            }
        }

        #endregion


        #region Selecting

        public bool isSelecting => isLassoing && !isGrouping;

        public Rect selectionArea => lassoArea;

        private void HandleSelecting()
        {
            if (e.IsValidateCommand("SelectAll"))
            {
                e.ValidateCommand();
            }
            else if (e.IsExecuteCommand("SelectAll"))
            {
                HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.SelectAll);

                selection.Select(elementWidgets.Where(widget => widget.canSelect).Select(widget => widget.element));
                e.Use();
            }
            else if (e.IsMouseDown(MouseButton.Left))
            {
                selection.Clear();
            }
            else if (e.IsMouseDrag(MouseButton.Left))
            {
                if (isSelecting)
                {
                    // In selection mode, include any widget that overlaps the lasso
                    selection.Select(elementWidgets.Where(widget => widget.canSelect && selectionArea.Overlaps(widget.hotArea)).Select(widget => widget.element));
                    e.Use();
                }
                else if (isGrouping)
                {
                    // In grouping mode, only inlude widgets that are encompassed by the lasso
                    selection.Select(elementWidgets.Where(widget => widget.canSelect && groupArea.Encompasses(widget.position)).Select(widget => widget.element));
                    e.Use();
                }
            }
        }

        #endregion


        #region Grouping

        public bool isGrouping => isLassoing && (e.ctrlOrCmd && graph.elements.Includes<GraphGroup>());

        public Rect groupArea
        {
            get
            {
                var groupBackground = GraphGroupWidget.Styles.group.normal.background;

                var groupRect = selectionArea;

                groupRect.y -= GraphGroupWidget.Styles.headerHeight;
                groupRect.height += GraphGroupWidget.Styles.headerHeight;

                if (groupRect.width < groupBackground.width)
                {
                    groupRect.width = groupBackground.width;
                }

                if (groupRect.height < groupBackground.height)
                {
                    groupRect.height = groupBackground.height;
                }

                return groupRect;
            }
        }

        #endregion


        #region Dragging

        private static readonly Vector2 HorizontalDrag = new Vector2(1, 0);

        private static readonly Vector2 VerticalDrag = new Vector2(0, 1);

        private static readonly Vector2 FreeDrag = new Vector2(1, 1);

        private Vector2 dragConstraint = FreeDrag;

        public bool isDragging { get; private set; }

        private readonly HashSet<IGraphElement> dragGroup = new HashSet<IGraphElement>();

        public void BeginDrag(EventWrapper e)
        {
            if (isDragging)
            {
                return;
            }

            UndoUtility.RecordEditedObject("Drag Graph Elements");

            isDragging = true;

            dragGroup.Clear();

            dragGroup.UnionWith(selection);

            foreach (var selected in selection)
            {
                this.Widget(selected).ExpandDragGroup(dragGroup);
            }

            LockDragOrigin();

            foreach (var dragged in dragGroup)
            {
                this.Widget(dragged).BeginDrag();
            }

            e.Use();
        }

        public void Drag(EventWrapper e)
        {
            if (!isDragging)
            {
                return;
            }

            if (e.shift)
            {
                if (dragConstraint == FreeDrag)
                {
                    LockDragOrigin();

                    if (Mathf.Abs(e.mouseDelta.x) > Mathf.Abs(e.mouseDelta.y))
                    {
                        dragConstraint = HorizontalDrag;
                    }
                    else if (Mathf.Abs(e.mouseDelta.y) > Mathf.Abs(e.mouseDelta.x))
                    {
                        dragConstraint = VerticalDrag;
                    }
                }
            }
            else
            {
                dragConstraint = FreeDrag;
            }

            foreach (var dragged in dragGroup)
            {
                this.Widget(dragged).Drag(e.mouseDelta, dragConstraint);
            }

            e.Use();
        }

        public void EndDrag(EventWrapper e)
        {
            if (!isDragging)
            {
                return;
            }

            UndoUtility.RecordEditedObject("Drag Graph Elements");

            isDragging = false;

            foreach (var dragged in dragGroup)
            {
                this.Widget(dragged).EndDrag();
            }

            dragGroup.Clear();

            dragConstraint = FreeDrag;

            e.Use();
        }

        private void LockDragOrigin()
        {
            foreach (var dragged in dragGroup)
            {
                this.Widget(dragged).LockDragOrigin();
            }
        }

        #endregion


        #region Deleting

        public void DeleteSelection()
        {
            var deleted = false;

            var deleteGroup = new HashSet<IGraphElement>();

            foreach (var element in selection)
            {
                deleteGroup.Add(element);
                this.Widget(element).ExpandDeleteGroup(deleteGroup);
            }

            foreach (var element in deleteGroup.OrderByDescending(e => e.dependencyOrder))
            {
                if (this.Widget(element).canDelete)
                {
                    UndoUtility.RecordEditedObject("Delete Graph Element");
                    graph.elements.Remove(element);
                    selection.Remove(element);
                    deleted = true;
                }
            }

            if (deleted)
            {
                GUI.changed = true;
                e.TryUse();
            }
        }

        private void HandleDeleting()
        {
            if (e.IsValidateCommand("Delete"))
            {
                if (selection.Count > 0)
                {
                    e.ValidateCommand();
                }
            }
            else if (e.IsExecuteCommand("Delete"))
            {
                HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.Delete);

                DeleteSelection();
            }
            else if (e.IsKeyDown(KeyCode.Delete))
            {
                DeleteSelection();
            }
        }

        #endregion


        #region Clipboard

        public virtual void ShrinkCopyGroup(HashSet<IGraphElement> copyGroup) { }

        private DateTime lastPasteTime;

        private void HandleClipboard()
        {
            if (e.IsValidateCommand("Copy"))
            {
                if (GraphClipboard.canCopySelection)
                {
                    e.ValidateCommand();
                }
            }
            else if (e.IsExecuteCommand("Copy"))
            {
                HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.Copy);

                GraphClipboard.CopySelection();
            }

            if (e.IsValidateCommand("Cut"))
            {
                if (GraphClipboard.canCopySelection)
                {
                    e.ValidateCommand();
                }
            }
            else if (e.IsExecuteCommand("Cut"))
            {
                HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.Cut);

                GraphClipboard.CutSelection();
            }

            if (e.IsValidateCommand("Paste"))
            {
                if (GraphClipboard.canPaste && (DateTime.Now - lastPasteTime).TotalSeconds >= 0.25)
                {
                    e.ValidateCommand();
                }
            }
            else if (e.IsExecuteCommand("Paste"))
            {
                HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.Paste);

                GraphClipboard.Paste();
                lastPasteTime = DateTime.Now;
            }

            if (e.IsValidateCommand("Duplicate"))
            {
                if (GraphClipboard.canDuplicateSelection && (DateTime.Now - lastPasteTime).TotalSeconds >= 0.25)
                {
                    e.Use();
                }
            }
            else if (e.IsExecuteCommand("Duplicate"))
            {
                HotkeyUsageAnalytics.HotkeyUsed(HotkeyUsageAnalytics.Hotkey.Duplicate);

                GraphClipboard.DuplicateSelection();
                lastPasteTime = DateTime.Now;
            }
        }

        #endregion


        #region Context

        protected IEnumerable<DropdownOption> GetExtendedContextOptions()
        {
            foreach (var item in context.extensions.SelectMany(extension => extension.contextMenuItems).OrderBy(item => item.label))
            {
                yield return new DropdownOption(item.action, item.label);
            }
        }

        protected virtual IEnumerable<DropdownOption> GetContextOptions()
        {
            foreach (var extendedOption in GetExtendedContextOptions())
            {
                yield return extendedOption;
            }

            if (GraphClipboard.canCopySelection)
            {
                yield return new DropdownOption((Action)GraphClipboard.CopySelection, "Copy Selection");
                yield return new DropdownOption((Action)GraphClipboard.CutSelection, "Cut Selection");
            }

            if (GraphClipboard.canDuplicateSelection)
            {
                yield return new DropdownOption((Action)GraphClipboard.DuplicateSelection, "Duplicate Selection");
            }

            if (selection.Count > 0)
            {
                yield return new DropdownOption((Action)DeleteSelection, "Delete Selection");
            }

            if (GraphClipboard.canPasteOutside)
            {
                yield return new DropdownOption((Action<Vector2>)((position) => GraphClipboard.PasteOutside(true, position)), "Paste");
            }
        }

        protected virtual void OnContext()
        {
            var contextOptions = this.GetContextOptions().ToArray();

            delayCall += () =>
            {
                var _mousePosition = mousePosition;

                LudiqGUI.Dropdown
                    (
                        e.mousePosition,
                        delegate (object _action)
                        {
                            delayCall += () =>
                            {
                                if (_action is Action action)
                                {
                                    action.Invoke();
                                }
                                else if (_action is Action<Vector2> positionedAction)
                                {
                                    positionedAction.Invoke(_mousePosition);
                                }
                            };
                        },
                        contextOptions,
                        null
                    );
            };
        }

        private void HandleContext()
        {
            if (e.IsContextClick)
            {
                OnContext();
                e.Use();
            }
        }

        #endregion


        #region Layout

        public IEnumerable<IGraphElementWidget> alignableAndDistributable
        {
            get
            {
                // [BOLT-1112]
                // Filter elements with a null graph reference as deserialization may occurs while parsing the list.
                // After deserialization elements are cleaned up before rebuilding the graph.
                // see Graph.OnAfterDependenciesDeserialized
                return selection
                    .Where(element => element.graph != null)
                    .Select(this.Widget)
                    .Where(element => element.canAlignAndDistribute);
            }
        }

        public void Align(AlignOperation operation)
        {
            UndoUtility.RecordEditedObject("Align Graph Elements");

            var alignable = alignableAndDistributable;

            var area = GraphGUI.CalculateArea(alignable);

            foreach (var widget in alignable)
            {
                var position = widget.position;
                var width = position.width;
                var height = position.height;

                switch (operation)
                {
                    case AlignOperation.AlignLeftEdges:
                        position.xMin = area.xMin;

                        break;

                    case AlignOperation.AlignCenters:
                        position.xMin = area.xMin + (area.width / 2) - (width / 2);

                        break;

                    case AlignOperation.AlignRightEdges:
                        position.xMin = area.xMax - width;

                        break;

                    case AlignOperation.AlignTopEdges:
                        position.yMin = area.yMin;

                        break;

                    case AlignOperation.AlignMiddles:
                        position.yMin = area.yMin + (area.height / 2) - (height / 2);

                        break;

                    case AlignOperation.AlignBottomEdges:
                        position.yMin = area.yMax - height;

                        break;
                }

                position.width = width;
                position.height = height;
                widget.position = position.PixelPerfect();
                widget.Reposition();
            }
        }

        public void Distribute(DistributeOperation operation)
        {
            UndoUtility.RecordEditedObject("Distribute Graph Elements");

            var distributable = alignableAndDistributable;

            switch (operation)
            {
                case DistributeOperation.DistributeLeftEdges:
                    distributable = distributable.OrderBy(editor => editor.position.xMin);

                    break;

                case DistributeOperation.DistributeCenters:
                    distributable = distributable.OrderBy(editor => editor.position.center.x);

                    break;

                case DistributeOperation.DistributeRightEdges:
                    distributable = distributable.OrderBy(editor => editor.position.xMax);

                    break;

                case DistributeOperation.DistributeHorizontalGaps:
                    distributable = distributable.OrderBy(editor => editor.position.xMin);

                    break;

                case DistributeOperation.DistributeTopEdges:
                    distributable = distributable.OrderBy(editor => editor.position.yMin);

                    break;

                case DistributeOperation.DistributeMiddles:
                    distributable = distributable.OrderBy(editor => editor.position.center.y);

                    break;

                case DistributeOperation.DistributeBottomEdges:
                    distributable = distributable.OrderBy(editor => editor.position.yMax);

                    break;

                case DistributeOperation.DistributeVerticalGaps:
                    distributable = distributable.OrderBy(editor => editor.position.yMin);

                    break;
            }

            var index = 0;
            var count = distributable.Count();
            var first = distributable.First();
            var last = distributable.Last();
            var currentX = first.position.xMin;
            var currentY = first.position.yMin;
            var innerTotalWidth = distributable.Skip(1).Take(count - 2).Sum(innerEditor => innerEditor.position.width);
            var innerTotalHeight = distributable.Skip(1).Take(count - 2).Sum(innerEditor => innerEditor.position.height);
            var horizontalGap = ((last.position.xMin - first.position.xMax) - innerTotalWidth) / (count - 1);
            var verticalGap = ((last.position.yMin - first.position.yMax) - innerTotalHeight) / (count - 1);

            foreach (var widget in distributable)
            {
                var ratio = (float)index / (count - 1);

                var position = widget.position;
                var width = position.width;
                var height = position.height;

                switch (operation)
                {
                    case DistributeOperation.DistributeLeftEdges:
                        position.xMin = first.position.xMin + (ratio * (last.position.xMin - first.position.xMin));
                        position.xMax = position.xMin + width;

                        break;

                    case DistributeOperation.DistributeCenters:
                        position.xMin = first.position.center.x + (ratio * (last.position.center.x - first.position.center.x)) - (width / 2);
                        position.xMax = position.xMin + width;

                        break;

                    case DistributeOperation.DistributeRightEdges:
                        position.xMax = last.position.xMax - ((1 - ratio) * (last.position.xMax - first.position.xMax));
                        position.xMin = position.xMax - width;

                        break;

                    case DistributeOperation.DistributeHorizontalGaps:
                        position.xMin = currentX;
                        position.xMax = position.xMin + width;
                        currentX = position.xMax + horizontalGap;

                        break;

                    case DistributeOperation.DistributeTopEdges:
                        position.yMin = first.position.yMin + (ratio * (last.position.yMin - first.position.yMin));
                        position.yMax = position.yMin + height;

                        break;

                    case DistributeOperation.DistributeMiddles:
                        position.yMin = first.position.center.y + (ratio * (last.position.center.y - first.position.center.y)) - (height / 2);
                        position.yMax = position.yMin + height;

                        break;

                    case DistributeOperation.DistributeBottomEdges:
                        position.yMax = last.position.yMax - ((1 - ratio) * (last.position.yMax - first.position.yMax));
                        position.yMin = position.yMax - height;

                        break;

                    case DistributeOperation.DistributeVerticalGaps:
                        position.yMin = currentY;
                        position.yMax = position.yMin + height;
                        currentY = position.yMax + verticalGap;

                        break;
                }

                widget.position = position.PixelPerfect();
                widget.Reposition();

                index++;
            }
        }

        #endregion


        #region Drawing

        protected virtual void DrawBackground()
        {
            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                if (e.controlsMouse && e.controlsKeyboard)
                {
                    EditorGUI.DrawRect(viewport, Color.cyan.WithAlpha(0.25f));
                }
                else if (e.controlsMouse)
                {
                    EditorGUI.DrawRect(viewport, Color.green.WithAlpha(0.25f));
                }
                else if (e.controlsKeyboard)
                {
                    EditorGUI.DrawRect(viewport, Color.blue.WithAlpha(0.25f));
                }
            }
        }

        protected void DrawWidgetsBackground()
        {
            foreach (var widget in visibleWidgetsByAscendingZ)
            {
                if (e.IsRepaint || widget.backgroundRequiresInput)
                {
                    widget.DrawBackground();
                }
            }
        }

        protected void DrawWidgetsForeground()
        {
            foreach (var widget in visibleWidgetsByAscendingZ)
            {
                if (e.IsRepaint || widget.foregroundRequiresInput)
                {
                    widget.DrawForeground();
                }
            }
        }

        protected void DrawWidgetsOverlay()
        {
            foreach (var widget in visibleWidgetsByAscendingZ)
            {
                if (e.IsRepaint || widget.overlayRequiresInput)
                {
                    widget.DrawOverlay();
                }
            }
        }

        protected virtual void DrawOverlay()
        {
            if (isSelecting)
            {
                if (e.IsRepaint)
                {
                    ((GUIStyle)"SelectionRect").Draw(selectionArea, false, false, false, false);
                }
            }

            if (isGrouping)
            {
                if (e.IsRepaint)
                {
                    using (LudiqGUI.color.Override(GraphGroupWidget.Styles.AdjustColor(GraphGroup.defaultColor, true)))
                    {
                        GraphGroupWidget.Styles.group.Draw(groupArea, false, false, true, false);
                    }
                }
            }

            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                EditorGUI.DrawRect(new Rect(graph.pan.x, graph.pan.y - 2, 1, 5), Color.white);
                EditorGUI.DrawRect(new Rect(graph.pan.x - 2, graph.pan.y, 5, 1), Color.white);
                GUI.Label(new Rect(graph.pan, new Vector2(300, 48)), graph.pan.ToString("0"), EditorStyles.whiteLabel);
                GUI.Label(new Rect(mousePosition + new Vector2(16, 0), new Vector2(300, 16)), mousePosition.ToString("0"), EditorStyles.whiteLabel);
                GUI.Label(new Rect(mousePosition + new Vector2(16, 16), new Vector2(300, 16)), GUIUtility.GUIToScreenPoint(mousePosition).ToString("0"), EditorStyles.whiteLabel);
                GUI.Label(new Rect(mousePosition + new Vector2(16, 32), new Vector2(300, 16)), $"Widgets: {widgets.Where(IsVisible).Count()} / {widgets.Count()}", EditorStyles.whiteLabel);

                if (selection.Count > 0)
                {
                    var selectionRect = GraphGUI.CalculateArea(selection.Select(e => this.Widget(e)));
                    LudiqGUI.DrawEmptyRect(selectionRect, Color.cyan.WithAlpha(0.5f));
                    GUI.Label(new Rect(selectionRect.center, new Vector2(300, 16)), selectionRect.center.ToString(), EditorStyles.whiteLabel);
                }
            }
        }

        #endregion


        #region Drag & Drop

        private IEnumerable<IDragAndDropHandler> potentialDragAndDropHandlers
        {
            get
            {
                if (hoveredWidget != null && hoveredWidget is IDragAndDropHandler)
                {
                    yield return (IDragAndDropHandler)hoveredWidget;
                }

                yield return this;

                foreach (var extension in context.extensions)
                {
                    yield return extension;
                }
            }
        }

        private IDragAndDropHandler dragAndDropHandler { get; set; }

        private void HandleDragAndDrop()
        {
            if (e.controlType == EventType.DragPerform || e.controlType == EventType.DragUpdated)
            {
                foreach (var potentialDragAndDropHandler in potentialDragAndDropHandlers)
                {
                    if (potentialDragAndDropHandler.AcceptsDragAndDrop())
                    {
                        dragAndDropHandler = potentialDragAndDropHandler;

                        break;
                    }
                }

                if (dragAndDropHandler != null)
                {
                    DragAndDrop.visualMode = dragAndDropHandler.dragAndDropVisualMode;

                    if (e.controlType == EventType.DragPerform)
                    {
                        dragAndDropHandler.PerformDragAndDrop();

                        // Don't wait another frame (shows visually), exit right away
                        dragAndDropHandler.ExitDragAndDrop();
                        dragAndDropHandler = null;
                    }
                    else if (e.controlType == EventType.DragUpdated)
                    {
                        dragAndDropHandler.UpdateDragAndDrop();
                    }
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }

                e.Use();
            }

            if (e.controlType == EventType.DragExited && dragAndDropHandler != null)
            {
                dragAndDropHandler.ExitDragAndDrop();
                dragAndDropHandler = null;
                e.Use();
            }
        }

        public virtual DragAndDropVisualMode dragAndDropVisualMode => DragAndDropVisualMode.Generic;

        public virtual bool AcceptsDragAndDrop()
        {
            return false;
        }

        public virtual void PerformDragAndDrop() { }

        public virtual void UpdateDragAndDrop() { }

        public virtual void DrawDragAndDropPreview() { }

        public virtual void ExitDragAndDrop() { }

        #endregion


        #region Timing

        private DateTime lastFrameTime;

        private DateTime lastEventTime;

        private DateTime lastRepaintTime;

        [DoNotSerialize]
        public float frameDeltaTime => (float)(DateTime.Now - lastFrameTime).TotalSeconds;

        [DoNotSerialize]
        public float eventDeltaTime => (float)(DateTime.Now - lastEventTime).TotalSeconds;

        [DoNotSerialize]
        public float repaintDeltaTime => (float)(DateTime.Now - lastRepaintTime).TotalSeconds;

        #endregion


        #region Window

        public virtual void OnToolbarGUI()
        {
            EditorGUI.BeginChangeCheck();

            BoltCore.Configuration.carryChildren = GUILayout.Toggle(BoltCore.Configuration.carryChildren, "Carry", LudiqStyles.toolbarButton);

            if (EditorGUI.EndChangeCheck())
            {
                BoltCore.Configuration.Save();
            }

            EditorGUI.BeginDisabledGroup(alignableAndDistributable.Count() < 2);

            var alignPosition = GUILayoutUtility.GetRect(new GUIContent("Align"), LudiqStyles.toolbarPopup);

            if (GUI.Button(alignPosition, "Align", LudiqStyles.toolbarPopup))
            {
                LudiqGUI.FuzzyDropdown
                    (
                        alignPosition,
                        EnumOptionTree.For<AlignOperation>(),
                        (object)null,
                        (operation) => Align((AlignOperation)operation)
                    );
            }

            var distributePosition = GUILayoutUtility.GetRect(new GUIContent("Distribute"), LudiqStyles.toolbarPopup);

            if (GUI.Button(distributePosition, "Distribute", LudiqStyles.toolbarPopup))
            {
                LudiqGUI.FuzzyDropdown
                    (
                        distributePosition,
                        EnumOptionTree.For<DistributeOperation>(),
                        (object)null,
                        (operation) => Distribute((DistributeOperation)operation)
                    );
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Overview", LudiqStyles.toolbarButton))
            {
                Overview();
            }

            if (GUILayout.Toggle(window.maximized, "Full Screen", LudiqStyles.toolbarButton) != window.maximized)
            {
                ToggleMaximized();
            }

            if (BoltCore.Configuration.developerMode)
            {
                EditorGUI.BeginChangeCheck();

                BoltCore.Configuration.debug = GUILayout.Toggle(BoltCore.Configuration.debug, "Debug", LudiqStyles.toolbarButton);

                if (EditorGUI.EndChangeCheck())
                {
                    BoltCore.Configuration.Save();
                }
            }
        }

        public Queue<Action> delayedCalls { get; } = new Queue<Action>();

        public event Action delayCall
        {
            add
            {
                lock (delayedCalls)
                {
                    delayedCalls.Enqueue(value);
                }
            }
            remove { }
        }

        #endregion
    }
}
