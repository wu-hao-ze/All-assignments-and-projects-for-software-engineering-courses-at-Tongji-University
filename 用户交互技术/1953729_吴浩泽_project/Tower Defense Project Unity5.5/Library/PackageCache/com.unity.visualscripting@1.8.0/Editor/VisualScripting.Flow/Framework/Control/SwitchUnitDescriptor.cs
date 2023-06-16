namespace Unity.VisualScripting
{
    public class SwitchUnitDescriptor<T> : UnitDescriptor<SwitchUnit<T>>
    {
        public SwitchUnitDescriptor(SwitchUnit<T> unit) : base(unit) { }

        protected virtual string GetLabelForOption(T option)
        {
            return option.ToString();
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            foreach (var branch in unit.branches)
            {
                if (branch.Value == port)
                {
                    var option = branch.Key;

                    description.label = GetLabelForOption(option);
                    description.summary = $"The action to execute if the selector has the value {GetLabelForOption(option)}.";
                }
            }
        }
    }
}
