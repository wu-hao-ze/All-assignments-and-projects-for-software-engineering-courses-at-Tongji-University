using System.Collections;
using System.Linq;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Counts all items in a collection or enumeration.
    /// </summary>
    [UnitCategory("Collections")]
    public sealed class CountItems : Unit
    {
        /// <summary>
        /// The collection.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput collection { get; private set; }

        /// <summary>
        /// The number of items contained in the collection.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput count { get; private set; }

        protected override void Definition()
        {
            collection = ValueInput<IEnumerable>(nameof(collection));
            count = ValueOutput(nameof(count), Count);

            Requirement(collection, count);
        }

        public int Count(Flow flow)
        {
            var enumerable = flow.GetValue<IEnumerable>(collection);

            if (enumerable is ICollection)
            {
                return ((ICollection)enumerable).Count;
            }
            else
            {
                return enumerable.Cast<object>().Count();
            }
        }
    }
}
