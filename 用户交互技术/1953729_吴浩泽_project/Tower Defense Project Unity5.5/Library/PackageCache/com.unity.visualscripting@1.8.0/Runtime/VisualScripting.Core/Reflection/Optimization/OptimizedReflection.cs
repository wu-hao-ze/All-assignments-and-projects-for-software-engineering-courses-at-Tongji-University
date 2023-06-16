using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    // Inspirations:
    // http://stackoverflow.com/a/26733318
    // http://stackoverflow.com/a/16136854
    // http://stackoverflow.com/a/321686

    public static class OptimizedReflection
    {
        static OptimizedReflection()
        {
            fieldAccessors = new Dictionary<FieldInfo, IOptimizedAccessor>();
            propertyAccessors = new Dictionary<PropertyInfo, IOptimizedAccessor>();
            methodInvokers = new Dictionary<MethodInfo, IOptimizedInvoker>();

            jitAvailable = PlatformUtility.supportsJit;
        }

        private static readonly Dictionary<FieldInfo, IOptimizedAccessor> fieldAccessors;
        private static readonly Dictionary<PropertyInfo, IOptimizedAccessor> propertyAccessors;
        private static readonly Dictionary<MethodInfo, IOptimizedInvoker> methodInvokers;

        public static readonly bool jitAvailable;

        private static bool _useJitIfAvailable = true;

        internal static bool useJit => useJitIfAvailable && jitAvailable;

        public static bool useJitIfAvailable
        {
            get
            {
                return _useJitIfAvailable;
            }
            set
            {
                _useJitIfAvailable = value;
                ClearCache();
            }
        }

        public static bool safeMode { get; set; }

        internal static void OnRuntimeMethodLoad()
        {
            safeMode = Application.isEditor || Debug.isDebugBuild;
        }

        public static void ClearCache()
        {
            fieldAccessors.Clear();
            propertyAccessors.Clear();
            methodInvokers.Clear();
        }

        internal static void VerifyStaticTarget(Type targetType, object target)
        {
            VerifyTarget(targetType, target, true);
        }

        internal static void VerifyInstanceTarget<TTArget>(object target)
        {
            VerifyTarget(typeof(TTArget), target, false);
        }

        private static void VerifyTarget(Type targetType, object target, bool @static)
        {
            Ensure.That(nameof(targetType)).IsNotNull(targetType);

            if (@static)
            {
                if (target != null)
                {
                    throw new TargetException($"Superfluous target object for '{targetType}'.");
                }
            }
            else
            {
                if (target == null)
                {
                    throw new TargetException($"Missing target object for '{targetType}'.");
                }

                if (!targetType.IsAssignableFrom(targetType))
                {
                    throw new TargetException($"The target object does not match the target type.\nProvided: {target.GetType()}\nExpected: {targetType}");
                }
            }
        }

        private static bool SupportsOptimization(MemberInfo memberInfo)
        {
            // Instance members on value types require by-ref passing of the target object:
            // https://stackoverflow.com/a/1212396/154502

            // However, a bug in Unity's Mono version prevents by-ref delegates from working:
            // http://stackoverflow.com/questions/34743176/#comment73561434_34744241

            // Therefore, instance members on value types have to use reflection.

            if (memberInfo.DeclaringType.IsValueType && !memberInfo.IsStatic())
            {
                return false;
            }

            return true;
        }

        #region Fields

        public static IOptimizedAccessor Prewarm(this FieldInfo fieldInfo)
        {
            return GetFieldAccessor(fieldInfo);
        }

        public static object GetValueOptimized(this FieldInfo fieldInfo, object target)
        {
            return GetFieldAccessor(fieldInfo).GetValue(target);
        }

        public static void SetValueOptimized(this FieldInfo fieldInfo, object target, object value)
        {
            GetFieldAccessor(fieldInfo).SetValue(target, value);
        }

        public static bool SupportsOptimization(this FieldInfo fieldInfo)
        {
            if (!SupportsOptimization((MemberInfo)fieldInfo))
            {
                return false;
            }

            return true;
        }

        private static IOptimizedAccessor GetFieldAccessor(FieldInfo fieldInfo)
        {
            Ensure.That(nameof(fieldInfo)).IsNotNull(fieldInfo);

            lock (fieldAccessors)
            {
                if (!fieldAccessors.TryGetValue(fieldInfo, out var accessor))
                {
                    if (SupportsOptimization(fieldInfo))
                    {
                        Type accessorType;

                        if (fieldInfo.IsStatic)
                        {
                            accessorType = typeof(StaticFieldAccessor<>).MakeGenericType(fieldInfo.FieldType);
                        }
                        else
                        {
                            accessorType = typeof(InstanceFieldAccessor<,>).MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType);
                        }

                        accessor = (IOptimizedAccessor)Activator.CreateInstance(accessorType, fieldInfo);
                    }
                    else
                    {
                        accessor = new ReflectionFieldAccessor(fieldInfo);
                    }

                    accessor.Compile();

                    fieldAccessors.Add(fieldInfo, accessor);
                }

                return accessor;
            }
        }

        #endregion

        #region Properties

        public static IOptimizedAccessor Prewarm(this PropertyInfo propertyInfo)
        {
            return GetPropertyAccessor(propertyInfo);
        }

        public static object GetValueOptimized(this PropertyInfo propertyInfo, object target)
        {
            return GetPropertyAccessor(propertyInfo).GetValue(target);
        }

        public static void SetValueOptimized(this PropertyInfo propertyInfo, object target, object value)
        {
            GetPropertyAccessor(propertyInfo).SetValue(target, value);
        }

        public static bool SupportsOptimization(this PropertyInfo propertyInfo)
        {
            if (!SupportsOptimization((MemberInfo)propertyInfo))
            {
                return false;
            }

            return true;
        }

        private static IOptimizedAccessor GetPropertyAccessor(PropertyInfo propertyInfo)
        {
            Ensure.That(nameof(propertyInfo)).IsNotNull(propertyInfo);

            lock (propertyAccessors)
            {
                if (!propertyAccessors.TryGetValue(propertyInfo, out var accessor))
                {
                    if (SupportsOptimization(propertyInfo))
                    {
                        Type accessorType;

                        if (propertyInfo.IsStatic())
                        {
                            accessorType = typeof(StaticPropertyAccessor<>).MakeGenericType(propertyInfo.PropertyType);
                        }
                        else
                        {
                            accessorType = typeof(InstancePropertyAccessor<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
                        }

                        accessor = (IOptimizedAccessor)Activator.CreateInstance(accessorType, propertyInfo);
                    }
                    else
                    {
                        accessor = new ReflectionPropertyAccessor(propertyInfo);
                    }

                    accessor.Compile();

                    propertyAccessors.Add(propertyInfo, accessor);
                }

                return accessor;
            }
        }

        #endregion

        #region Methods

        public static IOptimizedInvoker Prewarm(this MethodInfo methodInfo)
        {
            return GetMethodInvoker(methodInfo);
        }

        public static object InvokeOptimized(this MethodInfo methodInfo, object target, params object[] args)
        {
            return GetMethodInvoker(methodInfo).Invoke(target, args);
        }

        public static object InvokeOptimized(this MethodInfo methodInfo, object target)
        {
            return GetMethodInvoker(methodInfo).Invoke(target);
        }

        public static object InvokeOptimized(this MethodInfo methodInfo, object target, object arg0)
        {
            return GetMethodInvoker(methodInfo).Invoke(target, arg0);
        }

        public static object InvokeOptimized(this MethodInfo methodInfo, object target, object arg0, object arg1)
        {
            return GetMethodInvoker(methodInfo).Invoke(target, arg0, arg1);
        }

        public static object InvokeOptimized(this MethodInfo methodInfo, object target, object arg0, object arg1, object arg2)
        {
            return GetMethodInvoker(methodInfo).Invoke(target, arg0, arg1, arg2);
        }

        public static object InvokeOptimized(this MethodInfo methodInfo, object target, object arg0, object arg1, object arg2, object arg3)
        {
            return GetMethodInvoker(methodInfo).Invoke(target, arg0, arg1, arg2, arg3);
        }

        public static object InvokeOptimized(this MethodInfo methodInfo, object target, object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            return GetMethodInvoker(methodInfo).Invoke(target, arg0, arg1, arg2, arg3, arg4);
        }

        public static bool SupportsOptimization(this MethodInfo methodInfo)
        {
            if (!SupportsOptimization((MemberInfo)methodInfo))
            {
                return false;
            }

            var parameters = methodInfo.GetParameters();

            if (parameters.Length > 5)
            {
                return false;
            }

            if (parameters.Any(parameter => parameter.ParameterType.IsByRef))
            {
                return false;
            }

            // CreateDelegate in IL2CPP does not work properly for overridden methods, instead referring to the virtual method.
            // https://support.ludiq.io/forums/5-bolt/topics/872-virtual-method-overrides-not-used-on-aot/
            // https://fogbugz.unity3d.com/default.asp?980136_228np3be9idtbdtt
            if (!jitAvailable && methodInfo.IsVirtual && !methodInfo.IsFinal)
            {
                return false;
            }

            // Undocumented __arglist keyword as used in the 4+ overload of String.Concat causes runtime crash
            if (methodInfo.CallingConvention == CallingConventions.VarArgs)
            {
                return false;
            }

            return true;
        }

        private static IOptimizedInvoker GetMethodInvoker(MethodInfo methodInfo)
        {
            Ensure.That(nameof(methodInfo)).IsNotNull(methodInfo);

            lock (methodInvokers)
            {
                if (!methodInvokers.TryGetValue(methodInfo, out var invoker))
                {
                    if (SupportsOptimization(methodInfo))
                    {
                        Type invokerType;

                        var parameters = methodInfo.GetParameters();

                        if (methodInfo.ReturnType == typeof(void))
                        {
                            if (methodInfo.IsStatic)
                            {
                                if (parameters.Length == 0)
                                {
                                    invokerType = typeof(StaticActionInvoker);
                                }
                                else if (parameters.Length == 1)
                                {
                                    invokerType = typeof(StaticActionInvoker<>).MakeGenericType(parameters[0].ParameterType);
                                }
                                else if (parameters.Length == 2)
                                {
                                    invokerType = typeof(StaticActionInvoker<,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType);
                                }
                                else if (parameters.Length == 3)
                                {
                                    invokerType = typeof(StaticActionInvoker<,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType);
                                }
                                else if (parameters.Length == 4)
                                {
                                    invokerType = typeof(StaticActionInvoker<,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType);
                                }
                                else if (parameters.Length == 5)
                                {
                                    invokerType = typeof(StaticActionInvoker<,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType);
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                            }
                            else
                            {
                                if (parameters.Length == 0)
                                {
                                    invokerType = typeof(InstanceActionInvoker<>).MakeGenericType(methodInfo.DeclaringType);
                                }
                                else if (parameters.Length == 1)
                                {
                                    invokerType = typeof(InstanceActionInvoker<,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType);
                                }
                                else if (parameters.Length == 2)
                                {
                                    invokerType = typeof(InstanceActionInvoker<,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType);
                                }
                                else if (parameters.Length == 3)
                                {
                                    invokerType = typeof(InstanceActionInvoker<,,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType);
                                }
                                else if (parameters.Length == 4)
                                {
                                    invokerType = typeof(InstanceActionInvoker<,,,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType);
                                }
                                else if (parameters.Length == 5)
                                {
                                    invokerType = typeof(InstanceActionInvoker<,,,,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType);
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                            }
                        }
                        else
                        {
                            if (methodInfo.IsStatic)
                            {
                                if (parameters.Length == 0)
                                {
                                    invokerType = typeof(StaticFunctionInvoker<>).MakeGenericType(methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 1)
                                {
                                    invokerType = typeof(StaticFunctionInvoker<,>).MakeGenericType(parameters[0].ParameterType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 2)
                                {
                                    invokerType = typeof(StaticFunctionInvoker<,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 3)
                                {
                                    invokerType = typeof(StaticFunctionInvoker<,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 4)
                                {
                                    invokerType = typeof(StaticFunctionInvoker<,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 5)
                                {
                                    invokerType = typeof(StaticFunctionInvoker<,,,,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType, methodInfo.ReturnType);
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                            }
                            else
                            {
                                if (parameters.Length == 0)
                                {
                                    invokerType = typeof(InstanceFunctionInvoker<,>).MakeGenericType(methodInfo.DeclaringType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 1)
                                {
                                    invokerType = typeof(InstanceFunctionInvoker<,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 2)
                                {
                                    invokerType = typeof(InstanceFunctionInvoker<,,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 3)
                                {
                                    invokerType = typeof(InstanceFunctionInvoker<,,,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 4)
                                {
                                    invokerType = typeof(InstanceFunctionInvoker<,,,,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, methodInfo.ReturnType);
                                }
                                else if (parameters.Length == 5)
                                {
                                    invokerType = typeof(InstanceFunctionInvoker<,,,,,,>).MakeGenericType(methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType, methodInfo.ReturnType);
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                            }
                        }

                        invoker = (IOptimizedInvoker)Activator.CreateInstance(invokerType, methodInfo);
                    }
                    else
                    {
                        invoker = new ReflectionInvoker(methodInfo);
                    }

                    invoker.Compile();

                    methodInvokers.Add(methodInfo, invoker);
                }

                return invoker;
            }
        }

        #endregion
    }
}
