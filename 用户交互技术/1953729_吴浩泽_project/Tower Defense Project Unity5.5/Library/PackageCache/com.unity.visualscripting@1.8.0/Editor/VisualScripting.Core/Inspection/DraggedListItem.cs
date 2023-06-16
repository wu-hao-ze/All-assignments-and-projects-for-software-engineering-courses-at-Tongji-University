using System.Collections;

namespace Unity.VisualScripting
{
    public class DraggedListItem
    {
        public DraggedListItem(MetadataListAdaptor sourceListAdaptor, int index, object item)
        {
            this.sourceListAdaptor = sourceListAdaptor;
            this.index = index;
            this.item = item;
        }

        public readonly MetadataListAdaptor sourceListAdaptor;
        public readonly int index;
        public readonly object item;

        public IList sourceList => (IList)sourceListAdaptor.metadata.value;

        public static readonly string TypeName = typeof(DraggedListItem).FullName;

        public override string ToString()
        {
            return $"{item} ({sourceList}[{index}])";
        }
    }
}
