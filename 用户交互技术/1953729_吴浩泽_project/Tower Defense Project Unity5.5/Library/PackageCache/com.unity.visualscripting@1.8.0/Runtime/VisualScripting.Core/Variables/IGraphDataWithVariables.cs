namespace Unity.VisualScripting
{
    public interface IGraphDataWithVariables : IGraphData
    {
        VariableDeclarations variables { get; }
    }
}
