using System;
using System.ComponentModel;

namespace Unity.VisualScripting
{
    [TypeIcon(typeof(FlowGraph))]
    [UnitCategory("Nesting")]
    [UnitTitle("Subgraph")]
    [RenamedFrom("Bolt.SuperUnit")]
    [RenamedFrom("Unity.VisualScripting.SuperUnit")]
    [DisplayName("Subgraph Node")]
    public sealed class SubgraphUnit : NesterUnit<FlowGraph, ScriptGraphAsset>, IGraphEventListener, IGraphElementWithData
    {
        public sealed class Data : IGraphElementData
        {
            public bool isListening;
        }

        public IGraphElementData CreateData()
        {
            return new Data();
        }

        public SubgraphUnit() : base() { }

        public SubgraphUnit(ScriptGraphAsset macro) : base(macro) { }

        public static SubgraphUnit WithInputOutput()
        {
            var superUnit = new SubgraphUnit();
            superUnit.nest.source = GraphSource.Embed;
            superUnit.nest.embed = FlowGraph.WithInputOutput();
            return superUnit;
        }

        public static SubgraphUnit WithStartUpdate()
        {
            var superUnit = new SubgraphUnit();
            superUnit.nest.source = GraphSource.Embed;
            superUnit.nest.embed = FlowGraph.WithStartUpdate();
            return superUnit;
        }

        public override FlowGraph DefaultGraph()
        {
            return FlowGraph.WithInputOutput();
        }

        protected override void Definition()
        {
            isControlRoot = true; // TODO: Infer relations instead

            // Using portDefinitions and type checks instead of specific definition collections
            // to avoid duplicates. Iterating only once for speed.

            foreach (var definition in nest.graph.validPortDefinitions)
            {
                if (definition is ControlInputDefinition)
                {
                    var controlInputDefinition = (ControlInputDefinition)definition;
                    var key = controlInputDefinition.key;

                    ControlInput(key, (flow) =>
                    {
                        foreach (var unit in nest.graph.units)
                        {
                            if (unit is GraphInput)
                            {
                                var inputUnit = (GraphInput)unit;

                                flow.stack.EnterParentElement(this);

                                return inputUnit.controlOutputs[key];
                            }
                        }

                        return null;
                    });
                }
                else if (definition is ValueInputDefinition)
                {
                    var valueInputDefinition = (ValueInputDefinition)definition;
                    var key = valueInputDefinition.key;
                    var type = valueInputDefinition.type;
                    var hasDefaultValue = valueInputDefinition.hasDefaultValue;
                    var defaultValue = valueInputDefinition.defaultValue;

                    var port = ValueInput(type, key);

                    if (hasDefaultValue)
                    {
                        port.SetDefaultValue(defaultValue);
                    }
                }
                else if (definition is ControlOutputDefinition)
                {
                    var controlOutputDefinition = (ControlOutputDefinition)definition;
                    var key = controlOutputDefinition.key;

                    ControlOutput(key);
                }
                else if (definition is ValueOutputDefinition)
                {
                    var valueOutputDefinition = (ValueOutputDefinition)definition;
                    var key = valueOutputDefinition.key;
                    var type = valueOutputDefinition.type;

                    ValueOutput(type, key, (flow) =>
                    {
                        flow.stack.EnterParentElement(this);

                        // Manual looping to avoid LINQ allocation
                        // Also removing check for multiple output nodes for speed
                        // (The first output node will be used without any error)

                        foreach (var unit in nest.graph.units)
                        {
                            if (unit is GraphOutput)
                            {
                                var outputUnit = (GraphOutput)unit;

                                var value = flow.GetValue(outputUnit.valueInputs[key]);

                                flow.stack.ExitParentElement();

                                return value;
                            }
                        }

                        flow.stack.ExitParentElement();

                        throw new InvalidOperationException("Missing output node when to get value.");
                    });
                }
            }
        }

        public void StartListening(GraphStack stack)
        {
            if (stack.TryEnterParentElement(this))
            {
                nest.graph.StartListening(stack);
                stack.ExitParentElement();
            }

            stack.GetElementData<Data>(this).isListening = true;
        }

        public void StopListening(GraphStack stack)
        {
            stack.GetElementData<Data>(this).isListening = false;

            if (stack.TryEnterParentElement(this))
            {
                nest.graph.StopListening(stack);
                stack.ExitParentElement();
            }
        }

        public bool IsListening(GraphPointer pointer)
        {
            return pointer.GetElementData<Data>(this).isListening;
        }

        #region Editing

        public override void AfterAdd()
        {
            base.AfterAdd();

            nest.beforeGraphChange += StopWatchingPortDefinitions;
            nest.afterGraphChange += StartWatchingPortDefinitions;

            StartWatchingPortDefinitions();
        }

        public override void BeforeRemove()
        {
            base.BeforeRemove();

            StopWatchingPortDefinitions();

            nest.beforeGraphChange -= StopWatchingPortDefinitions;
            nest.afterGraphChange -= StartWatchingPortDefinitions;
        }

        private void StopWatchingPortDefinitions()
        {
            if (nest.graph != null)
            {
                nest.graph.onPortDefinitionsChanged -= Define;
            }
        }

        private void StartWatchingPortDefinitions()
        {
            if (nest.graph != null)
            {
                nest.graph.onPortDefinitionsChanged += Define;
            }
        }

        #endregion
    }
}
