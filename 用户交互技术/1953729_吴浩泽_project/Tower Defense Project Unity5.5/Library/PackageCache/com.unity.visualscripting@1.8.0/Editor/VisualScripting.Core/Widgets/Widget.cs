using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public abstract class Widget<TCanvas, TItem> : IWidget
        where TCanvas : class, ICanvas
        where TItem : class, IGraphItem
    {
        #region Context Shortcuts

        protected IGraphContext context => LudiqGraphsEditorUtility.editedContext.value;

        protected GraphReference reference => context.reference;

        protected IGraph graph => canvas.graph;

        protected TCanvas canvas { get; }

        ICanvas IWidget.canvas => canvas;

        protected GraphSelection selection => canvas.selection;

        protected ICanvasWindow window => canvas.window;

        protected Vector2 mousePosition => canvas.mousePosition;

        #endregion

        protected Widget(TCanvas canvas, TItem item)
        {
            Ensure.That(nameof(canvas)).IsNotNull(canvas);
            Ensure.That(nameof(item)).IsNotNull(item);

            this.canvas = canvas;

            this.item = item;

            e = new EventWrapper(GetType());

            // Micro optimization
            hasDescriptor = item.HasDescriptor();

            if (hasDescriptor)
            {
                DescriptorProvider.instance.AddListener(item, CacheDescription);
            }
        }

        public bool disposed { get; private set; }

        public virtual void Dispose()
        {
            if (hasDescriptor)
            {
                DescriptorProvider.instance.RemoveListener(item, CacheDescription);
            }

            disposed = true;
        }

        protected EventWrapper e { get; }

        public override string ToString()
        {
            return GetType().Name + "\nCID: " + e.control;
        }

        public virtual IEnumerable<IWidget> subWidgets => Enumerable.Empty<IWidget>();

        protected void SubWidgetsChanged()
        {
            canvas.Recollect();
        }

        #region Model

        protected TItem item { get; }

        IGraphItem IWidget.item => item;

        protected readonly bool hasDescriptor;

        public Metadata metadata { get; private set; }

        private Metadata lastFetchedMetadata;

        private bool cachedItemOnce = false;

        public virtual Metadata FetchMetadata()
        {
            return null;
        }

        protected virtual void CacheItemFirstTime()
        {
            CacheDescription();
        }

        public virtual void CacheItem()
        {
            if (!cachedItemOnce)
            {
                CacheItemFirstTime();
                cachedItemOnce = true;
            }

            // Note: on UnitPortWidget, FetchMetadata depends on the description, so it's important to validate it first.

            if (hasDescriptor)
            {
                item.Descriptor().Validate();
            }

            metadata = FetchMetadata();

            if (metadata != lastFetchedMetadata)
            {
                lastFetchedMetadata = metadata;
                CacheMetadata();
            }
        }

        protected virtual void CacheMetadata() { }

        protected virtual void CacheDescription() { }

        #endregion


        #region Lifecycle

        public void RegisterControl()
        {
            e.RegisterControl(FocusType.Passive);
        }

        public virtual void BeforeFrame()
        {
            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                debug = $"CID: {e.control}";
            }

            if (isVisible || !dimInitialized)
            {
                UpdateDim();
            }
        }

        public virtual void HandleCapture()
        {
            e.HandleCapture(isMouseOver, false);
        }

        public virtual void HandleInput()
        {
            HandleContext();
        }

        public virtual void HandleRelease()
        {
            e.HandleRelease();
        }

        public virtual void Update() { }

        #endregion


        #region Positioning

        protected virtual bool snapToGrid => false;

        public bool isPositionValid { get; set; }

        public virtual IEnumerable<IWidget> positionDependers => Enumerable.Empty<IWidget>();

        public virtual IEnumerable<IWidget> positionDependencies => Enumerable.Empty<IWidget>();

        public abstract Rect position { get; set; }

        public abstract float zIndex { get; set; }

        public void Reposition()
        {
            isPositionValid = false;

            foreach (var positionDepender in positionDependers)
            {
                positionDepender.Reposition();
            }
        }

        public virtual void CachePositionFirstPass() { }

        public virtual void CachePosition() { }

        public void BringToFront()
        {
            var frontmostWidget = canvas.widgets.OrderByDescending(widget => widget.zIndex).FirstOrDefault();

            if (frontmostWidget != null)
            {
                zIndex = frontmostWidget.zIndex + 1;
            }
        }

        public void SendToBack()
        {
            var backmostWidget = canvas.widgets.OrderBy(widget => widget.zIndex).FirstOrDefault();

            if (backmostWidget != null)
            {
                zIndex = backmostWidget.zIndex - 1;
            }
        }

        #endregion


        #region Viewing

        public virtual bool canClip => !e.controlsMouse;

        public virtual Rect clippingPosition => position;

        public virtual void OnViewportChange() { }

        public bool isVisible { get; set; }

        #endregion


        #region Hovering

        public virtual Rect hotArea => position;

        public bool isMouseThrough => hotArea.Contains(mousePosition);

        public bool isMouseOver => canvas.hoveredWidget == this;

        #endregion


        #region Context

        protected virtual void OnContext()
        {
            var contextOptions = this.contextOptions.ToArray();

            canvas.delayCall += () =>
            {
                LudiqGUI.Dropdown
                    (
                        e.mousePosition,
                        action => canvas.delayCall += (Action)action,
                        contextOptions,
                        null
                    );
            };
        }

        protected void HandleContext()
        {
            if (e.IsContextClick)
            {
                OnContext();
                e.Use();
            }
        }

        protected virtual IEnumerable<DropdownOption> contextOptions
        {
            get
            {
                yield break;
            }
        }

        #endregion


        #region Drawing

        protected string debug { get; set; }

        public virtual bool foregroundRequiresInput => false;

        public virtual bool backgroundRequiresInput => false;

        public virtual bool overlayRequiresInput => false;

        public virtual void DrawForeground()
        {
            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug && !string.IsNullOrEmpty(debug))
            {
                var debugContent = new GUIContent(debug);

                var debugLabelPosition = new Rect
                    (
                    position.x,
                    position.yMax + 5,
                    EditorStyles.whiteMiniLabel.CalcSize(debugContent).x,
                    EditorStyles.whiteMiniLabel.CalcSize(debugContent).y
                    );

                GUI.Label(debugLabelPosition, debugContent, EditorStyles.whiteMiniLabel);
            }
        }

        public virtual void DrawBackground()
        {
            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                EditorGUI.DrawRect(clippingPosition, new Color(0, 0, 0, 0.1f));
            }
        }

        public virtual void DrawOverlay()
        {
            if (BoltCore.Configuration.developerMode && BoltCore.Configuration.debug)
            {
                if (e.controlsMouse && e.controlsKeyboard)
                {
                    EditorGUI.DrawRect(position, Color.cyan.WithAlpha(0.25f));
                }
                else if (e.controlsMouse)
                {
                    EditorGUI.DrawRect(position, Color.green.WithAlpha(0.25f));
                }
                else if (e.controlsKeyboard)
                {
                    EditorGUI.DrawRect(position, Color.blue.WithAlpha(0.25f));
                }
                else if (isMouseOver)
                {
                    EditorGUI.DrawRect(position, Color.yellow.WithAlpha(0.25f));
                }
            }
        }

        #endregion


        #region Dimming

        private float dimAlpha = 1;

        private float dimTarget = 1;

        private bool dimInitialized = false;

        private const float dimFadeDuration = 0.075f;

        protected virtual bool dim => false;

        protected void BeginDim()
        {
            LudiqGUI.color.BeginOverride(LudiqGUI.color.value.WithAlphaMultiplied(dimAlpha));
        }

        protected void EndDim()
        {
            LudiqGUI.color.EndOverride();
        }

        protected void UpdateDim()
        {
            dimTarget = dim ? GraphGUI.Styles.dimAlpha : 1;

            if (!dimInitialized)
            {
                dimAlpha = dimTarget;
                dimInitialized = true;
            }

            dimAlpha = Mathf.MoveTowards(dimAlpha, dimTarget, (1 / dimFadeDuration) * canvas.repaintDeltaTime);
        }

        #endregion
    }
}
