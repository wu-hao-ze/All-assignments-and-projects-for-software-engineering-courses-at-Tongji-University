namespace Unity.VisualScripting
{
    public interface IDefaultValue<out T>
    {
        T defaultValue { get; }
    }
}
