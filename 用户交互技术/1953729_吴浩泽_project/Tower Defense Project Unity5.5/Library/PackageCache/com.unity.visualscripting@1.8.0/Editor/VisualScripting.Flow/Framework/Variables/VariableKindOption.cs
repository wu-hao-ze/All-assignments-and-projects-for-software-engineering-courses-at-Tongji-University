namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(VariableKind))]
    public class VariableKindOption : DocumentedOption<VariableKind>
    {
        public VariableKindOption(VariableKind kind)
        {
            value = kind;
            label = kind.HumanName();
            UnityAPI.Async(() => icon = BoltCore.Icons.VariableKind(kind));
            documentation = kind.Documentation();
            zoom = true;
            parentOnly = true;
        }
    }
}
