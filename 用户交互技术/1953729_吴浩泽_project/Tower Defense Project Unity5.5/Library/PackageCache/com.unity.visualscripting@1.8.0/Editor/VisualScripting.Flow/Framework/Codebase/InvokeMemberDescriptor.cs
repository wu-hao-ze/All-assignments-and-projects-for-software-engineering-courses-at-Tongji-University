using System.Linq;

namespace Unity.VisualScripting
{
    [Descriptor(typeof(InvokeMember))]
    public class InvokeMemberDescriptor : MemberUnitDescriptor<InvokeMember>
    {
        public InvokeMemberDescriptor(InvokeMember unit) : base(unit) { }

        protected override ActionDirection direction => ActionDirection.Any;

        protected override string DefinedShortTitle()
        {
            if (member.isConstructor)
            {
                return BoltCore.Configuration.humanNaming ? "Create" : "new";
            }

            return base.DefinedShortTitle();
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);

            var documentation = member.info.Documentation();

            if (port == unit.enter)
            {
                description.label = "Invoke";
                description.summary = "The entry point to invoke the method.";

                if (member.isGettable)
                {
                    description.summary += " You can still get the return value without connecting this port.";
                }
            }
            else if (port == unit.exit)
            {
                description.summary = "The action to call once the method has been invoked.";
            }
            else if (port == unit.result)
            {
                if (member.isGettable)
                {
                    description.summary = documentation?.returns;
                }

                if (unit.supportsChaining && unit.chainable)
                {
                    description.showLabel = true;
                }
            }
            else if (port == unit.targetOutput)
            {
                if (member.isGettable)
                {
                    description.showLabel = true;
                }
            }
            else if (port is ValueInput && unit.inputParameters.ContainsValue((ValueInput)port))
            {
                var parameter = member.GetParameterInfos().Single(p => "%" + p.Name == port.key);

                description.label = parameter.DisplayName();
                description.summary = documentation?.ParameterSummary(parameter);
            }
            else if (port is ValueOutput && unit.outputParameters.ContainsValue((ValueOutput)port))
            {
                var parameter = member.GetParameterInfos().Single(p => "&" + p.Name == port.key);

                description.label = parameter.DisplayName();
                description.summary = documentation?.ParameterSummary(parameter);
            }
        }
    }
}
