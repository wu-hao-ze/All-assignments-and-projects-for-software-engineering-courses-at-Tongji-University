namespace Unity.VisualScripting
{
    [GraphContextExtension(typeof(FlowGraphContext))]
    public sealed class FlowGraphContextStateExtension : GraphContextExtension<FlowGraphContext>
    {
        public FlowGraphContextStateExtension(FlowGraphContext context) : base(context) { }

        public override bool AcceptsDragAndDrop()
        {
            return DragAndDropUtility.Is<StateGraphAsset>();
        }

        public override void PerformDragAndDrop()
        {
            var statemacro = DragAndDropUtility.Get<StateGraphAsset>();
            var stateUnit = new StateUnit(statemacro);
            context.canvas.AddUnit(stateUnit, DragAndDropUtility.position);
        }

        public override void DrawDragAndDropPreview()
        {
            GraphGUI.DrawDragAndDropPreviewLabel(DragAndDropUtility.offsetedPosition, DragAndDropUtility.Get<StateGraphAsset>().name, typeof(StateGraphAsset).Icon());
        }
    }
}
