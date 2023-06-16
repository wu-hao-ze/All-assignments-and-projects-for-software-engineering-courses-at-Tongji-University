using System;

namespace Unity.VisualScripting
{
    public class InvalidConversionException : InvalidCastException
    {
        public InvalidConversionException() : base() { }
        public InvalidConversionException(string message) : base(message) { }
        public InvalidConversionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
