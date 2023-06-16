using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(FlowStateTransition))]
    public sealed class FlowStateTransitionWidget : NesterStateTransitionWidget<FlowStateTransition>, IDragAndDropHandler
    {
        public FlowStateTransitionWidget(StateCanvas canvas, FlowStateTransition transition) : base(canvas, transition) { }

        #region Drag & Drop

        public DragAndDropVisualMode dragAndDropVisualMode => DragAndDropVisualMode.Generic;

        public bool AcceptsDragAndDrop()
        {
            return DragAndDropUtility.Is<ScriptGraphAsset>();
        }

        public void PerformDragAndDrop()
        {
            UndoUtility.RecordEditedObject("Drag & Drop Macro");
            transition.nest.source = GraphSource.Macro;
            transition.nest.macro = DragAndDropUtility.Get<ScriptGraphAsset>();
            transition.nest.embed = null;
            GUI.changed = true;
        }

        public void UpdateDragAndDrop()
        {
        }

        public void DrawDragAndDropPreview()
        {
            GraphGUI.DrawDragAndDropPreviewLabel(new Vector2(edgePosition.x, outerPosition.yMax), "Replace with: " + DragAndDropUtility.Get<ScriptGraphAsset>().name, typeof(ScriptGraphAsset).Icon());
        }

        public void ExitDragAndDrop()
        {
        }

        #endregion
    }
}
