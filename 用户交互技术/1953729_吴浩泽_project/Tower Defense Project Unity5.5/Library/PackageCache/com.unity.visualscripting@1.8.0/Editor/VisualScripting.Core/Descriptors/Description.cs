namespace Unity.VisualScripting
{
    public class Description : IDescription
    {
        public virtual string title { get; set; }
        public virtual string summary { get; set; }
        public virtual EditorTexture icon { get; set; }
    }
}
