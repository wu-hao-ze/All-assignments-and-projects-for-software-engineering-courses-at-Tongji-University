using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.VisualScripting
{
    public static class EnumUtility
    {
        public static bool HasFlag(this Enum value, Enum flag)
        {
            var lValue = Convert.ToInt64(value);
            var lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) == lFlag;
        }

        public static Dictionary<string, Enum> ValuesByNames(Type enumType, bool obsolete = false)
        {
            Ensure.That(nameof(enumType)).IsNotNull(enumType);

            IEnumerable<FieldInfo> fields = enumType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (!obsolete)
            {
                fields = fields.Where(f => !f.IsDefined(typeof(ObsoleteAttribute), false));
            }

            return fields.ToDictionary(f => f.Name, f => (Enum)f.GetValue(null));
        }

        public static Dictionary<string, T> ValuesByNames<T>(bool obsolete = false)
        {
            IEnumerable<FieldInfo> fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (!obsolete)
            {
                fields = fields.Where(f => !f.IsDefined(typeof(ObsoleteAttribute), false));
            }

            return fields.ToDictionary(f => f.Name, f => (T)f.GetValue(null));
        }
    }
}
