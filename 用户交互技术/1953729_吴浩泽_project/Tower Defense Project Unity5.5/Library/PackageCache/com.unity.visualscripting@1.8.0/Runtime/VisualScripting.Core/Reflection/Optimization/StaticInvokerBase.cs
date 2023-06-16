using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class StaticInvokerBase : InvokerBase
    {
        protected StaticInvokerBase(MethodInfo methodInfo) : base(methodInfo)
        {
            if (OptimizedReflection.safeMode)
            {
                if (!methodInfo.IsStatic)
                {
                    throw new ArgumentException("The method isn't static.", nameof(methodInfo));
                }
            }
        }

        protected sealed override void CompileExpression()
        {
            var parameterExpressions = GetParameterExpressions();
            var callExpression = Expression.Call(methodInfo, parameterExpressions);

            CompileExpression(callExpression, parameterExpressions);
        }

        protected abstract void CompileExpression(MethodCallExpression callExpression, ParameterExpression[] parameterExpressions);

        protected override void VerifyTarget(object target)
        {
            OptimizedReflection.VerifyStaticTarget(targetType, target);
        }
    }
}
