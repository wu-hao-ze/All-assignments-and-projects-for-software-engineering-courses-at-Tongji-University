using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(IMacro))]
    public class MacroDescriptor<TMacro, TMacroDescription> : Descriptor<TMacro, TMacroDescription>
        where TMacro : UnityObject, IMacro
        where TMacroDescription : class, IMacroDescription, new()
    {
        protected MacroDescriptor(TMacro target) : base(target) { }

        protected TMacro macro => target;

        [Assigns(cache = false)]
        [RequiresUnityAPI]
        public override string Title()
        {
            return macro.name;
        }

        [Assigns]
        [RequiresUnityAPI]
        public override string Summary()
        {
            return null;
        }

        [Assigns]
        [RequiresUnityAPI]
        public override EditorTexture Icon()
        {
            return macro.GetType().Icon();
        }
    }
}
