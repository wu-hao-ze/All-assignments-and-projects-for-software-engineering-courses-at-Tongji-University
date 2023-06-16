using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class StaticActionInvokerBase : StaticInvokerBase
    {
        protected StaticActionInvokerBase(MethodInfo methodInfo) : base(methodInfo) { }
    }
}
