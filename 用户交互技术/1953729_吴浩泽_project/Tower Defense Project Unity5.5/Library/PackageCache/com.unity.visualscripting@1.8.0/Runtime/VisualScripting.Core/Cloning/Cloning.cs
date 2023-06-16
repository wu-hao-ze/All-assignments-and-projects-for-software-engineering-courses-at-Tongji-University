using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    public static class Cloning
    {
        static Cloning()
        {
            cloners.Add(arrayCloner);
            cloners.Add(dictionaryCloner);
            cloners.Add(enumerableCloner);
            cloners.Add(listCloner);
            cloners.Add(animationCurveCloner);
        }

        // Cloning has to be really fast, and skippable takes a while.
        private static readonly Dictionary<Type, bool> skippable = new Dictionary<Type, bool>();

        public static HashSet<ICloner> cloners { get; } = new HashSet<ICloner>();

        public static ArrayCloner arrayCloner { get; } = new ArrayCloner();
        public static DictionaryCloner dictionaryCloner { get; } = new DictionaryCloner();
        public static EnumerableCloner enumerableCloner { get; } = new EnumerableCloner();
        public static ListCloner listCloner { get; } = new ListCloner();
        public static AnimationCurveCloner animationCurveCloner { get; } = new AnimationCurveCloner();

        public static FieldsCloner fieldsCloner { get; } = new FieldsCloner();
        public static FakeSerializationCloner fakeSerializationCloner { get; } = new FakeSerializationCloner();

        public static object Clone(this object original, ICloner fallbackCloner, bool tryPreserveInstances)
        {
            using (var context = CloningContext.New(fallbackCloner, tryPreserveInstances))
            {
                return Clone(context, original);
            }
        }

        public static T Clone<T>(this T original, ICloner fallbackCloner, bool tryPreserveInstances)
        {
            return (T)Clone((object)original, fallbackCloner, tryPreserveInstances);
        }

        public static object CloneViaFakeSerialization(this object original)
        {
            return original.Clone(fakeSerializationCloner, true);
        }

        public static T CloneViaFakeSerialization<T>(this T original)
        {
            return (T)CloneViaFakeSerialization((object)original);
        }

        internal static object Clone(CloningContext context, object original)
        {
            object clone = null;
            CloneInto(context, ref clone, original);
            return clone;
        }

        internal static void CloneInto(CloningContext context, ref object clone, object original)
        {
            if (original == null)
            {
                clone = null;
                return;
            }

            var type = original.GetType();

            if (Skippable(type))
            {
                clone = original;
                return;
            }

            if (context.clonings.ContainsKey(original))
            {
                clone = context.clonings[original];
                return;
            }

            var cloner = GetCloner(original, type, context.fallbackCloner);

            if (clone == null)
            {
                clone = cloner.ConstructClone(type, original);
            }

            context.clonings.Add(original, clone);
            cloner.BeforeClone(type, original);
            cloner.FillClone(type, ref clone, original, context);
            cloner.AfterClone(type, clone);
            context.clonings[original] = clone; // In case the reference changed, for example in arrays
        }

        [CanBeNull]
        public static ICloner GetCloner(object original, Type type)
        {
            if (original is ISpecifiesCloner cloneableVia)
            {
                return cloneableVia.cloner;
            }

            return cloners.FirstOrDefault(cloner => cloner.Handles(type));
        }

        private static ICloner GetCloner(object original, Type type, ICloner fallbackCloner)
        {
            var cloner = GetCloner(original, type);

            if (cloner != null)
                return cloner;

            Ensure.That(nameof(fallbackCloner)).IsNotNull(fallbackCloner);

            return fallbackCloner;
        }

        private static bool Skippable(Type type)
        {
            bool result;

            if (!skippable.TryGetValue(type, out result))
            {
                result = type.IsValueType || // Value types are copied on assignment, so no cloning is necessary
                    type == typeof(string) ||      // Strings have copy on write semantics as well, but aren't value types
                    typeof(Type).IsAssignableFrom(type) ||      // Types are guaranteed to be singletons. Using inheritance because MonoType/RuntimeType extend Type
                    typeof(UnityObject).IsAssignableFrom(type);      // Unity objects act as pure references

                skippable.Add(type, result);
            }

            return result;
        }
    }
}
