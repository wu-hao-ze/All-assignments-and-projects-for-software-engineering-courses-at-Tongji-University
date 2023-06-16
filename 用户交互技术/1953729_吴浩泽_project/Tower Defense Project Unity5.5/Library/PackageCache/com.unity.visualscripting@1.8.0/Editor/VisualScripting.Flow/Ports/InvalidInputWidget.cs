using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(InvalidInput))]
    public class InvalidInputWidget : UnitInputPortWidget<InvalidInput>
    {
        public InvalidInputWidget(FlowCanvas canvas, InvalidInput port) : base(canvas, port) { }

        protected override Texture handleTextureConnected => BoltFlow.Icons.invalidPortConnected?[12];

        protected override Texture handleTextureUnconnected => BoltFlow.Icons.invalidPortUnconnected?[12];

        protected override bool colorIfActive => false;

        protected override bool canStartConnection => false;
    }
}
