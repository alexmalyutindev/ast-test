namespace AST.Nodes;

public class LiteralNode<T> : INode
{
    public string Name => nameof(LiteralNode<T>);
    public T Value { get; }

    public LiteralNode(T value)
    {
        Value = value;
    }
}