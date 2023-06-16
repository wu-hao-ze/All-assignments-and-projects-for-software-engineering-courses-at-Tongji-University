namespace Unity.VisualScripting
{
    public abstract class UnitInputPortWidget<TPort> : UnitPortWidget<TPort> where TPort : class, IUnitInputPort
    {
        protected UnitInputPortWidget(FlowCanvas canvas, TPort port) : base(canvas, port) { }

        protected override Edge edge => Edge.Left;
    }
}
