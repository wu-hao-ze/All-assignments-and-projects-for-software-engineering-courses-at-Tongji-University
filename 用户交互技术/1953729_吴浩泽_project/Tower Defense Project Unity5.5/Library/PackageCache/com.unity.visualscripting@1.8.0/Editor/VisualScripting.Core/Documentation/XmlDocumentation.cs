using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    public static class XmlDocumentation
    {
        class Data
        {
            public Dictionary<Assembly, Dictionary<string, XmlDocumentationTags>> documentations;
            public Dictionary<Type, XmlDocumentationTags> typeDocumentations;
            public Dictionary<MemberInfo, XmlDocumentationTags> memberDocumentations;
            public Dictionary<object, XmlDocumentationTags> enumDocumentations;

            public void Clear()
            {
                documentations.Clear();
                typeDocumentations.Clear();
                memberDocumentations.Clear();
                enumDocumentations.Clear();
            }
        }

        private static Lazy<Data> LazyData = new Lazy<Data>(Init);
        private static readonly object @lock = new object();

        public static event Action loadComplete;

        public static bool loaded { get; private set; }

        private static string[] fallbackDirectories;

        internal static void Initialize()
        {
            BackgroundWorker.Schedule(BackgroundWork);
        }

        private static Data Init()
        {
            return new Data
            {
                documentations = new Dictionary<Assembly, Dictionary<string, XmlDocumentationTags>>(),
                typeDocumentations = new Dictionary<Type, XmlDocumentationTags>(),
                memberDocumentations = new Dictionary<MemberInfo, XmlDocumentationTags>(),
                enumDocumentations = new Dictionary<object, XmlDocumentationTags>(),
            };
        }

        public static void BackgroundWork()
        {
            var preloadedAssemblies = new List<Assembly>();

            preloadedAssemblies.AddRange(Codebase.settingsAssemblies);
            preloadedAssemblies.AddRange(Codebase.ludiqEditorAssemblies);

            for (var i = 0; i < preloadedAssemblies.Count; i++)
            {
                var assembly = preloadedAssemblies[i];
                ProgressUtility.DisplayProgressBar($"Documentation ({assembly.GetName().Name})...", null, (float)i / Codebase.settingsAssemblies.Count);
                var documentation = GetDocumentationUncached(assembly);


                lock (@lock)
                {
                    if (!LazyData.Value.documentations.ContainsKey(assembly))
                    {
                        LazyData.Value.documentations.Add(assembly, documentation);
                    }
                }
            }

            UnityAPI.Async(() =>
            {
                loaded = true;
                loadComplete?.Invoke();
            });
        }

        public static void ClearCache()
        {
            if (!LazyData.IsValueCreated)
                return;
            lock (@lock)
            {
                LazyData.Value.Clear();
            }
        }

        private static Dictionary<string, XmlDocumentationTags> Documentation(Assembly assembly)
        {
            lock (@lock)
            {
                if (!loaded)
                {
                    return null;
                }

                var valueDocumentations = LazyData.Value.documentations;
                if (!valueDocumentations.ContainsKey(assembly))
                {
                    valueDocumentations.Add(assembly, GetDocumentationUncached(assembly));
                }

                return valueDocumentations[assembly];
            }
        }

        private static Dictionary<string, XmlDocumentationTags> GetDocumentationUncached(Assembly assembly)
        {
            var assemblyPath = assembly.Location;

            var documentationPath = Path.ChangeExtension(assemblyPath, ".xml");

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            documentationPath = "/" + documentationPath;
#endif

            if (fallbackDirectories == null)
            {
                fallbackDirectories = new[]
                {
                    BoltCore.Paths.dotNetDocumentation,
                    BoltCore.Paths.assemblyDocumentations
                };
            }
            if (!File.Exists(documentationPath))
            {
                foreach (var fallbackDirectory in fallbackDirectories)
                {
                    if (Directory.Exists(fallbackDirectory))
                    {
                        var fallbackPath = Path.Combine(fallbackDirectory, Path.GetFileName(documentationPath));

                        if (File.Exists(fallbackPath))
                        {
                            documentationPath = fallbackPath;
                            break;
                        }
                    }
                }
            }

            if (!File.Exists(documentationPath))
            {
                return null;
            }

            XDocument document;

            try
            {
                document = XDocument.Load(documentationPath);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Failed to load XML documentation:\n" + ex);

                return null;
            }

            var ns = document.Root.Name.Namespace;

            var dictionary = new Dictionary<string, XmlDocumentationTags>();

            foreach (var memberElement in document.Element(ns + "doc").Element(ns + "members").Elements(ns + "member"))
            {
                var nameAttribute = memberElement.Attribute("name");

                if (nameAttribute != null)
                {
                    if (dictionary.ContainsKey(nameAttribute.Value))
                    {
                        // Unity sometimes has duplicate member documentation in their XMLs.
                        // Safely skip.
                        continue;
                    }

                    dictionary.Add(nameAttribute.Value, new XmlDocumentationTags(memberElement));
                }
            }

            return dictionary;
        }

        public static XmlDocumentationTags Documentation(this MemberInfo memberInfo)
        {
            if (memberInfo is Type)
            {
                return ((Type)memberInfo).Documentation();
            }
            else if (memberInfo is MethodInfo)
            {
                return ((MethodInfo)memberInfo).Documentation();
            }
            else if (memberInfo is FieldInfo)
            {
                return ((FieldInfo)memberInfo).Documentation();
            }
            else if (memberInfo is PropertyInfo)
            {
                return ((PropertyInfo)memberInfo).Documentation();
            }
            else if (memberInfo is ConstructorInfo)
            {
                return ((ConstructorInfo)memberInfo).Documentation();
            }

            return null;
        }

        public static string ParameterSummary(this MethodBase methodBase, ParameterInfo parameterInfo)
        {
            return methodBase.Documentation()?.ParameterSummary(parameterInfo);
        }

        public static string Summary(this MemberInfo memberInfo)
        {
            return memberInfo.Documentation()?.summary;
        }

        public static string Summary(this Enum @enum)
        {
            return @enum.Documentation()?.summary;
        }

        public static XmlDocumentationTags Documentation(this Enum @enum)
        {
            lock (@lock)
            {
                if (!loaded)
                {
                    return null;
                }

                var xmlDocumentationTagsMap = LazyData.Value.enumDocumentations;
                if (!xmlDocumentationTagsMap.ContainsKey(@enum))
                {
                    xmlDocumentationTagsMap.Add(@enum, GetDocumentationFromNameInherited(@enum.GetType(), 'F', @enum.ToString(), null));
                }

                return xmlDocumentationTagsMap[@enum];
            }
        }

        private static XmlDocumentationTags Documentation(this MethodInfo methodInfo)
        {
            lock (@lock)
            {
                if (!loaded)
                {
                    return null;
                }

                var xmlDocumentationTagsMap = LazyData.Value.memberDocumentations;
                if (!xmlDocumentationTagsMap.ContainsKey(methodInfo))
                {
                    var methodDocumentation = GetDocumentationFromNameInherited(methodInfo.DeclaringType, 'M', methodInfo.Name, methodInfo.GetParameters());

                    methodDocumentation?.CompleteWithMethodBase(methodInfo, methodInfo.ReturnType);

                    xmlDocumentationTagsMap.Add(methodInfo, methodDocumentation);
                }

                return xmlDocumentationTagsMap[methodInfo];
            }
        }

        private static XmlDocumentationTags Documentation(this FieldInfo fieldInfo)
        {
            lock (@lock)
            {
                if (!loaded)
                {
                    return null;
                }

                var xmlDocumentationTagsMap = LazyData.Value.memberDocumentations;
                if (!xmlDocumentationTagsMap.ContainsKey(fieldInfo))
                {
                    xmlDocumentationTagsMap.Add(fieldInfo, GetDocumentationFromNameInherited(fieldInfo.DeclaringType, 'F', fieldInfo.Name, null));
                }

                return xmlDocumentationTagsMap[fieldInfo];
            }
        }

        private static XmlDocumentationTags Documentation(this PropertyInfo propertyInfo)
        {
            lock (@lock)
            {
                if (!loaded)
                {
                    return null;
                }

                var xmlDocumentationTagsMap = LazyData.Value.memberDocumentations;
                if (!xmlDocumentationTagsMap.ContainsKey(propertyInfo))
                {
                    xmlDocumentationTagsMap.Add(propertyInfo, GetDocumentationFromNameInherited(propertyInfo.DeclaringType, 'P', propertyInfo.Name, null));
                }

                return xmlDocumentationTagsMap[propertyInfo];
            }
        }

        private static XmlDocumentationTags Documentation(this ConstructorInfo constructorInfo)
        {
            lock (@lock)
            {
                if (!loaded)
                {
                    return null;
                }

                var xmlDocumentationTagsMap = LazyData.Value.memberDocumentations;
                if (!xmlDocumentationTagsMap.ContainsKey(constructorInfo))
                {
                    var constructorDocumentation = GetDocumentationFromNameInherited(constructorInfo.DeclaringType, 'M', "#ctor", constructorInfo.GetParameters());

                    constructorDocumentation?.CompleteWithMethodBase(constructorInfo, constructorInfo.DeclaringType);

                    xmlDocumentationTagsMap.Add(constructorInfo, constructorDocumentation);
                }

                return xmlDocumentationTagsMap[constructorInfo];
            }
        }

        private static XmlDocumentationTags Documentation(this Type type)
        {
            lock (@lock)
            {
                if (!loaded)
                {
                    return null;
                }

                var xmlDocumentationTagsMap = LazyData.Value.typeDocumentations;
                if (!xmlDocumentationTagsMap.ContainsKey(type))
                {
                    xmlDocumentationTagsMap.Add(type, GetDocumentationFromNameInherited(type, 'T', null, null));
                }

                return xmlDocumentationTagsMap[type];
            }
        }

        private static XmlDocumentationTags GetDocumentationFromNameInherited(Type type, char prefix, string memberName, IEnumerable<ParameterInfo> parameterTypes)
        {
            var documentation = GetDocumentationFromName(type, prefix, memberName, parameterTypes);

            if (documentation != null && documentation.inherit)
            {
                foreach (var implementedType in type.BaseTypeAndInterfaces())
                {
                    var implementedDocumentation = GetDocumentationFromNameInherited(implementedType, prefix, memberName, parameterTypes);

                    if (implementedDocumentation != null)
                    {
                        return implementedDocumentation;
                    }
                }

                return null;
            }

            return documentation;
        }

        private static XmlDocumentationTags GetDocumentationFromName(Type type, char prefix, string memberName, IEnumerable<ParameterInfo> parameterTypes)
        {
            var documentation = Documentation(type.Assembly);

            if (documentation == null)
            {
                return null;
            }

            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            var fullName = $"{prefix}:{type.Namespace}{(type.Namespace != null ? "." : "")}{type.Name.Replace('+', '.')}";

            if (!string.IsNullOrEmpty(memberName))
            {
                fullName += "." + memberName;

                if (parameterTypes != null && parameterTypes.Any())
                {
                    fullName += "(" + string.Join(",", parameterTypes.Select(p => p.ParameterType.ToString() + (p.IsOut || p.ParameterType.IsByRef ? "@" : "")).ToArray()) + ")";
                }
            }

            if (documentation.ContainsKey(fullName))
            {
                return documentation[fullName];
            }

            return null;
        }
    }
}
