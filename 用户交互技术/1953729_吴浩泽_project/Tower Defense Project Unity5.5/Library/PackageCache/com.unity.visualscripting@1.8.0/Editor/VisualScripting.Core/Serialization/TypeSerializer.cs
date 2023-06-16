using System;
using System.Collections.Generic;
#if UNITY_2019_2_OR_NEWER
using UnityEditor;
#else
using System.Linq;
#endif
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.VisualScripting
{
    static class TypeSerializer
    {
        static System.Text.RegularExpressions.Regex s_GenericTypeExtractionRegex = new System.Text.RegularExpressions.Regex(@"(?<=\[\[)(.*?)(?=\]\])");
        static Dictionary<string, Type> s_MovedFromTypes;

        static Dictionary<string, Type> movedFromTypes
        {
            get
            {
                if (s_MovedFromTypes == null)
                {
                    s_MovedFromTypes = new Dictionary<string, Type>();

#if UNITY_2019_2_OR_NEWER
                    var moved = TypeCache.GetTypesWithAttribute<MovedFromAttribute>();
#else
                    var moved = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes(), (assembly, type) => new { assembly, type })
                        .Where(t => Attribute.GetCustomAttributes(t.type, typeof(MovedFromAttribute)).Length > 0)
                        .Select(t => t.type);
#endif
                    foreach (var t in moved)
                    {
                        var attributes = Attribute.GetCustomAttributes(t, typeof(MovedFromAttribute));
                        foreach (var attribute in attributes)
                        {
                            var movedFromAttribute = (MovedFromAttribute)attribute;
                            movedFromAttribute.GetData(out _, out var nameSpace, out var assembly, out var className);

                            var currentClassName = GetFullNameNoNamespace(t.FullName, t.Namespace);
                            var currentNamespace = t.Namespace;
                            var currentAssembly = t.Assembly.GetName().Name;

                            var newNamespace = string.IsNullOrEmpty(nameSpace) ? currentNamespace : nameSpace;
                            var newClassName = string.IsNullOrEmpty(className) ? currentClassName : className;
                            var newAssembly = string.IsNullOrEmpty(assembly) ? currentAssembly : assembly;

                            var str = $"{newNamespace}.{newClassName}, {newAssembly}";

                            s_MovedFromTypes.Add(str, t);
                        }
                    }
                }

                return s_MovedFromTypes;
            }
        }

        /// <summary>
        /// Gets the full name of a type without the namespace.
        /// </summary>
        /// <remarks>
        /// The full name of a type nested type includes the outer class type name. The type names are normally
        /// separated by '+' but Unity serialization uses the '/' character as separator.
        ///
        /// This method returns the full type name of a class and switches the type separator to '/' to follow Unity.
        /// </remarks>
        /// <param name="typeName">The full type name, including the namespace.</param>
        /// <param name="nameSpace">The namespace to be removed.</param>
        /// <returns>Returns a string.</returns>
        static string GetFullNameNoNamespace(string typeName, string nameSpace)
        {
            return typeName.Contains(nameSpace)
                ? typeName.Substring(nameSpace.Length + 1).Replace("+", "/")
                : typeName;
        }

        static string Serialize(Type t)
        {
            return t.AssemblyQualifiedName;
        }

        static Type Deserialize(string serializedType)
        {
            return GetTypeFromName(serializedType);
        }

        static Type GetTypeFromName(string assemblyQualifiedName)
        {
            Type retType = typeof(Unknown);
            if (!string.IsNullOrEmpty(assemblyQualifiedName))
            {
                var type = Type.GetType(assemblyQualifiedName);
                if (type == null)
                {
                    // Check if the type has moved
                    assemblyQualifiedName = ExtractAssemblyQualifiedName(assemblyQualifiedName, out var isList);
                    if (movedFromTypes.ContainsKey(assemblyQualifiedName))
                    {
                        type = movedFromTypes[assemblyQualifiedName];
                        if (isList)
                        {
                            type = typeof(List<>).MakeGenericType(type);
                        }
                    }
                }

                retType = type ?? retType;
            }
            return retType;
        }

        static string ExtractAssemblyQualifiedName(string fullTypeName, out bool isList)
        {
            isList = false;
            if (fullTypeName.StartsWith("System.Collections.Generic.List"))
            {
                fullTypeName = s_GenericTypeExtractionRegex.Match(fullTypeName).Value;
                isList = true;
            }

            // remove the assembly version string
            var versionIdx = fullTypeName.IndexOf(", Version=");
            if (versionIdx > 0)
                fullTypeName = fullTypeName.Substring(0, versionIdx);
            // replace all '+' with '/' to follow the Unity serialization convention for nested types
            fullTypeName = fullTypeName.Replace("+", "/");
            return fullTypeName;
        }

        public static Type ResolveType(SerializableType th)
        {
            return Deserialize(th.Identification);
        }

        public static SerializableType GenerateTypeHandle<T>()
        {
            return GenerateTypeHandle(typeof(T));
        }

        public static SerializableType GenerateTypeHandle(Type t)
        {
            return new SerializableType(Serialize(t));
        }
    }
}
