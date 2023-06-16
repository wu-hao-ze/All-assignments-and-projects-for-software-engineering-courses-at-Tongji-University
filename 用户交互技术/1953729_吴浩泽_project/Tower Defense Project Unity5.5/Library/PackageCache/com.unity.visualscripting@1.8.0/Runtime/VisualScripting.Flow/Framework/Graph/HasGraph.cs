using System;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    [UnitCategory("Graphs/Graph Nodes")]
    public abstract class HasGraph<TGraph, TMacro, TMachine> : Unit
        where TGraph : class, IGraph, new()
        where TMacro : Macro<TGraph>
        where TMachine : Machine<TGraph, TMacro>
    {
        /// <summary>
        /// The entry point for the node.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>
        /// The GameObject or the Machine where to look for the graph.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput target { get; private set; }

        /// <summary>
        /// The Graph to look for.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Graph")]
        [PortLabelHidden]
        public ValueInput graphInput { get; private set; }

        /// <summary>
        /// True if a Graph if found.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Has Graph")]
        [PortLabelHidden]
        public ValueOutput hasGraphOutput { get; private set; }

        /// <summary>
        /// The action to execute once the graph has been set.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected abstract bool isGameObject { get; }
        Type targetType => isGameObject ? typeof(GameObject) : typeof(TMachine);

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), TriggerHasGraph);
            target = ValueInput(targetType, nameof(target)).NullMeansSelf();
            target.SetDefaultValue(targetType.PseudoDefault());

            graphInput = ValueInput<TMacro>(nameof(graphInput), null);
            hasGraphOutput = ValueOutput(nameof(hasGraphOutput), OutputHasGraph);
            exit = ControlOutput(nameof(exit));

            Requirement(graphInput, enter);
            Assignment(enter, hasGraphOutput);
            Succession(enter, exit);
        }

        ControlOutput TriggerHasGraph(Flow flow)
        {
            flow.SetValue(hasGraphOutput, OutputHasGraph(flow));
            return exit;
        }

        bool OutputHasGraph(Flow flow)
        {
            var macro = flow.GetValue<TMacro>(graphInput);
            var targetValue = flow.GetValue(target, targetType);

            if (targetValue is GameObject gameObject)
            {
                if (gameObject != null)
                {
                    var stateMachines = gameObject.GetComponents<TMachine>();
                    macro = flow.GetValue<TMacro>(graphInput);

                    return stateMachines
                        .Where(currentMachine => currentMachine != null)
                        .Any(currentMachine => currentMachine.graph != null && currentMachine.graph.Equals(macro.graph));
                }
            }
            else
            {
                TMachine machine = flow.GetValue<TMachine>(target);

                if (machine.graph != null && machine.graph.Equals(macro.graph))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
