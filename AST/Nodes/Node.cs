namespace AST.Nodes;

public class Node : INode
{
    public string Name => nameof(Node);
    public Token Token { get; init; } = default!;
}