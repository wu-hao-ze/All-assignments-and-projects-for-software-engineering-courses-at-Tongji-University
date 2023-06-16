using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IWidget : IDisposable
    {
        #region Model

        ICanvas canvas { get; }

        IEnumerable<IWidget> subWidgets { get; }

        IGraphItem item { get; }

        Metadata metadata { get; }

        void CacheItem();

        #endregion


        #region Lifecycle

        bool foregroundRequiresInput { get; }

        bool backgroundRequiresInput { get; }

        bool overlayRequiresInput { get; }

        void RegisterControl();

        void Update();

        void BeforeFrame();

        void HandleCapture();

        void HandleInput();

        void HandleRelease();

        #endregion


        #region Positioning

        Rect position { get; set; }

        IEnumerable<IWidget> positionDependencies { get; }

        IEnumerable<IWidget> positionDependers { get; }

        bool isPositionValid { get; set; }

        void Reposition();

        void CachePositionFirstPass();

        void CachePosition();

        float zIndex { get; set; }

        void BringToFront();

        void SendToBack();

        #endregion


        #region Viewing

        bool canClip { get; }

        Rect clippingPosition { get; }

        void OnViewportChange();

        bool isVisible { get; set; }

        #endregion


        #region Hovering

        Rect hotArea { get; }

        bool isMouseThrough { get; }

        bool isMouseOver { get; }

        #endregion


        #region Drawing

        void DrawForeground();

        void DrawBackground();

        void DrawOverlay();

        #endregion
    }
}
