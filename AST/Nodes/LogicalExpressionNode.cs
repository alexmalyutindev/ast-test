namespace AST.Nodes;

public class LogicalExpressionNode : INode
{
    public string Name => nameof(LogicalExpressionNode);

    public Token Token { get; init; } = default!;

    public INode? Left { get; init; }
    public INode? Right { get; init; }
}