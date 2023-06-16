using System.Collections;
using System.Linq;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the first item in a collection or enumeration.
    /// </summary>
    [UnitCategory("Collections")]
    public sealed class FirstItem : Unit
    {
        /// <summary>
        /// The collection.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput collection { get; private set; }

        /// <summary>
        /// The first item of the collection.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput firstItem { get; private set; }

        protected override void Definition()
        {
            collection = ValueInput<IEnumerable>(nameof(collection));
            firstItem = ValueOutput(nameof(firstItem), First);

            Requirement(collection, firstItem);
        }

        public object First(Flow flow)
        {
            var enumerable = flow.GetValue<IEnumerable>(collection);

            if (enumerable is IList)
            {
                return ((IList)enumerable)[0];
            }
            else
            {
                return enumerable.Cast<object>().First();
            }
        }
    }
}
