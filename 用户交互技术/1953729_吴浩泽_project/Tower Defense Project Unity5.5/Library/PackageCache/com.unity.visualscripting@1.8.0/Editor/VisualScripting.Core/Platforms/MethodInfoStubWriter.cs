using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.VisualScripting
{
    [AotStubWriter(typeof(MethodInfo))]
    public class MethodInfoStubWriter : MethodBaseStubWriter<MethodInfo>
    {
        public MethodInfoStubWriter(MethodInfo methodInfo) : base(methodInfo) { }

        public override IEnumerable<CodeStatement> GetStubStatements()
        {
            /*
             * Required output:
             * 1. Create a target expression
             * 2. Call its method with the correct number of args
             * 3. Call its optimized method with the correct number of args
             * 4. Call its optimized method with an args array
            */

            var targetType = new CodeTypeReference(manipulator.targetType, CodeTypeReferenceOptions.GlobalReference);
            var declaringType = new CodeTypeReference(stub.DeclaringType, CodeTypeReferenceOptions.GlobalReference);

            CodeExpression targetValue;

            CodeExpression targetReference;

            if (manipulator.requiresTarget && !manipulator.isExtension)
            {
                // default(Material)
                targetValue = new CodeDefaultValueExpression(targetType);

                // 1. Material target = default(Material);
                yield return new CodeVariableDeclarationStatement(targetType, "target", targetValue);

                targetReference = new CodeVariableReferenceExpression("target");
            }
            else
            {
                // null
                targetValue = new CodePrimitiveExpression(null);

                if (manipulator.isExtension)
                {
                    // 1. ShortcutExtensions
                    targetReference = new CodeTypeReferenceExpression(declaringType);
                }
                else
                {
                    // 1. Material
                    targetReference = new CodeTypeReferenceExpression(targetType);
                }
            }

            // target.SetColor
            var methodReference = new CodeMethodReferenceExpression(targetReference, manipulator.name);

            var arguments = new List<CodeExpression>();

            var includesOutOrRef = false;

            foreach (var parameterInfo in stub.GetParameters())
            {
                var parameterType = new CodeTypeReference(parameterInfo.UnderlyingParameterType(), CodeTypeReferenceOptions.GlobalReference);
                var argumentName = $"arg{arguments.Count}";

                // arg0 = default(string)
                // arg1 = default(Color)
                yield return new CodeVariableDeclarationStatement(parameterType, argumentName, new CodeDefaultValueExpression(parameterType));

                FieldDirection direction;

                if (parameterInfo.HasOutModifier())
                {
                    direction = FieldDirection.Out;
                    includesOutOrRef = true;
                }
                else if (parameterInfo.ParameterType.IsByRef)
                {
                    direction = FieldDirection.Ref;
                    includesOutOrRef = true;
                }
                else
                {
                    direction = FieldDirection.In;
                }

                var argument = new CodeDirectionExpression(direction, new CodeVariableReferenceExpression(argumentName));

                arguments.Add(argument);
            }

            if (operatorTypes.ContainsKey(manipulator.name))
            {
                // arg0 * arg1
                var operation = new CodeBinaryOperatorExpression(arguments[0], operatorTypes[manipulator.name], arguments[1]);

                // 2. var operator = arg0 * arg1;
                yield return new CodeVariableDeclarationStatement(manipulator.type, "operator", operation);
            }
            else if (manipulator.isConversion)
            {
                // (Vector3)arg0
                var cast = new CodeCastExpression(manipulator.type, arguments[0]);

                // 2. var conversion = (Vector3)arg0;
                yield return new CodeVariableDeclarationStatement(manipulator.type, "conversion", cast);
            }
            else if (manipulator.isPubliclyInvocable && !(manipulator.isOperator || manipulator.isConversion))
            {
                // 2. target.SetColor(arg0, arg1);
                yield return new CodeExpressionStatement(new CodeMethodInvokeExpression(methodReference, arguments.ToArray()));
            }

            var optimizedInvokerType = new CodeTypeReference(stub.Prewarm().GetType(), CodeTypeReferenceOptions.GlobalReference);

            // var invoker = new InstanceActionInvoker<Material, string, Color>(default(MethodInfo));
            yield return new CodeVariableDeclarationStatement(optimizedInvokerType, "optimized", new CodeObjectCreateExpression(optimizedInvokerType, new CodeDefaultValueExpression(new CodeTypeReference(typeof(MethodInfo), CodeTypeReferenceOptions.GlobalReference))));

            // [default(Material), arg0, arg1]
            var argumentsWithTarget = targetValue.Yield().Concat(arguments).ToArray();

            // Ref and out parameters are not supported in the numbered argument signatures
            if (!includesOutOrRef)
            {
                // 3. invoker.Invoke(default(Material), arg0, arg1);
                yield return new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("optimized"), nameof(IOptimizedInvoker.Invoke), argumentsWithTarget));
            }

            // 4. invoker.Invoke(default(Material), default(object[]));
            yield return new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("optimized"), nameof(IOptimizedInvoker.Invoke), new CodeDefaultValueExpression(new CodeTypeReference(typeof(object[])))));
        }

        public static readonly Dictionary<string, CodeBinaryOperatorType> operatorTypes = new Dictionary<string, CodeBinaryOperatorType>
        {
            { "op_Addition", CodeBinaryOperatorType.Add },
            { "op_Subtraction", CodeBinaryOperatorType.Subtract },
            { "op_Multiply", CodeBinaryOperatorType.Multiply },
            { "op_Division", CodeBinaryOperatorType.Divide },
            { "op_Modulus", CodeBinaryOperatorType.Modulus },
            { "op_BitwiseAnd", CodeBinaryOperatorType.BitwiseAnd },
            { "op_BitwiseOr", CodeBinaryOperatorType.BitwiseOr },
            { "op_LogicalAnd", CodeBinaryOperatorType.BooleanAnd },
            { "op_LogicalOr", CodeBinaryOperatorType.BooleanOr },
            { "op_Assign", CodeBinaryOperatorType.Assign },
            { "op_Equality", CodeBinaryOperatorType.IdentityEquality },
            { "op_GreaterThan", CodeBinaryOperatorType.GreaterThan },
            { "op_LessThan", CodeBinaryOperatorType.LessThan },
            { "op_Inequality", CodeBinaryOperatorType.IdentityInequality },
            { "op_GreaterThanOrEqual", CodeBinaryOperatorType.GreaterThanOrEqual },
            { "op_LessThanOrEqual", CodeBinaryOperatorType.LessThanOrEqual }
        };
    }
}
