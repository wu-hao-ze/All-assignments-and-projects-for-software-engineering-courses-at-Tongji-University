namespace Unity.VisualScripting
{
    [TypeIconPriority]
    public interface IBranchUnit : IUnit
    {
        ControlInput enter { get; }
    }
}
