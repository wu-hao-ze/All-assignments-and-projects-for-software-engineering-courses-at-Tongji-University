namespace Unity.VisualScripting
{
    public class SelectUnitDescriptor<T> : UnitDescriptor<SelectUnit<T>>
    {
        public SelectUnitDescriptor(SelectUnit<T> unit) : base(unit) { }

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
                    description.summary = $"The value to return if the enum has the value {GetLabelForOption(option)}.";
                }
            }
        }
    }
}
