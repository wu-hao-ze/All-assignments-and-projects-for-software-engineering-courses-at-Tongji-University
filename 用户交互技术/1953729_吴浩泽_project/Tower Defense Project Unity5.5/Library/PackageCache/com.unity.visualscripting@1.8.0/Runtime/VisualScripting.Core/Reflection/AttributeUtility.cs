using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class AttributeUtility
    {
        private static readonly Dictionary<object, AttributeCache> optimizedCaches = new Dictionary<object, AttributeCache>();

        private class AttributeCache
        {
            // Using lists instead of hashsets because:
            //  - Insertion will be faster
            //  - Iteration will be just as fast
            //  - We don't need contains lookups
            public List<Attribute> inheritedAttributes { get; } = new List<Attribute>();
            public List<Attribute> definedAttributes { get; } = new List<Attribute>();

            // Important to use Attribute.GetCustomAttributes, because MemberInfo.GetCustomAttributes
            // ignores the inherited parameter on properties and events

            // However, Attribute.GetCustomAttributes seems to have at least two obscure Mono 2.0 bugs.

            // 1. Basically, when a parameter is optional and is marked as [OptionalAttribute],
            // the custom attributes array is typed object[] instead of Attribute[], which
            // makes Mono throw an exception in Attribute.GetCustomAttributes when trying
            // to cast the array. After some testing, it appears this only happens for
            // non-inherited calls, and only for parameter infos (although I'm not sure why).
            // I *believe* the offending line in the Mono source is this one:
            // https://github.com/mono/mono/blob/mono-2-0/mcs/class/corlib/System/MonoCustomAttrs.cs#L143

            // 2. For some other implementation reason, on iOS, GetCustomAttributes on MemberInfo fails.
            // https://support.ludiq.io/forums/5-bolt/topics/729-systeminvalidcastexception-in-attributecache-on-ios/

            // As a fallback, we will use the GetCustomAttributes from the type itself,
            // which doesn't seem to be bugged (ugh). But because this method ignores the
            // inherited parameter on some occasions, we will warn if the inherited fetch fails.

            // Additionally, some Unity built-in attributes use threaded API methods in their
            // constructors and will therefore throw an error if GetCustomAttributes is called
            // from the serialization thread or from a secondary thread. We'll generally fallback
            // and warn on any exception to make sure not to block anything more than needed.
            // https://support.ludiq.io/communities/5/topics/2024-/

            public AttributeCache(MemberInfo element)
            {
                Ensure.That(nameof(element)).IsNotNull(element);

                try
                {
                    try
                    {
                        Cache(Attribute.GetCustomAttributes(element, true), inheritedAttributes);
                    }
                    catch (InvalidCastException ex)
                    {
                        Cache(element.GetCustomAttributes(true).Cast<Attribute>().ToArray(), inheritedAttributes);
                        Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
                }

                try
                {
                    try
                    {
                        Cache(Attribute.GetCustomAttributes(element, false), definedAttributes);
                    }
                    catch (InvalidCastException)
                    {
                        Cache(element.GetCustomAttributes(false).Cast<Attribute>().ToArray(), definedAttributes);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to fetch defined attributes on {element}.\n{ex}");
                }
            }

            public AttributeCache(ParameterInfo element)
            {
                Ensure.That(nameof(element)).IsNotNull(element);

                try
                {
                    try
                    {
                        Cache(Attribute.GetCustomAttributes(element, true), inheritedAttributes);
                    }
                    catch (InvalidCastException ex)
                    {
                        Cache(element.GetCustomAttributes(true).Cast<Attribute>().ToArray(), inheritedAttributes);
                        Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
                }

                try
                {
                    try
                    {
                        Cache(Attribute.GetCustomAttributes(element, false), definedAttributes);
                    }
                    catch (InvalidCastException)
                    {
                        Cache(element.GetCustomAttributes(false).Cast<Attribute>().ToArray(), definedAttributes);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to fetch defined attributes on {element}.\n{ex}");
                }
            }

            public AttributeCache(IAttributeProvider element)
            {
                Ensure.That(nameof(element)).IsNotNull(element);

                try
                {
                    Cache(element.GetCustomAttributes(true), inheritedAttributes);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
                }

                try
                {
                    Cache(element.GetCustomAttributes(false), definedAttributes);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to fetch defined attributes on {element}.\n{ex}");
                }
            }

            private void Cache(Attribute[] attributeObjects, List<Attribute> cache)
            {
                foreach (var attributeObject in attributeObjects)
                {
                    cache.Add(attributeObject);
                }
            }

            private bool HasAttribute(Type attributeType, List<Attribute> cache)
            {
                for (int i = 0; i < cache.Count; i++)
                {
                    var attribute = cache[i];

                    if (attributeType.IsInstanceOfType(attribute))
                    {
                        return true;
                    }
                }

                return false;
            }

            private Attribute GetAttribute(Type attributeType, List<Attribute> cache)
            {
                for (int i = 0; i < cache.Count; i++)
                {
                    var attribute = cache[i];

                    if (attributeType.IsInstanceOfType(attribute))
                    {
                        return attribute;
                    }
                }

                return null;
            }

            private IEnumerable<Attribute> GetAttributes(Type attributeType, List<Attribute> cache)
            {
                for (int i = 0; i < cache.Count; i++)
                {
                    var attribute = cache[i];

                    if (attributeType.IsInstanceOfType(attribute))
                    {
                        yield return attribute;
                    }
                }
            }

            public bool HasAttribute(Type attributeType, bool inherit = true)
            {
                if (inherit)
                {
                    return HasAttribute(attributeType, inheritedAttributes);
                }
                else
                {
                    return HasAttribute(attributeType, definedAttributes);
                }
            }

            public Attribute GetAttribute(Type attributeType, bool inherit = true)
            {
                if (inherit)
                {
                    return GetAttribute(attributeType, inheritedAttributes);
                }
                else
                {
                    return GetAttribute(attributeType, definedAttributes);
                }
            }

            public IEnumerable<Attribute> GetAttributes(Type attributeType, bool inherit = true)
            {
                if (inherit)
                {
                    return GetAttributes(attributeType, inheritedAttributes);
                }
                else
                {
                    return GetAttributes(attributeType, definedAttributes);
                }
            }

            public bool HasAttribute<TAttribute>(bool inherit = true)
                where TAttribute : Attribute
            {
                return HasAttribute(typeof(TAttribute), inherit);
            }

            public TAttribute GetAttribute<TAttribute>(bool inherit = true)
                where TAttribute : Attribute
            {
                return (TAttribute)GetAttribute(typeof(TAttribute), inherit);
            }

            public IEnumerable<TAttribute> GetAttributes<TAttribute>(bool inherit = true)
                where TAttribute : Attribute
            {
                return GetAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
            }
        }

        private static AttributeCache GetAttributeCache(MemberInfo element)
        {
            Ensure.That(nameof(element)).IsNotNull(element);

            // For MemberInfo (and therefore Type), we use the MetadataToken
            // as a key instead of the object itself, because member infos
            // are not singletons but their tokens are, optimizing the cache.
            var key = element;

            lock (optimizedCaches)
            {
                if (!optimizedCaches.TryGetValue(key, out var cache))
                {
                    cache = new AttributeCache(element);
                    optimizedCaches.Add(key, cache);
                }

                return cache;
            }
        }

        private static AttributeCache GetAttributeCache(ParameterInfo element)
        {
            Ensure.That(nameof(element)).IsNotNull(element);

            // For ParameterInfo, we maybe also should use the MetadataToken,
            // but I'm not sure they're globally unique or just locally unique. TODO: Check
            var key = element;

            lock (optimizedCaches)
            {
                if (!optimizedCaches.TryGetValue(key, out var cache))
                {
                    cache = new AttributeCache(element);
                    optimizedCaches.Add(key, cache);
                }

                return cache;
            }
        }

        private static AttributeCache GetAttributeCache(IAttributeProvider element)
        {
            Ensure.That(nameof(element)).IsNotNull(element);

            var key = element;

            lock (optimizedCaches)
            {
                if (!optimizedCaches.TryGetValue(key, out var cache))
                {
                    cache = new AttributeCache(element);
                    optimizedCaches.Add(key, cache);
                }

                return cache;
            }
        }

        #region Members (& Types)

        public static void CacheAttributes(MemberInfo element)
        {
            GetAttributeCache(element);
        }

        /// <summary>
        /// Gets attributes on an enum member, eg. enum E { [Attr] A }
        /// </summary>
        internal static IEnumerable<T> GetAttributeOfEnumMember<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Cast<T>();
        }

        public static bool HasAttribute(this MemberInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).HasAttribute(attributeType, inherit);
        }

        public static Attribute GetAttribute(this MemberInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttribute(attributeType, inherit);
        }

        public static IEnumerable<Attribute> GetAttributes(this MemberInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttributes(attributeType, inherit);
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).HasAttribute<TAttribute>(inherit);
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttribute<TAttribute>(inherit);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttributes<TAttribute>(inherit);
        }

        #endregion

        #region Parameters

        public static void CacheAttributes(ParameterInfo element)
        {
            GetAttributeCache(element);
        }

        public static bool HasAttribute(this ParameterInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).HasAttribute(attributeType, inherit);
        }

        public static Attribute GetAttribute(this ParameterInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttribute(attributeType, inherit);
        }

        public static IEnumerable<Attribute> GetAttributes(this ParameterInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttributes(attributeType, inherit);
        }

        public static bool HasAttribute<TAttribute>(this ParameterInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).HasAttribute<TAttribute>(inherit);
        }

        public static TAttribute GetAttribute<TAttribute>(this ParameterInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttribute<TAttribute>(inherit);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ParameterInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttributes<TAttribute>(inherit);
        }

        #endregion

        #region Providers

        public static void CacheAttributes(IAttributeProvider element)
        {
            GetAttributeCache(element);
        }

        public static bool HasAttribute(this IAttributeProvider element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).HasAttribute(attributeType, inherit);
        }

        public static Attribute GetAttribute(this IAttributeProvider element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttribute(attributeType, inherit);
        }

        public static IEnumerable<Attribute> GetAttributes(this IAttributeProvider element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttributes(attributeType, inherit);
        }

        public static bool HasAttribute<TAttribute>(this IAttributeProvider element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).HasAttribute<TAttribute>(inherit);
        }

        public static TAttribute GetAttribute<TAttribute>(this IAttributeProvider element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttribute<TAttribute>(inherit);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this IAttributeProvider element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttributes<TAttribute>(inherit);
        }

        #endregion

        #region Conditions

        public static bool CheckCondition(Type type, object target, string conditionMemberName, bool fallback)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            try
            {
                if (target != null && !type.IsInstanceOfType(target))
                {
                    throw new ArgumentException("Target is not an instance of type.", nameof(target));
                }

                if (conditionMemberName == null)
                {
                    return fallback;
                }

                var manipulator = type.GetMember(conditionMemberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault()?.ToManipulator();

                if (manipulator == null)
                {
                    throw new MissingMemberException(type.ToString(), conditionMemberName);
                }

                return manipulator.Get<bool>(target);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Failed to check attribute condition: \n" + ex);
                return fallback;
            }
        }

        public static bool CheckCondition<T>(T target, string conditionMemberName, bool fallback)
        {
            return CheckCondition(target?.GetType() ?? typeof(T), target, conditionMemberName, fallback);
        }

        #endregion
    }
}
