using System.Reflection;

namespace Unity.VisualScripting
{
    [AotStubWriter(typeof(PropertyInfo))]
    public class PropertyInfoStubWriter : AccessorInfoStubWriter<PropertyInfo>
    {
        public PropertyInfoStubWriter(PropertyInfo propertyInfo) : base(propertyInfo) { }

        protected override IOptimizedAccessor GetOptimizedAccessor(PropertyInfo propertyInfo)
        {
            return propertyInfo.Prewarm();
        }
    }
}
