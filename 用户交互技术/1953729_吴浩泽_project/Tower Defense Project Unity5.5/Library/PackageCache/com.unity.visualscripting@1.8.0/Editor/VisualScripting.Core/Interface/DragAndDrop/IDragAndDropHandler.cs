using UnityEditor;

namespace Unity.VisualScripting
{
    public interface IDragAndDropHandler
    {
        DragAndDropVisualMode dragAndDropVisualMode { get; }
        bool AcceptsDragAndDrop();
        void PerformDragAndDrop();
        void UpdateDragAndDrop();
        void DrawDragAndDropPreview();
        void ExitDragAndDrop();
    }
}
