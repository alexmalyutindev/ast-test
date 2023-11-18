using AST;

namespace AST.Nodes;

public class Node : INode
{
    public Token Token { get; init; } = default!;

    public int ChildCount => 0;
    public INode[] Children => Array.Empty<INode>();
}