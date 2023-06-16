namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(LooseAssemblyName))]
    public class LooseAssemblyNameOption : FuzzyOption<LooseAssemblyName>
    {
        public LooseAssemblyNameOption(LooseAssemblyName looseAssemblyName)
        {
            value = looseAssemblyName;
            label = value.name;
        }

        public static string Haystack(LooseAssemblyName looseAssemblyName)
        {
            return looseAssemblyName.name;
        }

        public static string SearchResultLabel(LooseAssemblyName looseAssemblyName, string query)
        {
            return SearchUtility.HighlightQuery(looseAssemblyName.name, query);
        }
    }
}
