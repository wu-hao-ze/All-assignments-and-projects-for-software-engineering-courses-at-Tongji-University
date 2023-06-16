using System;
using System.Text;

namespace Unity.VisualScripting
{
    public sealed class UnitOptionFilter : ICloneable
    {
        public UnitOptionFilter(bool @default)
        {
            NoControlInput = @default;
            SingleControlInput = @default;
            MultipleControlInputs = @default;

            NoValueInput = @default;
            SingleValueInput = @default;
            MultipleValueInputs = @default;

            NoControlOutput = @default;
            SingleControlOutput = @default;
            MultipleControlOutputs = @default;

            NoValueOutput = @default;
            SingleValueOutput = @default;
            MultipleValueOutputs = @default;

            Normals = @default;
            Self = @default;
            Events = @default;
            Literals = @default;
            Variables = @default;
            Members = @default;
            Nesters = @default;
            Expose = @default;
            NoConnection = @default;
            Obsolete = false;
            AllowSelfNestedGraph = false;
        }

        public bool NoControlInput { get; set; }
        public bool SingleControlInput { get; set; }
        public bool MultipleControlInputs { get; set; }

        public bool NoValueInput { get; set; }
        public bool SingleValueInput { get; set; }
        public bool MultipleValueInputs { get; set; }

        public bool NoControlOutput { get; set; }
        public bool SingleControlOutput { get; set; }
        public bool MultipleControlOutputs { get; set; }

        public bool NoValueOutput { get; set; }
        public bool SingleValueOutput { get; set; }
        public bool MultipleValueOutputs { get; set; }

        public bool Normals { get; set; }
        public bool NoConnection { get; set; }
        public bool Self { get; set; }
        public bool Events { get; set; }
        public bool Literals { get; set; }
        public bool Variables { get; set; }
        public bool Members { get; set; }
        public bool Nesters { get; set; }
        public bool Expose { get; set; }
        public bool Obsolete { get; set; }
        public bool AllowSelfNestedGraph { get; set; }

        public Type CompatibleInputType { get; set; }
        public Type CompatibleOutputType { get; set; }

        public int GraphHashCode { get; set; }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public UnitOptionFilter Clone()
        {
            return new UnitOptionFilter(true)
            {
                NoControlInput = NoControlInput,
                SingleControlInput = SingleControlInput,
                MultipleControlInputs = MultipleControlInputs,
                NoValueInput = NoValueInput,
                SingleValueInput = SingleValueInput,
                MultipleValueInputs = MultipleValueInputs,
                NoControlOutput = NoControlOutput,
                SingleControlOutput = SingleControlOutput,
                MultipleControlOutputs = MultipleControlOutputs,
                NoValueOutput = NoValueOutput,
                SingleValueOutput = SingleValueOutput,
                MultipleValueOutputs = MultipleValueOutputs,
                Normals = Normals,
                Self = Self,
                Events = Events,
                Literals = Literals,
                Variables = Variables,
                Members = Members,
                Nesters = Nesters,
                Expose = Expose,
                Obsolete = Obsolete,
                NoConnection = NoConnection,
                CompatibleInputType = CompatibleInputType,
                CompatibleOutputType = CompatibleOutputType,
                AllowSelfNestedGraph = AllowSelfNestedGraph,
                GraphHashCode = GraphHashCode
            };
        }

        public override bool Equals(object obj)
        {
            var other = obj as UnitOptionFilter;

            if (other == null)
            {
                return false;
            }

            return NoControlInput == other.NoControlInput &&
                SingleControlInput == other.SingleControlInput &&
                MultipleControlInputs == other.MultipleControlInputs &&
                NoValueInput == other.NoValueInput &&
                SingleValueInput == other.SingleValueInput &&
                MultipleValueInputs == other.MultipleValueInputs &&
                NoControlOutput == other.NoControlOutput &&
                SingleControlOutput == other.SingleControlOutput &&
                MultipleControlOutputs == other.MultipleControlOutputs &&
                NoValueOutput == other.NoValueOutput &&
                SingleValueOutput == other.SingleValueOutput &&
                MultipleValueOutputs == other.MultipleValueOutputs &&
                Normals == other.Normals &&
                Self == other.Self &&
                Events == other.Events &&
                Literals == other.Literals &&
                Variables == other.Variables &&
                Members == other.Members &&
                Nesters == other.Nesters &&
                Expose == other.Expose &&
                Obsolete == other.Obsolete &&
                NoConnection == other.NoConnection &&
                CompatibleInputType == other.CompatibleInputType &&
                CompatibleOutputType == other.CompatibleOutputType &&
                AllowSelfNestedGraph == other.AllowSelfNestedGraph &&
                GraphHashCode == other.GraphHashCode;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + NoControlInput.GetHashCode();
                hash = hash * 23 + SingleControlInput.GetHashCode();
                hash = hash * 23 + MultipleControlInputs.GetHashCode();

                hash = hash * 23 + NoValueInput.GetHashCode();
                hash = hash * 23 + SingleValueInput.GetHashCode();
                hash = hash * 23 + MultipleValueInputs.GetHashCode();

                hash = hash * 23 + NoControlOutput.GetHashCode();
                hash = hash * 23 + SingleControlOutput.GetHashCode();
                hash = hash * 23 + MultipleControlOutputs.GetHashCode();

                hash = hash * 23 + NoValueOutput.GetHashCode();
                hash = hash * 23 + SingleValueOutput.GetHashCode();
                hash = hash * 23 + MultipleValueOutputs.GetHashCode();

                hash = hash * 23 + Self.GetHashCode();
                hash = hash * 23 + Events.GetHashCode();
                hash = hash * 23 + Literals.GetHashCode();
                hash = hash * 23 + Variables.GetHashCode();
                hash = hash * 23 + Members.GetHashCode();
                hash = hash * 23 + Nesters.GetHashCode();
                hash = hash * 23 + Expose.GetHashCode();
                hash = hash * 23 + NoConnection.GetHashCode();
                hash = hash * 23 + Obsolete.GetHashCode();
                hash = hash * 23 + AllowSelfNestedGraph.GetHashCode();

                hash = hash * 23 + (CompatibleInputType?.GetHashCode() ?? 0);
                hash = hash * 23 + (CompatibleOutputType?.GetHashCode() ?? 0);

                hash = hash * 23 + GraphHashCode;

                return hash;
            }
        }

        public bool ValidateOption(IUnitOption option)
        {
            Ensure.That(nameof(option)).IsNotNull(option);

            if (!NoControlInput && option.controlInputCount == 0)
            {
                return false;
            }
            if (!SingleControlInput && option.controlInputCount == 1)
            {
                return false;
            }
            if (!MultipleControlInputs && option.controlInputCount > 1)
            {
                return false;
            }

            if (!NoValueInput && option.valueInputTypes.Count == 0)
            {
                return false;
            }
            if (!SingleValueInput && option.valueInputTypes.Count == 1)
            {
                return false;
            }
            if (!MultipleValueInputs && option.valueInputTypes.Count > 1)
            {
                return false;
            }

            if (!NoControlOutput && option.controlOutputCount == 0)
            {
                return false;
            }
            if (!SingleControlOutput && option.controlOutputCount == 1)
            {
                return false;
            }
            if (!MultipleControlOutputs && option.controlOutputCount > 1)
            {
                return false;
            }

            if (!NoValueOutput && option.valueOutputTypes.Count == 0)
            {
                return false;
            }
            if (!SingleValueOutput && option.valueOutputTypes.Count == 1)
            {
                return false;
            }
            if (!MultipleValueOutputs && option.valueOutputTypes.Count > 1)
            {
                return false;
            }

            var unitType = option.unitType;

            if (!Normals && !unitType.HasAttribute<SpecialUnitAttribute>())
            {
                return false;
            }

            if (!Self && option.UnitIs<This>())
            {
                return false;
            }

            if (!Events && option.UnitIs<IEventUnit>())
            {
                return false;
            }

            if (!Literals && option.UnitIs<Literal>())
            {
                return false;
            }

            if (!Variables && option.UnitIs<IUnifiedVariableUnit>())
            {
                return false;
            }

            if (!Members && option.UnitIs<MemberUnit>())
            {
                return false;
            }

            if (!Nesters && option.UnitIs<INesterUnit>())
            {
                return false;
            }

            if (!Expose && option.UnitIs<Expose>())
            {
                return false;
            }

            if (!Obsolete && unitType.HasAttribute<ObsoleteAttribute>())
            {
                return false;
            }

            if (CompatibleInputType != null && !option.HasCompatibleValueInput(CompatibleInputType))
            {
                return false;
            }

            if (CompatibleOutputType != null && !option.HasCompatibleValueOutput(CompatibleOutputType))
            {
                return false;
            }

            if (!AllowSelfNestedGraph && option.UnitIs<SubgraphUnit>())
            {
                if (((SubgraphUnit)option.unit).nest.graph.GetHashCode() == GraphHashCode)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"NoControlInput: {NoControlInput}");
            sb.AppendLine($"SingleControlInput: {SingleControlInput}");
            sb.AppendLine($"MultipleControlInputs: {MultipleControlInputs}");
            sb.AppendLine();
            sb.AppendLine($"NoValueInput: {NoValueInput}");
            sb.AppendLine($"SingleValueInput: {SingleValueInput}");
            sb.AppendLine($"MultipleValueInputs: {MultipleValueInputs}");
            sb.AppendLine();
            sb.AppendLine($"NoControlOutput: {NoControlOutput}");
            sb.AppendLine($"SingleControlOutput: {SingleControlOutput}");
            sb.AppendLine($"MultipleControlOutputs: {MultipleControlOutputs}");
            sb.AppendLine();
            sb.AppendLine($"NoValueOutput: {NoValueOutput}");
            sb.AppendLine($"SingleValueOutput: {SingleValueOutput}");
            sb.AppendLine($"MultipleValueOutputs: {MultipleValueOutputs}");
            sb.AppendLine();
            sb.AppendLine($"Self: {Self}");
            sb.AppendLine($"Events: {Events}");
            sb.AppendLine($"Literals: {Literals}");
            sb.AppendLine($"Variables: {Variables}");
            sb.AppendLine($"Members: {Members}");
            sb.AppendLine($"Nesters: {Nesters}");
            sb.AppendLine($"Expose: {Expose}");
            sb.AppendLine($"Obsolete: {Obsolete}");
            sb.AppendLine($"NoConnection: {NoConnection}");
            sb.AppendLine($"AllowSelfNestedGraph: {AllowSelfNestedGraph}");
            sb.AppendLine($"GraphHashCode: {GraphHashCode}");

            return sb.ToString();
        }

        public static UnitOptionFilter Any => new UnitOptionFilter(true);
        public static UnitOptionFilter None => new UnitOptionFilter(false);
    }
}
