using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(ControlInput))]
    public class
        ControlInputWidget : UnitInputPortWidget<ControlInput>
    {
        public ControlInputWidget(FlowCanvas canvas, ControlInput port) : base(canvas, port) { }

        protected override Texture handleTextureConnected => BoltFlow.Icons.controlPortConnected?[12];

        protected override Texture handleTextureUnconnected => BoltFlow.Icons.controlPortUnconnected?[12];

        protected override bool colorIfActive => !BoltFlow.Configuration.animateControlConnections || !BoltFlow.Configuration.animateValueConnections;
    }
}
