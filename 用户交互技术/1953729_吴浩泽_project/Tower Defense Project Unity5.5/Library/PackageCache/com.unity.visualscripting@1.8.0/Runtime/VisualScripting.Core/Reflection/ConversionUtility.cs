using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class ConversionUtility
    {
        public enum ConversionType
        {
            Impossible,
            Identity,
            Upcast,
            Downcast,
            NumericImplicit,
            NumericExplicit,
            UserDefinedImplicit,
            UserDefinedExplicit,
            UserDefinedThenNumericImplicit,
            UserDefinedThenNumericExplicit,
            UnityHierarchy,
            EnumerableToArray,
            EnumerableToList,
            ToString
        }

        private const BindingFlags UserDefinedBindingFlags = BindingFlags.Static | BindingFlags.Public;

        private static readonly Dictionary<ConversionQuery, ConversionType> conversionTypesCache = new Dictionary<ConversionQuery, ConversionType>(new ConversionQueryComparer());
        private static readonly Dictionary<ConversionQuery, MethodInfo[]> userConversionMethodsCache = new Dictionary<ConversionQuery, MethodInfo[]>(new ConversionQueryComparer());

        private static bool RespectsIdentity(Type source, Type destination)
        {
            return source == destination;
        }

        private static bool IsUpcast(Type source, Type destination)
        {
            return destination.IsAssignableFrom(source);
        }

        private static bool IsDowncast(Type source, Type destination)
        {
            return source.IsAssignableFrom(destination);
        }

        private static bool ExpectsString(Type source, Type destination)
        {
            return destination == typeof(string);
        }

        public static bool HasImplicitNumericConversion(Type source, Type destination)
        {
            return implicitNumericConversions.ContainsKey(source) && implicitNumericConversions[source].Contains(destination);
        }

        public static bool HasExplicitNumericConversion(Type source, Type destination)
        {
            return explicitNumericConversions.ContainsKey(source) && explicitNumericConversions[source].Contains(destination);
        }

        public static bool HasNumericConversion(Type source, Type destination)
        {
            return HasImplicitNumericConversion(source, destination) || HasExplicitNumericConversion(source, destination);
        }

        private static IEnumerable<MethodInfo> FindUserDefinedConversionMethods(ConversionQuery query)
        {
            var source = query.source;
            var destination = query.destination;

            var sourceMethods = source.GetMethods(UserDefinedBindingFlags)
                .Where(m => m.IsUserDefinedConversion());

            var destinationMethods = destination.GetMethods(UserDefinedBindingFlags)
                .Where(m => m.IsUserDefinedConversion());

            return sourceMethods.Concat(destinationMethods).Where
                (
                    m => m.GetParameters()[0].ParameterType.IsAssignableFrom(source) ||
                    source.IsAssignableFrom(m.GetParameters()[0].ParameterType)
                );
        }

        // Returning an array directly so that the enumeration in
        // UserDefinedConversion does not allocate memory
        private static MethodInfo[] GetUserDefinedConversionMethods(Type source, Type destination)
        {
            var query = new ConversionQuery(source, destination);

            if (!userConversionMethodsCache.ContainsKey(query))
            {
                userConversionMethodsCache.Add(query, FindUserDefinedConversionMethods(query).ToArray());
            }

            return userConversionMethodsCache[query];
        }

        private static ConversionType GetUserDefinedConversionType(Type source, Type destination)
        {
            var conversionMethods = GetUserDefinedConversionMethods(source, destination);

            // Duplicate user defined conversions are not allowed, so FirstOrDefault is safe.

            // Look for direct conversions.
            var conversionMethod = conversionMethods.FirstOrDefault(m => m.ReturnType == destination);

            if (conversionMethod != null)
            {
                if (conversionMethod.Name == "op_Implicit")
                {
                    return ConversionType.UserDefinedImplicit;
                }
                else if (conversionMethod.Name == "op_Explicit")
                {
                    return ConversionType.UserDefinedExplicit;
                }
            }
            // Primitive types can skip the middleman cast, even if it is explicit.
            else if (destination.IsPrimitive && destination != typeof(IntPtr) && destination != typeof(UIntPtr))
            {
                // Look for implicit conversions.
                conversionMethod = conversionMethods.FirstOrDefault(m => HasImplicitNumericConversion(m.ReturnType, destination));

                if (conversionMethod != null)
                {
                    if (conversionMethod.Name == "op_Implicit")
                    {
                        return ConversionType.UserDefinedThenNumericImplicit;
                    }
                    else if (conversionMethod.Name == "op_Explicit")
                    {
                        return ConversionType.UserDefinedThenNumericExplicit;
                    }
                }
                // Look for explicit conversions.
                else
                {
                    conversionMethod = conversionMethods.FirstOrDefault(m => HasExplicitNumericConversion(m.ReturnType, destination));

                    if (conversionMethod != null)
                    {
                        return ConversionType.UserDefinedThenNumericExplicit;
                    }
                }
            }

            return ConversionType.Impossible;
        }

        private static bool HasEnumerableToArrayConversion(Type source, Type destination)
        {
            return source != typeof(string) &&
                typeof(IEnumerable).IsAssignableFrom(source) &&
                destination.IsArray &&
                destination.GetArrayRank() == 1;
        }

        private static bool HasEnumerableToListConversion(Type source, Type destination)
        {
            return source != typeof(string) &&
                typeof(IEnumerable).IsAssignableFrom(source) &&
                destination.IsGenericType &&
                destination.GetGenericTypeDefinition() == typeof(List<>);
        }

        private static bool HasUnityHierarchyConversion(Type source, Type destination)
        {
            if (destination == typeof(GameObject))
            {
                return typeof(Component).IsAssignableFrom(source);
            }
            else if (typeof(Component).IsAssignableFrom(destination) || destination.IsInterface)
            {
                return source == typeof(GameObject) || typeof(Component).IsAssignableFrom(source);
            }

            return false;
        }

        private static bool IsValidConversion(ConversionType conversionType, bool guaranteed)
        {
            if (conversionType == ConversionType.Impossible)
            {
                return false;
            }

            if (guaranteed)
            {
                // Downcasts are not guaranteed to succeed.
                if (conversionType == ConversionType.Downcast)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CanConvert(object value, Type type, bool guaranteed)
        {
            return IsValidConversion(GetRequiredConversion(value, type), guaranteed);
        }

        public static bool CanConvert(Type source, Type destination, bool guaranteed)
        {
            return IsValidConversion(GetRequiredConversion(source, destination), guaranteed);
        }

        public static object Convert(object value, Type type)
        {
            return Convert(value, type, GetRequiredConversion(value, type));
        }

        public static T Convert<T>(object value)
        {
            return (T)Convert(value, typeof(T));
        }

        public static bool TryConvert(object value, Type type, out object result, bool guaranteed)
        {
            var conversionType = GetRequiredConversion(value, type);

            if (IsValidConversion(conversionType, guaranteed))
            {
                result = Convert(value, type, conversionType);
                return true;
            }

            result = value;
            return false;
        }

        public static bool TryConvert<T>(object value, out T result, bool guaranteed)
        {
            if (TryConvert(value, typeof(T), out var res, guaranteed))
            {
                result = (T)res;
                return true;
            }

            result = default;
            return false;
        }

        public static bool IsConvertibleTo(this Type source, Type destination, bool guaranteed)
        {
            return CanConvert(source, destination, guaranteed);
        }

        public static bool IsConvertibleTo(this object source, Type type, bool guaranteed)
        {
            return CanConvert(source, type, guaranteed);
        }

        public static bool IsConvertibleTo<T>(this object source, bool guaranteed)
        {
            return CanConvert(source, typeof(T), guaranteed);
        }

        public static object ConvertTo(this object source, Type type)
        {
            return Convert(source, type);
        }

        public static T ConvertTo<T>(this object source)
        {
            return (T)Convert(source, typeof(T));
        }

        public static ConversionType GetRequiredConversion(Type source, Type destination)
        {
            var query = new ConversionQuery(source, destination);

            if (!conversionTypesCache.TryGetValue(query, out var conversionType))
            {
                conversionType = DetermineConversionType(query);
                conversionTypesCache.Add(query, conversionType);
            }

            return conversionType;
        }

        private static ConversionType DetermineConversionType(ConversionQuery query)
        {
            var source = query.source;
            var destination = query.destination;

            if (source == null)
            {
                if (destination.IsNullable())
                {
                    return ConversionType.Identity;
                }
                else
                {
                    return ConversionType.Impossible;
                }
            }

            Ensure.That(nameof(destination)).IsNotNull(destination);

            if (RespectsIdentity(source, destination))
            {
                return ConversionType.Identity;
            }
            else if (IsUpcast(source, destination))
            {
                return ConversionType.Upcast;
            }
            else if (IsDowncast(source, destination))
            {
                return ConversionType.Downcast;
            }
            // Disabling *.ToString conversion, because it's more often than otherwise very confusing
            /*else if (ExpectsString(source, destination))
            {
                return ConversionType.ToString;
            }*/
            else if (HasImplicitNumericConversion(source, destination))
            {
                return ConversionType.NumericImplicit;
            }
            else if (HasExplicitNumericConversion(source, destination))
            {
                return ConversionType.NumericExplicit;
            }
            else if (HasUnityHierarchyConversion(source, destination))
            {
                return ConversionType.UnityHierarchy;
            }
            else if (HasEnumerableToArrayConversion(source, destination))
            {
                return ConversionType.EnumerableToArray;
            }
            else if (HasEnumerableToListConversion(source, destination))
            {
                return ConversionType.EnumerableToList;
            }
            else
            {
                var userDefinedConversionType = GetUserDefinedConversionType(source, destination);

                if (userDefinedConversionType != ConversionType.Impossible)
                {
                    return userDefinedConversionType;
                }
            }

            return ConversionType.Impossible;
        }

        public static ConversionType GetRequiredConversion(object value, Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            return GetRequiredConversion(value?.GetType(), type);
        }

        private static object NumericConversion(object value, Type type)
        {
            return System.Convert.ChangeType(value, type);
        }

        private static object UserDefinedConversion(ConversionType conversion, object value, Type type)
        {
            var valueType = value.GetType();
            var conversionMethods = GetUserDefinedConversionMethods(valueType, type);

            var numeric = conversion == ConversionType.UserDefinedThenNumericImplicit ||
                conversion == ConversionType.UserDefinedThenNumericExplicit;

            MethodInfo conversionMethod = null;

            if (numeric)
            {
                foreach (var m in conversionMethods)
                {
                    if (HasNumericConversion(m.ReturnType, type))
                    {
                        conversionMethod = m;
                        break;
                    }
                }
            }
            else
            {
                foreach (var m in conversionMethods)
                {
                    if (m.ReturnType == type)
                    {
                        conversionMethod = m;
                        break;
                    }
                }
            }

            var result = conversionMethod.InvokeOptimized(null, value);

            if (numeric)
            {
                result = NumericConversion(result, type);
            }

            return result;
        }

        private static object EnumerableToArrayConversion(object value, Type arrayType)
        {
            var elementType = arrayType.GetElementType();
            var objectArray = ((IEnumerable)value).Cast<object>().Where(elementType.IsAssignableFrom).ToArray(); // Non-generic OfType
            var typedArray = Array.CreateInstance(elementType, objectArray.Length);
            objectArray.CopyTo(typedArray, 0);
            return typedArray;
        }

        private static object EnumerableToListConversion(object value, Type listType)
        {
            var elementType = listType.GetGenericArguments()[0];
            var objectArray = ((IEnumerable)value).Cast<object>().Where(elementType.IsAssignableFrom).ToArray(); // Non-generic OfType
            var typedList = (IList)Activator.CreateInstance(listType);

            for (var i = 0; i < objectArray.Length; i++)
            {
                typedList.Add(objectArray[i]);
            }

            return typedList;
        }

        private static object UnityHierarchyConversion(object value, Type type)
        {
            if (value.IsUnityNull())
            {
                return null;
            }

            if (type == typeof(GameObject) && value is Component)
            {
                return ((Component)value).gameObject;
            }
            else if (typeof(Component).IsAssignableFrom(type) || type.IsInterface)
            {
                if (value is Component)
                {
                    return ((Component)value).GetComponent(type);
                }
                else if (value is GameObject)
                {
                    return ((GameObject)value).GetComponent(type);
                }
            }

            throw new InvalidConversionException();
        }

        private static object Convert(object value, Type type, ConversionType conversionType)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            if (conversionType == ConversionType.Impossible)
            {
                throw new InvalidConversionException($"Cannot convert from '{value?.GetType().ToString() ?? "null"}' to '{type}'.");
            }

            try
            {
                switch (conversionType)
                {
                    case ConversionType.Identity:
                    case ConversionType.Upcast:
                    case ConversionType.Downcast:
                        return value;

                    case ConversionType.ToString:
                        return value.ToString();

                    case ConversionType.NumericImplicit:
                    case ConversionType.NumericExplicit:
                        return NumericConversion(value, type);

                    case ConversionType.UserDefinedImplicit:
                    case ConversionType.UserDefinedExplicit:
                    case ConversionType.UserDefinedThenNumericImplicit:
                    case ConversionType.UserDefinedThenNumericExplicit:
                        return UserDefinedConversion(conversionType, value, type);

                    case ConversionType.EnumerableToArray:
                        return EnumerableToArrayConversion(value, type);

                    case ConversionType.EnumerableToList:
                        return EnumerableToListConversion(value, type);

                    case ConversionType.UnityHierarchy:
                        return UnityHierarchyConversion(value, type);

                    default:
                        throw new UnexpectedEnumValueException<ConversionType>(conversionType);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidConversionException($"Failed to convert from '{value?.GetType().ToString() ?? "null"}' to '{type}' via {conversionType}.", ex);
            }
        }

        private struct ConversionQuery : IEquatable<ConversionQuery>
        {
            public readonly Type source;
            public readonly Type destination;

            public ConversionQuery(Type source, Type destination)
            {
                this.source = source;
                this.destination = destination;
            }

            public bool Equals(ConversionQuery other)
            {
                return
                    source == other.source &&
                    destination == other.destination;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ConversionQuery))
                {
                    return false;
                }

                return Equals((ConversionQuery)obj);
            }

            public override int GetHashCode()
            {
                return HashUtility.GetHashCode(source, destination);
            }
        }

        // Make sure the equality comparer doesn't use boxing
        private struct ConversionQueryComparer : IEqualityComparer<ConversionQuery>
        {
            public bool Equals(ConversionQuery x, ConversionQuery y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(ConversionQuery obj)
            {
                return obj.GetHashCode();
            }
        }

        #region Numeric Conversions

        // https://msdn.microsoft.com/en-us/library/y5b434w4.aspx
        private static readonly Dictionary<Type, HashSet<Type>> implicitNumericConversions = new Dictionary<Type, HashSet<Type>>()
        {
            {
                typeof(sbyte),
                new HashSet<Type>()
                {
                    typeof(byte),
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(byte),
                new HashSet<Type>()
                {
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(short),
                new HashSet<Type>()
                {
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(ushort),
                new HashSet<Type>()
                {
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal),
                }
            },
            {
                typeof(int),
                new HashSet<Type>()
                {
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(uint),
                new HashSet<Type>()
                {
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(long),
                new HashSet<Type>()
                {
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(char),
                new HashSet<Type>()
                {
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(float),
                new HashSet<Type>()
                {
                    typeof(double)
                }
            },
            {
                typeof(ulong),
                new HashSet<Type>()
                {
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
        };

        // https://msdn.microsoft.com/en-us/library/yht2cx7b.aspx
        private static readonly Dictionary<Type, HashSet<Type>> explicitNumericConversions = new Dictionary<Type, HashSet<Type>>()
        {
            {
                typeof(sbyte),
                new HashSet<Type>()
                {
                    typeof(byte),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong),
                    typeof(char)
                }
            },
            {
                typeof(byte),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(char)
                }
            },
            {
                typeof(short),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong),
                    typeof(char)
                }
            },
            {
                typeof(ushort),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(char)
                }
            },
            {
                typeof(int),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong),
                    typeof(char)
                }
            },
            {
                typeof(uint),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(char)
                }
            },
            {
                typeof(long),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(ulong),
                    typeof(char)
                }
            },
            {
                typeof(ulong),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(char)
                }
            },
            {
                typeof(char),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short)
                }
            },
            {
                typeof(float),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(char),
                    typeof(decimal)
                }
            },
            {
                typeof(double),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(char),
                    typeof(float),
                    typeof(decimal),
                }
            },
            {
                typeof(decimal),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(char),
                    typeof(float),
                    typeof(double)
                }
            }
        };

        #endregion
    }
}
