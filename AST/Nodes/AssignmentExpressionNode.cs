namespace AST.Nodes;

public class AssignmentExpressionNode : INode
{
    public string Name => nameof(AssignmentExpressionNode);

    public Token Token { get; init; } = default!;

    public INode? Left;
    public INode? Right;
}