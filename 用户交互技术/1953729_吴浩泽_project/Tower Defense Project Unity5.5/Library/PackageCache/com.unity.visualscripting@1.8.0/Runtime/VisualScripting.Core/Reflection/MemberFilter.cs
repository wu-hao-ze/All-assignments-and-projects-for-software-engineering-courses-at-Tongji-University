using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class MemberFilter : Attribute, ICloneable
    {
        public MemberFilter()
        {
            // Whitelist
            Fields = false;
            Properties = false;
            Methods = false;
            Constructors = false;
            Gettable = false;
            Settable = false;

            // Blacklist
            Inherited = true;
            Targeted = true;
            NonTargeted = true;
            Public = true;
            NonPublic = false;
            ReadOnly = true;
            WriteOnly = true;
            Extensions = true;
            Operators = true;
            Conversions = true;
            Parameters = true;
            Obsolete = false;
            OpenConstructedGeneric = false;
            TypeInitializers = true;
            ClsNonCompliant = true;
        }

        public bool Fields { get; set; }
        public bool Properties { get; set; }
        public bool Methods { get; set; }
        public bool Constructors { get; set; }
        public bool Gettable { get; set; }
        public bool Settable { get; set; }

        public bool Inherited { get; set; }
        public bool Targeted { get; set; }
        public bool NonTargeted { get; set; }
        public bool Public { get; set; }
        public bool NonPublic { get; set; }
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public bool Extensions { get; set; }
        public bool Operators { get; set; }
        public bool Conversions { get; set; }
        public bool Setters { get; set; }
        public bool Parameters { get; set; }
        public bool Obsolete { get; set; }
        public bool OpenConstructedGeneric { get; set; }
        public bool TypeInitializers { get; set; }
        public bool ClsNonCompliant { get; set; }

        public BindingFlags validBindingFlags
        {
            get
            {
                BindingFlags flags = 0;

                if (Public)
                {
                    flags |= BindingFlags.Public;
                }
                if (NonPublic)
                {
                    flags |= BindingFlags.NonPublic;
                }
                if (Targeted || Constructors)
                {
                    flags |= BindingFlags.Instance;
                }
                if (NonTargeted)
                {
                    flags |= BindingFlags.Static;
                }
                if (!Inherited)
                {
                    flags |= BindingFlags.DeclaredOnly;
                }
                if (NonTargeted && Inherited)
                {
                    flags |= BindingFlags.FlattenHierarchy;
                }

                return flags;
            }
        }

        public MemberTypes validMemberTypes
        {
            get
            {
                MemberTypes types = 0;

                if (Fields || Gettable || Settable)
                {
                    types |= MemberTypes.Field;
                }

                if (Properties || Gettable || Settable)
                {
                    types |= MemberTypes.Property;
                }

                if (Methods || Gettable)
                {
                    types |= MemberTypes.Method;
                }

                if (Constructors || Gettable)
                {
                    types |= MemberTypes.Constructor;
                }

                return types;
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public MemberFilter Clone()
        {
            return new MemberFilter()
            {
                Fields = Fields,
                Properties = Properties,
                Methods = Methods,
                Constructors = Constructors,
                Gettable = Gettable,
                Settable = Settable,
                Inherited = Inherited,
                Targeted = Targeted,
                NonTargeted = NonTargeted,
                Public = Public,
                NonPublic = NonPublic,
                ReadOnly = ReadOnly,
                WriteOnly = WriteOnly,
                Extensions = Extensions,
                Operators = Operators,
                Conversions = Conversions,
                Parameters = Parameters,
                Obsolete = Obsolete,
                OpenConstructedGeneric = OpenConstructedGeneric,
                TypeInitializers = TypeInitializers,
                ClsNonCompliant = ClsNonCompliant
            };
        }

        public override bool Equals(object obj)
        {
            var other = obj as MemberFilter;

            if (other == null)
            {
                return false;
            }

            return
                Fields == other.Fields &&
                Properties == other.Properties &&
                Methods == other.Methods &&
                Constructors == other.Constructors &&
                Gettable == other.Gettable &&
                Settable == other.Settable &&
                Inherited == other.Inherited &&
                Targeted == other.Targeted &&
                NonTargeted == other.NonTargeted &&
                Public == other.Public &&
                NonPublic == other.NonPublic &&
                ReadOnly == other.ReadOnly &&
                WriteOnly == other.WriteOnly &&
                Extensions == other.Extensions &&
                Operators == other.Operators &&
                Conversions == other.Conversions &&
                Parameters == other.Parameters &&
                Obsolete == other.Obsolete &&
                OpenConstructedGeneric == other.OpenConstructedGeneric &&
                TypeInitializers == other.TypeInitializers &&
                ClsNonCompliant == other.ClsNonCompliant;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + Fields.GetHashCode();
                hash = hash * 23 + Properties.GetHashCode();
                hash = hash * 23 + Methods.GetHashCode();
                hash = hash * 23 + Constructors.GetHashCode();
                hash = hash * 23 + Gettable.GetHashCode();
                hash = hash * 23 + Settable.GetHashCode();

                hash = hash * 23 + Inherited.GetHashCode();
                hash = hash * 23 + Targeted.GetHashCode();
                hash = hash * 23 + NonTargeted.GetHashCode();
                hash = hash * 23 + Public.GetHashCode();
                hash = hash * 23 + NonPublic.GetHashCode();
                hash = hash * 23 + ReadOnly.GetHashCode();
                hash = hash * 23 + WriteOnly.GetHashCode();
                hash = hash * 23 + Extensions.GetHashCode();
                hash = hash * 23 + Operators.GetHashCode();
                hash = hash * 23 + Conversions.GetHashCode();
                hash = hash * 23 + Parameters.GetHashCode();
                hash = hash * 23 + Obsolete.GetHashCode();
                hash = hash * 23 + OpenConstructedGeneric.GetHashCode();
                hash = hash * 23 + TypeInitializers.GetHashCode();
                hash = hash * 23 + ClsNonCompliant.GetHashCode();

                return hash;
            }
        }

        public bool ValidateMember(MemberInfo member, TypeFilter typeFilter = null)
        {
            if (member is FieldInfo)
            {
                var field = (FieldInfo)member;

                // Whitelist
                var isGettable = true;
                var isSettable = !field.IsLiteral && !field.IsInitOnly;
                var whitelisted = Fields || (Gettable && isGettable) || (Settable && isSettable);
                if (!whitelisted)
                {
                    return false;
                }

                // Targetting
                var isTargeted = !field.IsStatic;
                if (!Targeted && isTargeted)
                {
                    return false;
                }
                if (!NonTargeted && !isTargeted)
                {
                    return false;
                }

                // Accessibility
                if (!WriteOnly && !isGettable)
                {
                    return false;
                }
                if (!ReadOnly && !isSettable)
                {
                    return false;
                }

                // Visibility
                if (!Public && field.IsPublic)
                {
                    return false;
                }
                if (!NonPublic && !field.IsPublic)
                {
                    return false;
                }

                // Type
                if (typeFilter != null && !typeFilter.ValidateType(field.FieldType))
                {
                    return false;
                }

                // Other
                if (field.IsSpecialName)
                {
                    return false;
                }
            }
            else if (member is PropertyInfo)
            {
                var property = (PropertyInfo)member;
                var getter = property.GetGetMethod(true);
                var setter = property.GetSetMethod(true);

                // Whitelist
                var isGettable = property.CanRead;
                var isSettable = property.CanWrite;
                var whitelisted = Properties || (Gettable && isGettable) || (Settable && isSettable);
                if (!whitelisted)
                {
                    return false;
                }

                // Visibility & Accessibility
                // TODO: Refactor + Take into account when Public = false
                var requiresRead = (!WriteOnly || (!Properties && Gettable));
                var requiresWrite = (!ReadOnly || (!Properties && Settable));
                var canRead = property.CanRead && (NonPublic || getter.IsPublic);
                var canWrite = property.CanWrite && (NonPublic || setter.IsPublic);
                if (requiresRead && !canRead)
                {
                    return false;
                }
                if (requiresWrite && !canWrite)
                {
                    return false;
                }

                // Targetting
                var isTargeted = !(getter ?? setter).IsStatic;
                if (!Targeted && isTargeted)
                {
                    return false;
                }
                if (!NonTargeted && !isTargeted)
                {
                    return false;
                }

                // Type
                if (typeFilter != null && !typeFilter.ValidateType(property.PropertyType))
                {
                    return false;
                }

                // Other
                if (property.IsSpecialName)
                {
                    return false;
                }
                if (property.GetIndexParameters().Any())
                {
                    return false;
                }
            }
            else if (member is MethodBase)
            {
                var methodOrConstructor = (MethodBase)member;
                var isExtension = methodOrConstructor.IsExtensionMethod();
                var isTargeted = !methodOrConstructor.IsStatic || isExtension;

                // Visibility
                if (!Public && methodOrConstructor.IsPublic)
                {
                    return false;
                }
                if (!NonPublic && !methodOrConstructor.IsPublic)
                {
                    return false;
                }

                // Other
                if (!Parameters && (methodOrConstructor.GetParameters().Length > (isExtension ? 1 : 0)))
                {
                    return false;
                }
                if (!OpenConstructedGeneric && methodOrConstructor.ContainsGenericParameters)
                {
                    return false;
                }

                if (member is MethodInfo)
                {
                    var method = (MethodInfo)member;
                    var isOperator = method.IsOperator();
                    var isConversion = method.IsUserDefinedConversion();

                    // Whitelist
                    var isGettable = method.ReturnType != typeof(void);
                    var isSettable = false;
                    var whitelisted = Methods || (Gettable && isGettable) || (Settable && isSettable);
                    if (!whitelisted)
                    {
                        return false;
                    }

                    // Targetting
                    if (!Targeted && isTargeted)
                    {
                        return false;
                    }
                    if (!NonTargeted && !isTargeted)
                    {
                        return false;
                    }

                    // Operators
                    if (!Operators && isOperator)
                    {
                        return false;
                    }

                    // Extensions
                    if (!Extensions && isExtension)
                    {
                        return false;
                    }

                    // Type
                    if (typeFilter != null && !typeFilter.ValidateType(method.ReturnType))
                    {
                        return false;
                    }

                    // Other
                    if (method.IsSpecialName && !(isOperator || isConversion))
                    {
                        return false;
                    }

                    // Not supported return type
#if !UNITY_2022_1_OR_NEWER
                    if (isGettable && method.ReturnType.ToString().Contains("ReadOnlySpan"))
                    {
                        return false;
                    }
#else
                    if (isGettable && method.ReturnType.IsByRefLike)
                    {
                        return false;
                    }
#endif
                }
                else if (member is ConstructorInfo)
                {
                    var constructor = (ConstructorInfo)member;

                    // Whitelist
                    var isGettable = true;
                    var isSettable = false;
                    var whitelisted = Constructors || (Gettable && isGettable) || (Settable && isSettable);
                    if (!whitelisted)
                    {
                        return false;
                    }

                    // Type
                    if (typeFilter != null && !typeFilter.ValidateType(constructor.DeclaringType))
                    {
                        return false;
                    }

                    // Type Initializers
                    if (constructor.IsStatic && !TypeInitializers)
                    {
                        return false;
                    }

                    // Other
                    if (typeof(Component).IsAssignableFrom(member.DeclaringType) || typeof(ScriptableObject).IsAssignableFrom(member.DeclaringType))
                    {
                        return false;
                    }
                }
            }

            // Obsolete
            if (!Obsolete && member.HasAttribute<ObsoleteAttribute>(false))
            {
                return false;
            }

            // CLS Compliance
            if (!ClsNonCompliant)
            {
                var clsCompliantAttribute = member.GetAttribute<CLSCompliantAttribute>();

                if (clsCompliantAttribute != null && !clsCompliantAttribute.IsCompliant)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Fields: {Fields}");
            sb.AppendLine($"Properties: {Properties}");
            sb.AppendLine($"Methods: {Methods}");
            sb.AppendLine($"Constructors: {Constructors}");
            sb.AppendLine($"Gettable: {Gettable}");
            sb.AppendLine($"Settable: {Settable}");
            sb.AppendLine();
            sb.AppendLine($"Inherited: {Inherited}");
            sb.AppendLine($"Instance: {Targeted}");
            sb.AppendLine($"Static: {NonTargeted}");
            sb.AppendLine($"Public: {Public}");
            sb.AppendLine($"NonPublic: {NonPublic}");
            sb.AppendLine($"ReadOnly: {ReadOnly}");
            sb.AppendLine($"WriteOnly: {WriteOnly}");
            sb.AppendLine($"Extensions: {Extensions}");
            sb.AppendLine($"Operators: {Operators}");
            sb.AppendLine($"Conversions: {Conversions}");
            sb.AppendLine($"Parameters: {Parameters}");
            sb.AppendLine($"Obsolete: {Obsolete}");
            sb.AppendLine($"OpenConstructedGeneric: {OpenConstructedGeneric}");
            sb.AppendLine($"TypeInitializers: {TypeInitializers}");
            sb.AppendLine($"ClsNonCompliant: {ClsNonCompliant}");

            return sb.ToString();
        }

        public static MemberFilter Any => new MemberFilter()
        {
            Fields = true,
            Properties = true,
            Methods = true,
            Constructors = true
        };
    }
}
