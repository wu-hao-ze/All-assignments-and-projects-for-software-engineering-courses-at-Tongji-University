using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public interface IGraphContextExtension : IDragAndDropHandler
    {
        IEnumerable<GraphContextMenuItem> contextMenuItems { get; }
    }
}
