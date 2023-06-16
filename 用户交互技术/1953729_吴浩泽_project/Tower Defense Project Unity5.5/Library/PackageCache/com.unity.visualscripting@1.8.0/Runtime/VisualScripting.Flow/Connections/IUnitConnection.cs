namespace Unity.VisualScripting
{
    /* Implementation notes:
     *
     * IUnitConnection cannot implement IConnection<IUnitOutputPort, IUnitInputPort> because
     * the compiler will be overly strict and complain that types may unify.
     * https://stackoverflow.com/questions/7664790
     *
     * Additionally, using contravariance for the type parameters will compile but
     * fail at runtime, because Unity's Mono version does not properly support variance,
     * even though it's supposed to be a CLR feature since .NET 1.1.
     * https://forum.unity3d.com/threads/398665/
     * https://github.com/jacobdufault/fullinspector/issues/9
     *
     * Therefore, the only remaining solution is to re-implement source and destination
     * manually. This introduces ambiguity, as the compiler will warn, but it's fine
     * if the implementations point both members to the same actual object.
     */

    public interface IUnitConnection : IConnection<IUnitOutputPort, IUnitInputPort>, IGraphElementWithDebugData
    {
        new FlowGraph graph { get; }
    }
}
