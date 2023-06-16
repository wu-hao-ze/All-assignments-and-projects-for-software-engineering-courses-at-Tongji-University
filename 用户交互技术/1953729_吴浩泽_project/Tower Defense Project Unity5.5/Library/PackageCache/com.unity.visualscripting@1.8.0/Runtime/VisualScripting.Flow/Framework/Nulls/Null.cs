namespace Unity.VisualScripting
{
    /// <summary>
    /// Returns a null value.
    /// </summary>
    [UnitCategory("Nulls")]
    public sealed class Null : Unit
    {
        /// <summary>
        /// A null value.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput @null { get; private set; }

        protected override void Definition()
        {
            @null = ValueOutput<object>(nameof(@null), (recursion) => null).Predictable();
        }
    }
}
