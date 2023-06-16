using System;

namespace Unity.VisualScripting
{
    public interface IUnitValuePort : IUnitPort
    {
        Type type { get; }
    }
}
