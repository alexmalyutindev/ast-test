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