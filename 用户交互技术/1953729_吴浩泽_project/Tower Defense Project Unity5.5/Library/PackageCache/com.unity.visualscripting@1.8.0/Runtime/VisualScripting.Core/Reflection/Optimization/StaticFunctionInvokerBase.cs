using System;
using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class StaticFunctionInvokerBase<TResult> : StaticInvokerBase
    {
        protected StaticFunctionInvokerBase(MethodInfo methodInfo) : base(methodInfo)
        {
            if (OptimizedReflection.safeMode)
            {
                if (methodInfo.ReturnType != typeof(TResult))
                {
                    throw new ArgumentException("Return type of method info doesn't match generic type.", nameof(methodInfo));
                }
            }
        }
    }
}
