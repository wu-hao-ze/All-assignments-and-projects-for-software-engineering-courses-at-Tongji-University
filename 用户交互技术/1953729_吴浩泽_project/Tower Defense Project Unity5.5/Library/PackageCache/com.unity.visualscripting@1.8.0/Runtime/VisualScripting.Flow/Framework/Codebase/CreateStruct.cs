using System;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Creates a struct with its default initializer.
    /// </summary>
    [SpecialUnit]
    public sealed class CreateStruct : Unit
    {
        [Obsolete(Serialization.ConstructorWarning)]
        public CreateStruct() : base() { }

        public CreateStruct(Type type) : base()
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            if (!type.IsStruct())
            {
                throw new ArgumentException($"Type {type} must be a struct.", nameof(type));
            }

            this.type = type;
        }

        [Serialize]
        public Type type { get; internal set; }

        // Shouldn't happen through normal use, but can happen
        // if deserialization fails to find the type
        // https://support.ludiq.io/communities/5/topics/1661-x
        public override bool canDefine => type != null;

        /// <summary>
        /// The entry point to create the struct. You can
        /// still get the return value without connecting this port.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The action to call once the struct has been created.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        /// <summary>
        /// The created struct.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput output { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            exit = ControlOutput(nameof(exit));
            output = ValueOutput(type, nameof(output), Create);

            Succession(enter, exit);
        }

        private ControlOutput Enter(Flow flow)
        {
            flow.SetValue(output, Activator.CreateInstance(type));

            return exit;
        }

        private object Create(Flow flow)
        {
            return Activator.CreateInstance(type);
        }
    }
}
