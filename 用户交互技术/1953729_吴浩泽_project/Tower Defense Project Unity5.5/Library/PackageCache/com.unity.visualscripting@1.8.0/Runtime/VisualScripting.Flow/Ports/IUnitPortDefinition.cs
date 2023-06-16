namespace Unity.VisualScripting
{
    public interface IUnitPortDefinition
    {
        string key { get; }
        string label { get; }
        string summary { get; }
        bool hideLabel { get; }
        bool isValid { get; }
    }
}
