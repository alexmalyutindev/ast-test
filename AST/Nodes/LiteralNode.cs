namespace AST.Nodes;

public class LiteralNode : INode
{
    public string Name { get; private init; } = nameof(LiteralNode);

    public Token Token { get; init; } = default!;

    public static readonly INode Empty = new LiteralNode()
    {
        Name = "Empty",
        Token = new Token(TokenKind.Unknown)
    };
}

public class LiteralNode<T> : INode
{
    public string Name => nameof(LiteralNode<T>);
    public Token Token { get; init; } = default!;
    public T Value => GetValue<T>();

    private TLiteral GetValue<TLiteral>()
    {
        throw new Exception($"Literal of type {typeof(T).Name} is not supported!");
    }

    private bool GetValue()
    {
        return bool.Parse(Token.Value);
    }
}