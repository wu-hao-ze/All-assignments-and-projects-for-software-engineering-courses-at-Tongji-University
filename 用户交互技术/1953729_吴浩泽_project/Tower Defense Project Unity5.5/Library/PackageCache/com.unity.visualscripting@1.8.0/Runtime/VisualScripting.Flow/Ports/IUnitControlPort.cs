namespace Unity.VisualScripting
{
    public interface IUnitControlPort : IUnitPort
    {
        bool isPredictable { get; }
        bool couldBeEntered { get; }
    }
}
