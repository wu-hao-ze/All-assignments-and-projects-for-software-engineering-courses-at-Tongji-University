namespace Unity.VisualScripting
{
    public class FuzzyGroup
    {
        public FuzzyGroup(string label)
        {
            this.label = label;
        }

        public FuzzyGroup(string label, EditorTexture icon)
        {
            this.label = label;
            this.icon = icon;
        }

        public FuzzyGroup(string label, object data)
        {
            this.label = label;
            this.data = data;
        }

        public FuzzyGroup(string label, EditorTexture icon, object data)
        {
            this.label = label;
            this.icon = icon;
            this.data = data;
        }

        public string label { get; set; }
        public EditorTexture icon { get; set; }
        public object data { get; set; }
    }
}
