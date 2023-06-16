using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Gets a dictionary item with the specified key.
    /// </summary>
    [UnitCategory("Collections/Dictionaries")]
    [UnitSurtitle("Dictionary")]
    [UnitShortTitle("Get Item")]
    [UnitOrder(0)]
    [TypeIcon(typeof(IDictionary))]
    public sealed class GetDictionaryItem : Unit
    {
        /// <summary>
        /// The dictionary.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput dictionary { get; private set; }

        /// <summary>
        /// The key of the item.
        /// </summary>
        [DoNotSerialize]
        public ValueInput key { get; private set; }

        /// <summary>
        /// The value of the item.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            dictionary = ValueInput<IDictionary>(nameof(dictionary));
            key = ValueInput<object>(nameof(key));
            value = ValueOutput(nameof(value), Get);

            Requirement(dictionary, value);
            Requirement(key, value);
        }

        private object Get(Flow flow)
        {
            var dictionary = flow.GetValue<IDictionary>(this.dictionary);
            var key = flow.GetValue<object>(this.key);

            return dictionary[key];
        }
    }
}
