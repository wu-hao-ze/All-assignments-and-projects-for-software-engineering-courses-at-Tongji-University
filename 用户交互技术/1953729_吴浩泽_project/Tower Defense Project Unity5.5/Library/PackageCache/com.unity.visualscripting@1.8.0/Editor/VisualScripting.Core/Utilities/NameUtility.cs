using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class NameUtility
    {
        private static readonly Dictionary<Type, string> humanPrimitiveNames = new Dictionary<Type, string>
        {
            { typeof(byte), "Byte" },
            { typeof(sbyte), "Signed Byte" },
            { typeof(short), "Short" },
            { typeof(ushort), "Unsigned Short" },
            { typeof(int), "Integer" },
            { typeof(uint), "Unsigned Integer" },
            { typeof(long), "Long" },
            { typeof(ulong), "Unsigned Long" },
            { typeof(float), "Float" },
            { typeof(double), "Double" },
            { typeof(decimal), "Decimal" },
            { typeof(string), "String" },
            { typeof(char), "Character" },
            { typeof(bool), "Boolean" },
            { typeof(void), "Void" },
            { typeof(object), "Object" },
        };

        public static readonly Dictionary<string, string> humanOperatorNames = new Dictionary<string, string>
        {
            { "op_Addition", "Add" },
            { "op_Subtraction", "Subtract" },
            { "op_Multiply", "Multiply" },
            { "op_Division", "Divide" },
            { "op_Modulus", "Modulo" },
            { "op_ExclusiveOr", "Exclusive Or" },
            { "op_BitwiseAnd", "Bitwise And" },
            { "op_BitwiseOr", "Bitwise Or" },
            { "op_LogicalAnd", "Logical And" },
            { "op_LogicalOr", "Logical Or" },
            { "op_Assign", "Assign" },
            { "op_LeftShift", "Left Shift" },
            { "op_RightShift", "Right Shift" },
            { "op_Equality", "Equals" },
            { "op_GreaterThan", "Greater Than" },
            { "op_LessThan", "Less Than" },
            { "op_Inequality", "Not Equals" },
            { "op_GreaterThanOrEqual", "Greater Than Or Equals" },
            { "op_LessThanOrEqual", "Less Than Or Equals" },
            { "op_MultiplicationAssignment", "Multiplication Assignment" },
            { "op_SubtractionAssignment", "Subtraction Assignment" },
            { "op_ExclusiveOrAssignment", "Exclusive Or Assignment" },
            { "op_LeftShiftAssignment", "Left Shift Assignment" },
            { "op_ModulusAssignment", "Modulus Assignment" },
            { "op_AdditionAssignment", "Addition Assignment" },
            { "op_BitwiseAndAssignment", "Bitwise And Assignment" },
            { "op_BitwiseOrAssignment", "Bitwise Or Assignment" },
            { "op_Comma", "Comma" },
            { "op_DivisionAssignment", "Division Assignment" },
            { "op_Decrement", "Decrement" },
            { "op_Increment", "Increment" },
            { "op_UnaryNegation", "Negate" },
            { "op_UnaryPlus", "Positive" },
            { "op_OnesComplement", "One's Complement" },
        };

        private static readonly HashSet<string> booleanVerbs = new HashSet<string>
        {
            "Is",
            "Can",
            "Has",
            "Are",
            "Will",
            "Was",
            "Had",
            "Were"
        };

        public static string SelectedName(this Type type, bool human, bool includeGenericParameters = true)
        {
            return human ? type.HumanName(includeGenericParameters) : type.CSharpName(includeGenericParameters);
        }

        public static string SelectedName(this MemberInfo member, bool human, ActionDirection direction = ActionDirection.Any, bool expectingBoolean = false)
        {
            return human ? member.HumanName(direction) : member.CSharpName(direction);
        }

        public static string SelectedName(this ParameterInfo parameter, bool human)
        {
            return human ? parameter.HumanName() : parameter.Name;
        }

        public static string SelectedName(this Exception exception, bool human)
        {
            return human ? exception.HumanName() : exception.GetType().CSharpName(false);
        }

        public static string SelectedName(this Enum @enum, bool human)
        {
            return human ? HumanName(@enum) : @enum.ToString();
        }

        public static string SelectedName(this Namespace @namespace, bool human, bool full = true)
        {
            return human ? @namespace.HumanName(full) : @namespace.CSharpName(full);
        }

        public static string SelectedParameterString(this MethodBase methodBase, Type targetType, bool human)
        {
            return string.Join(", ", methodBase.GetInvocationParameters(targetType).Select(p => p.SelectedName(human)).ToArray());
        }

        public static string DisplayName(this Type type, bool includeGenericParameters = true)
        {
            return SelectedName(type, BoltCore.Configuration.humanNaming, includeGenericParameters);
        }

        public static string DisplayName(this MemberInfo member, ActionDirection direction = ActionDirection.Any, bool expectingBoolean = false)
        {
            return SelectedName(member, BoltCore.Configuration.humanNaming, direction, expectingBoolean);
        }

        public static string DisplayName(this ParameterInfo parameter)
        {
            return SelectedName(parameter, BoltCore.Configuration.humanNaming);
        }

        public static string DisplayName(this Exception exception)
        {
            return SelectedName(exception, BoltCore.Configuration.humanNaming);
        }

        public static string DisplayName(this Enum @enum)
        {
            return SelectedName(@enum, BoltCore.Configuration.humanNaming);
        }

        public static string DisplayName(this Namespace @namespace, bool full = true)
        {
            return SelectedName(@namespace, BoltCore.Configuration.humanNaming, full);
        }

        public static string DisplayParameterString(this MethodBase methodBase, Type targetType)
        {
            return SelectedParameterString(methodBase, targetType, BoltCore.Configuration.humanNaming);
        }

        public static string HumanName(this Type type, bool includeGenericParameters = true)
        {
            if (type == typeof(UnityObject))
            {
                return "Unity Object";
            }

            if (humanPrimitiveNames.ContainsKey(type))
            {
                return humanPrimitiveNames[type];
            }
            else if (type.IsGenericParameter)
            {
                var genericParameterName = type.Name.Prettify();

                if (genericParameterName == "T")
                {
                    return "Generic";
                }
                else if (genericParameterName.StartsWith("T "))
                {
                    return genericParameterName.Substring(2).Prettify() + " Generic";
                }
                else
                {
                    return genericParameterName.Prettify();
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nonNullable = Nullable.GetUnderlyingType(type);

                var underlyingName = nonNullable.HumanName(includeGenericParameters);

                return "Nullable " + underlyingName;
            }
            else
            {
                string name;

                if (Attribute.GetCustomAttribute(type, typeof(DisplayNameAttribute)) is DisplayNameAttribute displayNameAttribute)
                    name = displayNameAttribute.DisplayName;
                else
                    name = type.Name.Prettify();

                if (type.IsInterface && name.StartsWith("I "))
                {
                    name = name.Substring(2) + " Interface";
                }

                if (type.IsArray && name.Contains("[]"))
                {
                    name = name.Replace("[]", " Array");
                }

                if (type.IsGenericType && name.Contains('`'))
                {
                    name = name.Substring(0, name.IndexOf('`'));
                }

                var genericArguments = (IEnumerable<Type>)type.GetGenericArguments();

                if (type.IsNested)
                {
                    name += " of " + type.DeclaringType.HumanName(includeGenericParameters);

                    if (type.DeclaringType.IsGenericType)
                    {
                        genericArguments.Skip(type.DeclaringType.GetGenericArguments().Length);
                    }
                }

                if (genericArguments.Any())
                {
                    if (type.ContainsGenericParameters)
                    {
                        name = "Generic " + name;

                        var count = genericArguments.Count();

                        if (count > 1)
                        {
                            name += " (" + genericArguments.Count() + " parameters)";
                        }
                    }
                    else
                    {
                        name += " of ";
                        name += string.Join(" and ", genericArguments.Select(t => t.HumanName(includeGenericParameters)).ToArray());
                    }
                }

                return name;
            }
        }

        public static string HumanName(this MemberInfo member, ActionDirection direction = ActionDirection.Any, bool expectingBoolean = false)
        {
            var words = member.Name.Prettify();

            if (member is MethodInfo)
            {
                if (((MethodInfo)member).IsOperator())
                {
                    return humanOperatorNames[member.Name];
                }
                else
                {
                    return words;
                }
            }
            else if (member is FieldInfo || member is PropertyInfo)
            {
                if (direction == ActionDirection.Any)
                {
                    return words;
                }

                var type = member is FieldInfo ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType;

                // Fix for Unity's object-to-boolean implicit null-check operators
                if (direction == ActionDirection.Get && typeof(UnityObject).IsAssignableFrom(type) && expectingBoolean)
                {
                    return words + " Is Not Null";
                }

                string verb;

                switch (direction)
                {
                    case ActionDirection.Get:
                        verb = "Get";
                        break;

                    case ActionDirection.Set:
                        verb = "Set";
                        break;

                    default:
                        throw new UnexpectedEnumValueException<ActionDirection>(direction);
                }

                if (type == typeof(bool))
                {
                    // Check for boolean verbs like IsReady, HasChildren, etc.
                    if (words.Contains(' ') && booleanVerbs.Contains(words.Split(' ')[0]))
                    {
                        // Return them as-is for gets
                        if (direction == ActionDirection.Get)
                        {
                            return words;
                        }
                        // Skip them for sets
                        else if (direction == ActionDirection.Set)
                        {
                            return verb + " " + words.Substring(words.IndexOf(' ') + 1);
                        }
                        else
                        {
                            throw new UnexpectedEnumValueException<ActionDirection>(direction);
                        }
                    }
                    else
                    {
                        return verb + " " + words;
                    }
                }
                // Otherwise, add get/set the verb prefix
                else
                {
                    return verb + " " + words;
                }
            }
            else if (member is ConstructorInfo)
            {
                return "Create " + member.DeclaringType.HumanName();
            }
            else
            {
                throw new UnexpectedEnumValueException<ActionDirection>(direction);
            }
        }

        public static string HumanName(this ParameterInfo parameter)
        {
            return parameter.Name.Prettify();
        }

        public static string HumanName(this Exception exception)
        {
            return exception.GetType().CSharpName(false).Prettify().Replace(" Exception", "");
        }

        public static string HumanName(this Enum @enum)
        {
            return @enum.ToString().Prettify();
        }

        public static string CSharpName(this Namespace @namespace, bool full = true)
        {
            return @namespace.IsGlobal ? "(global)" : (full ? @namespace.FullName : @namespace.Name);
        }

        public static string HumanName(this Namespace @namespace, bool full = true)
        {
            return @namespace.IsGlobal ? "(Global Namespace)" : (full ? @namespace.FullName.Replace(".", "/").Prettify() : @namespace.Name.Prettify());
        }

        public static string ToSummaryString(this Exception ex)
        {
            return $"{ex.GetType().DisplayName()}: {ex.Message}";
        }
    }
}
