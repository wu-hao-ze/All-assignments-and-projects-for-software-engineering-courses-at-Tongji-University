using System;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(IUnitPort))]
    public sealed class UnitPortDescriptor : IDescriptor
    {
        public UnitPortDescriptor(IUnitPort target)
        {
            Ensure.That(nameof(target)).IsNotNull(target);

            this.target = target;

            description.portType = target;
        }

        public IUnitPort target { get; }

        object IDescriptor.target => target;

        public UnitPortDescription description { get; private set; } = new UnitPortDescription();

        IDescription IDescriptor.description => description;

        public bool isDirty { get; set; } = true;

        public void Validate()
        {
            if (isDirty)
            {
                isDirty = false;

                description.fallbackLabel = target.key.Filter(symbols: false, punctuation: false).Prettify();

                description.portType = target;

                target.unit?.Descriptor<IUnitDescriptor>().DescribePort(target, description);

                // No DescriptionAssignment is run, so we'll just always assume that the description changes.
                DescriptorProvider.instance.TriggerDescriptionChange(target);
            }
        }
    }
}
