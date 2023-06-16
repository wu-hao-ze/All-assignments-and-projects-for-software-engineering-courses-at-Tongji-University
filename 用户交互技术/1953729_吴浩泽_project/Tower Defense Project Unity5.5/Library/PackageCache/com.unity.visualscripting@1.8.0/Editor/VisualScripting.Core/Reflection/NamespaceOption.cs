namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(Namespace))]
    public class NamespaceOption : FuzzyOption<Namespace>
    {
        public NamespaceOption(Namespace @namespace)
        {
            value = @namespace;
            label = @namespace.DisplayName(false);
            UnityAPI.Async(() => icon = @namespace.Icon());
        }

        public NamespaceOption(Namespace @namespace, bool parentOnly) : this(@namespace)
        {
            this.parentOnly = parentOnly;
        }
    }
}
