namespace AST.Nodes;

public class UnaryExpressionNode : INode
{
    public string Name => nameof(UnaryExpressionNode);
    public Token Token { get; init; }
    public INode Argument { get; init; }
}