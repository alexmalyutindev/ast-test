namespace AST.Nodes;

public class BinaryExpressionNode : INode
{
    public string Name => nameof(BinaryExpressionNode);

    public Token Token { get; init; } = default!;

    public BinaryOperator Operator;
    public INode? Left { get; init; }
    public INode? Right { get; init; }
}

public enum BinaryOperator
{
    Unknown = 0,
    Plus,
    Minus,
    Multiply,
    Divide,
}