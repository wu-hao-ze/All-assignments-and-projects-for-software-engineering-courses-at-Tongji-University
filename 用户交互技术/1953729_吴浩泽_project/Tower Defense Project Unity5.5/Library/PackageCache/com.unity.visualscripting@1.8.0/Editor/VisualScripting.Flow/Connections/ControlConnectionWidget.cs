using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(ControlConnection))]
    public sealed class ControlConnectionWidget : UnitConnectionWidget<ControlConnection>
    {
        public ControlConnectionWidget(FlowCanvas canvas, ControlConnection connection) : base(canvas, connection) { }


        #region Drawing

        public override Color color => Color.white;

        protected override bool colorIfActive => !BoltFlow.Configuration.animateControlConnections || !BoltFlow.Configuration.animateValueConnections;

        #endregion


        #region Droplets

        protected override bool showDroplets => BoltFlow.Configuration.animateControlConnections;

        protected override Vector2 GetDropletSize()
        {
            return BoltFlow.Icons.valuePortConnected?[12].Size() ?? 12 * Vector2.one;
        }

        protected override void DrawDroplet(Rect position)
        {
            if (BoltFlow.Icons.valuePortConnected != null)
            {
                GUI.DrawTexture(position, BoltFlow.Icons.valuePortConnected[12]);
            }
        }

        #endregion
    }
}
