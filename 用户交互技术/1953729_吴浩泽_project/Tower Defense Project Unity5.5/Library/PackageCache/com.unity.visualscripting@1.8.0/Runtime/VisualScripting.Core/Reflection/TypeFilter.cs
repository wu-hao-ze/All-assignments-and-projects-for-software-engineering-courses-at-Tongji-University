using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Filters the list of types displayed in the inspector drawer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class TypeFilter : Attribute, ICloneable
    {
        public TypeFilter(TypesMatching matching, IEnumerable<Type> types)
        {
            Ensure.That(nameof(types)).IsNotNull(types);

            Matching = matching;
            this.types = new HashSet<Type>(types);

            Value = true;
            Reference = true;
            Classes = true;
            Interfaces = true;
            Structs = true;
            Enums = true;
            Public = true;
            NonPublic = false;
            Abstract = true;
            Generic = true;
            OpenConstructedGeneric = false;
            Static = true;
            Sealed = true;
            Nested = true;
            Primitives = true;
            Object = true;
            NonSerializable = true;
            Obsolete = false;
        }

        public TypeFilter(TypesMatching matching, params Type[] types) : this(matching, (IEnumerable<Type>)types) { }

        public TypeFilter(IEnumerable<Type> types) : this(TypesMatching.ConvertibleToAny, types) { }

        public TypeFilter(params Type[] types) : this(TypesMatching.ConvertibleToAny, types) { }

        private readonly HashSet<Type> types;

        public TypesMatching Matching { get; set; }

        public HashSet<Type> Types => types;

        public bool Value { get; set; }
        public bool Reference { get; set; }
        public bool Classes { get; set; }
        public bool Interfaces { get; set; }
        public bool Structs { get; set; }
        public bool Enums { get; set; }
        public bool Public { get; set; }
        public bool NonPublic { get; set; }
        public bool Abstract { get; set; }
        public bool Generic { get; set; }
        public bool OpenConstructedGeneric { get; set; }
        public bool Static { get; set; }
        public bool Sealed { get; set; }
        public bool Nested { get; set; }
        public bool Primitives { get; set; }
        public bool Object { get; set; }
        public bool NonSerializable { get; set; }
        public bool Obsolete { get; set; }

        public bool ExpectsBoolean => Types.Count == 1 && Types.Single() == typeof(bool);

        object ICloneable.Clone()
        {
            return Clone();
        }

        public TypeFilter Clone()
        {
            return new TypeFilter(Matching, Types.ToArray())
            {
                Value = Value,
                Reference = Reference,
                Classes = Classes,
                Interfaces = Interfaces,
                Structs = Structs,
                Enums = Enums,
                Public = Public,
                NonPublic = NonPublic,
                Abstract = Abstract,
                Generic = Generic,
                OpenConstructedGeneric = OpenConstructedGeneric,
                Static = Static,
                Sealed = Sealed,
                Nested = Nested,
                Primitives = Primitives,
                Object = Object,
                NonSerializable = NonSerializable,
                Obsolete = Obsolete
            };
        }

        public override bool Equals(object obj)
        {
            var other = obj as TypeFilter;

            if (other == null)
            {
                return false;
            }

            return
                Matching == other.Matching &&
                types.SetEquals(other.types) &&
                Value == other.Value &&
                Reference == other.Reference &&
                Classes == other.Classes &&
                Interfaces == other.Interfaces &&
                Structs == other.Structs &&
                Enums == other.Enums &&
                Public == other.Public &&
                NonPublic == other.NonPublic &&
                Abstract == other.Abstract &&
                Generic == other.Generic &&
                OpenConstructedGeneric == other.OpenConstructedGeneric &&
                Static == other.Static &&
                Sealed == other.Sealed &&
                Nested == other.Nested &&
                Primitives == other.Primitives &&
                Object == other.Object &&
                NonSerializable == other.NonSerializable &&
                Obsolete == other.Obsolete;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + Matching.GetHashCode();

                foreach (var type in types)
                {
                    if (type != null)
                    {
                        hash = hash * 23 + type.GetHashCode();
                    }
                }

                hash = hash * 23 + Value.GetHashCode();
                hash = hash * 23 + Reference.GetHashCode();
                hash = hash * 23 + Classes.GetHashCode();
                hash = hash * 23 + Interfaces.GetHashCode();
                hash = hash * 23 + Structs.GetHashCode();
                hash = hash * 23 + Enums.GetHashCode();
                hash = hash * 23 + Public.GetHashCode();
                hash = hash * 23 + NonPublic.GetHashCode();
                hash = hash * 23 + Abstract.GetHashCode();
                hash = hash * 23 + Generic.GetHashCode();
                hash = hash * 23 + OpenConstructedGeneric.GetHashCode();
                hash = hash * 23 + Static.GetHashCode();
                hash = hash * 23 + Sealed.GetHashCode();
                hash = hash * 23 + Nested.GetHashCode();
                hash = hash * 23 + Primitives.GetHashCode();
                hash = hash * 23 + Object.GetHashCode();
                hash = hash * 23 + NonSerializable.GetHashCode();
                hash = hash * 23 + Obsolete.GetHashCode();

                return hash;
            }
        }

        public bool ValidateType(Type type)
        {
            Ensure.That(nameof(type)).IsNotNull(type);

            if (!Generic && type.IsGenericType)
            {
                return false;
            }
            if (!OpenConstructedGeneric && type.ContainsGenericParameters)
            {
                return false;
            }
            if (!Value && type.IsValueType)
            {
                return false;
            }
            if (!Reference && !type.IsValueType)
            {
                return false;
            }
            if (!Classes && type.IsClass)
            {
                return false;
            }
            if (!Interfaces && type.IsInterface)
            {
                return false;
            }
            if (!Structs && (type.IsValueType && !type.IsEnum && !type.IsPrimitive))
            {
                return false;
            }
            if (!Enums && type.IsEnum)
            {
                return false;
            }
            if (!Public && type.IsVisible)
            {
                return false;
            }
            if (!NonPublic && !type.IsVisible)
            {
                return false;
            }
            if (!Abstract && type.IsAbstract())
            {
                return false;
            }
            if (!Static && type.IsStatic())
            {
                return false;
            }
            if (!Sealed && type.IsSealed)
            {
                return false;
            }
            if (!Nested && type.IsNested)
            {
                return false;
            }
            if (!Primitives && type.IsPrimitive)
            {
                return false;
            }
            if (!Object && type == typeof(object))
            {
                return false;
            }
            if (!NonSerializable && !type.IsSerializable)
            {
                return false;
            }
            if (type.IsSpecialName || type.HasAttribute<CompilerGeneratedAttribute>())
            {
                return false;
            }
            if (!Obsolete && type.HasAttribute<ObsoleteAttribute>())
            {
                return false;
            }

            var valid = true;

            if (Types.Count > 0)
            {
                valid = Matching == TypesMatching.AssignableToAll;

                foreach (var allowedType in Types)
                {
                    if (Matching == TypesMatching.Any)
                    {
                        if (type == allowedType)
                        {
                            valid = true;
                            break;
                        }
                    }
                    else if (Matching == TypesMatching.ConvertibleToAny)
                    {
                        if (type.IsConvertibleTo(allowedType, true))
                        {
                            valid = true;
                            break;
                        }
                    }
                    else if (Matching == TypesMatching.AssignableToAll)
                    {
                        valid &= allowedType.IsAssignableFrom(type);

                        if (!valid)
                        {
                            break;
                        }
                    }
                    else
                    {
                        throw new UnexpectedEnumValueException<TypesMatching>(Matching);
                    }
                }
            }

            return valid;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Matching: {Matching}");
            sb.AppendLine($"Types: {types.ToCommaSeparatedString()}");
            sb.AppendLine();
            sb.AppendLine($"Value: {Value}");
            sb.AppendLine($"Reference: {Reference}");
            sb.AppendLine($"Classes: {Classes}");
            sb.AppendLine($"Interfaces: {Interfaces}");
            sb.AppendLine($"Structs: {Structs}");
            sb.AppendLine($"Enums: {Enums}");
            sb.AppendLine($"Public: {Public}");
            sb.AppendLine($"NonPublic: {NonPublic}");
            sb.AppendLine($"Abstract: {Abstract}");
            sb.AppendLine($"Generic: {Generic}");
            sb.AppendLine($"OpenConstructedGeneric: {OpenConstructedGeneric}");
            sb.AppendLine($"Static: {Static}");
            sb.AppendLine($"Sealed: {Sealed}");
            sb.AppendLine($"Nested: {Nested}");
            sb.AppendLine($"Primitives: {Primitives}");
            sb.AppendLine($"Object: {Object}");
            sb.AppendLine($"NonSerializable: {NonSerializable}");
            sb.AppendLine($"Obsolete: {Obsolete}");

            return sb.ToString();
        }

        public static TypeFilter Any => new TypeFilter();
    }
}
