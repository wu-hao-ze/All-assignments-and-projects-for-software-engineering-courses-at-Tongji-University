using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class SerializedPropertyProviderProvider : SingleDecoratorProvider<Type, ISerializedPropertyProvider, SerializedPropertyProviderAttribute>
    {
        protected override ISerializedPropertyProvider CreateDecorator(Type providerType, Type type)
        {
            var targetObject = ScriptableObject.CreateInstance(providerType);
            targetObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave; // HideAndDontSave would define NotEditable
            return (ISerializedPropertyProvider)targetObject;
        }

        protected override IEnumerable<Type> typeset => Codebase.types;

        protected override bool cache => false;

        protected override Type GetDecoratedType(Type decorated)
        {
            return decorated;
        }

        public override bool IsValid(Type decorated)
        {
            return true;
        }

        static SerializedPropertyProviderProvider()
        {
            instance = new SerializedPropertyProviderProvider();
        }

        public static SerializedPropertyProviderProvider instance { get; private set; }

        public void GenerateProviderScripts()
        {
            if (Directory.Exists(BoltCore.Paths.propertyProviders))
            {
                foreach (var file in Directory.GetFiles(BoltCore.Paths.propertyProviders))
                {
                    File.Delete(file);
                }
            }

            if (Directory.Exists(BoltCore.Paths.propertyProvidersEditor))
            {
                foreach (var file in Directory.GetFiles(BoltCore.Paths.propertyProvidersEditor))
                {
                    File.Delete(file);
                }
            }

            PathUtility.CreateDirectoryIfNeeded(BoltCore.Paths.propertyProviders);
            PathUtility.CreateDirectoryIfNeeded(BoltCore.Paths.propertyProvidersEditor);

            foreach (var type in typeset.Where(SerializedPropertyUtility.HasCustomDrawer))
            {
                var directory = Codebase.IsEditorType(type) ? BoltCore.Paths.propertyProvidersEditor : BoltCore.Paths.propertyProviders;
                var path = Path.Combine(directory, GetProviderScriptName(type) + ".cs");

                VersionControlUtility.Unlock(path);
                File.WriteAllText(path, GenerateProviderSource(type));
            }

            AssetDatabase.Refresh();
        }

        private static string GetProviderScriptName(Type type)
        {
            // The file name has to match the class name for Unity
            return "PropertyProvider_" + type.CSharpFullName().Replace(".", "_");
        }

        private static string GenerateProviderSource(Type type)
        {
            /* Example:

            namespace Unity.VisualScripting.Generated.PropertyProviders
            {
                [Ludiq.SerializedPropertyProvider(typeof(MyNamespace.MyType))]
                public class MyNamespace_MyType : SerializedPropertyProvider<MyNamespace.MyType> { }
            }

            */

            Ensure.That(nameof(type)).IsNotNull(type);

            var unit = new CodeCompileUnit();

            var @namespace = new CodeNamespace("Unity.VisualScripting.Generated.PropertyProviders");

            unit.Namespaces.Add(@namespace);

            var @class = new CodeTypeDeclaration(GetProviderScriptName(type))
            {
                IsClass = true
            };

            @class.BaseTypes.Add(typeof(SerializedPropertyProvider<>).MakeGenericType(type));

            var serializedPropertyProviderAttribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializedPropertyProviderAttribute), CodeTypeReferenceOptions.GlobalReference), new CodeAttributeArgument(new CodeTypeOfExpression(new CodeTypeReference(type, CodeTypeReferenceOptions.GlobalReference))));

            @class.CustomAttributes.Add(serializedPropertyProviderAttribute);

            @namespace.Types.Add(@class);

            using (var provider = CodeDomProvider.CreateProvider("CSharp"))
            {
                var options = new CodeGeneratorOptions
                {
                    BracingStyle = "C",
                    IndentString = "\t",
                    BlankLinesBetweenMembers = true,
                    ElseOnClosing = false,
                    VerbatimOrder = true
                };

                using (var stringWriter = new StringWriter())
                {
                    provider.GenerateCodeFromCompileUnit(unit, stringWriter, options);

                    return stringWriter.ToString();
                }
            }
        }
    }
}
