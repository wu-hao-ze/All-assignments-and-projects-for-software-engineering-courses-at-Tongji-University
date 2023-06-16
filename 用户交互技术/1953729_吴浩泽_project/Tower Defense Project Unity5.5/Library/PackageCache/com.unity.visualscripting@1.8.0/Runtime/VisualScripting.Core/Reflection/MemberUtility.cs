using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class MemberUtility
    {
        static MemberUtility()
        {
            ExtensionMethodsCache = new Lazy<ExtensionMethodCache>(() => new ExtensionMethodCache(), true);
            InheritedExtensionMethodsCache = new Lazy<Dictionary<Type, MethodInfo[]>>(() => new Dictionary<Type, MethodInfo[]>(), true);
            GenericExtensionMethods = new Lazy<HashSet<MethodInfo>>(() => new HashSet<MethodInfo>(), true);
        }

        // The process of resolving generic methods is very expensive.
        // Cache the results for each this parameter type.
        private static readonly Lazy<ExtensionMethodCache> ExtensionMethodsCache;
        private static readonly Lazy<Dictionary<Type, MethodInfo[]>> InheritedExtensionMethodsCache;
        private static readonly Lazy<HashSet<MethodInfo>> GenericExtensionMethods;

        public static bool IsOperator(this MethodInfo method)
        {
            return method.IsSpecialName && OperatorUtility.operatorNames.ContainsKey(method.Name);
        }

        public static bool IsUserDefinedConversion(this MethodInfo method)
        {
            return method.IsSpecialName && (method.Name == "op_Implicit" || method.Name == "op_Explicit");
        }

        /// <remarks>This may return an open-constructed method as well.</remarks>
        public static MethodInfo MakeGenericMethodVia(this MethodInfo openConstructedMethod, params Type[] closedConstructedParameterTypes)
        {
            Ensure.That(nameof(openConstructedMethod)).IsNotNull(openConstructedMethod);
            Ensure.That(nameof(closedConstructedParameterTypes)).IsNotNull(closedConstructedParameterTypes);

            if (!openConstructedMethod.ContainsGenericParameters)
            {
                // The method contains no generic parameters,
                // it is by definition already resolved.
                return openConstructedMethod;
            }

            var openConstructedParameterTypes = openConstructedMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (openConstructedParameterTypes.Length != closedConstructedParameterTypes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(closedConstructedParameterTypes));
            }

            var resolvedGenericParameters = new Dictionary<Type, Type>();

            for (var i = 0; i < openConstructedParameterTypes.Length; i++)
            {
                // Resolve each open-constructed parameter type via the equivalent
                // closed-constructed parameter type.

                var openConstructedParameterType = openConstructedParameterTypes[i];
                var closedConstructedParameterType = closedConstructedParameterTypes[i];

                openConstructedParameterType.MakeGenericTypeVia(closedConstructedParameterType, resolvedGenericParameters);
            }

            // Construct the final closed-constructed method from the resolved arguments

            var openConstructedGenericArguments = openConstructedMethod.GetGenericArguments();
            var closedConstructedGenericArguments = openConstructedGenericArguments.Select(openConstructedGenericArgument =>
            {
                // If the generic argument has been successfully resolved, use it;
                // otherwise, leave the open-constructed argument in place.

                if (resolvedGenericParameters.ContainsKey(openConstructedGenericArgument))
                {
                    return resolvedGenericParameters[openConstructedGenericArgument];
                }
                else
                {
                    return openConstructedGenericArgument;
                }
            }).ToArray();

            return openConstructedMethod.MakeGenericMethod(closedConstructedGenericArguments);
        }

        public static bool IsGenericExtension(this MethodInfo methodInfo)
        {
            return GenericExtensionMethods.Value.Contains(methodInfo);
        }

        private static IEnumerable<MethodInfo> GetInheritedExtensionMethods(Type thisArgumentType)
        {
            var methodInfos = ExtensionMethodsCache.Value.Cache;
            foreach (var extensionMethod in methodInfos)
            {
                var compatibleThis = extensionMethod.GetParameters()[0].ParameterType.CanMakeGenericTypeVia(thisArgumentType);

                if (compatibleThis)
                {
                    if (extensionMethod.ContainsGenericParameters)
                    {
                        var closedConstructedParameterTypes = thisArgumentType.Yield().Concat(extensionMethod.GetParametersWithoutThis().Select(p => p.ParameterType));

                        var closedConstructedMethod = extensionMethod.MakeGenericMethodVia(closedConstructedParameterTypes.ToArray());

                        GenericExtensionMethods.Value.Add(closedConstructedMethod);

                        yield return closedConstructedMethod;
                    }
                    else
                    {
                        yield return extensionMethod;
                    }
                }
            }
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(this Type thisArgumentType, bool inherited = true)
        {
            if (inherited)
            {
                lock (InheritedExtensionMethodsCache)
                {
                    if (!InheritedExtensionMethodsCache.Value.TryGetValue(thisArgumentType, out var inheritedExtensionMethods))
                    {
                        inheritedExtensionMethods = GetInheritedExtensionMethods(thisArgumentType).ToArray();
                        InheritedExtensionMethodsCache.Value.Add(thisArgumentType, inheritedExtensionMethods);
                    }

                    return inheritedExtensionMethods;
                }
            }
            else
            {
                var methodInfos = ExtensionMethodsCache.Value.Cache;
                return methodInfos.Where(method => method.GetParameters()[0].ParameterType == thisArgumentType);
            }
        }

        public static bool IsExtension(this MethodInfo methodInfo)
        {
            return methodInfo.HasAttribute<ExtensionAttribute>(false);
        }

        public static bool IsExtensionMethod(this MemberInfo memberInfo)
        {
            return memberInfo is MethodInfo methodInfo && methodInfo.IsExtension();
        }

        public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType)
        {
            return Delegate.CreateDelegate(delegateType, methodInfo);
        }

        public static bool IsAccessor(this MemberInfo memberInfo)
        {
            return memberInfo is FieldInfo || memberInfo is PropertyInfo;
        }

        public static Type GetAccessorType(this MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
            {
                return ((FieldInfo)memberInfo).FieldType;
            }
            else if (memberInfo is PropertyInfo)
            {
                return ((PropertyInfo)memberInfo).PropertyType;
            }
            else
            {
                return null;
            }
        }

        public static bool IsPubliclyGettable(this MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
            {
                return ((FieldInfo)memberInfo).IsPublic;
            }
            else if (memberInfo is PropertyInfo)
            {
                var propertyInfo = (PropertyInfo)memberInfo;

                return propertyInfo.CanRead && propertyInfo.GetGetMethod(false) != null;
            }
            else if (memberInfo is MethodInfo)
            {
                return ((MethodInfo)memberInfo).IsPublic;
            }
            else if (memberInfo is ConstructorInfo)
            {
                return ((ConstructorInfo)memberInfo).IsPublic;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static Type ExtendedDeclaringType(this MemberInfo memberInfo)
        {
            if (memberInfo is MethodInfo methodInfo && methodInfo.IsExtension())
            {
                return methodInfo.GetParameters()[0].ParameterType;
            }
            else
            {
                return memberInfo.DeclaringType;
            }
        }

        public static Type ExtendedDeclaringType(this MemberInfo memberInfo, bool invokeAsExtension)
        {
            if (invokeAsExtension)
            {
                return memberInfo.ExtendedDeclaringType();
            }
            else
            {
                return memberInfo.DeclaringType;
            }
        }

        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            return (propertyInfo.GetGetMethod(true)?.IsStatic ?? false) ||
                (propertyInfo.GetSetMethod(true)?.IsStatic ?? false);
        }

        public static bool IsStatic(this MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
            {
                return ((FieldInfo)memberInfo).IsStatic;
            }
            else if (memberInfo is PropertyInfo)
            {
                return ((PropertyInfo)memberInfo).IsStatic();
            }
            else if (memberInfo is MethodBase)
            {
                return ((MethodBase)memberInfo).IsStatic;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static IEnumerable<ParameterInfo> GetParametersWithoutThis(this MethodBase methodBase)
        {
            return methodBase.GetParameters().Skip(methodBase.IsExtensionMethod() ? 1 : 0);
        }

        public static bool IsInvokedAsExtension(this MethodBase methodBase, Type targetType)
        {
            return methodBase.IsExtensionMethod() && methodBase.DeclaringType != targetType;
        }

        public static IEnumerable<ParameterInfo> GetInvocationParameters(this MethodBase methodBase, bool invokeAsExtension)
        {
            if (invokeAsExtension)
            {
                return methodBase.GetParametersWithoutThis();
            }
            else
            {
                return methodBase.GetParameters();
            }
        }

        public static IEnumerable<ParameterInfo> GetInvocationParameters(this MethodBase methodBase, Type targetType)
        {
            return methodBase.GetInvocationParameters(methodBase.IsInvokedAsExtension(targetType));
        }

        public static Type UnderlyingParameterType(this ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.IsByRef)
            {
                return parameterInfo.ParameterType.GetElementType();
            }
            else
            {
                return parameterInfo.ParameterType;
            }
        }

        // https://stackoverflow.com/questions/9977530/
        // https://stackoverflow.com/questions/16186694
        public static bool HasDefaultValue(this ParameterInfo parameterInfo)
        {
            return (parameterInfo.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault;
        }

        public static object DefaultValue(this ParameterInfo parameterInfo)
        {
            if (parameterInfo.HasDefaultValue())
            {
                var defaultValue = parameterInfo.DefaultValue;

                // https://stackoverflow.com/questions/45393580
                if (defaultValue == null && parameterInfo.ParameterType.IsValueType)
                {
                    defaultValue = parameterInfo.ParameterType.Default();
                }

                return defaultValue;
            }
            else
            {
                return parameterInfo.UnderlyingParameterType().Default();
            }
        }

        public static object PseudoDefaultValue(this ParameterInfo parameterInfo)
        {
            if (parameterInfo.HasDefaultValue())
            {
                var defaultValue = parameterInfo.DefaultValue;

                // https://stackoverflow.com/questions/45393580
                if (defaultValue == null && parameterInfo.ParameterType.IsValueType)
                {
                    defaultValue = parameterInfo.ParameterType.PseudoDefault();
                }

                return defaultValue;
            }
            else
            {
                return parameterInfo.UnderlyingParameterType().PseudoDefault();
            }
        }

        public static bool AllowsNull(this ParameterInfo parameterInfo)
        {
            var type = parameterInfo.ParameterType;

            return (type.IsReferenceType() && parameterInfo.HasAttribute<AllowsNullAttribute>()) || Nullable.GetUnderlyingType(type) != null;
        }

        // https://stackoverflow.com/questions/30102174/
        public static bool HasOutModifier(this ParameterInfo parameterInfo)
        {
            Ensure.That(nameof(parameterInfo)).IsNotNull(parameterInfo);

            // Checking for IsOut is not enough, because parameters marked with the [Out] attribute
            // also return true, while not necessarily having the "out" modifier. This is common for P/Invoke,
            // for example in Unity's ParticleSystem.GetParticles.
            return parameterInfo.IsOut && parameterInfo.ParameterType.IsByRef;
        }

        public static bool CanWrite(this FieldInfo fieldInfo)
        {
            return !(fieldInfo.IsInitOnly || fieldInfo.IsLiteral);
        }

        public static Member ToManipulator(this MemberInfo memberInfo)
        {
            return ToManipulator(memberInfo, memberInfo.DeclaringType);
        }

        public static Member ToManipulator(this MemberInfo memberInfo, Type targetType)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.ToManipulator(targetType);
            }

            if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.ToManipulator(targetType);
            }

            if (memberInfo is MethodInfo methodInfo)
            {
                return methodInfo.ToManipulator(targetType);
            }

            if (memberInfo is ConstructorInfo constructorInfo)
            {
                return constructorInfo.ToManipulator(targetType);
            }

            throw new InvalidOperationException();
        }

        public static Member ToManipulator(this FieldInfo fieldInfo, Type targetType)
        {
            return new Member(targetType, fieldInfo);
        }

        public static Member ToManipulator(this PropertyInfo propertyInfo, Type targetType)
        {
            return new Member(targetType, propertyInfo);
        }

        public static Member ToManipulator(this MethodInfo methodInfo, Type targetType)
        {
            return new Member(targetType, methodInfo);
        }

        public static Member ToManipulator(this ConstructorInfo constructorInfo, Type targetType)
        {
            return new Member(targetType, constructorInfo);
        }

        public static ConstructorInfo GetConstructorAccepting(this Type type, Type[] paramTypes, bool nonPublic)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            if (nonPublic)
            {
                bindingFlags |= BindingFlags.NonPublic;
            }

            return type
                .GetConstructors(bindingFlags)
                .FirstOrDefault(constructor =>
                {
                    var parameters = constructor.GetParameters();

                    if (parameters.Length != paramTypes.Length)
                    {
                        return false;
                    }

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        if (paramTypes[i] == null)
                        {
                            if (!parameters[i].ParameterType.IsNullable())
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!parameters[i].ParameterType.IsAssignableFrom(paramTypes[i]))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                });
        }

        public static ConstructorInfo GetConstructorAccepting(this Type type, params Type[] paramTypes)
        {
            return GetConstructorAccepting(type, paramTypes, true);
        }

        public static ConstructorInfo GetPublicConstructorAccepting(this Type type, params Type[] paramTypes)
        {
            return GetConstructorAccepting(type, paramTypes, false);
        }

        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            return GetConstructorAccepting(type);
        }

        public static ConstructorInfo GetPublicDefaultConstructor(this Type type)
        {
            return GetPublicConstructorAccepting(type);
        }

        public static MemberInfo[] GetExtendedMember(this Type type, string name, MemberTypes types, BindingFlags flags)
        {
            var members = type.GetMember(name, types, flags).ToList();

            if (types.HasFlag(MemberTypes.Method)) // Check for extension methods
            {
                members.AddRange(type.GetExtensionMethods()
                    .Where(extension => extension.Name == name)
                    .Cast<MemberInfo>());
            }

            return members.ToArray();
        }

        public static MemberInfo[] GetExtendedMembers(this Type type, BindingFlags flags)
        {
            var members = type.GetMembers(flags).ToHashSet();

            foreach (var extensionMethod in type.GetExtensionMethods())
            {
                members.Add(extensionMethod);
            }

            return members.ToArray();
        }

        #region Signature Disambiguation

        private static bool NameMatches(this MemberInfo member, string name)
        {
            return member.Name == name;
        }

        private static bool ParametersMatch(this MethodBase methodBase, IEnumerable<Type> parameterTypes, bool invokeAsExtension)
        {
            Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);

            return methodBase.GetInvocationParameters(invokeAsExtension).Select(paramInfo => paramInfo.ParameterType).SequenceEqual(parameterTypes);
        }

        private static bool GenericArgumentsMatch(this MethodInfo method, IEnumerable<Type> genericArgumentTypes)
        {
            Ensure.That(nameof(genericArgumentTypes)).IsNotNull(genericArgumentTypes);

            if (method.ContainsGenericParameters)
            {
                return false;
            }

            return method.GetGenericArguments().SequenceEqual(genericArgumentTypes);
        }

        public static bool SignatureMatches(this FieldInfo field, string name)
        {
            return field.NameMatches(name);
        }

        public static bool SignatureMatches(this PropertyInfo property, string name)
        {
            return property.NameMatches(name);
        }

        public static bool SignatureMatches(this ConstructorInfo constructor, string name, IEnumerable<Type> parameterTypes)
        {
            return constructor.NameMatches(name) && constructor.ParametersMatch(parameterTypes, false);
        }

        public static bool SignatureMatches(this MethodInfo method, string name, IEnumerable<Type> parameterTypes, bool invokeAsExtension)
        {
            return method.NameMatches(name) && method.ParametersMatch(parameterTypes, invokeAsExtension) && !method.ContainsGenericParameters;
        }

        public static bool SignatureMatches(this MethodInfo method, string name, IEnumerable<Type> parameterTypes, IEnumerable<Type> genericArgumentTypes, bool invokeAsExtension)
        {
            return method.NameMatches(name) && method.ParametersMatch(parameterTypes, invokeAsExtension) && method.GenericArgumentsMatch(genericArgumentTypes);
        }

        public static FieldInfo GetFieldUnambiguous(this Type type, string name, BindingFlags flags)
        {
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(name)).IsNotNull(name);

            flags |= BindingFlags.DeclaredOnly;

            while (type != null)
            {
                var field = type.GetField(name, flags);

                if (field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static PropertyInfo GetPropertyUnambiguous(this Type type, string name, BindingFlags flags)
        {
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(name)).IsNotNull(name);

            flags |= BindingFlags.DeclaredOnly;

            while (type != null)
            {
                var property = type.GetProperty(name, flags);

                if (property != null)
                {
                    return property;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static MethodInfo GetMethodUnambiguous(this Type type, string name, BindingFlags flags)
        {
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(name)).IsNotNull(name);

            flags |= BindingFlags.DeclaredOnly;

            while (type != null)
            {
                var method = type.GetMethod(name, flags);

                if (method != null)
                {
                    return method;
                }

                type = type.BaseType;
            }

            return null;
        }

        private static TMemberInfo DisambiguateHierarchy<TMemberInfo>(this IEnumerable<TMemberInfo> members, Type type) where TMemberInfo : MemberInfo
        {
            while (type != null)
            {
                foreach (var member in members)
                {
                    var methodInfo = member as MethodInfo;
                    var invokedAsExtension = methodInfo != null && methodInfo.IsInvokedAsExtension(type);

                    if (member.ExtendedDeclaringType(invokedAsExtension) == type)
                    {
                        return member;
                    }
                }

                type = type.BaseType;
            }

            return null;
        }

        public static FieldInfo Disambiguate(this IEnumerable<FieldInfo> fields, Type type)
        {
            Ensure.That(nameof(fields)).IsNotNull(fields);
            Ensure.That(nameof(type)).IsNotNull(type);

            return fields.DisambiguateHierarchy(type);
        }

        public static PropertyInfo Disambiguate(this IEnumerable<PropertyInfo> properties, Type type)
        {
            Ensure.That(nameof(properties)).IsNotNull(properties);
            Ensure.That(nameof(type)).IsNotNull(type);

            return properties.DisambiguateHierarchy(type);
        }

        public static ConstructorInfo Disambiguate(this IEnumerable<ConstructorInfo> constructors, Type type, IEnumerable<Type> parameterTypes)
        {
            Ensure.That(nameof(constructors)).IsNotNull(constructors);
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);

            return constructors.Where(m => m.ParametersMatch(parameterTypes, false) && !m.ContainsGenericParameters).DisambiguateHierarchy(type);
        }

        public static MethodInfo Disambiguate(this IEnumerable<MethodInfo> methods, Type type, IEnumerable<Type> parameterTypes)
        {
            Ensure.That(nameof(methods)).IsNotNull(methods);
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);

            return methods.Where(m => m.ParametersMatch(parameterTypes, m.IsInvokedAsExtension(type)) && !m.ContainsGenericParameters).DisambiguateHierarchy(type);
        }

        public static MethodInfo Disambiguate(this IEnumerable<MethodInfo> methods, Type type, IEnumerable<Type> parameterTypes, IEnumerable<Type> genericArgumentTypes)
        {
            Ensure.That(nameof(methods)).IsNotNull(methods);
            Ensure.That(nameof(type)).IsNotNull(type);
            Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);
            Ensure.That(nameof(genericArgumentTypes)).IsNotNull(genericArgumentTypes);

            return methods.Where(m => m.ParametersMatch(parameterTypes, m.IsInvokedAsExtension(type)) && m.GenericArgumentsMatch(genericArgumentTypes)).DisambiguateHierarchy(type);
        }

        #endregion
    }

    internal class ExtensionMethodCache
    {
        internal ExtensionMethodCache()
        {
            // Cache a list of all extension methods in assemblies
            // http://stackoverflow.com/a/299526
            Cache = RuntimeCodebase.types
                .Where(type => type.IsStatic() && !type.IsGenericType && !type.IsNested)
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsExtension())
                .ToArray();
        }

        internal readonly MethodInfo[] Cache;
    }
}
