using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.VisualScripting
{
    public static class CSharpNameUtility
    {
        private static readonly Dictionary<Type, string> primitives = new Dictionary<Type, string>
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(string), "string" },
            { typeof(char), "char" },
            { typeof(bool), "bool" },
            { typeof(void), "void" },
            { typeof(object), "object" },
        };

        public static readonly Dictionary<string, string> operators = new Dictionary<string, string>
        {
            { "op_Addition", "+" },
            { "op_Subtraction", "-" },
            { "op_Multiply", "*" },
            { "op_Division", "/" },
            { "op_Modulus", "%" },
            { "op_ExclusiveOr", "^" },
            { "op_BitwiseAnd", "&" },
            { "op_BitwiseOr", "|" },
            { "op_LogicalAnd", "&&" },
            { "op_LogicalOr", "||" },
            { "op_Assign", "=" },
            { "op_LeftShift", "<<" },
            { "op_RightShift", ">>" },
            { "op_Equality", "==" },
            { "op_GreaterThan", ">" },
            { "op_LessThan", "<" },
            { "op_Inequality", "!=" },
            { "op_GreaterThanOrEqual", ">=" },
            { "op_LessThanOrEqual", "<=" },
            { "op_MultiplicationAssignment", "*=" },
            { "op_SubtractionAssignment", "-=" },
            { "op_ExclusiveOrAssignment", "^=" },
            { "op_LeftShiftAssignment", "<<=" },
            { "op_ModulusAssignment", "%=" },
            { "op_AdditionAssignment", "+=" },
            { "op_BitwiseAndAssignment", "&=" },
            { "op_BitwiseOrAssignment", "|=" },
            { "op_Comma", "," },
            { "op_DivisionAssignment", "/=" },
            { "op_Decrement", "--" },
            { "op_Increment", "++" },
            { "op_UnaryNegation", "-" },
            { "op_UnaryPlus", "+" },
            { "op_OnesComplement", "~" },
        };

        private static readonly HashSet<char> illegalTypeFileNameCharacters = new HashSet<char>()
        {
            '<',
            '>',
            '?',
            ' ',
            ',',
            ':',
        };

        public static string CSharpName(this MemberInfo member, ActionDirection direction)
        {
            if (member is MethodInfo && ((MethodInfo)member).IsOperator())
            {
                return operators[member.Name] + " operator";
            }

            if (member is ConstructorInfo)
            {
                return "new " + member.DeclaringType.CSharpName();
            }

            if ((member is FieldInfo || member is PropertyInfo) && direction != ActionDirection.Any)
            {
                return $"{member.Name} ({direction.ToString().ToLower()})";
            }

            return member.Name;
        }

        public static string CSharpName(this Type type, bool includeGenericParameters = true)
        {
            return type.CSharpName(TypeQualifier.Name, includeGenericParameters);
        }

        public static string CSharpFullName(this Type type, bool includeGenericParameters = true)
        {
            return type.CSharpName(TypeQualifier.Namespace, includeGenericParameters);
        }

        public static string CSharpUniqueName(this Type type, bool includeGenericParameters = true)
        {
            return type.CSharpName(TypeQualifier.GlobalNamespace, includeGenericParameters);
        }

        public static string CSharpFileName(this Type type, bool includeNamespace, bool includeGenericParameters = false)
        {
            var fileName = type.CSharpName(includeNamespace ? TypeQualifier.Namespace : TypeQualifier.Name, includeGenericParameters);

            if (!includeGenericParameters && type.IsGenericType && fileName.Contains('<'))
            {
                fileName = fileName.Substring(0, fileName.IndexOf('<'));
            }

            fileName = fileName.ReplaceMultiple(illegalTypeFileNameCharacters, '_')
                .Trim('_')
                .RemoveConsecutiveCharacters('_');

            return fileName;
        }

        private static string CSharpName(this Type type, TypeQualifier qualifier, bool includeGenericParameters = true)
        {
            if (primitives.ContainsKey(type))
            {
                return primitives[type];
            }
            else if (type.IsGenericParameter)
            {
                return includeGenericParameters ? type.Name : "";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nonNullable = Nullable.GetUnderlyingType(type);

                var underlyingName = nonNullable.CSharpName(qualifier, includeGenericParameters);

                return underlyingName + "?";
            }
            else
            {
                var name = type.Name;

                if (type.IsGenericType && name.Contains('`'))
                {
                    name = name.Substring(0, name.IndexOf('`'));
                }

                var genericArguments = (IEnumerable<Type>)type.GetGenericArguments();

                if (type.IsNested)
                {
                    name = type.DeclaringType.CSharpName(qualifier, includeGenericParameters) + "." + name;

                    if (type.DeclaringType.IsGenericType)
                    {
                        genericArguments.Skip(type.DeclaringType.GetGenericArguments().Length);
                    }
                }

                if (!type.IsNested)
                {
                    if ((qualifier == TypeQualifier.Namespace || qualifier == TypeQualifier.GlobalNamespace) && type.Namespace != null)
                    {
                        name = type.Namespace + "." + name;
                    }

                    if (qualifier == TypeQualifier.GlobalNamespace)
                    {
                        name = "global::" + name;
                    }
                }

                if (genericArguments.Any())
                {
                    name += "<";
                    name += string.Join(includeGenericParameters ? ", " : ",", genericArguments.Select(t => t.CSharpName(qualifier, includeGenericParameters)).ToArray());
                    name += ">";
                }

                return name;
            }
        }
    }
}
