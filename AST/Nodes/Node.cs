namespace AST.Nodes;

public class Node : INode
{
    public string Name { get; private init; } = nameof(Node);

    public Token Token { get; init; } = default!;

    public static readonly INode Empty = new Node()
    {
        Name = "Empty",
        Token = new Token(TokenKind.Unknown)
    };
}