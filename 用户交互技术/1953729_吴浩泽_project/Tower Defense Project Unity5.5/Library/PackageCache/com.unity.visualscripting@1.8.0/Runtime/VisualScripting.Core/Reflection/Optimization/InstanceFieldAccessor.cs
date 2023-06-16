using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class InstanceFieldAccessor<TTarget, TField> : IOptimizedAccessor
    {
        public InstanceFieldAccessor(FieldInfo fieldInfo)
        {
            if (OptimizedReflection.safeMode)
            {
                Ensure.That(nameof(fieldInfo)).IsNotNull(fieldInfo);

                if (fieldInfo.DeclaringType != typeof(TTarget))
                {
                    throw new ArgumentException("Declaring type of field info doesn't match generic type.", nameof(fieldInfo));
                }

                if (fieldInfo.FieldType != typeof(TField))
                {
                    throw new ArgumentException("Field type of field info doesn't match generic type.", nameof(fieldInfo));
                }

                if (fieldInfo.IsStatic)
                {
                    throw new ArgumentException("The field is static.", nameof(fieldInfo));
                }
            }

            this.fieldInfo = fieldInfo;
        }

        private readonly FieldInfo fieldInfo;
        private Func<TTarget, TField> getter;
        private Action<TTarget, TField> setter;

        public void Compile()
        {
            if (OptimizedReflection.useJit)
            {
                var targetExpression = Expression.Parameter(typeof(TTarget), "target");

                // Getter

                var fieldExpression = Expression.Field(targetExpression, fieldInfo);
                getter = Expression.Lambda<Func<TTarget, TField>>(fieldExpression, targetExpression).Compile();

                // Setter

                if (fieldInfo.CanWrite())
                {
#if UNITY_2018_3_OR_NEWER
                    try
                    {
                        var valueExpression = Expression.Parameter(typeof(TField));
                        var assignExpression = Expression.Assign(fieldExpression, valueExpression);
                        setter = Expression.Lambda<Action<TTarget, TField>>(assignExpression, targetExpression, valueExpression).Compile();
                    }
                    catch
                    {
                        Debug.Log("Failed instance field: " + fieldInfo);
                        throw;
                    }
#else
                    var setterMethod = new DynamicMethod
                        (
                        "setter",
                        typeof(void),
                        new[] { typeof(TTarget), typeof(TField) },
                        typeof(TTarget),
                        true
                        );

                    var setterIL = setterMethod.GetILGenerator();

                    setterIL.Emit(OpCodes.Ldarg_0);
                    setterIL.Emit(OpCodes.Ldarg_1);
                    setterIL.Emit(OpCodes.Stfld, fieldInfo);
                    setterIL.Emit(OpCodes.Ret);

                    setter = (Action<TTarget, TField>)setterMethod.CreateDelegate(typeof(Action<TTarget, TField>));
#endif
                }
            }
            else
            {
                getter = (instance) => (TField)fieldInfo.GetValue(instance);

                if (fieldInfo.CanWrite())
                {
                    setter = (instance, value) => fieldInfo.SetValue(instance, value);
                }
            }
        }

        public object GetValue(object target)
        {
            if (OptimizedReflection.safeMode)
            {
                OptimizedReflection.VerifyInstanceTarget<TTarget>(target);

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
            // No need for special handling of value types, because field accessing cannot have side effects.
            // Therefore, working on a copy of the instance is faster and equivalent.

            return getter.Invoke((TTarget)target);
        }

        public void SetValue(object target, object value)
        {
            if (OptimizedReflection.safeMode)
            {
                OptimizedReflection.VerifyInstanceTarget<TTarget>(target);

                if (setter == null)
                {
                    throw new TargetException($"The field '{typeof(TTarget)}.{fieldInfo.Name}' cannot be assigned.");
                }

                if (!typeof(TField).IsAssignableFrom(value))
                {
                    throw new ArgumentException($"The provided value for '{typeof(TTarget)}.{fieldInfo.Name}' does not match the field type.\nProvided: {value?.GetType()?.ToString() ?? "null"}\nExpected: {typeof(TField)}");
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
            setter.Invoke((TTarget)target, (TField)value);
        }
    }
}
