using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [InitializeAfterPlugins]
    public static class Icons
    {
        static Icons()
        {
            EditorGUIUtility_GetScriptObjectFromClass = typeof(EditorGUIUtility).GetMethod("GetScript", BindingFlags.Static | BindingFlags.NonPublic);
            EditorGUIUtility_GetIconForObject = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            Load();
        }

        private static readonly MethodInfo EditorGUIUtility_GetScriptObjectFromClass; // UnityObject EditorGUIUtility.GetScript(string scriptClass);

        private static readonly MethodInfo EditorGUIUtility_GetIconForObject; // Texture2D EditorGUIUtility.GetIconForObject(UnityObject obj);

        private static readonly Dictionary<Type, EditorTexture> typeIcons = new Dictionary<Type, EditorTexture>();

        private static readonly Dictionary<Namespace, EditorTexture> namespaceIcons = new Dictionary<Namespace, EditorTexture>();

        private static readonly Dictionary<Enum, EditorTexture> enumIcons = new Dictionary<Enum, EditorTexture>();

        private static readonly Dictionary<string, EditorTexture> resourcesTypeIcons = new Dictionary<string, EditorTexture>();

        public static void Load()
        {
            using (ProfilingUtility.SampleBlock("Load Icons"))
            {
                Language.Load();
            }
        }

        internal static void Clear()
        {
            typeIcons.Clear();
            namespaceIcons.Clear();
            enumIcons.Clear();
            resourcesTypeIcons.Clear();
        }

        public static EditorTexture Icon(this Type type)
        {
            return Type(type);
        }

        public static EditorTexture Icon(this MemberInfo member, ActionDirection direction = ActionDirection.Any)
        {
            return Member(member, direction);
        }

        public static EditorTexture Icon(this Enum @enum)
        {
            return Enum(@enum);
        }

        public static EditorTexture Icon(this Namespace @namespace)
        {
            return Namespace(@namespace);
        }

        public static EditorTexture Type(Type type)
        {
            if (type == null)
            {
                return BoltCore.Icons.@null;
            }

            if (!typeIcons.ContainsKey(type))
            {
                var icon = GetCustomTypeIcon(type);

                if (icon == null)
                {
                    if (type.IsClass)
                    {
                        icon = GetTypeVisibilityIcon(type, Language.@class);
                    }
                    else if (type.IsInterface)
                    {
                        icon = GetTypeVisibilityIcon(type, Language.@interface);
                    }
                    else if (type.IsPrimitive)
                    {
                        icon = GetTypeVisibilityIcon(type, Language.primitive);
                    }
                    else if (type.IsEnum)
                    {
                        icon = GetTypeVisibilityIcon(type, Language.@enum);
                    }
                    else if (type.IsValueType)
                    {
                        icon = GetTypeVisibilityIcon(type, Language.@struct);
                    }
                }

                typeIcons.Add(type, icon);
            }

            return typeIcons[type];
        }

        private static EditorTexture GetCustomTypeIcon(Type type, bool inherit = true)
        {
            var attribute = type.GetAttribute<TypeIconAttribute>();

            if (attribute != null)
            {
                type = attribute.type;
            }

            var resourcesIcon = GetResourcesTypeIcon(type.CSharpFileName(true, true)) ??
                GetResourcesTypeIcon(type.CSharpFileName(true, false)) ??
                GetResourcesTypeIcon(type.CSharpFileName(false, true)) ??
                GetResourcesTypeIcon(type.CSharpFileName(false, false));

            if (resourcesIcon != null)
            {
                return resourcesIcon;
            }

            if (typeof(UnityObject).IsAssignableFrom(type))
            {
                var unityIcon = GetBuiltInUnityTypeIcon(type);

                if (unityIcon != null)
                {
                    return unityIcon;
                }
            }

            if (type.IsEnum || type == typeof(Enum))
            {
                return GetResourcesTypeIcon("enum");
            }

            if (type.IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();
                var isList = definition == typeof(IList<>);
                var isEnumerable = definition == typeof(IEnumerable<>);

                if (isList || isEnumerable)
                {
                    var argument = type.GetGenericArguments()[0];

                    if (argument.IsEnum || argument == typeof(Enum))
                    {
                        if (isList)
                        {
                            return GetResourcesTypeIcon("System.Collections.Generic.IList_enum");
                        }

                        return GetResourcesTypeIcon("System.Collections.Generic.IEnumerable_enum");
                    }
                }
            }

            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                var scriptIcon = GetScriptTypeIcon(type.Name);

                if (scriptIcon != null)
                {
                    return scriptIcon;
                }
            }

            if (inherit)
            {
                foreach (var inherited in type.BaseTypeAndInterfaces(false).OrderBy(InterfaceImplementationPriority))
                {
                    if (inherited == typeof(object))
                    {
                        continue;
                    }

                    var inheritedIcon = GetCustomTypeIcon(inherited, false);

                    if (inheritedIcon != null)
                    {
                        return inheritedIcon;
                    }
                }

                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    var baseTypeIcon = GetCustomTypeIcon(type.BaseType, true);

                    if (baseTypeIcon != null)
                    {
                        return baseTypeIcon;
                    }
                }
            }

            if (typeof(UnityObject).IsAssignableFrom(type))
            {
                return GetBuiltInUnityTypeIcon(typeof(UnityObject));
            }

            return null;
        }

        private static int InterfaceImplementationPriority(Type type)
        {
            // Quick and dirty method to determine interface implementation priority.

            /* Desired order is:
             *
             * IList<>
             * IList
             * ICollection<>
             * ICollection
             * IEnumerable<>
             * IEnumerable
             */

            var priority = type.GetAttribute<TypeIconPriorityAttribute>()?.priority;

            if (priority != null)
            {
                return priority.Value;
            }

            if (type.IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();

                if (definition == typeof(IList<>))
                {
                    return 0;
                }

                if (definition == typeof(ICollection<>))
                {
                    return 2;
                }

                if (definition == typeof(IEnumerable<>))
                {
                    return 4;
                }

                return int.MaxValue - 1;
            }

            if (type == typeof(IList))
            {
                return 1;
            }

            if (type == typeof(ICollection))
            {
                return 3;
            }

            if (type == typeof(IEnumerable))
            {
                return 5;
            }

            return int.MaxValue;
        }

        public static EditorTexture Icon(this UnityObject obj)
        {
            var icon = (Texture2D)EditorGUIUtility.ObjectContent(obj, obj?.GetType()).image;

            if (icon != null)
            {
                return EditorTexture.Single(icon);
            }

            return null;
        }

        private static EditorTexture GetResourcesTypeIcon(string fileName)
        {
            if (!resourcesTypeIcons.TryGetValue(fileName, out var icon))
            {
                icon = PluginResources.LoadSharedIcon($"{fileName}.png", false);
                resourcesTypeIcons.Add(fileName, icon);
            }

            return icon;
        }

        private static EditorTexture GetBuiltInUnityTypeIcon(Type type)
        {
            var icon = (Texture2D)EditorGUIUtility.ObjectContent(null, type).image;

            // Change the blank file icon to a Unity-logo file icon
            if (icon == EditorGUIUtility.FindTexture("DefaultAsset Icon"))
            {
                icon = EditorGUIUtility.FindTexture("ScriptableObject Icon");
            }

            if (icon != null)
            {
                return EditorTexture.Single(icon);
            }

            return null;
        }

        private static EditorTexture GetScriptTypeIcon(string scriptName)
        {
            var scriptObject = (UnityObject)EditorGUIUtility_GetScriptObjectFromClass.Invoke(null, new object[] { scriptName });

            if (scriptObject != null)
            {
                var scriptIcon = (Texture2D)EditorGUIUtility_GetIconForObject.Invoke(null, new object[] { scriptObject });

                if (scriptIcon != null)
                {
                    return EditorTexture.Single(scriptIcon);
                }
            }

            var scriptPath = AssetDatabase.GetAssetPath(scriptObject);

            if (scriptPath != null)
            {
                switch (Path.GetExtension(scriptPath))
                {
                    case ".js":

                        return EditorTexture.Single((Texture2D)EditorGUIUtility.IconContent("js Script Icon").image);
                    case ".cs":

                        return EditorTexture.Single((Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image);
                    case ".boo":

                        return EditorTexture.Single((Texture2D)EditorGUIUtility.IconContent("boo Script Icon").image);
                }
            }

            return null;
        }

        private static EditorTexture GetTypeVisibilityIcon(Type type, LanguageIconSet languageIcon)
        {
            if (languageIcon == null)
            {
                return null;
            }

            if (type.IsNested)
            {
                if (type.IsNestedPrivate)
                {
                    return languageIcon.@private;
                }

                if (type.IsNestedFamily)
                {
                    return languageIcon.@protected;
                }

                if (type.IsNestedAssembly)
                {
                    return languageIcon.@internal;
                }

                if (type.IsNestedPublic)
                {
                    return languageIcon.@public;
                }
            }
            else
            {
                if (type.IsPublic)
                {
                    return languageIcon.@public;
                }

                return languageIcon.@internal;
            }

            return null;
        }

        public static EditorTexture Member(MemberInfo member, ActionDirection direction)
        {
            Ensure.That(nameof(member)).IsNotNull(member);

            var method = member as MethodInfo;
            var field = member as FieldInfo;
            var property = member as PropertyInfo;
            var constructor = member as ConstructorInfo;

            if (method != null)
            {
                if (method.IsExtension())
                {
                    return Language.extensionMethod.@public;
                }

                if (method.IsPrivate)
                {
                    return Language.method.@private;
                }

                if (method.IsFamily)
                {
                    return Language.method.@protected;
                }

                if (method.IsAssembly)
                {
                    return Language.method.@internal;
                }

                if (method.IsPublic)
                {
                    return Language.method.@public;
                }
            }
            else if (constructor != null)
            {
                if (constructor.IsPrivate)
                {
                    return Language.constructor.@private;
                }

                if (constructor.IsFamily)
                {
                    return Language.constructor.@protected;
                }

                if (constructor.IsAssembly)
                {
                    return Language.constructor.@internal;
                }

                if (constructor.IsPublic)
                {
                    return Language.constructor.@public;
                }
            }
            else if (field != null)
            {
                if (field.IsLiteral)
                {
                    if (field.IsPrivate)
                    {
                        return Language.method.@private;
                    }

                    if (field.IsFamily)
                    {
                        return Language.method.@protected;
                    }

                    if (field.IsAssembly)
                    {
                        return Language.method.@internal;
                    }

                    if (field.IsPublic)
                    {
                        return Language.method.@public;
                    }
                }
                else
                {
                    if (field.IsPrivate)
                    {
                        return Language.method.@private;
                    }

                    if (field.IsFamily)
                    {
                        return Language.method.@protected;
                    }

                    if (field.IsAssembly)
                    {
                        return Language.method.@internal;
                    }

                    if (field.IsPublic)
                    {
                        return Language.method.@public;
                    }
                }
            }
            else if (property != null)
            {
                var accessors = property.GetAccessors(true);
                var getter = accessors.FirstOrDefault(accessor => accessor.ReturnType != typeof(void));
                var setter = accessors.FirstOrDefault(accessor => accessor.ReturnType == typeof(void));

                bool isPrivate, isProtected, isInternal, isPublic;

                if (direction == ActionDirection.Any)
                {
                    isPrivate = getter == null || getter.IsPrivate || setter == null || setter.IsPrivate;

                    if (isPrivate)
                    {
                        isProtected = false;
                        isInternal = false;
                        isPublic = false;
                    }
                    else
                    {
                        isProtected = getter.IsFamily || setter.IsFamily;
                        isInternal = getter.IsAssembly || setter.IsAssembly;
                        isPublic = getter.IsPublic && setter.IsPublic;
                    }
                }
                else if (direction == ActionDirection.Get && getter != null)
                {
                    isPrivate = getter.IsPrivate;
                    isProtected = getter.IsFamily;
                    isInternal = getter.IsAssembly;
                    isPublic = getter.IsPublic;
                }
                else if (direction == ActionDirection.Set && setter != null)
                {
                    isPrivate = setter.IsPrivate;
                    isProtected = setter.IsFamily;
                    isInternal = setter.IsAssembly;
                    isPublic = setter.IsPublic;
                }
                else
                {
                    return null;
                }

                if (isPrivate)
                {
                    return Language.property.@private;
                }

                if (isProtected)
                {
                    return Language.property.@protected;
                }

                if (isInternal)
                {
                    return Language.property.@internal;
                }

                if (isPublic)
                {
                    return Language.property.@public;
                }
            }

            return null;
        }

        public static EditorTexture Enum(Enum @enum)
        {
            Ensure.That(nameof(@enum)).IsNotNull(@enum);

            if (!enumIcons.ContainsKey(@enum))
            {
                var enumType = @enum.GetType();

                if (!enumType.IsEnum)
                {
                    throw new ArgumentException(nameof(@enum));
                }

                var namespaced = PluginResources.LoadSharedIcon($"{enumType.CSharpFileName(true)}/{@enum}.png", false);
                var nonNamespaced = PluginResources.LoadSharedIcon($"{enumType.CSharpFileName(false)}/{@enum}.png", false);

                enumIcons.Add(@enum, namespaced ?? nonNamespaced);
            }

            return enumIcons[@enum];
        }

        public static EditorTexture Namespace(Namespace @namespace)
        {
            Ensure.That(nameof(@namespace)).IsNotNull(@namespace);

            if (@namespace.IsGlobal)
            {
                return Language.@namespace.@public;
            }

            if (!namespaceIcons.ContainsKey(@namespace))
            {
                var path = $"{@namespace}.png";

                namespaceIcons.Add(@namespace, PluginResources.LoadSharedIcon(path, false) ?? Language.@namespace.@public);
            }

            return namespaceIcons[@namespace];
        }

        public static class Language
        {
            public static LanguageIconSet method;

            public static LanguageIconSet @namespace { get; private set; }

            public static LanguageIconSet @class { get; private set; }

            public static LanguageIconSet @interface { get; private set; }

            public static LanguageIconSet @struct { get; private set; }

            public static LanguageIconSet @enum { get; private set; }

            public static LanguageIconSet primitive { get; private set; }

            public static LanguageIconSet field { get; private set; }

            public static LanguageIconSet property { get; private set; }

            public static LanguageIconSet extensionMethod { get; private set; }

            public static LanguageIconSet constructor { get; private set; }

            public static LanguageIconSet @const { get; private set; }

            public static LanguageIconSet favorite { get; private set; }

            internal static void Load()
            {
                @namespace = LanguageIconSet.Load("Namespace");
                @interface = LanguageIconSet.Load("Interface");
                @class = LanguageIconSet.Load("Class");
                @struct = LanguageIconSet.Load("Struct");
                @enum = LanguageIconSet.Load("Enum");
                primitive = LanguageIconSet.Load("Primitive");
                field = LanguageIconSet.Load("Field");
                property = LanguageIconSet.Load("Property");
                method = LanguageIconSet.Load("Method");
                extensionMethod = LanguageIconSet.Load("ExtensionMethod");
                constructor = LanguageIconSet.Load("Constructor");
                @const = LanguageIconSet.Load("Const");
                favorite = LanguageIconSet.Load("Favorite");
            }
        }
    }
}
