using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Unity.VisualScripting
{
    [SerializationVersion("A")]
    [DisplayName("Script Graph")]
    public sealed class FlowGraph : Graph, IGraphWithVariables, IGraphEventListener
    {
        public FlowGraph()
        {
            units = new GraphElementCollection<IUnit>(this);
            controlConnections = new GraphConnectionCollection<ControlConnection, ControlOutput, ControlInput>(this);
            valueConnections = new GraphConnectionCollection<ValueConnection, ValueOutput, ValueInput>(this);
            invalidConnections = new GraphConnectionCollection<InvalidConnection, IUnitOutputPort, IUnitInputPort>(this);
            groups = new GraphElementCollection<GraphGroup>(this);
            sticky = new GraphElementCollection<StickyNote>(this);

            elements.Include(units);
            elements.Include(controlConnections);
            elements.Include(valueConnections);
            elements.Include(invalidConnections);
            elements.Include(groups);
            elements.Include(sticky);

            controlInputDefinitions = new UnitPortDefinitionCollection<ControlInputDefinition>();
            controlOutputDefinitions = new UnitPortDefinitionCollection<ControlOutputDefinition>();
            valueInputDefinitions = new UnitPortDefinitionCollection<ValueInputDefinition>();
            valueOutputDefinitions = new UnitPortDefinitionCollection<ValueOutputDefinition>();

            variables = new VariableDeclarations();
        }

        public override IGraphData CreateData()
        {
            return new FlowGraphData(this);
        }

        public void StartListening(GraphStack stack)
        {
            stack.GetGraphData<FlowGraphData>().isListening = true;

            foreach (var unit in units)
            {
                (unit as IGraphEventListener)?.StartListening(stack);
            }
        }

        public void StopListening(GraphStack stack)
        {
            foreach (var unit in units)
            {
                (unit as IGraphEventListener)?.StopListening(stack);
            }

            stack.GetGraphData<FlowGraphData>().isListening = false;
        }

        public bool IsListening(GraphPointer pointer)
        {
            return pointer.GetGraphData<FlowGraphData>().isListening;
        }

        #region Variables

        [Serialize]
        public VariableDeclarations variables { get; private set; }

        public IEnumerable<string> GetDynamicVariableNames(VariableKind kind, GraphReference reference)
        {
            return units.OfType<IUnifiedVariableUnit>()
                .Where(v => v.kind == kind && Flow.CanPredict(v.name, reference))
                .Select(v => Flow.Predict<string>(v.name, reference))
                .Where(name => !StringUtility.IsNullOrWhiteSpace(name))
                .Distinct()
                .OrderBy(name => name);
        }

        #endregion


        #region Elements

        [DoNotSerialize]
        public GraphElementCollection<IUnit> units { get; private set; }

        [DoNotSerialize]
        public GraphConnectionCollection<ControlConnection, ControlOutput, ControlInput> controlConnections { get; private set; }

        [DoNotSerialize]
        public GraphConnectionCollection<ValueConnection, ValueOutput, ValueInput> valueConnections { get; private set; }

        [DoNotSerialize]
        public GraphConnectionCollection<InvalidConnection, IUnitOutputPort, IUnitInputPort> invalidConnections { get; private set; }

        [DoNotSerialize]
        public GraphElementCollection<GraphGroup> groups { get; private set; }

        [DoNotSerialize]
        public GraphElementCollection<StickyNote> sticky { get; private set; }
        #endregion


        #region Definition

        private const string DefinitionRemoveWarningTitle = "Remove Port Definition";

        private const string DefinitionRemoveWarningMessage = "Removing this definition will break any existing connection to this port. Are you sure you want to continue?";

        [Serialize]
        [InspectorLabel("Trigger Inputs")]
        [InspectorWide(true)]
        [WarnBeforeRemoving(DefinitionRemoveWarningTitle, DefinitionRemoveWarningMessage)]
        public UnitPortDefinitionCollection<ControlInputDefinition> controlInputDefinitions { get; private set; }

        [Serialize]
        [InspectorLabel("Trigger Outputs")]
        [InspectorWide(true)]
        [WarnBeforeRemoving(DefinitionRemoveWarningTitle, DefinitionRemoveWarningMessage)]
        public UnitPortDefinitionCollection<ControlOutputDefinition> controlOutputDefinitions { get; private set; }

        [Serialize]
        [InspectorLabel("Data Inputs")]
        [InspectorWide(true)]
        [WarnBeforeRemoving(DefinitionRemoveWarningTitle, DefinitionRemoveWarningMessage)]
        public UnitPortDefinitionCollection<ValueInputDefinition> valueInputDefinitions { get; private set; }

        [Serialize]
        [InspectorLabel("Data Outputs")]
        [InspectorWide(true)]
        [WarnBeforeRemoving(DefinitionRemoveWarningTitle, DefinitionRemoveWarningMessage)]
        public UnitPortDefinitionCollection<ValueOutputDefinition> valueOutputDefinitions { get; private set; }

        public IEnumerable<IUnitPortDefinition> validPortDefinitions =>
            LinqUtility.Concat<IUnitPortDefinition>(controlInputDefinitions,
                controlOutputDefinitions,
                valueInputDefinitions,
                valueOutputDefinitions)
                .Where(upd => upd.isValid)
                .DistinctBy(upd => upd.key);

        public event Action onPortDefinitionsChanged;

        public void PortDefinitionsChanged()
        {
            onPortDefinitionsChanged?.Invoke();
        }

        #endregion

        public static FlowGraph WithInputOutput()
        {
            return new FlowGraph()
            {
                units =
                {
                    new GraphInput() { position = new Vector2(-250, -30) },
                    new GraphOutput() { position = new Vector2(105, -30) }
                }
            };
        }

        public static FlowGraph WithStartUpdate()
        {
            return new FlowGraph()
            {
                units =
                {
                    new Start() { position = new Vector2(-204, -144) },
                    new Update() { position = new Vector2(-204, 60) }
                }
            };
        }
    }
}
