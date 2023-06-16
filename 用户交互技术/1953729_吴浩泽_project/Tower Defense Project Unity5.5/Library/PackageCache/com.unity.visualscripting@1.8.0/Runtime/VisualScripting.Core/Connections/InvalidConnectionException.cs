using System;

namespace Unity.VisualScripting
{
    public class InvalidConnectionException : Exception
    {
        public InvalidConnectionException() : base("") { }
        public InvalidConnectionException(string message) : base(message) { }
    }
}
