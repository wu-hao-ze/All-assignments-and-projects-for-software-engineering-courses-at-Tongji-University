using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public sealed class InstanceFunctionInvoker<TTarget, TResult> : InstanceFunctionInvokerBase<TTarget, TResult>
    {
        public InstanceFunctionInvoker(MethodInfo methodInfo) : base(methodInfo) { }

        private Func<TTarget, TResult> invoke;

        public override object Invoke(object target, params object[] args)
        {
            if (args.Length != 0)
            {
                throw new TargetParameterCountException();
            }

            return Invoke(target);
        }

        public override object Invoke(object target)
        {
            if (OptimizedReflection.safeMode)
            {
                VerifyTarget(target);

                try
                {
                    return InvokeUnsafe(target);
                }
                catch (TargetInvocationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new TargetInvocationException(ex);
                }
            }
            else
            {
                return InvokeUnsafe(target);
            }
        }

        public object InvokeUnsafe(object target)
        {
            return invoke.Invoke((TTarget)target);
        }

        protected override Type[] GetParameterTypes()
        {
            return Type.EmptyTypes;
        }

        protected override void CompileExpression(MethodCallExpression callExpression, ParameterExpression[] parameterExpressions)
        {
            invoke = Expression.Lambda<Func<TTarget, TResult>>(callExpression, parameterExpressions).Compile();
        }

        protected override void CreateDelegate()
        {
            invoke = (Func<TTarget, TResult>)methodInfo.CreateDelegate(typeof(Func<TTarget, TResult>));
        }
    }
}
