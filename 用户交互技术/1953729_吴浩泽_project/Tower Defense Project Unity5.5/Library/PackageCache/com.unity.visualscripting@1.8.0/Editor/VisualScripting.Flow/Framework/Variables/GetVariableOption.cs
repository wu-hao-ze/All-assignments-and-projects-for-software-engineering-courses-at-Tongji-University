using System;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(GetVariable))]
    public class GetVariableOption : UnifiedVariableUnitOption<GetVariable>
    {
        [Obsolete(Serialization.ConstructorWarning)]
        public GetVariableOption() : base() { }

        public GetVariableOption(VariableKind kind, string defaultName = null) : base(kind, defaultName) { }

        protected override string NamedLabel(bool human)
        {
            return $"Get {name}";
        }

        protected override string UnnamedLabel(bool human)
        {
            return $"Get {kind} Variable";
        }
    }
}
