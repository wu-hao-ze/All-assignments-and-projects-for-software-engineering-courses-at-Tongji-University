namespace Unity.VisualScripting
{
    [Descriptor(typeof(SwitchOnInteger))]
    public class SwitchOnIntegerDescriptor : SwitchUnitDescriptor<int>
    {
        public SwitchOnIntegerDescriptor(SwitchOnInteger unit) : base(unit) { }

        protected override string GetLabelForOption(int option)
        {
            return option.ToString();
        }
    }
}
