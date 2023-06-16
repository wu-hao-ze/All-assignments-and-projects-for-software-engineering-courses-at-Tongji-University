using System;

namespace Unity.VisualScripting
{
    public interface IGettable
    {
        object GetValue();
    }

    public static class XGettable
    {
        public static object GetValue(this IGettable gettable, Type type)
        {
            return ConversionUtility.Convert(gettable.GetValue(), type);
        }

        public static T GetValue<T>(this IGettable gettable)
        {
            return (T)gettable.GetValue(typeof(T));
        }
    }
}
