using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class InstanceActionInvokerBase<TTarget> : InstanceInvokerBase<TTarget>
    {
        protected InstanceActionInvokerBase(MethodInfo methodInfo) : base(methodInfo) { }
    }
}
