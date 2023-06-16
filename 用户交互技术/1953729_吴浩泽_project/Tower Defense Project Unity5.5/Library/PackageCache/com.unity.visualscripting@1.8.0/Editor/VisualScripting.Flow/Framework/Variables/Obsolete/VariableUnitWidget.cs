#pragma warning disable 618

namespace Unity.VisualScripting
{
    [Widget(typeof(VariableUnit))]
    public sealed class VariableUnitWidget : UnitWidget<VariableUnit>
    {
        public VariableUnitWidget(FlowCanvas canvas, VariableUnit unit) : base(canvas, unit) { }

        protected override NodeColorMix baseColor => NodeColorMix.TealReadable;
    }
}
