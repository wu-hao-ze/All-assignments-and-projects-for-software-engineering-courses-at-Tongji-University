using System.Collections;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Checks whether a dictionary contains the specified key.
    /// </summary>
    [UnitCategory("Collections/Dictionaries")]
    [UnitSurtitle("Dictionary")]
    [UnitShortTitle("Contains Key")]
    [TypeIcon(typeof(IDictionary))]
    public sealed class DictionaryContainsKey : Unit
    {
        /// <summary>
        /// The dictionary.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput dictionary { get; private set; }

        /// <summary>
        /// The key.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput key { get; private set; }

        /// <summary>
        /// Whether the list contains the item.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput contains { get; private set; }

        protected override void Definition()
        {
            dictionary = ValueInput<IDictionary>(nameof(dictionary));
            key = ValueInput<object>(nameof(key));
            contains = ValueOutput(nameof(contains), Contains);

            Requirement(dictionary, contains);
            Requirement(key, contains);
        }

        private bool Contains(Flow flow)
        {
            var dictionary = flow.GetValue<IDictionary>(this.dictionary);
            var key = flow.GetValue<object>(this.key);

            return dictionary.Contains(key);
        }
    }
}
