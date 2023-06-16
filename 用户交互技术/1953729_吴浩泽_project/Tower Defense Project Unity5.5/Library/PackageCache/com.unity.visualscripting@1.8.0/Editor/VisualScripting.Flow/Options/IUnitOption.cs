using System;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IUnitOption : IFuzzyOption
    {
        IUnit unit { get; }

        IUnit InstantiateUnit();
        void PreconfigureUnit(IUnit unit);

        HashSet<string> sourceScriptGuids { get; }
        int order { get; }
        UnitCategory category { get; }
        string favoriteKey { get; }
        bool favoritable { get; }
        Type unitType { get; }

        #region Serialization

        void Deserialize(UnitOptionRow row);
        UnitOptionRow Serialize();

        #endregion

        #region Filtering

        int controlInputCount { get; }
        int controlOutputCount { get; }
        HashSet<Type> valueInputTypes { get; }
        HashSet<Type> valueOutputTypes { get; }

        #endregion

        #region Search

        string haystack { get; }
        string formerHaystack { get; }
        string SearchResultLabel(string query);

        #endregion
    }

    public static class XUnitOption
    {
        public static bool UnitIs(this IUnitOption option, Type type)
        {
            return type.IsAssignableFrom(option.unitType);
        }

        public static bool UnitIs<T>(this IUnitOption option)
        {
            return option.UnitIs(typeof(T));
        }

        public static bool HasCompatibleValueInput(this IUnitOption option, Type outputType)
        {
            Ensure.That(nameof(outputType)).IsNotNull(outputType);

            foreach (var valueInputType in option.valueInputTypes)
            {
                if (ConversionUtility.CanConvert(outputType, valueInputType, false))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasCompatibleValueOutput(this IUnitOption option, Type inputType)
        {
            Ensure.That(nameof(inputType)).IsNotNull(inputType);

            foreach (var valueOutputType in option.valueOutputTypes)
            {
                if (ConversionUtility.CanConvert(valueOutputType, inputType, false))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
