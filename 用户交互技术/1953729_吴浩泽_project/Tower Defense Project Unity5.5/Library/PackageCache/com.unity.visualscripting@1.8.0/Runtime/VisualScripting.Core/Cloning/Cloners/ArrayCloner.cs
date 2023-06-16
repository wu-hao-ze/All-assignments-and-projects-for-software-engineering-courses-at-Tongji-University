using System;

namespace Unity.VisualScripting
{
    public sealed class ArrayCloner : Cloner<Array>
    {
        public override bool Handles(Type type)
        {
            return type.IsArray;
        }

        public override Array ConstructClone(Type type, Array original)
        {
            return Array.CreateInstance(type.GetElementType(), 0);
        }

        public override void FillClone(Type type, ref Array clone, Array original, CloningContext context)
        {
            var length = original.GetLength(0);

            clone = Array.CreateInstance(type.GetElementType(), length);

            for (int i = 0; i < length; i++)
            {
                clone.SetValue(Cloning.Clone(context, original.GetValue(i)), i);
            }
        }
    }
}
