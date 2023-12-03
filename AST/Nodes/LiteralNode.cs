namespace AST.Nodes;

public class LiteralNode : INode
{
    public string Name => nameof(LiteralNode);

    public Token Token { get; init; } = default!;
}

public class LiteralNode<T> : INode
{
    public string Name => nameof(LiteralNode<T>);
    public T Value { get; }

    public LiteralNode(T value)
    {
        Value = value;
    }
}