using UnityEngine;

namespace Unity.VisualScripting
{
    [Widget(typeof(InvalidConnection))]
    public sealed class InvalidConnectionWidget : UnitConnectionWidget<InvalidConnection>
    {
        public InvalidConnectionWidget(FlowCanvas canvas, InvalidConnection connection) : base(canvas, connection) { }


        #region Drawing

        public override Color color => UnitConnectionStyles.invalidColor;

        #endregion


        #region Droplets

        protected override bool showDroplets => false;

        protected override Vector2 GetDropletSize() => Vector2.zero;

        protected override void DrawDroplet(Rect position) { }

        #endregion
    }
}
