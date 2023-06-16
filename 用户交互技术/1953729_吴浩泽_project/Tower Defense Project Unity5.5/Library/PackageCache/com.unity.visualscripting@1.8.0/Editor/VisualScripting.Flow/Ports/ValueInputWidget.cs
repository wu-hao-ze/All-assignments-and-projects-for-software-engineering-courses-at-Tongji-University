using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(ValueInput))]
    public class ValueInputWidget : UnitInputPortWidget<ValueInput>
    {
        public ValueInputWidget(FlowCanvas canvas, ValueInput port) : base(canvas, port)
        {
            color = ValueConnectionWidget.DetermineColor(port.type);
        }

        protected override bool showInspector => port.hasDefaultValue && !port.hasValidConnection;

        protected override bool colorIfActive => !BoltFlow.Configuration.animateControlConnections || !BoltFlow.Configuration.animateValueConnections;

        public override Color color { get; }

        protected override Texture handleTextureConnected => BoltFlow.Icons.valuePortConnected?[12];

        protected override Texture handleTextureUnconnected => BoltFlow.Icons.valuePortUnconnected?[12];

        public override Metadata FetchInspectorMetadata()
        {
            if (port.hasDefaultValue)
            {
                return metadata["_defaultValue"].Cast(port.type);
            }
            else
            {
                return null;
            }
        }
    }
}
