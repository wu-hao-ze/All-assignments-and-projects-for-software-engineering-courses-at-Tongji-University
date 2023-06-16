using System;
using System.Linq;
using Unity.VisualScripting.AssemblyQualifiedNameParser;

namespace Unity.VisualScripting
{
    public abstract class MemberUnitDescriptor<TMemberUnit> : UnitDescriptor<TMemberUnit> where TMemberUnit : MemberUnit
    {
        protected MemberUnitDescriptor(TMemberUnit unit) : base(unit)
        {
        }

        protected Member member => unit.member;

        protected abstract ActionDirection direction { get; }

        private string Name()
        {
            return unit.member.info.DisplayName(direction);
        }

        protected override string DefinedTitle()
        {
            return Name();
        }

        protected override string ErrorSurtitle(Exception exception)
        {
            if (member?.targetType != null)
            {
                return member.targetType.DisplayName();
            }
            else if (member?.targetTypeName != null)
            {
                try
                {
                    var parsedName = new ParsedAssemblyQualifiedName(member.targetTypeName).TypeName.Split('.').Last();

                    if (BoltCore.Configuration.humanNaming)
                    {
                        return parsedName.Prettify();
                    }
                    else
                    {
                        return parsedName;
                    }
                }
                catch
                {
                    return "Malformed Type Name";
                }
            }
            else
            {
                return "Missing Type";
            }
        }

        protected override string ErrorTitle(Exception exception)
        {
            if (!string.IsNullOrEmpty(member?.name))
            {
                if (BoltCore.Configuration.humanNaming)
                {
                    return member.name.Prettify();
                }
                else
                {
                    return member.name;
                }
            }

            return base.ErrorTitle(exception);
        }

        protected override string DefinedShortTitle()
        {
            return Name();
        }

        protected override EditorTexture DefinedIcon()
        {
            return member.targetType.Icon();
        }

        protected override EditorTexture ErrorIcon(Exception exception)
        {
            if (member.targetType != null)
            {
                return member.targetType.Icon();
            }

            return base.ErrorIcon(exception);
        }

        protected override string DefinedSurtitle()
        {
            return member.targetType.DisplayName();
        }

        protected override string DefinedSummary()
        {
            return member.info.Summary();
        }
    }
}
