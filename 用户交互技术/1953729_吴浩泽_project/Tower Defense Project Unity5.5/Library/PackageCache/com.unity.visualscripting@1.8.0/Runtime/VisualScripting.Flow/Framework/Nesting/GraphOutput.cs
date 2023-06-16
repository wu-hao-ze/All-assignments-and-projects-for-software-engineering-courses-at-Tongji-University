using System.Linq;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Passes output values from this graph to the parent super unit.
    /// </summary>
    [UnitCategory("Nesting")]
    [UnitOrder(2)]
    [UnitTitle("Output")]
    public sealed class GraphOutput : Unit
    {
        public override bool canDefine => graph != null;

        protected override void Definition()
        {
            isControlRoot = true;

            foreach (var controlOutputDefinition in graph.validPortDefinitions.OfType<ControlOutputDefinition>())
            {
                var key = controlOutputDefinition.key;

                ControlInput(key, (flow) =>
                {
                    var superUnit = flow.stack.GetParent<SubgraphUnit>();

                    flow.stack.ExitParentElement();

                    superUnit.EnsureDefined();

                    return superUnit.controlOutputs[key];
                });
            }

            foreach (var valueOutputDefinition in graph.validPortDefinitions.OfType<ValueOutputDefinition>())
            {
                var key = valueOutputDefinition.key;
                var type = valueOutputDefinition.type;

                ValueInput(type, key);
            }
        }

        protected override void AfterDefine()
        {
            graph.onPortDefinitionsChanged += Define;
        }

        protected override void BeforeUndefine()
        {
            graph.onPortDefinitionsChanged -= Define;
        }
    }
}
