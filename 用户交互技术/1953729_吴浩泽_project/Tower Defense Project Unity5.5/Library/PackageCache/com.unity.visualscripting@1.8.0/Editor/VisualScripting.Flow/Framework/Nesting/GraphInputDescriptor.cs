using System.Linq;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(GraphInput))]
    public class GraphInputDescriptor : UnitDescriptor<GraphInput>
    {
        public GraphInputDescriptor(GraphInput unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            var definition = unit.graph.validPortDefinitions.OfType<IUnitInputPortDefinition>().SingleOrDefault(d => d.key == port.key);

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
