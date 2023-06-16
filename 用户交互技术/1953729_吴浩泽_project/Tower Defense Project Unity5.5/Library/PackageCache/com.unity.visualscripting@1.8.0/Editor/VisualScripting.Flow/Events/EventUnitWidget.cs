namespace Unity.VisualScripting
{
    [Widget(typeof(IEventUnit))]
    public sealed class EventUnitWidget : UnitWidget<IEventUnit>
    {
        public EventUnitWidget(FlowCanvas canvas, IEventUnit unit) : base(canvas, unit) { }

        protected override NodeColorMix baseColor => NodeColor.Green;
    }
}
