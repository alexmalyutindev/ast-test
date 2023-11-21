namespace AST.Nodes;

public class BinaryExpressionNode : INode
{
    public string Name => nameof(BinaryExpressionNode);

    public Token Token { get; init; } = default!;

    public INode? Left;
    public INode? Right;
}

public class AssignmentExpressionNode : INode
{
    public string Name => nameof(AssignmentExpressionNode);

    public Token Token { get; init; } = default!;

    public INode? Left;
    public INode? Right;
}