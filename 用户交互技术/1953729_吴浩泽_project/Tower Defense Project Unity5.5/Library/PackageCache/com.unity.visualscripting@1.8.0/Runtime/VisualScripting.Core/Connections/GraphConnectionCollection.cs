using System;

namespace Unity.VisualScripting
{
    public class GraphConnectionCollection<TConnection, TSource, TDestination> :
        ConnectionCollectionBase<TConnection, TSource, TDestination, GraphElementCollection<TConnection>>,
        IGraphElementCollection<TConnection>
        where TConnection : IConnection<TSource, TDestination>, IGraphElement
    {
        public GraphConnectionCollection(IGraph graph) : base(new GraphElementCollection<TConnection>(graph))
        {
            // The issue of reusing GEC as the internal collection a CCB is that
            // the add / remove events will NOT be in sync with the CCB's dictionaries
            // if we just watched the collection's insertion.
            // Therefore, we must provide a way to let the CCB proxy its own events
            // and to disable our the GEC's events by default.
            collection.ProxyCollectionChange = true;
        }

        TConnection IKeyedCollection<Guid, TConnection>.this[Guid key] => collection[key];

        TConnection IKeyedCollection<Guid, TConnection>.this[int index] => collection[index];

        public bool TryGetValue(Guid key, out TConnection value)
        {
            return collection.TryGetValue(key, out value);
        }

        public bool Contains(Guid key)
        {
            return collection.Contains(key);
        }

        public bool Remove(Guid key)
        {
            if (Contains(key))
            {
                // Call base remove to remove from dictionaries as well
                return Remove(collection[key]);
            }

            return false;
        }

        public event Action<TConnection> ItemAdded
        {
            add { collection.ItemAdded += value; }
            remove { collection.ItemAdded -= value; }
        }

        public event Action<TConnection> ItemRemoved
        {
            add { collection.ItemRemoved += value; }
            remove { collection.ItemRemoved -= value; }
        }

        public event Action CollectionChanged
        {
            add { collection.CollectionChanged += value; }
            remove { collection.CollectionChanged -= value; }
        }

        protected override void BeforeAdd(TConnection item)
        {
            collection.BeforeAdd(item);
        }

        protected override void AfterAdd(TConnection item)
        {
            collection.AfterAdd(item);
        }

        protected override void BeforeRemove(TConnection item)
        {
            collection.BeforeRemove(item);
        }

        protected override void AfterRemove(TConnection item)
        {
            collection.AfterRemove(item);
        }
    }
}
