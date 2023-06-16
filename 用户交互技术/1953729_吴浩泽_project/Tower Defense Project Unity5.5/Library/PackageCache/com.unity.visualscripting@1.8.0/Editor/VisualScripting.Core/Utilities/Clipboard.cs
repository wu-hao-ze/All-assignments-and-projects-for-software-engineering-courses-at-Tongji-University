using System;

namespace Unity.VisualScripting
{
    public class Clipboard
    {
        public SerializationData data { get; private set; }
        public Type dataType { get; private set; }
        public bool containsData => dataType != null;

        public void Clear()
        {
            data = default(SerializationData);
            dataType = null;
        }

        public void Copy(object value)
        {
            Ensure.That(nameof(value)).IsNotNull(value);

            data = value.Serialize();
            dataType = value.GetType();
        }

        public bool CanPaste(Type type)
        {
            return containsData && dataType.IsConvertibleTo(type, true);
        }

        public bool CanPaste<T>()
        {
            return CanPaste(typeof(T));
        }

        public T Paste<T>()
        {
            return (T)Paste(typeof(T));
        }

        public object Paste()
        {
            if (!containsData)
            {
                throw new InvalidOperationException($"Graph clipboard does not contain data.");
            }

            return data.Deserialize();
        }

        public object Paste(Type type)
        {
            if (!CanPaste(type))
            {
                throw new InvalidOperationException($"Graph clipboard does not contain '{type.CSharpName(false)}'.");
            }

            return ConversionUtility.Convert(Paste(), type);
        }
    }
}
