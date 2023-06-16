namespace Unity.VisualScripting
{
    [Descriptor(typeof(SelectOnString))]
    public class SelectOnStringDescriptor : SelectUnitDescriptor<string>
    {
        public SelectOnStringDescriptor(SelectOnString unit) : base(unit) { }

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
