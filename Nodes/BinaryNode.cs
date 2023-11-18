namespace AST.Nodes;

public class BinaryNode : INode
{
    public Token Token { get; init; } = default!;

    public INode? Left;
    public INode? Right;

    public int ChildCount => 2;
    public INode[] Children => new[] { Left!, Right! };
}