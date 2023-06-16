using System;

namespace Unity.VisualScripting.FullSerializer
{
    /// <summary>
    /// The given property or field annotated with [JsonIgnore] will not be
    /// serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class fsIgnoreAttribute : Attribute { }
}
