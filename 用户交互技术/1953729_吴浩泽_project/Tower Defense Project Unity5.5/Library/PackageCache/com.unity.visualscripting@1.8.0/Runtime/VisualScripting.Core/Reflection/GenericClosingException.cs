using System;

namespace Unity.VisualScripting
{
    public sealed class GenericClosingException : Exception
    {
        public GenericClosingException(string message) : base(message) { }
        public GenericClosingException(Type open, Type closed) : base($"Open-constructed type '{open}' is not assignable from closed-constructed type '{closed}'.") { }
    }
}
