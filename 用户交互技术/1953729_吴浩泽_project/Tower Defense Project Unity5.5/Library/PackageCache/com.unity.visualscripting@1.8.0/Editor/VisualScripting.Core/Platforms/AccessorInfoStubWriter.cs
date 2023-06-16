using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class AccessorInfoStubWriter<TAccessor> : MemberInfoStubWriter<TAccessor> where TAccessor : MemberInfo
    {
        protected AccessorInfoStubWriter(TAccessor accessorInfo) : base(accessorInfo) { }

        protected abstract IOptimizedAccessor GetOptimizedAccessor(TAccessor accessorInfo);

        public override IEnumerable<CodeStatement> GetStubStatements()
        {
            /*
             * Required output:
             * 1. Create a target variable
             * 2. Call its getter to prevent stripping
             * 3. Call its setter to prevent stripping
             * 4. Create its optimized accessor to explicitly compile generic type
             * 5. Call its optimized getter to explicitly compile generic method
             * 6. Call its optimized setter to explicitly compile generic method
            */

            var targetType = new CodeTypeReference(manipulator.targetType, CodeTypeReferenceOptions.GlobalReference);
            var accessorType = new CodeTypeReference(manipulator.type, CodeTypeReferenceOptions.GlobalReference);

            CodeExpression property;

            if (manipulator.requiresTarget)
            {
                // 1. Material target = default(Material);
                yield return new CodeVariableDeclarationStatement(targetType, "target", new CodeDefaultValueExpression(targetType));

                property = new CodeVariableReferenceExpression("target");
            }
            else
            {
                property = new CodeTypeReferenceExpression(targetType);
            }

            // target.color
            var propertyReference = new CodePropertyReferenceExpression(property, manipulator.name);

            if (manipulator.isPubliclyGettable)
            {
                // 2. Color accessor = target.color;
                yield return new CodeVariableDeclarationStatement(accessorType, "accessor", propertyReference);
            }

            if (manipulator.isPubliclySettable)
            {
                // 3. target.color = default(Color);
                yield return new CodeAssignStatement(propertyReference, new CodeDefaultValueExpression(accessorType));
            }

            var optimizedAccessorType = new CodeTypeReference(GetOptimizedAccessor(stub).GetType(), CodeTypeReferenceOptions.GlobalReference);

            // 4. var accessor = new PropertyAccessor<Material, Color>(default(PropertyInfo));
            yield return new CodeVariableDeclarationStatement(optimizedAccessorType,
                "optimized",
                new CodeObjectCreateExpression(optimizedAccessorType,
                    new CodeDefaultValueExpression(new CodeTypeReference(typeof(TAccessor), CodeTypeReferenceOptions.GlobalReference))));

            CodeExpression target;

            if (manipulator.requiresTarget)
            {
                // default(Material)
                target = new CodeDefaultValueExpression(targetType);
            }
            else
            {
                // null for static types
                target = new CodePrimitiveExpression(null);
            }

            if (manipulator.isGettable)
            {
                // 5. accessor.GetValue(default(Material));
                yield return new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("optimized"),
                    nameof(IOptimizedAccessor.GetValue),
                    target));
            }

            if (manipulator.isSettable)
            {
                // 6. accessor.SetValue(default(Material), default(Color));
                yield return new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("optimized"),
                    nameof(IOptimizedAccessor.SetValue),
                    target,
                    new CodeDefaultValueExpression(accessorType)));
            }
        }
    }
}
