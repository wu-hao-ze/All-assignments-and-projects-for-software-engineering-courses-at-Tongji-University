using System;
using UnityEngine;

namespace Unity.VisualScripting
{
    [UnitCategory("Graphs/Graph Nodes")]
    public abstract class SetGraph<TGraph, TMacro, TMachine> : Unit
        where TGraph : class, IGraph, new()
        where TMacro : Macro<TGraph>
        where TMachine : Machine<TGraph, TMacro>
    {
        /// <summary>
        /// The entry point for the node.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; protected set; }

        /// <summary>
        /// The GameObject or the ScriptMachine where the graph will be set.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput target { get; protected set; }

        /// <summary>
        /// The script graph.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Graph")]
        [PortLabelHidden]
        public ValueInput graphInput { get; protected set; }

        /// <summary>
        /// The graph that has been set to the ScriptMachine.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Graph")]
        [PortLabelHidden]
        public ValueOutput graphOutput { get; protected set; }

        /// <summary>
        /// The action to execute once the graph has been set.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; protected set; }

        protected abstract bool isGameObject { get; }
        Type targetType => isGameObject ? typeof(GameObject) : typeof(TMachine);

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), SetMacro);
            target = ValueInput(targetType, nameof(target)).NullMeansSelf();
            target.SetDefaultValue(targetType.PseudoDefault());

            graphInput = ValueInput<TMacro>(nameof(graphInput), null);
            graphOutput = ValueOutput<TMacro>(nameof(graphOutput));
            exit = ControlOutput(nameof(exit));

            Requirement(graphInput, enter);
            Assignment(enter, graphOutput);
            Succession(enter, exit);
        }

        ControlOutput SetMacro(Flow flow)
        {
            var macro = flow.GetValue<TMacro>(graphInput);
            var targetValue = flow.GetValue(target, targetType);

            if (targetValue is GameObject go)
            {
                go.GetComponent<TMachine>().nest.SwitchToMacro(macro);
            }
            else
            {
                ((TMachine)targetValue).nest.SwitchToMacro(macro);
            }

            flow.SetValue(graphOutput, macro);

            return exit;
        }
    }
}
