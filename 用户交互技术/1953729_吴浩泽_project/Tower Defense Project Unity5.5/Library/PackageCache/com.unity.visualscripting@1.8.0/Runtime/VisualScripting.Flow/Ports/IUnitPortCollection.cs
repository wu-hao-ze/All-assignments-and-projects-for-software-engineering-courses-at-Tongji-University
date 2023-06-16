namespace Unity.VisualScripting
{
    public interface IUnitPortCollection<TPort> : IKeyedCollection<string, TPort> where TPort : IUnitPort
    {
        TPort Single();
    }
}
