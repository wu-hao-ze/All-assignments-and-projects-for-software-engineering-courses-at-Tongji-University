using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns a constant value defined from the editor.
    /// </summary>
    [SpecialUnit]
    public sealed class Literal : Unit
    {
        [Obsolete(Serialization.ConstructorWarning)]
        public Literal() : base() { }

        public Literal(Type type) : this(type, type.PseudoDefault()) { }

        public Literal(Type type, object value) : base()
        {
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(value)).IsOfType(value, type);

            this.type = type;
            this.value = value;
        }

        // Shouldn't happen through normal use, but can happen
        // if deserialization fails to find the type
        // https://support.ludiq.io/communities/5/topics/1661-x
        public override bool canDefine => type != null;

        [SerializeAs(nameof(value))]
        private object _value;

        [Serialize]
        public Type type { get; internal set; }

        [DoNotSerialize]
        public object value
        {
            get => _value;
            set
            {
                Ensure.That(nameof(value)).IsOfType(value, type);

                _value = value;
            }
        }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            output = ValueOutput(type, nameof(output), (flow) => value).Predictable();
        }

        #region Analytics

        public override AnalyticsIdentifier GetAnalyticsIdentifier()
        {
            var aid = new AnalyticsIdentifier
            {
                Identifier = $"{GetType().FullName}({type.Name})",
                Namespace = type.Namespace,
            };
            aid.Hashcode = aid.Identifier.GetHashCode();
            return aid;
        }

        #endregion
    }
}
