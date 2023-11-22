namespace AST.Nodes;

public class BinaryExpressionNode : INode
{
    public string Name => nameof(BinaryExpressionNode);

    public Token Token { get; init; } = default!;

    public INode? Left { get; init; }
    public INode? Right { get; init; }
}

public class LogicalExpressionNode : INode
{
    public string Name => nameof(LogicalExpressionNode);

    public Token Token { get; init; } = default!;

    public INode? Left { get; init; }
    public INode? Right { get; init; }
}

public class AssignmentExpressionNode : INode
{
    public string Name => nameof(AssignmentExpressionNode);

    public Token Token { get; init; } = default!;

    public INode? Left;
    public INode? Right;
}