using System.CodeDom;
using System.Collections.Generic;

namespace Unity.VisualScripting
{
    public abstract class AotStubWriter
    {
        protected AotStubWriter(object stub)
        {
            this.stub = stub;
        }

        public object stub { get; }

        public virtual bool skip => false;
        public abstract string stubMethodComment { get; }
        public abstract string stubMethodName { get; }
        public abstract IEnumerable<CodeStatement> GetStubStatements();
    }
}
