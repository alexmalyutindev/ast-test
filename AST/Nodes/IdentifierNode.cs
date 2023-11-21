namespace AST.Nodes;

public class IdentifierNode : INode
{
    public string Name => nameof(IdentifierNode);
    public Token Token { get; init; }
}