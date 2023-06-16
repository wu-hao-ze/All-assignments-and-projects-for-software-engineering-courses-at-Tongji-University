using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public sealed class InstanceActionInvoker<TTarget> : InstanceActionInvokerBase<TTarget>
    {
        public InstanceActionInvoker(MethodInfo methodInfo) : base(methodInfo) { }

        private Action<TTarget> invoke;

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

        private object InvokeUnsafe(object target)
        {
            invoke.Invoke((TTarget)target);

            return null;
        }

        protected override Type[] GetParameterTypes()
        {
            return Type.EmptyTypes;
        }

        protected override void CompileExpression(MethodCallExpression callExpression, ParameterExpression[] parameterExpressions)
        {
            invoke = Expression.Lambda<Action<TTarget>>(callExpression, parameterExpressions).Compile();
        }

        protected override void CreateDelegate()
        {
            invoke = (Action<TTarget>)methodInfo.CreateDelegate(typeof(Action<TTarget>));
        }
    }
}
