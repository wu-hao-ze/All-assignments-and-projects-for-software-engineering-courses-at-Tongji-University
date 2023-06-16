using System;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(SelectOnEnum))]
    public class SelectOnEnumDescriptor : UnitDescriptor<SelectOnEnum>
    {
        public SelectOnEnumDescriptor(SelectOnEnum unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            foreach (var branch in unit.branches)
            {
                if (branch.Value == port)
                {
                    var enumValue = (Enum)branch.Key;

                    description.label = enumValue.DisplayName();
                    description.summary = $"The value to return if the enum has the value '{enumValue}'.";
                }
            }
        }
    }
}
