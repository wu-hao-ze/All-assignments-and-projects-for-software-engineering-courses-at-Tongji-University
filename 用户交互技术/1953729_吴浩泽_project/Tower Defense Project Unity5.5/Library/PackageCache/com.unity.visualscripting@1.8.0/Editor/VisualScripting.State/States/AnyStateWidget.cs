namespace Unity.VisualScripting
{
    [Widget(typeof(AnyState))]
    public class AnyStateWidget : StateWidget<AnyState>
    {
        public AnyStateWidget(StateCanvas canvas, AnyState state) : base(canvas, state) { }

        protected override NodeColorMix color => NodeColorMix.TealReadable;

        protected override string summary => null;

        public override bool canToggleStart => false;

        public override bool canForceEnter => false;

        public override bool canForceExit => false;
    }
}
