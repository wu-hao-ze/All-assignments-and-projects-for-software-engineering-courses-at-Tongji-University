using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.VisualScripting
{
    [AotStubWriter(typeof(ConstructorInfo))]
    public class ConstructorInfoStubWriter : MethodBaseStubWriter<ConstructorInfo>
    {
        public ConstructorInfoStubWriter(ConstructorInfo constructorInfo) : base(constructorInfo) { }

        public override IEnumerable<CodeStatement> GetStubStatements()
        {
            /*
             * Required output:
             * 1. Call the constructor with the correct number of args
             * (No optimization available for constructors)
            */

            var arguments = new List<CodeExpression>();

            foreach (var parameterInfo in stub.GetParameters())
            {
                var parameterType = new CodeTypeReference(parameterInfo.UnderlyingParameterType(), CodeTypeReferenceOptions.GlobalReference);
                var argumentName = $"arg{arguments.Count}";

                yield return new CodeVariableDeclarationStatement(parameterType, argumentName, new CodeDefaultValueExpression(parameterType));

                FieldDirection direction;

                if (parameterInfo.HasOutModifier())
                {
                    direction = FieldDirection.Out;
                }
                else if (parameterInfo.ParameterType.IsByRef)
                {
                    direction = FieldDirection.Ref;
                }
                else
                {
                    direction = FieldDirection.In;
                }

                var argument = new CodeDirectionExpression(direction, new CodeVariableReferenceExpression(argumentName));

                arguments.Add(argument);
            }

            if (manipulator.isPubliclyInvocable)
            {
                yield return new CodeExpressionStatement(new CodeObjectCreateExpression(stub.DeclaringType, arguments.ToArray()));
            }
        }
    }
}
