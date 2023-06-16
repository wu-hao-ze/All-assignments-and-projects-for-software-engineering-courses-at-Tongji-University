namespace Unity.VisualScripting
{
    public interface IProxyableNotifyCollectionChanged<T>
    {
        bool ProxyCollectionChange { get; set; }

        void BeforeAdd(T item);

        void AfterAdd(T item);

        void BeforeRemove(T item);

        void AfterRemove(T item);
    }
}
