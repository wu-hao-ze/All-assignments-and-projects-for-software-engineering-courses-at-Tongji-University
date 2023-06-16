#if PACKAGE_INPUT_SYSTEM_EXISTS
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine.InputSystem;

namespace Unity.VisualScripting
{
    [Analyser(typeof(OnInputSystemEvent))]
    public class OnInputSystemEventAnalyser : UnitAnalyser<OnInputSystemEvent>
    {
        public OnInputSystemEventAnalyser(GraphReference reference, OnInputSystemEvent target) : base(reference, target) {}

        protected override IEnumerable<Warning> Warnings()
        {
            foreach (var baseWarning in base.Warnings())
                yield return baseWarning;

            if (target.InputActionChangeType == InputActionChangeOption.OnHold ||
                target.InputActionChangeType == InputActionChangeOption.OnReleased)
            {
                if (Flow.CanPredict(target.InputAction, reference))
                {
                    var inputAction = Flow.Predict<InputAction>(target.InputAction, reference);
                    if (inputAction.type == InputActionType.PassThrough)
                        yield return Warning.Caution($"Input action '{inputAction.name}' is of type 'Passthrough', which do not support 'On Hold' or 'On Released' events");
                }
            }
        }
    }
}
#endif
