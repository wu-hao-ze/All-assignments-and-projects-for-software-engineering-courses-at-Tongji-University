namespace Unity.VisualScripting
{
    public interface IPoolable
    {
        void New();
        void Free();
    }
}
