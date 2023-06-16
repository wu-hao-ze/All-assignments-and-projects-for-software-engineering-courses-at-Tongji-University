namespace Unity.VisualScripting
{
    public class DropdownSeparator : DropdownOption
    {
        public DropdownSeparator() : this(string.Empty) { }

        public DropdownSeparator(string path) : base(null, null)
        {
            this.path = path;
        }

        public string path { get; set; }
    }
}
