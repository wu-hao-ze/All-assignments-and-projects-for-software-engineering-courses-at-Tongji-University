namespace Unity.VisualScripting
{
    public interface IDescriptor
    {
        object target { get; }

        IDescription description { get; }

        bool isDirty { get; set; }

        void Validate();
    }
}
