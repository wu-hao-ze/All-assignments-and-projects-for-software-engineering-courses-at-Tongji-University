using System.Collections;
using System.Linq;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns the first item in a collection or enumeration.
    /// </summary>
    [UnitCategory("Collections")]
    public sealed class LastItem : Unit
    {
        /// <summary>
        /// The collection.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput collection { get; private set; }

        /// <summary>
        /// The last item of the collection.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput lastItem { get; private set; }

        protected override void Definition()
        {
            collection = ValueInput<IEnumerable>(nameof(collection));
            lastItem = ValueOutput(nameof(lastItem), First);

            Requirement(collection, lastItem);
        }

        public object First(Flow flow)
        {
            var enumerable = flow.GetValue<IEnumerable>(collection);

            if (enumerable is IList)
            {
                var list = (IList)enumerable;

                return list[list.Count - 1];
            }
            else
            {
                return enumerable.Cast<object>().Last();
            }
        }
    }
}
