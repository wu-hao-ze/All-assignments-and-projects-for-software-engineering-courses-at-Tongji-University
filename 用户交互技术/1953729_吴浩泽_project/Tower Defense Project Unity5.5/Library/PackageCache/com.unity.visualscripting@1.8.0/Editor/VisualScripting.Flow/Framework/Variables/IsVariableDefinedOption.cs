using System;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(IsVariableDefined))]
    public class IsVariableDefinedOption : UnifiedVariableUnitOption<IsVariableDefined>
    {
        [Obsolete(Serialization.ConstructorWarning)]
        public IsVariableDefinedOption() : base() { }

        public IsVariableDefinedOption(VariableKind kind, string defaultName = null) : base(kind, defaultName) { }

        protected override string NamedLabel(bool human)
        {
            return $"{kind} Has {name} Variable";
        }

        protected override string UnnamedLabel(bool human)
        {
            return $"{kind} Has Variable";
        }

        public override string SearchResultLabel(string query)
        {
            return SearchUtility.HighlightQuery(haystack, query);
        }
    }
}
