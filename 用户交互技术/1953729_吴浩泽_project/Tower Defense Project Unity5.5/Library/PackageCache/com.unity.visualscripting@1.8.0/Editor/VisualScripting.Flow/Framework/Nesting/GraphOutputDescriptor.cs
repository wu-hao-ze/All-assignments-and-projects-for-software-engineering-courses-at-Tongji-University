using System.Linq;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(GraphOutput))]
    public class GraphOutputDescriptor : UnitDescriptor<GraphOutput>
    {
        public GraphOutputDescriptor(GraphOutput unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            var definition = unit.graph.validPortDefinitions.OfType<IUnitOutputPortDefinition>().SingleOrDefault(d => d.key == port.key);

            if (definition != null)
            {
                description.label = definition.Label();
                description.summary = definition.summary;

                if (definition.hideLabel)
                {
                    description.showLabel = false;
                }
            }
        }
    }
}
