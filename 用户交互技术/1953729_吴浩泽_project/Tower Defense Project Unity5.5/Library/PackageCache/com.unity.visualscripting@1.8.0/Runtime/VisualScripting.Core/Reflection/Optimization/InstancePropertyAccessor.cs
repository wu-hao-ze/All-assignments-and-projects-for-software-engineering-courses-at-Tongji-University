using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.VisualScripting
{
    public class InstancePropertyAccessor<TTarget, TProperty> : IOptimizedAccessor
    {
        public InstancePropertyAccessor(PropertyInfo propertyInfo)
        {
            if (OptimizedReflection.safeMode)
            {
                Ensure.That(nameof(propertyInfo)).IsNotNull(propertyInfo);

                if (propertyInfo.DeclaringType != typeof(TTarget))
                {
                    throw new ArgumentException("The declaring type of the property info doesn't match the generic type.", nameof(propertyInfo));
                }

                if (propertyInfo.PropertyType != typeof(TProperty))
                {
                    throw new ArgumentException("The property type of the property info doesn't match the generic type.", nameof(propertyInfo));
                }

                if (propertyInfo.IsStatic())
                {
                    throw new ArgumentException("The property is static.", nameof(propertyInfo));
                }
            }

            this.propertyInfo = propertyInfo;
        }

        private readonly PropertyInfo propertyInfo;
        private Func<TTarget, TProperty> getter;
        private Action<TTarget, TProperty> setter;

        public void Compile()
        {
            var getterInfo = propertyInfo.GetGetMethod(true);
            var setterInfo = propertyInfo.GetSetMethod(true);

            if (OptimizedReflection.useJit)
            {
                var targetExpression = Expression.Parameter(typeof(TTarget), "target");

                if (getterInfo != null)
                {
                    var propertyExpression = Expression.Property(targetExpression, propertyInfo);
                    getter = Expression.Lambda<Func<TTarget, TProperty>>(propertyExpression, targetExpression).Compile();
                }

                if (setterInfo != null)
                {
                    setter = (Action<TTarget, TProperty>)setterInfo.CreateDelegate(typeof(Action<TTarget, TProperty>));
                }
            }
            else
            {
                if (getterInfo != null)
                {
                    getter = (Func<TTarget, TProperty>)getterInfo.CreateDelegate(typeof(Func<TTarget, TProperty>));
                }

                if (setterInfo != null)
                {
                    setter = (Action<TTarget, TProperty>)setterInfo.CreateDelegate(typeof(Action<TTarget, TProperty>));
                }
            }
        }

        public object GetValue(object target)
        {
            if (OptimizedReflection.safeMode)
            {
                OptimizedReflection.VerifyInstanceTarget<TTarget>(target);

                if (getter == null)
                {
                    throw new TargetException($"The property '{typeof(TTarget)}.{propertyInfo.Name}' has no get accessor.");
                }

                try
                {
                    return GetValueUnsafe(target);
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
                return GetValueUnsafe(target);
            }
        }

        private object GetValueUnsafe(object target)
        {
            return getter.Invoke((TTarget)target);
        }

        public void SetValue(object target, object value)
        {
            if (OptimizedReflection.safeMode)
            {
                OptimizedReflection.VerifyInstanceTarget<TTarget>(target);

                if (setter == null)
                {
                    throw new TargetException($"The property '{typeof(TTarget)}.{propertyInfo.Name}' has no set accessor.");
                }

                if (!typeof(TProperty).IsAssignableFrom(value))
                {
                    throw new ArgumentException($"The provided value for '{typeof(TTarget)}.{propertyInfo.Name}' does not match the property type.\nProvided: {value?.GetType()?.ToString() ?? "null"}\nExpected: {typeof(TProperty)}");
                }

                try
                {
                    SetValueUnsafe(target, value);
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
                SetValueUnsafe(target, value);
            }
        }

        private void SetValueUnsafe(object target, object value)
        {
            setter.Invoke((TTarget)target, (TProperty)value);
        }
    }
}
