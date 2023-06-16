using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public sealed class StaticActionInvoker<TParam0, TParam1> : StaticActionInvokerBase
    {
        public StaticActionInvoker(MethodInfo methodInfo) : base(methodInfo) { }

        private Action<TParam0, TParam1> invoke;

        public override object Invoke(object target, params object[] args)
        {
            if (args.Length != 2)
            {
                throw new TargetParameterCountException();
            }

            return Invoke(target, args[0], args[1]);
        }

        public override object Invoke(object target, object arg0, object arg1)
        {
            if (OptimizedReflection.safeMode)
            {
                VerifyTarget(target);
                VerifyArgument<TParam0>(methodInfo, 0, arg0);
                VerifyArgument<TParam1>(methodInfo, 0, arg1);

                try
                {
                    return InvokeUnsafe(target, arg0, arg1);
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
                return InvokeUnsafe(target, arg0, arg1);
            }
        }

        public object InvokeUnsafe(object target, object arg0, object arg1)
        {
            invoke.Invoke((TParam0)arg0, (TParam1)arg1);

            return null;
        }

        protected override Type[] GetParameterTypes()
        {
            return new[] { typeof(TParam0), typeof(TParam1) };
        }

        protected override void CompileExpression(MethodCallExpression callExpression, ParameterExpression[] parameterExpressions)
        {
            invoke = Expression.Lambda<Action<TParam0, TParam1>>(callExpression, parameterExpressions).Compile();
        }

        protected override void CreateDelegate()
        {
            invoke = (param0, param1) => ((Action<TParam0, TParam1>)methodInfo.CreateDelegate(typeof(Action<TParam0, TParam1>)))(param0, param1);
        }
    }
}
