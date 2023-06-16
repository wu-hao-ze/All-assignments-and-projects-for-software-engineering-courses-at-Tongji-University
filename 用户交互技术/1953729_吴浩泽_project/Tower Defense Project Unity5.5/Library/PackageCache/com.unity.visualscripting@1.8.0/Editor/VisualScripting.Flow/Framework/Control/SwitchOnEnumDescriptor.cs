namespace Unity.VisualScripting
{
    [Descriptor(typeof(SwitchOnEnum))]
    public class SwitchOnEnumDescriptor : UnitDescriptor<SwitchOnEnum>
    {
        public SwitchOnEnumDescriptor(SwitchOnEnum unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            foreach (var branch in unit.branches)
            {
                if (branch.Value == port)
                {
                    var enumValue = branch.Key;
                    description.label = enumValue.DisplayName();
                    description.summary = $"The action to execute if the enum has the value '{enumValue}'.";
                }
            }
        }
    }
}
