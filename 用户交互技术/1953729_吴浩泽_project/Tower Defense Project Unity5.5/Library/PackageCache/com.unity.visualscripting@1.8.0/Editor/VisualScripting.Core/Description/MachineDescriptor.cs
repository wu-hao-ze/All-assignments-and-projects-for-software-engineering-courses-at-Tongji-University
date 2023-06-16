using UnityObject = UnityEngine.Object;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(IMachine))]
    public class MachineDescriptor<TMachine, TMachineDescription> : Descriptor<TMachine, TMachineDescription>
        where TMachine : UnityObject, IMachine
        where TMachineDescription : class, IMachineDescription, new()
    {
        protected MachineDescriptor(TMachine target) : base(target) { }

        protected TMachine machine => target;

        [Assigns(cache = false)]
        [RequiresUnityAPI]
        public override string Title()
        {
            return machine.name;
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
            return machine.GetType().Icon();
        }
    }
}
