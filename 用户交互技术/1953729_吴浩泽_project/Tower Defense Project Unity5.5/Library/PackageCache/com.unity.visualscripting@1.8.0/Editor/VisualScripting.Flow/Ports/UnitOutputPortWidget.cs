namespace Unity.VisualScripting
{
    public abstract class UnitOutputPortWidget<TPort> : UnitPortWidget<TPort> where TPort : class, IUnitOutputPort
    {
        protected UnitOutputPortWidget(FlowCanvas canvas, TPort port) : base(canvas, port) { }

        protected override Edge edge => Edge.Right;
    }
}
