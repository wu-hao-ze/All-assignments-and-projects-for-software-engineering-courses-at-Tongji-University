namespace Unity.VisualScripting
{
    public interface IAnalyser
    {
        IAnalysis analysis { get; }

        bool isDirty { get; set; }

        void Validate();
    }
}
