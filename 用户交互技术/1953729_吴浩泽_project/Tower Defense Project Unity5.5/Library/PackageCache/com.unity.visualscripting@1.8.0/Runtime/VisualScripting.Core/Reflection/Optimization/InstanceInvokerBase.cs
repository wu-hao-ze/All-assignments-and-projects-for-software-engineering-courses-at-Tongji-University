using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class InstanceInvokerBase<TTarget> : InvokerBase
    {
        protected InstanceInvokerBase(MethodInfo methodInfo) : base(methodInfo)
        {
            if (OptimizedReflection.safeMode)
            {
                if (methodInfo.DeclaringType != typeof(TTarget))
                {
                    throw new ArgumentException("Declaring type of method info doesn't match generic type.", nameof(methodInfo));
                }

                if (methodInfo.IsStatic)
                {
                    throw new ArgumentException("The method is static.", nameof(methodInfo));
                }
            }
        }

        protected sealed override void CompileExpression()
        {
            var targetExpression = Expression.Parameter(typeof(TTarget), "target");

            var parameterExpressions = GetParameterExpressions();

            var parameterExpressionsIncludingTarget = new ParameterExpression[1 + parameterExpressions.Length];
            parameterExpressionsIncludingTarget[0] = targetExpression;
            Array.Copy(parameterExpressions, 0, parameterExpressionsIncludingTarget, 1, parameterExpressions.Length);

            var callExpression = Expression.Call(targetExpression, methodInfo, parameterExpressions);

            CompileExpression(callExpression, parameterExpressionsIncludingTarget);
        }

        protected abstract void CompileExpression(MethodCallExpression callExpression, ParameterExpression[] parameterExpressions);

        protected override void VerifyTarget(object target)
        {
            OptimizedReflection.VerifyInstanceTarget<TTarget>(target);
        }
    }
}
