using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Sets the value of a dictionary item with the specified key.
    /// </summary>
    [UnitCategory("Collections/Dictionaries")]
    [UnitSurtitle("Dictionary")]
    [UnitShortTitle("Set Item")]
    [UnitOrder(1)]
    [TypeIcon(typeof(IDictionary))]
    public sealed class SetDictionaryItem : Unit
    {
        /// <summary>
        /// The entry point for the node.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The dictionary.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput dictionary { get; private set; }

        /// <summary>
        /// The key of the item to set.
        /// </summary>
        [DoNotSerialize]
        public ValueInput key { get; private set; }

        /// <summary>
        /// The value to assign to the item.
        /// </summary>
        [DoNotSerialize]
        public ValueInput value { get; private set; }

        /// <summary>
        /// The action to execute once the item has been assigned.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Set);
            dictionary = ValueInput<IDictionary>(nameof(dictionary));
            key = ValueInput<object>(nameof(key));
            value = ValueInput<object>(nameof(value));
            exit = ControlOutput(nameof(exit));

            Requirement(dictionary, enter);
            Requirement(key, enter);
            Requirement(value, enter);
            Succession(enter, exit);
        }

        public ControlOutput Set(Flow flow)
        {
            var dictionary = flow.GetValue<IDictionary>(this.dictionary);
            var key = flow.GetValue<object>(this.key);
            var value = flow.GetValue<object>(this.value);

            dictionary[key] = value;

            return exit;
        }
    }
}
