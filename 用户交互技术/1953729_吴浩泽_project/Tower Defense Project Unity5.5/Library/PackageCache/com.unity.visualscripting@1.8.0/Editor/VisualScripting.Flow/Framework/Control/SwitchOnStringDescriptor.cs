namespace Unity.VisualScripting
{
    [Descriptor(typeof(SwitchOnString))]
    public class SwitchOnStringDescriptor : SwitchUnitDescriptor<string>
    {
        public SwitchOnStringDescriptor(SwitchOnString unit) : base(unit) { }

        protected override string GetLabelForOption(string option)
        {
            if (string.IsNullOrEmpty(option))
            {
                return "Null / Empty";
            }

            return $"\"{option}\"";
        }
    }
}
