using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public sealed class StaticFunctionInvoker<TParam0, TResult> : StaticFunctionInvokerBase<TResult>
    {
        public StaticFunctionInvoker(MethodInfo methodInfo) : base(methodInfo) { }

        private Func<TParam0, TResult> invoke;

        public override object Invoke(object target, params object[] args)
        {
            if (args.Length != 1)
            {
                throw new TargetParameterCountException();
            }

            return Invoke(target, args[0]);
        }

        public override object Invoke(object target, object arg0)
        {
            if (OptimizedReflection.safeMode)
            {
                VerifyTarget(target);
                VerifyArgument<TParam0>(methodInfo, 0, arg0);

                try
                {
                    return InvokeUnsafe(target, arg0);
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
                return InvokeUnsafe(target, arg0);
            }
        }

        public object InvokeUnsafe(object target, object arg0)
        {
            return invoke.Invoke((TParam0)arg0);
        }

        protected override Type[] GetParameterTypes()
        {
            return new[] { typeof(TParam0) };
        }

        protected override void CompileExpression(MethodCallExpression callExpression, ParameterExpression[] parameterExpressions)
        {
            invoke = Expression.Lambda<Func<TParam0, TResult>>(callExpression, parameterExpressions).Compile();
        }

        protected override void CreateDelegate()
        {
            invoke = param0 => ((Func<TParam0, TResult>)methodInfo.CreateDelegate(typeof(Func<TParam0, TResult>)))(param0);
        }
    }
}
