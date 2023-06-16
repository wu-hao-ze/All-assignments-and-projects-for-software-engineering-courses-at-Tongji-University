using System;

namespace Unity.VisualScripting
{
    public interface IGraphElementCollection<T> : IKeyedCollection<Guid, T>, INotifyCollectionChanged<T> where T : IGraphElement
    {
    }
}
