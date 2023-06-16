using System.Collections.Generic;
using UnityEngine;

namespace Unity.VisualScripting
{
    public interface IGraphElementWidget : IWidget
    {
        IGraphElement element { get; }

        #region Selecting

        bool canSelect { get; }

        bool isSelected { get; }

        #endregion

        #region Layouting

        bool canAlignAndDistribute { get; }

        #endregion

        #region Clipboard

        void ExpandCopyGroup(HashSet<IGraphElement> group);

        #endregion

        #region Resizing

        bool canResizeHorizontal { get; }
        bool canResizeVertical { get; }
        bool isResizing { get; }

        #endregion

        #region Dragging

        bool canDrag { get; }
        bool isDragging { get; }
        void BeginDrag();
        void LockDragOrigin();
        void Drag(Vector2 delta, Vector2 constraint);
        void EndDrag();
        void ExpandDragGroup(HashSet<IGraphElement> group);

        #endregion

        #region Deleting

        bool canDelete { get; }
        void Delete();
        void ExpandDeleteGroup(HashSet<IGraphElement> group);

        #endregion
    }
}
