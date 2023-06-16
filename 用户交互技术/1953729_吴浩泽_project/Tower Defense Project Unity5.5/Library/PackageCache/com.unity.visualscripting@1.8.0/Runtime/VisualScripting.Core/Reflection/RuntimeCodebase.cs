using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using Exception = System.Exception;
using Unity.VisualScripting;

[assembly: Unity.VisualScripting.RenamedNamespace("Bolt", "Unity.VisualScripting")]
[assembly: Unity.VisualScripting.RenamedNamespace("Ludiq", "Unity.VisualScripting")]

namespace Unity.VisualScripting
{
    public static class RuntimeCodebase
    {
        private static readonly object @lock = new object();

        private static readonly List<Type> _types = new List<Type>();

        public static IEnumerable<Type> types => _types;

        private static readonly List<Assembly> _assemblies = new List<Assembly>();

        public static IEnumerable<Assembly> assemblies => _assemblies;

        /* (disallowedAssemblies)
           This is a hack to force our RuntimeCodebase to use the RenamedTypeLookup for certain types when we deserialize them.
           When we migrate from asset store to package assemblies (With new names), we want to deserialize our types
           to the new types with new namespaces that exist in our new assemblies
           (Ex: Unity.VisualScripting.SuperUnit instead of Bolt.SuperUnit).

           Problem arises because we're migrating via script. Deleting the old assembly files on the disk doesn't remove
           them from our AppDomain, and we can't unload specific assemblies.
           Reloading the whole AppDomain would reload the migration scripts too, which would re-trigger the whole
           migration flow and be bad UX.

           So to avoid this problem, we don't reload the AppDomain (old assemblies still loaded) but just avoid them when
           trying to deserialize types temporarily. When we Domain Reload at the end, it's cleaned up.

           Without this, we get deserialization errors on migration to do with trying to instantiate a new type from an
           old interface type.

           This shouldn't cause much of a perf difference for most use because all our types are cached anyway,
           and logic to do with this sits beyond the cached types layer.
        */
        public static HashSet<string> disallowedAssemblies = new HashSet<string>();

        private static readonly Dictionary<string, Type> typeSerializations = new Dictionary<string, Type>();

        private static Dictionary<string, Type> _renamedTypes = null;

        private static Dictionary<string, string> _renamedNamespaces = null;

        private static Dictionary<string, string> _renamedAssemblies = null;

        private static readonly Dictionary<Type, Dictionary<string, string>> _renamedMembers = new Dictionary<Type, Dictionary<string, string>>();

        static RuntimeCodebase()
        {
            lock (@lock)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    _assemblies.Add(assembly);

                    foreach (var assemblyType in assembly.GetTypesSafely())
                    {
                        _types.Add(assemblyType);
                    }
                }
            }
        }

        #region Assembly Attributes

        public static IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType)
        {
            return GetAssemblyAttributes(attributeType, assemblies);
        }

        public static IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType, IEnumerable<Assembly> assemblies)
        {
            Ensure.That(nameof(attributeType)).IsNotNull(attributeType);
            Ensure.That(nameof(assemblies)).IsNotNull(assemblies);

            foreach (var assembly in assemblies)
            {
                foreach (var attribute in assembly.GetCustomAttributes(attributeType))
                {
                    if (attributeType.IsInstanceOfType(attribute))
                    {
                        yield return attribute;
                    }
                }
            }
        }

        public static IEnumerable<TAttribute> GetAssemblyAttributes<TAttribute>(IEnumerable<Assembly> assemblies) where TAttribute : Attribute
        {
            return GetAssemblyAttributes(typeof(TAttribute), assemblies).Cast<TAttribute>();
        }

        public static IEnumerable<TAttribute> GetAssemblyAttributes<TAttribute>() where TAttribute : Attribute
        {
            return GetAssemblyAttributes(typeof(TAttribute)).Cast<TAttribute>();
        }

        #endregion

        #region Serialization

        public static void PrewarmTypeDeserialization(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            var serialization = SerializeType(type);

            if (typeSerializations.ContainsKey(serialization))
            {
                // Some are duplicates, but almost always compiler generated stuff.
                // Safe to ignore, and anyway what would we even do to deserialize them properly?
            }
            else
            {
                typeSerializations.Add(serialization, type);
            }
        }

        public static string SerializeType(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            return type?.FullName;
        }

        public static bool TryDeserializeType(string typeName, out Type type)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                type = null;
                return false;
            }

            lock (@lock)
            {
                if (!TryCachedTypeLookup(typeName, out type))
                {
                    if (!TrySystemTypeLookup(typeName, out type))
                    {
                        if (!TryRenamedTypeLookup(typeName, out type))
                        {
                            return false;
                        }
                    }

                    typeSerializations.Add(typeName, type);
                }

                return true;
            }
        }

        public static Type DeserializeType(string typeName)
        {
            if (!TryDeserializeType(typeName, out var type))
            {
                throw new SerializationException($"Unable to find type: '{typeName ?? "(null)"}'.");
            }

            return type;
        }

        public static void ClearCachedTypes()
        {
            typeSerializations.Clear();
        }

        private static bool TryCachedTypeLookup(string typeName, out Type type)
        {
            return typeSerializations.TryGetValue(typeName, out type);
        }

        private static bool TrySystemTypeLookup(string typeName, out Type type)
        {
            foreach (var assembly in _assemblies)
            {
                if (disallowedAssemblies.Contains(assembly.GetName().Name))
                    continue;

                type = assembly.GetType(typeName);

                if (type != null)
                {
                    // This catches things like generic parameters of system collection types using disallowed assembly types
                    // Ex: System HashSet<Ludiq.xyz>
                    foreach (var disallowed in disallowedAssemblies)
                    {
                        if (type.FullName.Contains(disallowed))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            type = null;
            return false;
        }

        private static bool TrySystemTypeLookup(TypeName typeName, out Type type)
        {
            if (disallowedAssemblies.Contains(typeName.AssemblyName))
            {
                type = null;
                return false;
            }

            // Can't retrieve an array with the ToLooseString format so use the type Name and compare Assemblies
            if (typeName.IsArray)
            {
                foreach (var assembly in _assemblies.Where(a => typeName.AssemblyName == a.GetName().Name))
                {
                    type = assembly.GetType(typeName.Name);
                    if (type != null)
                    {
                        return true;
                    }
                }

                type = null;
                return false;
            }

            return TrySystemTypeLookup(typeName.ToLooseString(), out type);
        }

        private static bool TryRenamedTypeLookup(string previousTypeName, out Type type)
        {
            // Try for an exact match in our renamed types dictionary.
            // That should work for every non-generic type.
            if (renamedTypes.TryGetValue(previousTypeName, out var newType))
            {
                type = newType;
                return true;
            }
            // If we can't get an exact match, we'll try parsing the previous type name,
            // replacing all the renamed types we can find, then reconstructing it.
            else
            {
                var parsedTypeName = TypeName.Parse(previousTypeName);

                foreach (var renamedType in renamedTypes)
                {
                    parsedTypeName.ReplaceName(renamedType.Key, renamedType.Value);
                }

                foreach (var renamedNamespace in renamedNamespaces)
                {
                    parsedTypeName.ReplaceNamespace(renamedNamespace.Key, renamedNamespace.Value);
                }

                foreach (var renamedAssembly in renamedAssemblies)
                {
                    parsedTypeName.ReplaceAssembly(renamedAssembly.Key, renamedAssembly.Value);
                }

                // Run the system lookup
                if (TrySystemTypeLookup(parsedTypeName, out type))
                {
                    return true;
                }

                type = null;
                return false;
            }
        }

        #endregion

        #region Renaming

        // Can't use AttributeUtility here, because the caching system will
        // try to load all attributes of the type for efficiency, which is
        // not allowed on the serialization thread because some of Unity's
        // attribute constructors use Unity API methods (ugh!).

        public static Dictionary<string, string> renamedNamespaces
        {
            get
            {
                if (_renamedNamespaces == null)
                {
                    _renamedNamespaces = FetchRenamedNamespaces();
                }

                return _renamedNamespaces;
            }
        }

        public static Dictionary<string, string> renamedAssemblies
        {
            get
            {
                if (_renamedAssemblies == null)
                {
                    _renamedAssemblies = FetchRenamedAssemblies();
                }

                return _renamedAssemblies;
            }
        }

        public static Dictionary<string, Type> renamedTypes
        {
            get
            {
                if (_renamedTypes == null)
                {
                    // Fetch only on demand because attribute lookups are expensive
                    _renamedTypes = FetchRenamedTypes();
                }

                return _renamedTypes;
            }
        }

        public static Dictionary<string, string> RenamedMembers(Type type)
        {
            Dictionary<string, string> renamedMembers;

            if (!_renamedMembers.TryGetValue(type, out renamedMembers))
            {
                renamedMembers = FetchRenamedMembers(type);
                _renamedMembers.Add(type, renamedMembers);
            }

            return renamedMembers;
        }

        private static Dictionary<string, string> FetchRenamedMembers(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            var renamedMembers = new Dictionary<string, string>();

            var members = type.GetExtendedMembers(Member.SupportedBindingFlags);

            foreach (var member in members)
            {
                IEnumerable<RenamedFromAttribute> renamedFromAttributes;

                try
                {
                    renamedFromAttributes = Attribute.GetCustomAttributes(member, typeof(RenamedFromAttribute), false).Cast<RenamedFromAttribute>();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to fetch RenamedFrom attributes for member '{member}':\n{ex}");
                    continue;
                }

                var newMemberName = member.Name;

                foreach (var renamedFromAttribute in renamedFromAttributes)
                {
                    var previousMemberName = renamedFromAttribute.previousName;

                    if (renamedMembers.ContainsKey(previousMemberName))
                    {
                        Debug.LogWarning($"Multiple members on '{type}' indicate having been renamed from '{previousMemberName}'.\nIgnoring renamed attributes for '{member}'.");

                        continue;
                    }

                    renamedMembers.Add(previousMemberName, newMemberName);
                }
            }

            return renamedMembers;
        }

        private static Dictionary<string, string> FetchRenamedNamespaces()
        {
            var renamedNamespaces = new Dictionary<string, string>();

            foreach (var renamedNamespaceAttribute in GetAssemblyAttributes<RenamedNamespaceAttribute>())
            {
                var previousNamespaceName = renamedNamespaceAttribute.previousName;
                var newNamespaceName = renamedNamespaceAttribute.newName;

                if (renamedNamespaces.ContainsKey(previousNamespaceName))
                {
                    Debug.LogWarning($"Multiple new names have been provided for namespace '{previousNamespaceName}'.\nIgnoring new name '{newNamespaceName}'.");

                    continue;
                }

                renamedNamespaces.Add(previousNamespaceName, newNamespaceName);
            }

            return renamedNamespaces;
        }

        private static Dictionary<string, string> FetchRenamedAssemblies()
        {
            var renamedAssemblies = new Dictionary<string, string>();

            foreach (var renamedAssemblyAttribute in GetAssemblyAttributes<RenamedAssemblyAttribute>())
            {
                var previousAssemblyName = renamedAssemblyAttribute.previousName;
                var newAssemblyName = renamedAssemblyAttribute.newName;

                if (renamedAssemblies.ContainsKey(previousAssemblyName))
                {
                    Debug.LogWarning($"Multiple new names have been provided for assembly '{previousAssemblyName}'.\nIgnoring new name '{newAssemblyName}'.");

                    continue;
                }

                renamedAssemblies.Add(previousAssemblyName, newAssemblyName);
            }

            return renamedAssemblies;
        }

        private static Dictionary<string, Type> FetchRenamedTypes()
        {
            var renamedTypes = new Dictionary<string, Type>();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypesSafely())
                {
                    IEnumerable<RenamedFromAttribute> renamedFromAttributes;

                    try
                    {
                        renamedFromAttributes = Attribute.GetCustomAttributes(type, typeof(RenamedFromAttribute), false).Cast<RenamedFromAttribute>();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to fetch RenamedFrom attributes for type '{type}':\n{ex}");
                        continue;
                    }

                    var newTypeName = type.FullName;

                    foreach (var renamedFromAttribute in renamedFromAttributes)
                    {
                        var previousTypeName = renamedFromAttribute.previousName;

                        if (renamedTypes.ContainsKey(previousTypeName))
                        {
                            Debug.LogWarning($"Multiple types indicate having been renamed from '{previousTypeName}'.\nIgnoring renamed attributes for '{type}'.");

                            continue;
                        }

                        renamedTypes.Add(previousTypeName, type);
                    }
                }
            }

            return renamedTypes;
        }

        #endregion
    }
}
