namespace Unity.VisualScripting
{
    [Descriptor(typeof(Minimum<>))]
    [Descriptor(typeof(Maximum<>))]
    [Descriptor(typeof(Sum<>))]
    [Descriptor(typeof(Average<>))]
    [Descriptor(typeof(MergeDictionaries))]
    [Descriptor(typeof(Formula))]
    public class MultiInputUnitAlphabeticDescriptor : UnitDescriptor<IMultiInputUnit>
    {
        public MultiInputUnitAlphabeticDescriptor(IMultiInputUnit unit) : base(unit) { }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            if (port is ValueInput)
            {
                var index = unit.multiInputs.IndexOf((ValueInput)port);

                if (index >= 0)
                {
                    description.label = ((char)('A' + index)).ToString();
                }
            }
        }
    }

    [FuzzyOption(typeof(Minimum<>))]
    [FuzzyOption(typeof(Maximum<>))]
    [FuzzyOption(typeof(Sum<>))]
    [FuzzyOption(typeof(Average<>))]
    [FuzzyOption(typeof(MergeDictionaries))]
    [FuzzyOption(typeof(Formula))]
    public class MultiInputUnitAlphabeticOption : UnitOption<IMultiInputUnit>
    {
        public MultiInputUnitAlphabeticOption() : base() { }

        public MultiInputUnitAlphabeticOption(IMultiInputUnit unit) : base(unit) { }

        protected override bool ShowValueInputsInFooter()
        {
            return false;
        }
    }
}
