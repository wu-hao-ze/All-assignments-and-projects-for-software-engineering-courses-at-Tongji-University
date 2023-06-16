using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting.FullSerializer.Internal;

namespace Unity.VisualScripting.FullSerializer
{
    /// <summary>
    /// Serializes and deserializes enums by their current name.
    /// </summary>
    public class fsEnumConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            return type.Resolve().IsEnum;
        }

        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            // In .NET compact, Enum.ToObject(Type, Object) is defined but the
            // overloads like Enum.ToObject(Type, int) are not -- so we get
            // around this by boxing the value.
            return Enum.ToObject(storageType, (object)0);
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            if (Serializer.Config.SerializeEnumsAsInteger)
            {
                serialized = new fsData(Convert.ToInt64(instance));
            }
            else if (fsPortableReflection.GetAttribute<FlagsAttribute>(storageType) != null)
            {
                var lValue = Convert.ToInt64(instance);
                var result = new StringBuilder();

                var first = true;
                foreach (var flag in Enum.GetValues(storageType))
                {
                    var lFlag = Convert.ToInt64(flag);

                    if (lFlag == 0)
                    {
                        continue;
                    }

                    var isSet = (lValue & lFlag) == lFlag;

                    if (isSet)
                    {
                        if (first == false)
                        {
                            result.Append(",");
                        }
                        first = false;
                        result.Append(flag.ToString());
                    }
                }

                serialized = new fsData(result.ToString());
            }
            else
            {
                serialized = new fsData(Enum.GetName(storageType, instance));
            }
            return fsResult.Success;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            if (data.IsString)
            {
                var enumValues = data.AsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < enumValues.Length; ++i)
                {
                    var enumValue = enumValues[i];

                    // Verify that the enum name exists; Enum.TryParse is only
                    // available in .NET 4.0 and above :(.
                    var strings = Enum.GetNames(storageType);
                    if (ArrayContains(strings, enumValue) == false)
                    {
                        // Look for a [RenamedFrom("prevName")] attribute on the enum member
                        IEnumerable<(Enum enumMember, string previousName)> valueTuples = Enum.GetValues(storageType).Cast<Enum>().SelectMany(
                            x => x.GetAttributeOfEnumMember<RenamedFromAttribute>().Select(attr => (x, attr.previousName)));
                        Dictionary<string, Enum> enumToRenamedFrom = valueTuples
                            .ToDictionary(x => x.previousName, x => x.enumMember);
                        if (enumToRenamedFrom.TryGetValue(enumValue, out var newName))
                            enumValues[i] = newName.ToString();
                        else
                            return fsResult.Fail("Cannot find enum name " + enumValue + " on type " + storageType);
                    }
                }

                var underlyingType = Enum.GetUnderlyingType(storageType);

                if (underlyingType == typeof(ulong))
                {
                    ulong instanceValue = 0;

                    for (var i = 0; i < enumValues.Length; ++i)
                    {
                        var enumValue = enumValues[i];
                        var flagValue = (ulong)Convert.ChangeType(Enum.Parse(storageType, enumValue), typeof(ulong));
                        instanceValue |= flagValue;
                    }

                    instance = Enum.ToObject(storageType, (object)instanceValue);
                }
                else
                {
                    long instanceValue = 0;

                    for (var i = 0; i < enumValues.Length; ++i)
                    {
                        var enumValue = enumValues[i];
                        var flagValue = (long)Convert.ChangeType(Enum.Parse(storageType, enumValue), typeof(long));
                        instanceValue |= flagValue;
                    }

                    instance = Enum.ToObject(storageType, (object)instanceValue);
                }

                return fsResult.Success;
            }
            else if (data.IsInt64)
            {
                var enumValue = (int)data.AsInt64;

                // In .NET compact, Enum.ToObject(Type, Object) is defined but
                // the overloads like Enum.ToObject(Type, int) are not -- so we
                // get around this by boxing the value.
                instance = Enum.ToObject(storageType, (object)enumValue);

                return fsResult.Success;
            }

            return fsResult.Fail($"EnumConverter encountered an unknown JSON data type for {storageType}: {data.Type}");
        }

        /// <summary>
        /// Returns true if the given value is contained within the specified
        /// array.
        /// </summary>
        private static bool ArrayContains<T>(T[] values, T value)
        {
            // note: We don't use LINQ because this function will *not* allocate
            for (var i = 0; i < values.Length; ++i)
            {
                if (EqualityComparer<T>.Default.Equals(values[i], value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
