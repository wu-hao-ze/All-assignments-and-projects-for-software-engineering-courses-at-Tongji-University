using System.Reflection;

namespace Unity.VisualScripting
{
    public abstract class MemberInfoStubWriter<T> : AotStubWriter where T : MemberInfo
    {
        protected MemberInfoStubWriter(T memberInfo) : base(memberInfo)
        {
            stub = memberInfo;
            manipulator = stub.ToManipulator();
        }

        public new T stub { get; }
        protected Member manipulator { get; }

        public override string stubMethodComment => stub.ReflectedType.CSharpFullName() + "." + stub.Name;

        public override string stubMethodName => stubMethodComment.FilterReplace('_', true, symbols: false, whitespace: false, punctuation: false);
    }
}
