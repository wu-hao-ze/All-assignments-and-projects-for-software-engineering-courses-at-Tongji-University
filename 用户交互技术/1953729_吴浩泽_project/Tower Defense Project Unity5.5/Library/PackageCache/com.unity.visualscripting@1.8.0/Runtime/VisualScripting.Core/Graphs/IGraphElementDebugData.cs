using System;

namespace Unity.VisualScripting
{
    public interface IGraphElementDebugData
    {
        // Being lazy with the interfaces here to simplify things
        Exception runtimeException { get; set; }
    }
}
