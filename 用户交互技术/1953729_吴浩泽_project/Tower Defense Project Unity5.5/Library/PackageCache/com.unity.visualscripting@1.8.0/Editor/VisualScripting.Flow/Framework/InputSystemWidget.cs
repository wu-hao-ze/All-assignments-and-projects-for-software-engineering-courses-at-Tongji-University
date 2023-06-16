#if PACKAGE_INPUT_SYSTEM_EXISTS
using System;
using JetBrains.Annotations;
using Unity.VisualScripting.InputSystem;

namespace Unity.VisualScripting
{
    [Widget(typeof(OnInputSystemEvent)), UsedImplicitly]
    public class InputSystemWidget : UnitWidget<OnInputSystemEvent>
    {
        public InputSystemWidget(FlowCanvas canvas, OnInputSystemEvent unit) : base(canvas, unit)
        {
            inputActionInspectorConstructor = metadata => new InputActionInspector(metadata, reference, unit);
        }

        protected override NodeColorMix baseColor => NodeColor.Green;

        private InputActionInspector nameInspector;

        private Func<Metadata, InputActionInspector> inputActionInspectorConstructor;

        public override Inspector GetPortInspector(IUnitPort port, Metadata metadata)
        {
            if (port == unit.InputAction)
            {
                InspectorProvider.instance.Renew(ref nameInspector, metadata, inputActionInspectorConstructor);
                return nameInspector;
            }

            return base.GetPortInspector(port, metadata);
        }
    }

    [Descriptor(typeof(OnInputSystemEvent)), UsedImplicitly]
    public class OnInputSystemButtonDescriptor : UnitDescriptor<OnInputSystemEvent>
    {
        public OnInputSystemButtonDescriptor(OnInputSystemEvent unit) : base(unit) {}

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (port == unit.Target)
                description.summary =
                    "A player input component used to list available actions and find the referenced InputAction";
            if (port == unit.InputAction)
                description.summary =
                    "An input action, either from the linked player input component or directly connected";
        }
    }
}
#endif
