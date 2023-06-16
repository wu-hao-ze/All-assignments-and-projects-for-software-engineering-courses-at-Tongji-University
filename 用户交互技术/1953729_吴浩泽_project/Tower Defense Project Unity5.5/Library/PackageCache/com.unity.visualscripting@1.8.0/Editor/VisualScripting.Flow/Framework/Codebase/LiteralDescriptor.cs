namespace Unity.VisualScripting
{
    [Descriptor(typeof(Literal))]
    public class LiteralDescriptor : UnitDescriptor<Literal>
    {
        public LiteralDescriptor(Literal unit) : base(unit) { }

        protected override string DefinedTitle()
        {
            return unit.type.DisplayName() + " Literal";
        }

        protected override string DefinedShortTitle()
        {
            return unit.type.DisplayName();
        }

        protected override string DefinedSummary()
        {
            return unit.type.Summary();
        }

        protected override EditorTexture DefinedIcon()
        {
            return unit.type.Icon();
        }
    }
}
