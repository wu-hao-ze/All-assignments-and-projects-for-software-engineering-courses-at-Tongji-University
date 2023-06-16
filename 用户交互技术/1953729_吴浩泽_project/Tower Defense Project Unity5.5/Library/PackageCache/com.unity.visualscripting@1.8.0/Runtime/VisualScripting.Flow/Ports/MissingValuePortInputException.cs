using System;

namespace Unity.VisualScripting
{
    public sealed class MissingValuePortInputException : Exception
    {
        public MissingValuePortInputException(string key) : base($"Missing input value for '{key}'.") { }
    }
}
