namespace Unity.VisualScripting
{
    public class VariantKeyedCollection<TBase, TImplementation, TKey> :
        VariantCollection<TBase, TImplementation>,
        IKeyedCollection<TKey, TBase>
        where TImplementation : TBase
    {
        public VariantKeyedCollection(IKeyedCollection<TKey, TImplementation> implementation) : base(implementation)
        {
            this.implementation = implementation;
        }

        public TBase this[TKey key] => implementation[key];

        public new IKeyedCollection<TKey, TImplementation> implementation { get; private set; }

        public bool TryGetValue(TKey key, out TBase value)
        {
            TImplementation implementationValue;
            var result = implementation.TryGetValue(key, out implementationValue);
            value = implementationValue;
            return result;
        }

        public bool Contains(TKey key)
        {
            return implementation.Contains(key);
        }

        public bool Remove(TKey key)
        {
            return implementation.Remove(key);
        }

        TBase IKeyedCollection<TKey, TBase>.this[int index] => implementation[index];
    }
}
