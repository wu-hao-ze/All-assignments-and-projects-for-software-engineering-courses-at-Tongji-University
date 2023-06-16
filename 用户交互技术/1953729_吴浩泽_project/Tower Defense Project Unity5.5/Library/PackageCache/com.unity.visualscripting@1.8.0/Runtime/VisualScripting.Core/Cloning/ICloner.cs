using System;

namespace Unity.VisualScripting
{
    public interface ICloner
    {
        bool Handles(Type type);
        object ConstructClone(Type type, object original);
        void BeforeClone(Type type, object original);
        void FillClone(Type type, ref object clone, object original, CloningContext context);
        void AfterClone(Type type, object clone);
    }
}
