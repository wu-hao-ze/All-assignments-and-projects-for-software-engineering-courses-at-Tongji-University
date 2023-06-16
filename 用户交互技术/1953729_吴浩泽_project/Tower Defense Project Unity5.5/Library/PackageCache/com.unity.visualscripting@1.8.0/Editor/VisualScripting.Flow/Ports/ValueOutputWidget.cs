using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(ValueOutput))]
    public class ValueOutputWidget : UnitOutputPortWidget<ValueOutput>
    {
        public ValueOutputWidget(FlowCanvas canvas, ValueOutput port) : base(canvas, port)
        {
            color = ValueConnectionWidget.DetermineColor(port.type);
        }

        protected override bool colorIfActive => !BoltFlow.Configuration.animateControlConnections || !BoltFlow.Configuration.animateValueConnections;

        public override Color color { get; }

        protected override Texture handleTextureConnected => BoltFlow.Icons.valuePortConnected?[12];

        protected override Texture handleTextureUnconnected => BoltFlow.Icons.valuePortUnconnected?[12];
    }
}
