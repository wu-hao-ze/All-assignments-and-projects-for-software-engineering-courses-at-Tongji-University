using System;

namespace Unity.VisualScripting
{
    [FuzzyOption(typeof(SetVariable))]
    public class SetVariableOption : UnifiedVariableUnitOption<SetVariable>
    {
        [Obsolete(Serialization.ConstructorWarning)]
        public SetVariableOption() : base() { }

        public SetVariableOption(VariableKind kind, string defaultName = null) : base(kind, defaultName) { }

        protected override string NamedLabel(bool human)
        {
            return $"Set {name}";
        }

        protected override string UnnamedLabel(bool human)
        {
            return $"Set {kind} Variable";
        }
    }
}
