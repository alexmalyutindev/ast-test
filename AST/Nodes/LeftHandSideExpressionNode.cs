namespace AST.Nodes;

public class LeftHandSideExpressionNode : INode
{
    public string Name => nameof(LeftHandSideExpressionNode);
    public INode Expression { get; init; }
}