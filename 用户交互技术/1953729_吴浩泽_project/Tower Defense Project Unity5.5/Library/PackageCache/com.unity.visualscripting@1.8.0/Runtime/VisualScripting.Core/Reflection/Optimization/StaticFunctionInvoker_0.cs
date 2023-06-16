using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public sealed class StaticFunctionInvoker<TResult> : StaticFunctionInvokerBase<TResult>
    {
        public StaticFunctionInvoker(MethodInfo methodInfo) : base(methodInfo) { }

        private Func<TResult> invoke;

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
            return invoke.Invoke();
        }

        protected override Type[] GetParameterTypes()
        {
            return Type.EmptyTypes;
        }

        protected override void CompileExpression(MethodCallExpression callExpression, ParameterExpression[] parameterExpressions)
        {
            invoke = Expression.Lambda<Func<TResult>>(callExpression, parameterExpressions).Compile();
        }

        protected override void CreateDelegate()
        {
            invoke = () => ((Func<TResult>)methodInfo.CreateDelegate(typeof(Func<TResult>)))();
        }
    }
}
