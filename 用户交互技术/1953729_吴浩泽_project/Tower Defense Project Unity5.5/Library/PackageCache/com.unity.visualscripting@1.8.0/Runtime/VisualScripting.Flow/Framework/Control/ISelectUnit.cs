namespace Unity.VisualScripting
{
    [TypeIconPriority]
    public interface ISelectUnit : IUnit
    {
        ValueOutput selection { get; }
    }
}
