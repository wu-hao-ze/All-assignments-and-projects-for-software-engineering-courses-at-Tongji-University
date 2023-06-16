using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(ControlOutput))]
    public class ControlOutputWidget : UnitOutputPortWidget<ControlOutput>
    {
        public ControlOutputWidget(FlowCanvas canvas, ControlOutput port) : base(canvas, port) { }

        protected override Texture handleTextureConnected => BoltFlow.Icons.controlPortConnected?[12];

        protected override Texture handleTextureUnconnected => BoltFlow.Icons.controlPortUnconnected?[12];

        protected override bool colorIfActive => !BoltFlow.Configuration.animateControlConnections || !BoltFlow.Configuration.animateValueConnections;
    }
}
