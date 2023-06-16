namespace Unity.VisualScripting
{
    [Descriptor(typeof(SelectOnFlow))]
    public class SelectOnFlowDescriptor : UnitDescriptor<SelectOnFlow>
    {
        public SelectOnFlowDescriptor(SelectOnFlow unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            foreach (var branch in unit.branches)
            {
                if (port == branch.Key || port == branch.Value)
                {
                    var index = int.Parse(port.key.PartAfter('_'));

                    var letter = ((char)('A' + index)).ToString();

                    description.label = letter;

                    if (port == branch.Key)
                    {
                        description.summary = $"Trigger to select the {letter} value.";
                    }
                    else if (port == branch.Value)
                    {
                        description.summary = $"The value to return if the {letter} control input is triggered.";
                    }
                }
            }
        }
    }
}
