using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    public class EnumOptionTree : FuzzyOptionTree
    {
        public EnumOptionTree(Type enumType) : base(new GUIContent(enumType.HumanName()))
        {
            Ensure.That(nameof(enumType)).IsNotNull(enumType);

            enums = Enum.GetValues(enumType).Cast<Enum>().ToList();
        }

        private readonly List<Enum> enums;

        public override IFuzzyOption Option(object item)
        {
            return new EnumOption((Enum)item);
        }

        public override IEnumerable<object> Root()
        {
            return enums.Cast<object>();
        }

        public override IEnumerable<object> Children(object item)
        {
            return Enumerable.Empty<object>();
        }

        public static EnumOptionTree For<T>()
        {
            return new EnumOptionTree(typeof(T));
        }
    }
}
