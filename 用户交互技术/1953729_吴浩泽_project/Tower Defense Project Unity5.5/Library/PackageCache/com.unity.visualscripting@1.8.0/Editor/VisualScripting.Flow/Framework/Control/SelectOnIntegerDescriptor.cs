namespace Unity.VisualScripting
{
    [Descriptor(typeof(SelectOnInteger))]
    public class SelectOnIntegerDescriptor : SelectUnitDescriptor<int>
    {
        public SelectOnIntegerDescriptor(SelectOnInteger unit) : base(unit) { }

        protected override string GetLabelForOption(int option)
        {
            return option.ToString();
        }
    }
}
