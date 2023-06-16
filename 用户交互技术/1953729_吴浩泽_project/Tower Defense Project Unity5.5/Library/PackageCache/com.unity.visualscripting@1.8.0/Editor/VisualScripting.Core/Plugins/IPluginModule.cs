namespace Unity.VisualScripting
{
    public interface IPluginModule : IPluginLinked
    {
        void Initialize();
        void LateInitialize();
    }
}
