using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IConnectionCollection<TConnection, TSource, TDestination> : ICollection<TConnection>
        where TConnection : IConnection<TSource, TDestination>
    {
        IEnumerable<TConnection> this[TSource source] { get; }
        IEnumerable<TConnection> this[TDestination destination] { get; }
        IEnumerable<TConnection> WithSource(TSource source);
        IEnumerable<TConnection> WithDestination(TDestination destination);
    }
}
