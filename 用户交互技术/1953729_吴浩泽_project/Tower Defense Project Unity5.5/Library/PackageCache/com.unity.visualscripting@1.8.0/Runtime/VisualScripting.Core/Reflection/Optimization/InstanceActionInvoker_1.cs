using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public sealed class InstanceActionInvoker<TTarget, TParam0> : InstanceActionInvokerBase<TTarget>
    {
        public InstanceActionInvoker(MethodInfo methodInfo) : base(methodInfo) { }

        private Action<TTarget, TParam0> invoke;

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

        private object InvokeUnsafe(object target, object arg0)
        {
            invoke.Invoke((TTarget)target, (TParam0)arg0);

            return null;
        }

        protected override Type[] GetParameterTypes()
        {
            return new[] { typeof(TParam0) };
        }

        protected override void CompileExpression(MethodCallExpression callExpression, ParameterExpression[] parameterExpressions)
        {
            invoke = Expression.Lambda<Action<TTarget, TParam0>>(callExpression, parameterExpressions).Compile();
        }

        protected override void CreateDelegate()
        {
            invoke = (Action<TTarget, TParam0>)methodInfo.CreateDelegate(typeof(Action<TTarget, TParam0>));
        }
    }
}
