using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.VisualScripting
{
    [AotStubWriter(typeof(FieldInfo))]
    public class FieldInfoStubWriter : AccessorInfoStubWriter<FieldInfo>
    {
        public FieldInfoStubWriter(FieldInfo fieldInfo) : base(fieldInfo) { }

        protected override IOptimizedAccessor GetOptimizedAccessor(FieldInfo fieldInfo)
        {
            return fieldInfo.Prewarm();
        }

        public override IEnumerable<CodeStatement> GetStubStatements()
        {
            if (stub.IsPublic && stub.IsLiteral && !stub.IsInitOnly && stub.DeclaringType != null)
            {
                if (manipulator.isPubliclyGettable)
                {
                    var targetType = new CodeTypeReference(manipulator.targetType, CodeTypeReferenceOptions.GlobalReference);
                    var accessorType = new CodeTypeReference(manipulator.type, CodeTypeReferenceOptions.GlobalReference);
                    var property = new CodeTypeReferenceExpression(targetType);
                    var propertyReference = new CodePropertyReferenceExpression(property, manipulator.name);
                    yield return new CodeVariableDeclarationStatement(accessorType, "accessor", propertyReference);
                }

                const string variableName = "optimized";

                var optimizedAccessorType = new CodeTypeReference(
                    GetOptimizedAccessor(stub).GetType(), CodeTypeReferenceOptions.GlobalReference);
                yield return new CodeVariableDeclarationStatement(optimizedAccessorType,
                    variableName,
                    new CodeObjectCreateExpression(optimizedAccessorType,
                        new CodeMethodInvokeExpression(
                            new CodeMethodReferenceExpression(
                                new CodeTypeOfExpression(
                                    new CodeTypeReference(stub.DeclaringType, CodeTypeReferenceOptions.GlobalReference)),
                                nameof(Type.GetField)),
                            new CodePrimitiveExpression(stub.Name),
                            new CodeSnippetExpression("System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static"))));

                if (manipulator.isGettable)
                {
                    var target = new CodePrimitiveExpression(null);
                    yield return new CodeExpressionStatement(
                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(variableName),
                            nameof(IOptimizedAccessor.GetValue),
                            target));
                }

                yield break;
            }

            foreach (var statement in base.GetStubStatements())
                yield return statement;
        }
    }
}
