using System;

namespace Unity.VisualScripting
{
    // Variable declarations are cloned for every graph instantiation, it's worth speeding it up.

    public sealed class VariableDeclarationsCloner : Cloner<VariableDeclarations>
    {
        public static readonly VariableDeclarationsCloner instance = new VariableDeclarationsCloner();

        public override bool Handles(Type type)
        {
            return type == typeof(VariableDeclarations);
        }

        public override VariableDeclarations ConstructClone(Type type, VariableDeclarations original)
        {
            return new VariableDeclarations();
        }

        public override void FillClone(Type type, ref VariableDeclarations clone, VariableDeclarations original, CloningContext context)
        {
            foreach (var variable in original)
            {
                clone[variable.name] = variable.value.CloneViaFakeSerialization();
            }
        }
    }
}
