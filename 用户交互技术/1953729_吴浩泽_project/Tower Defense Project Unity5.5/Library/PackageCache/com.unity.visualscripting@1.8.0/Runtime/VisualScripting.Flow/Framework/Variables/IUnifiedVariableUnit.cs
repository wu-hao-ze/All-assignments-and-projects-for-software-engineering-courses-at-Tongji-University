namespace Unity.VisualScripting
{
    public interface IUnifiedVariableUnit : IUnit
    {
        VariableKind kind { get; }
        ValueInput name { get; }
    }
}
