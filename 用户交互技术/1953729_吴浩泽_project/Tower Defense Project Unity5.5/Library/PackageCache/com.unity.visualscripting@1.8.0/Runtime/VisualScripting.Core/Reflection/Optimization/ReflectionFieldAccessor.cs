using System.Reflection;

namespace Unity.VisualScripting
{
    public sealed class ReflectionFieldAccessor : IOptimizedAccessor
    {
        public ReflectionFieldAccessor(FieldInfo fieldInfo)
        {
            if (OptimizedReflection.safeMode)
            {
                Ensure.That(nameof(fieldInfo)).IsNotNull(fieldInfo);
            }

            this.fieldInfo = fieldInfo;
        }

        private readonly FieldInfo fieldInfo;

        public void Compile() { }

        public object GetValue(object target)
        {
            return fieldInfo.GetValue(target);
        }

        public void SetValue(object target, object value)
        {
            fieldInfo.SetValue(target, value);
        }
    }
}
