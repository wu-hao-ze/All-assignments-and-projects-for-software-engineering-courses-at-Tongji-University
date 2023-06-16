using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public class ConnectionCollectionBase<TConnection, TSource, TDestination, TCollection> : IConnectionCollection<TConnection, TSource, TDestination>
        where TConnection : IConnection<TSource, TDestination>
        where TCollection : ICollection<TConnection>
    {
        public ConnectionCollectionBase(TCollection collection)
        {
            this.collection = collection;
            bySource = new Dictionary<TSource, List<TConnection>>();
            byDestination = new Dictionary<TDestination, List<TConnection>>();
        }

        // Using lists instead of HashSet to allow access by index
        // instead of creating an enumeration and allocating memory
        // specifically for the "With*NoAlloc" methods, used
        // very often in flow graphs.
        private readonly Dictionary<TDestination, List<TConnection>> byDestination;
        private readonly Dictionary<TSource, List<TConnection>> bySource;
        protected readonly TCollection collection;

        public IEnumerable<TConnection> this[TSource source] => WithSource(source);

        public IEnumerable<TConnection> this[TDestination destination] => WithDestination(destination);

        public int Count => collection.Count;

        public bool IsReadOnly => false;

        public IEnumerator<TConnection> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<TConnection> WithSource(TSource source)
        {
            return WithSourceNoAlloc(source);
        }

        public List<TConnection> WithSourceNoAlloc(TSource source)
        {
            Ensure.That(nameof(source)).IsNotNull(source);

            if (bySource.TryGetValue(source, out var withSource))
            {
                return withSource;
            }
            else
            {
                return Empty<TConnection>.list;
            }
        }

        public TConnection SingleOrDefaultWithSource(TSource source)
        {
            Ensure.That(nameof(source)).IsNotNull(source);

            if (bySource.TryGetValue(source, out var withSource))
            {
                if (withSource.Count == 1)
                {
                    return withSource[0];
                }
                else if (withSource.Count == 0)
                {
                    return default(TConnection);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                return default(TConnection);
            }
        }

        public IEnumerable<TConnection> WithDestination(TDestination destination)
        {
            return WithDestinationNoAlloc(destination);
        }

        public List<TConnection> WithDestinationNoAlloc(TDestination destination)
        {
            Ensure.That(nameof(destination)).IsNotNull(destination);

            if (byDestination.TryGetValue(destination, out var withDestination))
            {
                return withDestination;
            }
            else
            {
                return Empty<TConnection>.list;
            }
        }

        public TConnection SingleOrDefaultWithDestination(TDestination destination)
        {
            Ensure.That(nameof(destination)).IsNotNull(destination);

            if (byDestination.TryGetValue(destination, out var withDestination))
            {
                if (withDestination.Count == 1)
                {
                    return withDestination[0];
                }
                else if (withDestination.Count == 0)
                {
                    return default(TConnection);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                return default(TConnection);
            }
        }

        public void Add(TConnection item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.source == null)
            {
                throw new ArgumentNullException("item.source");
            }

            if (item.destination == null)
            {
                throw new ArgumentNullException("item.destination");
            }

            BeforeAdd(item);
            collection.Add(item);
            AddToDictionaries(item);
            AfterAdd(item);
        }

        public void Clear()
        {
            collection.Clear();
            bySource.Clear();
            byDestination.Clear();
        }

        public bool Contains(TConnection item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return collection.Contains(item);
        }

        public void CopyTo(TConnection[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(TConnection item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.source == null)
            {
                throw new ArgumentNullException("item.source");
            }

            if (item.destination == null)
            {
                throw new ArgumentNullException("item.destination");
            }

            if (!collection.Contains(item))
            {
                return false;
            }

            BeforeRemove(item);
            collection.Remove(item);
            RemoveFromDictionaries(item);
            AfterRemove(item);
            return true;
        }

        protected virtual void BeforeAdd(TConnection item) { }
        protected virtual void AfterAdd(TConnection item) { }
        protected virtual void BeforeRemove(TConnection item) { }
        protected virtual void AfterRemove(TConnection item) { }

        private void AddToDictionaries(TConnection item)
        {
            if (!bySource.ContainsKey(item.source))
            {
                bySource.Add(item.source, new List<TConnection>());
            }

            bySource[item.source].Add(item);

            if (!byDestination.ContainsKey(item.destination))
            {
                byDestination.Add(item.destination, new List<TConnection>());
            }

            byDestination[item.destination].Add(item);
        }

        private void RemoveFromDictionaries(TConnection item)
        {
            bySource[item.source].Remove(item);
            byDestination[item.destination].Remove(item);
        }
    }
}
