using System;

namespace Unity.VisualScripting
{
    public class InvalidImplementationException : Exception
    {
        public InvalidImplementationException() : base() { }
        public InvalidImplementationException(string message) : base(message) { }
    }
}
