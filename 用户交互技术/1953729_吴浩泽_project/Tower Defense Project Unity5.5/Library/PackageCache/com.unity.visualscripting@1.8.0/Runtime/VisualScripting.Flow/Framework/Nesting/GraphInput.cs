using System.Linq;

namespace Unity.VisualScripting
{
    /// <summary>
    /// Fetches input values from the parent super unit for this graph.
    /// </summary>
    [UnitCategory("Nesting")]
    [UnitOrder(1)]
    [UnitTitle("Input")]
    public sealed class GraphInput : Unit
    {
        public override bool canDefine => graph != null;

        protected override void Definition()
        {
            isControlRoot = true;

            foreach (var controlInputDefinition in graph.validPortDefinitions.OfType<ControlInputDefinition>())
            {
                ControlOutput(controlInputDefinition.key);
            }

            foreach (var valueInputDefinition in graph.validPortDefinitions.OfType<ValueInputDefinition>())
            {
                var key = valueInputDefinition.key;
                var type = valueInputDefinition.type;

                ValueOutput(type, key, (flow) =>
                {
                    var superUnit = flow.stack.GetParent<SubgraphUnit>();

                    if (flow.enableDebug)
                    {
                        var editorData = flow.stack.GetElementDebugData<IUnitDebugData>(superUnit);

                        editorData.lastInvokeFrame = EditorTimeBinding.frame;
                        editorData.lastInvokeTime = EditorTimeBinding.time;
                    }

                    flow.stack.ExitParentElement();
                    superUnit.EnsureDefined();
                    var value = flow.GetValue(superUnit.valueInputs[key], type);
                    flow.stack.EnterParentElement(superUnit);

                    return value;
                });
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
