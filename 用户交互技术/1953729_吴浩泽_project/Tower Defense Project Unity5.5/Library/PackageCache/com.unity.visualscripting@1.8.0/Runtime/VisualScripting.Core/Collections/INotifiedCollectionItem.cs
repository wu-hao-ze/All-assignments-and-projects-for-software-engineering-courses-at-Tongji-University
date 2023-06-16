namespace Unity.VisualScripting
{
    public interface INotifiedCollectionItem
    {
        void BeforeAdd();

        void AfterAdd();

        void BeforeRemove();

        void AfterRemove();
    }
}
