namespace Unity.VisualScripting
{
    [Descriptor(typeof(CreateList))]
    [Descriptor(typeof(MergeLists))]
    public class MultiInputUnitNumericDescriptor : UnitDescriptor<IMultiInputUnit>
    {
        public MultiInputUnitNumericDescriptor(IMultiInputUnit unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (port is ValueInput)
            {
                var index = unit.multiInputs.IndexOf((ValueInput)port);

                if (index >= 0)
                {
                    description.label = index.ToString();
                }
            }
        }
    }

    [FuzzyOption(typeof(CreateList))]
    [FuzzyOption(typeof(MergeLists))]
    public class MultiInputUnitNumericOption : UnitOption<IMultiInputUnit>
    {
        public MultiInputUnitNumericOption() : base() { }

        public MultiInputUnitNumericOption(IMultiInputUnit unit) : base(unit) { }

        protected override bool ShowValueInputsInFooter()
        {
            return false;
        }
    }
}
