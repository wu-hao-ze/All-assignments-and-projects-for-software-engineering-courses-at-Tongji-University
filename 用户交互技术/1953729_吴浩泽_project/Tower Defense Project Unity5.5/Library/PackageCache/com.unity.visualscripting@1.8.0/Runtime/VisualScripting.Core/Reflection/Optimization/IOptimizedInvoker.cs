namespace Unity.VisualScripting
{
    public interface IOptimizedInvoker
    {
        void Compile();
        object Invoke(object target);
        object Invoke(object target, object arg0);
        object Invoke(object target, object arg0, object arg1);
        object Invoke(object target, object arg0, object arg1, object arg2);
        object Invoke(object target, object arg0, object arg1, object arg2, object arg3);
        object Invoke(object target, object arg0, object arg1, object arg2, object arg3, object arg4);
        object Invoke(object target, params object[] args);
    }
}
