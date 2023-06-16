using System;

namespace Unity.VisualScripting
{
    public interface IUnitValuePortDefinition : IUnitPortDefinition
    {
        Type type { get; }
    }
}
