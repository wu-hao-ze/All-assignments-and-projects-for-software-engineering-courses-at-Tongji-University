namespace Unity.VisualScripting
{
    public interface IMacro : IGraphRoot, ISerializationDependency, IAotStubbable
    {
        IGraph graph { get; set; }
    }
}
