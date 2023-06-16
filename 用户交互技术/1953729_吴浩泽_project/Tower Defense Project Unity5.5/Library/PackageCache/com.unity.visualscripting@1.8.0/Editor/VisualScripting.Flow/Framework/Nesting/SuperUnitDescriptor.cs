using System.Linq;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(SubgraphUnit))]
    public class SuperUnitDescriptor : NesterUnitDescriptor<SubgraphUnit>
    {
        public SuperUnitDescriptor(SubgraphUnit unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (unit.graph == null)
            {
                return;
            }

            var definition = unit.nest.graph.validPortDefinitions.SingleOrDefault(d => d.key == port.key);

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
